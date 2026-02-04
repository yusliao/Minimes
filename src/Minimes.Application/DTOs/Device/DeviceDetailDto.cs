namespace Minimes.Application.DTOs.Device;

/// <summary>
/// 设备详情DTO（详情页面用）
/// </summary>
public class DeviceDetailDto
{
    /// <summary>
    /// 设备ID（唯一标识）
    /// </summary>
    public string DeviceId { get; set; } = string.Empty;

    /// <summary>
    /// 设备类型
    /// </summary>
    public string DeviceType { get; set; } = string.Empty;

    /// <summary>
    /// 设备名称
    /// </summary>
    public string DeviceName { get; set; } = string.Empty;

    /// <summary>
    /// 制造商
    /// </summary>
    public string? Manufacturer { get; set; }

    /// <summary>
    /// 型号
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// 序列号
    /// </summary>
    public string? SerialNumber { get; set; }

    /// <summary>
    /// 协议类型
    /// </summary>
    public string ProtocolType { get; set; } = string.Empty;

    /// <summary>
    /// 数据类型
    /// </summary>
    public string DataType { get; set; } = string.Empty;

    /// <summary>
    /// 设备能力标签
    /// </summary>
    public List<string> Capabilities { get; set; } = new();

    /// <summary>
    /// 扩展属性
    /// </summary>
    public Dictionary<string, object> ExtendedProperties { get; set; } = new();

    /// <summary>
    /// 当前状态
    /// </summary>
    public string State { get; set; } = string.Empty;

    /// <summary>
    /// 状态描述
    /// </summary>
    public string StateDescription { get; set; } = string.Empty;

    /// <summary>
    /// 是否已连接
    /// </summary>
    public bool IsConnected { get; set; }

    /// <summary>
    /// 是否正在运行
    /// </summary>
    public bool IsRunning { get; set; }

    /// <summary>
    /// 连接时间
    /// </summary>
    public DateTime? ConnectedAt { get; set; }

    /// <summary>
    /// 启动时间
    /// </summary>
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// 总接收数据量
    /// </summary>
    public long TotalDataReceived { get; set; }

    /// <summary>
    /// 错误计数
    /// </summary>
    public int ErrorCount { get; set; }

    /// <summary>
    /// 最后错误信息
    /// </summary>
    public string? LastError { get; set; }

    /// <summary>
    /// 最后错误时间
    /// </summary>
    public DateTime? LastErrorAt { get; set; }

    /// <summary>
    /// 设备健康状态
    /// </summary>
    public DeviceHealthDto? Health { get; set; }
}
