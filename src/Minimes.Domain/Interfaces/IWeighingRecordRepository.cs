using Minimes.Domain.Entities;
using Minimes.Domain.Enums;
using Minimes.Domain.QueryResults;

namespace Minimes.Domain.Interfaces;

/// <summary>
/// 称重记录仓储接口 - 简化版
/// </summary>
public interface IWeighingRecordRepository : IRepository<WeighingRecord>
{
    /// <summary>
    /// 根据日期范围获取记录
    /// </summary>
    Task<IEnumerable<WeighingRecord>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// 根据条码获取记录
    /// </summary>
    Task<IEnumerable<WeighingRecord>> GetByBarcodeAsync(string barcode);

    /// <summary>
    /// 获取最新的N条记录
    /// </summary>
    Task<IEnumerable<WeighingRecord>> GetLatestAsync(int count);

    /// <summary>
    /// 分页查询称重记录 - 数据库层面过滤和分页
    /// </summary>
    Task<(List<WeighingRecord> Records, int TotalCount)> QueryPagedAsync(
        string? barcode,
        int? processStageId,
        DateTime? startDate,
        DateTime? endDate,
        string? createdBy,
        int pageNumber,
        int pageSize);

    /// <summary>
    /// 获取条码统计 - 数据库聚合查询
    /// </summary>
    Task<List<BarcodeStatistic>> GetBarcodeStatisticsAsync(
        DateTime? startDate = null,
        DateTime? endDate = null);

    /// <summary>
    /// 获取今日统计 - 数据库聚合查询
    /// </summary>
    Task<TodayStatistic> GetTodayStatisticsAsync();

    /// <summary>
    /// 获取用户操作统计 - 数据库聚合查询
    /// </summary>
    Task<UserOperationStatistic> GetUserOperationStatisticsAsync(string username);
}
