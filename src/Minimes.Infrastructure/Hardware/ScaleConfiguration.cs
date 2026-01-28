using System.IO.Ports;

namespace Minimes.Infrastructure.Hardware;

/// <summary>
/// 电子秤配置 - 从appsettings.json读取
/// </summary>
public class ScaleConfiguration
{
    public string PortName { get; set; } = "COM3";
    public int BaudRate { get; set; } = 9600;
    public int DataBits { get; set; } = 8;
    public StopBits StopBits { get; set; } = StopBits.One;
    public Parity Parity { get; set; } = Parity.None;
    public string Protocol { get; set; } = "Generic"; // Generic, Toledo, Mettler等
    public int ReadIntervalMs { get; set; } = 500; // 读取间隔毫秒
    public int StableThresholdMs { get; set; } = 1000; // 稳定阈值毫秒
    public decimal StableWeightTolerance { get; set; } = 0.5m; // 稳定重量容差（克）

    /// <summary>
    /// 最小重量（磅/lb）- 低于此值将被拒绝
    /// </summary>
    public decimal MinWeightLb { get; set; } = 0.002m; // 默认0.002磅（约1克）

    /// <summary>
    /// 最大重量（磅/lb）- 超过此值将被拒绝
    /// </summary>
    public decimal MaxWeightLb { get; set; } = 440m; // 默认440磅（约200千克）
}
