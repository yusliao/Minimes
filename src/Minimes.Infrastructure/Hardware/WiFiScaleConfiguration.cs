namespace Minimes.Infrastructure.Hardware;

/// <summary>
/// WiFi电子秤配置 - 从appsettings.json读取
/// </summary>
public class WiFiScaleConfiguration
{
    /// <summary>
    /// 电子秤IP地址
    /// </summary>
    public string IpAddress { get; set; } = "192.168.1.200";

    /// <summary>
    /// 通信端口
    /// </summary>
    public int Port { get; set; } = 80;

    /// <summary>
    /// 通信协议：HTTP、WebSocket、TCP
    /// </summary>
    public string Protocol { get; set; } = "HTTP";

    /// <summary>
    /// HTTP API路径 - 获取重量
    /// </summary>
    public string WeightApiPath { get; set; } = "/api/weight";

    /// <summary>
    /// HTTP API路径 - 去皮
    /// </summary>
    public string TareApiPath { get; set; } = "/api/tare";

    /// <summary>
    /// 读取间隔（毫秒）- 轮询模式使用
    /// </summary>
    public int ReadIntervalMs { get; set; } = 500;

    /// <summary>
    /// 稳定阈值（毫秒）- 重量保持不变的时间
    /// </summary>
    public int StableThresholdMs { get; set; } = 1000;

    /// <summary>
    /// 稳定重量容差（克）
    /// </summary>
    public decimal StableWeightTolerance { get; set; } = 0.5m;

    /// <summary>
    /// 连接超时（毫秒）
    /// </summary>
    public int ConnectionTimeoutMs { get; set; } = 5000;

    /// <summary>
    /// 请求超时（毫秒）
    /// </summary>
    public int RequestTimeoutMs { get; set; } = 3000;
}
