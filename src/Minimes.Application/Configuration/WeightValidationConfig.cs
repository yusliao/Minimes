namespace Minimes.Application.Configuration;

/// <summary>
/// 重量验证配置 - 用于验证称重记录的重量上下限
/// </summary>
public class WeightValidationConfig
{
    /// <summary>
    /// 最小重量（千克）- 低于此值将被拒绝
    /// </summary>
    public decimal MinWeightKg { get; set; } = 0.001m; // 默认1克

    /// <summary>
    /// 最大重量（千克）- 超过此值将被拒绝
    /// </summary>
    public decimal MaxWeightKg { get; set; } = 200m; // 默认200千克
}
