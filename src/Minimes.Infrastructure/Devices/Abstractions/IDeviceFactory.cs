namespace Minimes.Infrastructure.Devices.Abstractions;

using Minimes.Infrastructure.Devices.Models;

/// <summary>
/// 设备工厂接口
/// </summary>
public interface IDeviceFactory
{
    /// <summary>
    /// 根据配置创建设备实例
    /// </summary>
    IDevice<TData> CreateDevice<TData>(DeviceConfiguration configuration) where TData : class;

    /// <summary>
    /// 注册设备驱动
    /// </summary>
    void RegisterDriver<TData>(string deviceType, Func<DeviceConfiguration, IDevice<TData>> factory) where TData : class;

    /// <summary>
    /// 获取支持的设备类型列表
    /// </summary>
    IReadOnlyList<string> GetSupportedDeviceTypes();
}
