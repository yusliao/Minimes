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
    /// 总重量（磅）
    /// </summary>
    public decimal TotalWeight { get; set; }

    /// <summary>
    /// 涉及的唯一条码数
    /// </summary>
    public int UniqueBarcodes { get; set; }
}
