namespace Minimes.Application.DTOs.Device;

/// <summary>
/// 设备日志DTO
/// </summary>
public class DeviceLogDto
{
    /// <summary>
    /// 日志ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 设备ID
    /// </summary>
    public string DeviceId { get; set; } = string.Empty;

    /// <summary>
    /// 日志级别（Info, Warning, Error）
    /// </summary>
    public string Level { get; set; } = string.Empty;

    /// <summary>
    /// 日志消息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 异常信息（如果有）
    /// </summary>
    public string? Exception { get; set; }

    /// <summary>
    /// 日志时间
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// 扩展数据
    /// </summary>
    public Dictionary<string, object>? Data { get; set; }
}
