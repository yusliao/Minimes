namespace Minimes.Infrastructure.Devices.Abstractions;

using Minimes.Infrastructure.Devices.Models;
using Minimes.Infrastructure.Devices.Models.EventArgs;

/// <summary>
/// 通用设备接口
/// </summary>
/// <typeparam name="TData">设备数据类型</typeparam>
public interface IDevice<TData> : IDisposable where TData : class
{
    #region 属性

    /// <summary>设备ID（唯一标识）</summary>
    string DeviceId { get; }

    /// <summary>设备元数据</summary>
    DeviceMetadata Metadata { get; }

    /// <summary>设备状态</summary>
    DeviceStatus Status { get; }

    /// <summary>是否已连接</summary>
    bool IsConnected { get; }

    /// <summary>是否正在运行</summary>
    bool IsRunning { get; }

    #endregion

    #region 事件

    /// <summary>数据接收事件</summary>
    event EventHandler<DeviceDataEventArgs<TData>>? DataReceived;

    /// <summary>状态变化事件</summary>
    event EventHandler<DeviceStatusEventArgs>? StatusChanged;

    /// <summary>错误事件</summary>
    event EventHandler<DeviceErrorEventArgs>? ErrorOccurred;

    #endregion

    #region 方法

    /// <summary>
    /// 连接设备
    /// </summary>
    Task<bool> ConnectAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 断开设备连接
    /// </summary>
    Task DisconnectAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 启动数据采集
    /// </summary>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 停止数据采集
    /// </summary>
    Task StopAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 执行设备命令
    /// </summary>
    Task<TResult?> ExecuteCommandAsync<TResult>(string command, object? parameters = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取设备健康状态
    /// </summary>
    Task<DeviceHealth> GetHealthAsync(CancellationToken cancellationToken = default);

    #endregion
}
