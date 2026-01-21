namespace Minimes.Domain.QueryResults;

/// <summary>
/// 今日统计结果 - 用于数据库聚合查询
/// </summary>
public class TodayStatistic
{
    /// <summary>
    /// 总记录数
    /// </summary>
    public int TotalRecords { get; set; }

    /// <summary>
    /// 总重量（克）
    /// </summary>
    public decimal TotalWeight { get; set; }

    /// <summary>
    /// 唯一条码数
    /// </summary>
    public int UniqueBarcodes { get; set; }
}
