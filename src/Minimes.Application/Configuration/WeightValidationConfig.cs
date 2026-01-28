namespace Minimes.Application.Configuration;

/// <summary>
/// 重量验证配置 - 用于验证称重记录的重量上下限（英镑单位）
/// </summary>
public class WeightValidationConfig
{
    /// <summary>
    /// 最小重量（磅/lb）- 低于此值将被拒绝
    /// </summary>
    public decimal MinWeightLb { get; set; } = 0.002m; // 默认0.002磅（约1克）

    /// <summary>
    /// 最大重量（磅/lb）- 超过此值将被拒绝
    /// </summary>
    public decimal MaxWeightLb { get; set; } = 440m; // 默认440磅（约200千克）
}
