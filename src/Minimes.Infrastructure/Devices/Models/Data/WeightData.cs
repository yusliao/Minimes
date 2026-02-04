namespace Minimes.Infrastructure.Devices.Models.Data;

/// <summary>
/// 重量单位枚举
/// </summary>
public enum WeightUnit
{
    /// <summary>克</summary>
    Gram = 0,

    /// <summary>千克</summary>
    Kilogram = 1,

    /// <summary>磅</summary>
    Pound = 2,

    /// <summary>盎司</summary>
    Ounce = 3
}

/// <summary>
/// 称重数据
/// </summary>
public class WeightData
{
    /// <summary>重量值</summary>
    public decimal Weight { get; set; }

    /// <summary>重量单位</summary>
    public WeightUnit Unit { get; set; } = WeightUnit.Gram;

    /// <summary>是否稳定</summary>
    public bool IsStable { get; set; }

    /// <summary>是否为净重</summary>
    public bool IsNet { get; set; }

    /// <summary>皮重</summary>
    public decimal? TareWeight { get; set; }

    /// <summary>时间戳</summary>
    public DateTime Timestamp { get; set; } = DateTime.Now;

    /// <summary>
    /// 转换为克
    /// </summary>
    public decimal ToGrams()
    {
        return Unit switch
        {
            WeightUnit.Gram => Weight,
            WeightUnit.Kilogram => Weight * 1000,
            WeightUnit.Pound => Weight * 453.592m,
            WeightUnit.Ounce => Weight * 28.3495m,
            _ => Weight
        };
    }

    /// <summary>
    /// 转换为千克
    /// </summary>
    public decimal ToKilograms()
    {
        return ToGrams() / 1000;
    }

    public override string ToString()
    {
        var unitStr = Unit switch
        {
            WeightUnit.Gram => "g",
            WeightUnit.Kilogram => "kg",
            WeightUnit.Pound => "lb",
            WeightUnit.Ounce => "oz",
            _ => ""
        };

        var stableStr = IsStable ? "稳定" : "不稳定";
        var netStr = IsNet ? "净重" : "毛重";

        return $"{Weight}{unitStr} ({stableStr}, {netStr})";
    }
}
