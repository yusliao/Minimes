using Microsoft.AspNetCore.Authentication.Cookies;

namespace Minimes.Web.Extensions;

/// <summary>
/// 认证配置扩展
/// </summary>
public static class AuthenticationExtensions
{
    /// <summary>
    /// 添加Cookie认证配置
    /// </summary>
    public static IServiceCollection AddCustomAuthentication(this IServiceCollection services)
    {
        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                // Cookie设置
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/Login";

                // Cookie参数
                options.Cookie.Name = "MinimesAuth";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // HTTP/HTTPS自适应

                // 过期时间：30天（支持"记住我"功能）
                options.ExpireTimeSpan = TimeSpan.FromDays(30);

                // 滑动过期时间：用户活动时自动延期
                options.SlidingExpiration = true;

                // Events配置
                options.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = context =>
                    {
                        // API请求时返回401而不是重定向
                        if (context.Request.Path.StartsWithSegments("/api"))
                        {
                            context.Response.StatusCode = 401;
                            return Task.CompletedTask;
                        }

                        context.Response.Redirect(context.RedirectUri);
                        return Task.CompletedTask;
                    },
                    OnRedirectToAccessDenied = context =>
                    {
                        // API请求时返回403而不是重定向
                        if (context.Request.Path.StartsWithSegments("/api"))
                        {
                            context.Response.StatusCode = 403;
                            return Task.CompletedTask;
                        }

                        context.Response.Redirect(context.RedirectUri);
                        return Task.CompletedTask;
                    }
                };
            });

        // 添加授权
        services.AddAuthorization(options =>
        {
            // 定义"Admin"策略
            options.AddPolicy("Admin", policy =>
                policy.RequireRole("Administrator"));

            // 定义"Operator"策略（操作员和管理员都可以访问）
            options.AddPolicy("Operator", policy =>
                policy.RequireRole("Operator", "Administrator"));

            // 定义"Authenticated"策略（任何已认证用户）
            options.AddPolicy("Authenticated", policy =>
                policy.RequireAuthenticatedUser());
        });
        services.AddCascadingAuthenticationState();

        return services;
    }
}
