namespace Minimes.Domain.Entities;

/// <summary>
/// 商品实体 - 可称重的商品
/// </summary>
public class Product : BaseEntity
{
    /// <summary>
    /// 条形码 - 唯一标识（扫码枪扫描）
    /// </summary>
    public string Barcode { get; set; } = string.Empty;

    /// <summary>
    /// 商品名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 规格说明
    /// </summary>
    public string? Specification { get; set; }

    /// <summary>
    /// 单位 - 如：箱、件、个
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// 参考价格（可选）
    /// </summary>
    public decimal? ReferencePrice { get; set; }

    /// <summary>
    /// 是否激活
    /// </summary>
    public bool IsActive { get; set; } = true;
}
