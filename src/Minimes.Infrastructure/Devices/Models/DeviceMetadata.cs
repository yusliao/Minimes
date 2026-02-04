namespace Minimes.Infrastructure.Devices.Models;

/// <summary>
/// 设备元数据
/// </summary>
public class DeviceMetadata
{
    /// <summary>设备类型（Scale, DewPointMeter等）</summary>
    public string DeviceType { get; set; } = string.Empty;

    /// <summary>设备名称</summary>
    public string DeviceName { get; set; } = string.Empty;

    /// <summary>制造商</summary>
    public string? Manufacturer { get; set; }

    /// <summary>型号</summary>
    public string? Model { get; set; }

    /// <summary>序列号</summary>
    public string? SerialNumber { get; set; }

    /// <summary>协议类型（Serial, HTTP, TCP等）</summary>
    public string ProtocolType { get; set; } = string.Empty;

    /// <summary>数据类型（用于反射）</summary>
    public string DataType { get; set; } = string.Empty;

    /// <summary>设备能力标签</summary>
    public List<string> Capabilities { get; set; } = new();

    /// <summary>扩展属性</summary>
    public Dictionary<string, object> ExtendedProperties { get; set; } = new();
}
