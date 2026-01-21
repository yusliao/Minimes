namespace Minimes.Domain.QueryResults;

/// <summary>
/// 条码统计结果 - 用于数据库聚合查询
/// </summary>
public class BarcodeStatistic
{
    /// <summary>
    /// 条码
    /// </summary>
    public string Barcode { get; set; } = string.Empty;

    /// <summary>
    /// 记录数量
    /// </summary>
    public int RecordCount { get; set; }

    /// <summary>
    /// 总重量（克）
    /// </summary>
    public decimal TotalWeight { get; set; }
}
