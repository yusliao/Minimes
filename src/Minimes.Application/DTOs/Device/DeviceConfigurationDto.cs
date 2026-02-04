namespace Minimes.Application.DTOs.Device;

/// <summary>
/// 设备配置DTO
/// </summary>
public class DeviceConfigurationDto
{
    /// <summary>
    /// 设备ID
    /// </summary>
    public string DeviceId { get; set; } = string.Empty;

    /// <summary>
    /// 设备类型
    /// </summary>
    public string DeviceType { get; set; } = string.Empty;

    /// <summary>
    /// 配置项字典（键值对）
    /// </summary>
    public Dictionary<string, object> ConfigurationItems { get; set; } = new();

    /// <summary>
    /// 配置模式（Serial, HTTP, TCP等）
    /// </summary>
    public string ConfigurationMode { get; set; } = string.Empty;

    /// <summary>
    /// 最后更新时间
    /// </summary>
    public DateTime? LastUpdated { get; set; }
}
