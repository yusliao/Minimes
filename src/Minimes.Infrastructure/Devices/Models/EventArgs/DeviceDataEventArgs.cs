namespace Minimes.Infrastructure.Devices.Models.EventArgs;

/// <summary>
/// 设备数据事件参数
/// </summary>
/// <typeparam name="TData">数据类型</typeparam>
public class DeviceDataEventArgs<TData> : System.EventArgs where TData : class
{
    /// <summary>设备ID</summary>
    public string DeviceId { get; set; } = string.Empty;

    /// <summary>设备数据</summary>
    public TData Data { get; set; } = default!;

    /// <summary>是否为演示数据</summary>
    public bool IsDemo { get; set; }

    /// <summary>序列号</summary>
    public long SequenceNumber { get; set; }

    /// <summary>时间戳</summary>
    public DateTime Timestamp { get; set; } = DateTime.Now;

    /// <summary>
    /// 构造函数
    /// </summary>
    public DeviceDataEventArgs(string deviceId, TData data, bool isDemo = false, long sequenceNumber = 0)
    {
        DeviceId = deviceId;
        Data = data;
        IsDemo = isDemo;
        SequenceNumber = sequenceNumber;
    }
}
