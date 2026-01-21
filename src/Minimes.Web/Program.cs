using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Minimes.Application;
using Minimes.Application.Interfaces;
using Minimes.Infrastructure;
using Minimes.Infrastructure.Hardware;
using Minimes.Infrastructure.Persistence;
using Minimes.Web.Extensions;
using Minimes.Web.Hubs;

using Minimes.Web.Services;
using OfficeOpenXml;

// 配置EPPlus许可证（EPPlus 8新API，必须在ExcelPackage实例化之前设置）
// 非商业用途：可选择个人或组织许可证
ExcelPackage.License.SetNonCommercialPersonal("MiniMES Development");

var builder = WebApplication.CreateBuilder(args);

// 数据库配置 - 支持SQLite和MySQL切换
builder.Services.AddDatabase(builder.Configuration);

// 硬件配置
var scaleType = builder.Configuration.GetValue<string>("Hardware:ScaleType") ?? "Serial";

if (scaleType.Equals("WiFi", StringComparison.OrdinalIgnoreCase))
{
    // WiFi电子秤配置
    builder.Services.Configure<WiFiScaleConfiguration>(builder.Configuration.GetSection("Hardware:WiFiScale"));
    // 注册HttpClient（WiFiScaleService需要）
    builder.Services.AddHttpClient();
    // 注册WiFiScaleService为IScaleService
    builder.Services.AddSingleton<IScaleService, WiFiScaleService>();
}
else
{
    // 串口电子秤配置（默认）
    builder.Services.Configure<ScaleConfiguration>(builder.Configuration.GetSection("Hardware:Scale"));
    builder.Services.AddSingleton<IScaleService, ScaleService>();
}

// 注册应用层和基础设施层服务
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();

// 认证和授权配置
builder.Services.AddCustomAuthentication();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// 国际化配置
builder.Services.AddLocalization(); // 不指定ResourcesPath
builder.Services.AddControllers(); // CultureController需要

// 添加SignalR支持
builder.Services.AddSignalR();

// 注册硬件后台服务
builder.Services.AddHostedService<HardwareBackgroundService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// 国际化中间件配置
var supportedCultures = new[] { new CultureInfo("en-US"), new CultureInfo("zh-CN") };
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-US"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};
// 清空默认Provider，只用Cookie（避免浏览器语言干扰）
localizationOptions.RequestCultureProviders.Clear();
localizationOptions.RequestCultureProviders.Add(new CookieRequestCultureProvider());
app.UseRequestLocalization(localizationOptions);

// 认证和授权中间件（顺序重要！）
app.UseAuthentication();
app.UseAuthorization();

app.UseRouting();

// 初始化数据库种子数据
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    Minimes.Infrastructure.Persistence.SeedData.Initialize(dbContext);
}

// 健康检查端点（Docker容器健康检查用）
app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    timestamp = DateTime.UtcNow,
    version = "1.0.0"
}));

app.MapBlazorHub();
app.MapHub<HardwareHub>("/hardwareHub");
app.MapControllers(); // 语言切换API
app.MapFallbackToPage("/_Host");

app.Run();
