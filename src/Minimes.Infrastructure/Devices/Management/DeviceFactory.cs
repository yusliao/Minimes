namespace Minimes.Infrastructure.Devices.Management;

using Microsoft.Extensions.Logging;
using Minimes.Infrastructure.Devices.Abstractions;
using Minimes.Infrastructure.Devices.Models;
using System.Collections.Concurrent;

/// <summary>
/// 设备工厂实现
/// </summary>
public class DeviceFactory : IDeviceFactory
{
    private readonly ILogger<DeviceFactory> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentDictionary<string, object> _driverFactories = new();

    public DeviceFactory(ILogger<DeviceFactory> logger, IServiceProvider serviceProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger.LogInformation("设备工厂已创建");
    }

    /// <inheritdoc/>
    public IDevice<TData> CreateDevice<TData>(DeviceConfiguration configuration) where TData : class
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        if (string.IsNullOrEmpty(configuration.DeviceType))
        {
            throw new ArgumentException("设备类型不能为空", nameof(configuration));
        }

        _logger.LogInformation("开始创建设备: DeviceId={DeviceId}, Type={DeviceType}",
            configuration.DeviceId, configuration.DeviceType);

        // 查找设备驱动工厂
        var key = $"{configuration.DeviceType}_{typeof(TData).Name}";
        if (!_driverFactories.TryGetValue(key, out var factoryObj))
        {
            throw new InvalidOperationException($"未找到设备驱动: DeviceType={configuration.DeviceType}, DataType={typeof(TData).Name}");
        }

        if (factoryObj is not Func<DeviceConfiguration, IDevice<TData>> factory)
        {
            throw new InvalidOperationException($"设备驱动类型不匹配: DeviceType={configuration.DeviceType}");
        }

        // 创建设备实例
        var device = factory(configuration);

        _logger.LogInformation("设备创建成功: DeviceId={DeviceId}, Type={DeviceType}",
            configuration.DeviceId, configuration.DeviceType);

        return device;
    }

    /// <inheritdoc/>
    public void RegisterDriver<TData>(string deviceType, Func<DeviceConfiguration, IDevice<TData>> factory) where TData : class
    {
        if (string.IsNullOrEmpty(deviceType))
        {
            throw new ArgumentNullException(nameof(deviceType));
        }

        if (factory == null)
        {
            throw new ArgumentNullException(nameof(factory));
        }

        var key = $"{deviceType}_{typeof(TData).Name}";
        if (_driverFactories.TryAdd(key, factory))
        {
            _logger.LogInformation("设备驱动已注册: DeviceType={DeviceType}, DataType={DataType}",
                deviceType, typeof(TData).Name);
        }
        else
        {
            throw new InvalidOperationException($"设备驱动已存在: DeviceType={deviceType}, DataType={typeof(TData).Name}");
        }
    }

    /// <inheritdoc/>
    public IReadOnlyList<string> GetSupportedDeviceTypes()
    {
        return _driverFactories.Keys
            .Select(key => key.Split('_')[0])
            .Distinct()
            .ToList();
    }
}
