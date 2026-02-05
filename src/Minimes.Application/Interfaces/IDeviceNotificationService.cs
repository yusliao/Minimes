namespace Minimes.Application.Interfaces;

/// <summary>
/// 设备通知服务接口
/// 艹，这个接口用于解耦Infrastructure层和Web层，避免循环依赖
/// Infrastructure层通过这个接口推送设备事件，Web层实现SignalR推送
/// </summary>
public interface IDeviceNotificationService
{
    /// <summary>
    /// 通知设备状态更新
    /// </summary>
    /// <param name="deviceId">设备ID</param>
    /// <param name="deviceType">设备类型</param>
    /// <param name="oldState">旧状态</param>
    /// <param name="newState">新状态</param>
    Task NotifyDeviceStatusUpdateAsync(string deviceId, string deviceType, string oldState, string newState);

    /// <summary>
    /// 通知设备错误
    /// </summary>
    /// <param name="deviceId">设备ID</param>
    /// <param name="deviceType">设备类型</param>
    /// <param name="errorMessage">错误消息</param>
    /// <param name="severity">严重程度（Info/Warning/Error/Fatal）</param>
    Task NotifyDeviceErrorAsync(string deviceId, string deviceType, string errorMessage, string severity);

    /// <summary>
    /// 通知设备列表更新
    /// 艹，当设备注册/注销时调用这个方法，让前端刷新设备列表
    /// </summary>
    Task NotifyDeviceListUpdateAsync();
}
