namespace Minimes.Infrastructure.Devices.Models;

/// <summary>
/// 设备配置
/// </summary>
public class DeviceConfiguration
{
    /// <summary>设备ID（唯一标识）</summary>
    public string DeviceId { get; set; } = string.Empty;

    /// <summary>设备类型</summary>
    public string DeviceType { get; set; } = string.Empty;

    /// <summary>设备名称</summary>
    public string DeviceName { get; set; } = string.Empty;

    /// <summary>是否启用</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>是否自动连接</summary>
    public bool AutoConnect { get; set; } = true;

    /// <summary>是否自动启动</summary>
    public bool AutoStart { get; set; } = true;

    /// <summary>协议类型</summary>
    public string ProtocolType { get; set; } = string.Empty;

    /// <summary>协议设置</summary>
    public Dictionary<string, object> ProtocolSettings { get; set; } = new();

    /// <summary>设备特定设置</summary>
    public Dictionary<string, object> DeviceSettings { get; set; } = new();

    /// <summary>连接超时（毫秒）</summary>
    public int ConnectionTimeoutMs { get; set; } = 5000;

    /// <summary>读取间隔（毫秒）</summary>
    public int ReadIntervalMs { get; set; } = 200;

    /// <summary>重连策略</summary>
    public ReconnectionPolicy? ReconnectionPolicy { get; set; }

    /// <summary>数据过滤器配置</summary>
    public DataFilterConfiguration? DataFilter { get; set; }
}

/// <summary>
/// 重连策略
/// </summary>
public class ReconnectionPolicy
{
    /// <summary>是否启用自动重连</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>最大重试次数</summary>
    public int MaxRetries { get; set; } = 5;

    /// <summary>重试间隔（毫秒）</summary>
    public int RetryIntervalMs { get; set; } = 3000;

    /// <summary>是否使用指数退避</summary>
    public bool UseExponentialBackoff { get; set; } = true;

    /// <summary>最大退避时间（毫秒）</summary>
    public int MaxBackoffMs { get; set; } = 30000;
}

/// <summary>
/// 数据过滤器配置
/// </summary>
public class DataFilterConfiguration
{
    /// <summary>是否启用过滤</summary>
    public bool Enabled { get; set; } = false;

    /// <summary>最小变化阈值（用于去重）</summary>
    public double? MinChangeThreshold { get; set; }

    /// <summary>最小时间间隔（毫秒，用于限流）</summary>
    public int? MinIntervalMs { get; set; }

    /// <summary>自定义过滤规则</summary>
    public Dictionary<string, object> CustomRules { get; set; } = new();
}
