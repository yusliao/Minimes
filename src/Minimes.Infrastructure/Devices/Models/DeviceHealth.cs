namespace Minimes.Infrastructure.Devices.Models;

/// <summary>
/// 设备健康等级
/// </summary>
public enum HealthStatus
{
    /// <summary>健康</summary>
    Healthy = 0,

    /// <summary>降级（部分功能受限）</summary>
    Degraded = 1,

    /// <summary>不健康</summary>
    Unhealthy = 2,

    /// <summary>未知</summary>
    Unknown = 3
}

/// <summary>
/// 设备健康状态
/// </summary>
public class DeviceHealth
{
    /// <summary>健康状态</summary>
    public HealthStatus Status { get; set; } = HealthStatus.Unknown;

    /// <summary>健康描述</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>响应时间（毫秒）</summary>
    public double ResponseTimeMs { get; set; }

    /// <summary>数据速率（每秒）</summary>
    public double DataRatePerSecond { get; set; }

    /// <summary>错误率（0-1）</summary>
    public double ErrorRate { get; set; }

    /// <summary>健康检查项列表</summary>
    public List<HealthCheckItem> CheckItems { get; set; } = new();

    /// <summary>建议操作</summary>
    public List<string> Recommendations { get; set; } = new();

    /// <summary>检查时间</summary>
    public DateTime CheckedAt { get; set; } = DateTime.Now;
}

/// <summary>
/// 健康检查项
/// </summary>
public class HealthCheckItem
{
    /// <summary>检查项名称</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>检查结果</summary>
    public bool Passed { get; set; }

    /// <summary>检查描述</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>检查值</summary>
    public object? Value { get; set; }
}
