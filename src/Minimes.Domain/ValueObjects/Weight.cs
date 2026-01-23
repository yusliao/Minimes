using Minimes.Domain.Enums;

namespace Minimes.Domain.ValueObjects;

/// <summary>
/// 重量值对象 - 包含数值和单位的不可变对象
/// </summary>
public class Weight
{
    /// <summary>
    /// 重量数值
    /// </summary>
    public decimal Value { get; private set; }

    /// <summary>
    /// 重量单位
    /// </summary>
    public WeightUnit Unit { get; private set; }

    /// <summary>
    /// 构造函数 - 确保重量合法性
    /// </summary>
    /// <param name="value">重量数值</param>
    /// <param name="unit">重量单位</param>
    /// <exception cref="ArgumentException">重量为负数时抛出异常</exception>
    public Weight(decimal value, WeightUnit unit)
    {
        if (value < 0)
            throw new ArgumentException("重量不能为负数", nameof(value));

        Value = value;
        Unit = unit;
    }

    /// <summary>
    /// 转换为千克 - 统一计算标准
    /// </summary>
    public decimal ToKilograms()
    {
        return Unit switch
        {
            WeightUnit.Gram => Value / 1000m,
            WeightUnit.Kilogram => Value,
            WeightUnit.Ton => Value * 1000m,
            WeightUnit.Pound => Value * 0.45359237m,  // 1磅 = 0.45359237千克
            _ => throw new NotSupportedException($"不支持的单位: {Unit}")
        };
    }

    /// <summary>
    /// 值对象相等性比较（统一转换为千克后比较）
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is not Weight other)
            return false;

        return ToKilograms() == other.ToKilograms();
    }

    /// <summary>
    /// 获取哈希码
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(Value, Unit);
    }

    /// <summary>
    /// 转换为字符串
    /// </summary>
    public override string ToString()
    {
        var unitText = Unit switch
        {
            WeightUnit.Gram => "g",
            WeightUnit.Kilogram => "kg",
            WeightUnit.Ton => "t",
            WeightUnit.Pound => "lb",
            _ => Unit.ToString()
        };

        return $"{Value:F2} {unitText}";
    }

    /// <summary>
    /// 从磅创建Weight对象
    /// </summary>
    public static Weight FromPounds(decimal pounds)
    {
        return new Weight(pounds, WeightUnit.Pound);
    }

    /// <summary>
    /// 转换为磅
    /// </summary>
    public decimal ToPounds()
    {
        return ToKilograms() / 0.45359237m;
    }
}
