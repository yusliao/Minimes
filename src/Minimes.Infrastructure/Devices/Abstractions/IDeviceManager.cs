namespace Minimes.Infrastructure.Devices.Abstractions;

using Minimes.Infrastructure.Devices.Models.EventArgs;

/// <summary>
/// 设备管理器接口
/// </summary>
public interface IDeviceManager : IDisposable
{
    #region 事件

    /// <summary>设备状态变化事件（所有设备）</summary>
    event EventHandler<DeviceStatusEventArgs>? DeviceStatusChanged;

    /// <summary>设备错误事件（所有设备）</summary>
    event EventHandler<DeviceErrorEventArgs>? DeviceErrorOccurred;

    #endregion

    #region 设备注册

    /// <summary>
    /// 注册设备
    /// </summary>
    void RegisterDevice<TData>(IDevice<TData> device) where TData : class;

    /// <summary>
    /// 注销设备
    /// </summary>
    void UnregisterDevice(string deviceId);

    #endregion

    #region 设备查询

    /// <summary>
    /// 获取设备
    /// </summary>
    IDevice<TData>? GetDevice<TData>(string deviceId) where TData : class;

    /// <summary>
    /// 获取所有设备
    /// </summary>
    IReadOnlyList<object> GetAllDevices();

    /// <summary>
    /// 检查设备是否存在
    /// </summary>
    bool DeviceExists(string deviceId);

    #endregion

    #region 批量操作

    /// <summary>
    /// 连接所有设备
    /// </summary>
    Task ConnectAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 断开所有设备
    /// </summary>
    Task DisconnectAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 启动所有设备
    /// </summary>
    Task StartAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 停止所有设备
    /// </summary>
    Task StopAllAsync(CancellationToken cancellationToken = default);

    #endregion
}
