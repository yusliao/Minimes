namespace Minimes.Domain.ValueObjects;

/// <summary>
/// 条形码值对象 - 不可变的商品标识
/// </summary>
public class Barcode
{
    /// <summary>
    /// 条形码值
    /// </summary>
    public string Value { get; private set; }

    /// <summary>
    /// 构造函数 - 确保条形码合法性
    /// </summary>
    /// <param name="value">条形码字符串</param>
    /// <exception cref="ArgumentException">条形码为空时抛出异常</exception>
    public Barcode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("条形码不能为空", nameof(value));

        Value = value.Trim();
    }

    /// <summary>
    /// 值对象相等性比较
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is not Barcode other)
            return false;

        return Value == other.Value;
    }

    /// <summary>
    /// 获取哈希码
    /// </summary>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    /// <summary>
    /// 转换为字符串
    /// </summary>
    public override string ToString()
    {
        return Value;
    }
}
