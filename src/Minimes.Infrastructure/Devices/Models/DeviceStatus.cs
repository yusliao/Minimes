namespace Minimes.Infrastructure.Devices.Models;

/// <summary>
/// 设备状态枚举
/// </summary>
public enum DeviceState
{
    /// <summary>未初始化</summary>
    Uninitialized = 0,

    /// <summary>已断开</summary>
    Disconnected = 1,

    /// <summary>正在连接</summary>
    Connecting = 2,

    /// <summary>已连接</summary>
    Connected = 3,

    /// <summary>正在运行</summary>
    Running = 4,

    /// <summary>已暂停</summary>
    Paused = 5,

    /// <summary>错误状态</summary>
    Error = 6,

    /// <summary>演示模式</summary>
    Demo = 7
}

/// <summary>
/// 设备状态信息
/// </summary>
public class DeviceStatus
{
    /// <summary>当前状态</summary>
    public DeviceState State { get; set; }

    /// <summary>状态描述</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>连接时间</summary>
    public DateTime? ConnectedAt { get; set; }

    /// <summary>启动时间</summary>
    public DateTime? StartedAt { get; set; }

    /// <summary>总接收数据量</summary>
    public long TotalDataReceived { get; set; }

    /// <summary>错误计数</summary>
    public int ErrorCount { get; set; }

    /// <summary>最后错误信息</summary>
    public string? LastError { get; set; }

    /// <summary>最后错误时间</summary>
    public DateTime? LastErrorAt { get; set; }

    /// <summary>
    /// 克隆状态对象（线程安全）
    /// </summary>
    public DeviceStatus Clone()
    {
        return new DeviceStatus
        {
            State = State,
            Description = Description,
            ConnectedAt = ConnectedAt,
            StartedAt = StartedAt,
            TotalDataReceived = TotalDataReceived,
            ErrorCount = ErrorCount,
            LastError = LastError,
            LastErrorAt = LastErrorAt
        };
    }
}
