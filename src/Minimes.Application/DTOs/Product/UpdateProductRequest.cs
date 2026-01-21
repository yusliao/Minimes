namespace Minimes.Application.DTOs.Product;

/// <summary>
/// 更新商品请求DTO
/// </summary>
public class UpdateProductRequest
{
    /// <summary>
    /// 商品ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 商品条形码
    /// </summary>
    public string Barcode { get; set; } = string.Empty;

    /// <summary>
    /// 商品名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 商品规格说明
    /// </summary>
    public string? Specification { get; set; }

    /// <summary>
    /// 计量单位
    /// </summary>
    public string Unit { get; set; } = "kg";

    /// <summary>
    /// 参考价格
    /// </summary>
    public decimal? ReferencePrice { get; set; }

    /// <summary>
    /// 是否激活
    /// </summary>
    public bool IsActive { get; set; } = true;
}
