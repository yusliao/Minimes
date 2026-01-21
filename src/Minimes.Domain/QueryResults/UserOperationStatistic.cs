namespace Minimes.Domain.QueryResults;

/// <summary>
/// 用户操作统计结果 - 用于数据库聚合查询
/// </summary>
public class UserOperationStatistic
{
    /// <summary>
    /// 今日记录数
    /// </summary>
    public int TodayRecords { get; set; }

    /// <summary>
    /// 今日总重量（克）
    /// </summary>
    public decimal TodayWeight { get; set; }

    /// <summary>
    /// 本月记录数
    /// </summary>
    public int MonthRecords { get; set; }

    /// <summary>
    /// 本月总重量（克）
    /// </summary>
    public decimal MonthWeight { get; set; }

    /// <summary>
    /// 总记录数
    /// </summary>
    public int TotalRecords { get; set; }

    /// <summary>
    /// 总重量（克）
    /// </summary>
    public decimal TotalWeight { get; set; }
}
