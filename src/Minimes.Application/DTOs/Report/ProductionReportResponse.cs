namespace Minimes.Application.DTOs.Report;

/// <summary>
/// 生产报表统计响应 - 简化版
/// </summary>
public class ProductionReportResponse
{
    /// <summary>
    /// 总记录数
    /// </summary>
    public int TotalRecords { get; set; }

    /// <summary>
    /// 入库总重量（克）
    /// </summary>
    public decimal ReceivingWeight { get; set; }

    /// <summary>
    /// 加工过程总重量（克）
    /// </summary>
    public decimal ProcessingWeight { get; set; }

    /// <summary>
    /// 出库总重量（克）
    /// </summary>
    public decimal ShippingWeight { get; set; }

    /// <summary>
    /// 涉及的唯一条码数
    /// </summary>
    public int UniqueBarcodes { get; set; }
}
