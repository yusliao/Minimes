using Microsoft.Extensions.DependencyInjection;
using Minimes.Application.Interfaces;
using Minimes.Domain.Interfaces;
using Minimes.Infrastructure.Devices.Abstractions;
using Minimes.Infrastructure.Devices.Management;
using Minimes.Infrastructure.Excel;
using Minimes.Infrastructure.Hardware;
using Minimes.Infrastructure.QRCodeGenerator;
using Minimes.Infrastructure.Repositories;

namespace Minimes.Infrastructure;

/// <summary>
/// 基础设施层依赖注入扩展
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 注册基础设施层服务和仓储
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // 注册仓储
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IWeighingRecordRepository, WeighingRecordRepository>();
        services.AddScoped<IUserOAuthAccountRepository, UserOAuthAccountRepository>();
        services.AddScoped<IMeatTypeRepository, MeatTypeRepository>();
        services.AddScoped<IQRCodeRepository, QRCodeRepository>();

        // 注册二维码生成服务（Singleton生命周期，无状态服务可全局共享）
        services.AddSingleton<IQRCodeGeneratorService, QRCodeGeneratorService>();

        // 注册扫码枪服务（Singleton生命周期，全局共享）
        // 注意：IScaleService不在这里注册，由Program.cs根据配置条件注册
        services.AddSingleton<IBarcodeScannerService, BarcodeScannerService>();

        // 注册Excel导出服务
        services.AddScoped<IExcelExportService, ExcelExportService>();

        // 注册设备管理服务（艹，这些SB服务负责管理硬件设备）
        services.AddSingleton<DeviceLogManager>();  // 设备日志管理器（单例）
        services.AddSingleton<IDeviceManager, DeviceManager>();  // 设备管理器（单例）
        services.AddScoped<IDeviceManagementService, DeviceManagementService>();  // 设备管理服务（Scoped）

        return services;
    }
}
