namespace Minimes.Application.DTOs.Device;

/// <summary>
/// 设备健康状态DTO
/// </summary>
public class DeviceHealthDto
{
    /// <summary>
    /// 是否健康
    /// </summary>
    public bool IsHealthy { get; set; }

    /// <summary>
    /// 健康分数（0-100）
    /// </summary>
    public int HealthScore { get; set; }

    /// <summary>
    /// 运行时长（秒）
    /// </summary>
    public long UptimeSeconds { get; set; }

    /// <summary>
    /// 最后心跳时间
    /// </summary>
    public DateTime? LastHeartbeat { get; set; }

    /// <summary>
    /// 诊断信息
    /// </summary>
    public Dictionary<string, string> Diagnostics { get; set; } = new();

    /// <summary>
    /// 警告信息列表
    /// </summary>
    public List<string> Warnings { get; set; } = new();
}
