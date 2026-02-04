namespace Minimes.Application.DTOs.Device;

/// <summary>
/// 设备信息DTO（列表展示用）
/// </summary>
public class DeviceInfoDto
{
    /// <summary>
    /// 设备ID（唯一标识）
    /// </summary>
    public string DeviceId { get; set; } = string.Empty;

    /// <summary>
    /// 设备类型（Scale, BarcodeScanner, DewPointMeter等）
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
    /// 协议类型（Serial, HTTP, TCP等）
    /// </summary>
    public string ProtocolType { get; set; } = string.Empty;

    /// <summary>
    /// 当前状态（Uninitialized, Disconnected, Connecting, Connected, Running, Paused, Error, Demo）
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
}
