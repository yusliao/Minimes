namespace Minimes.Application.DTOs.Report;

/// <summary>
/// 生产报表查询请求 - 简化版
/// </summary>
public class ProductionReportRequest
{
    /// <summary>
    /// 开始日期
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// 结束日期
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// 条码（可选，模糊匹配）
    /// </summary>
    public string? Barcode { get; set; }
}
