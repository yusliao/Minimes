namespace Minimes.Infrastructure.Devices.Models.EventArgs;

/// <summary>
/// 设备状态事件参数
/// </summary>
public class DeviceStatusEventArgs : System.EventArgs
{
    /// <summary>设备ID</summary>
    public string DeviceId { get; set; } = string.Empty;

    /// <summary>旧状态</summary>
    public DeviceState OldState { get; set; }

    /// <summary>新状态</summary>
    public DeviceState NewState { get; set; }

    /// <summary>状态描述</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>时间戳</summary>
    public DateTime Timestamp { get; set; } = DateTime.Now;

    /// <summary>
    /// 构造函数
    /// </summary>
    public DeviceStatusEventArgs(string deviceId, DeviceState oldState, DeviceState newState, string description)
    {
        DeviceId = deviceId;
        OldState = oldState;
        NewState = newState;
        Description = description;
    }
}
