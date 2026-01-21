namespace Minimes.Domain.QueryResults;

/// <summary>
/// 生产报表数据 - 用于数据库聚合查询
/// </summary>
public class ProductionReportData
{
    /// <summary>
    /// 总记录数
    /// </summary>
    public int TotalRecords { get; set; }

    /// <summary>
    /// 原料入库总重量（克）
    /// </summary>
    public decimal ReceivingWeight { get; set; }

    /// <summary>
    /// 加工过程总重量（克）
    /// </summary>
    public decimal ProcessingWeight { get; set; }

    /// <summary>
    /// 成品出库总重量（克）
    /// </summary>
    public decimal ShippingWeight { get; set; }

    /// <summary>
    /// 唯一条码数
    /// </summary>
    public int UniqueBarcodes { get; set; }
}
