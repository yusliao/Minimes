using Minimes.Application.DTOs.WeighingRecord;

namespace Minimes.Application.Interfaces;

/// <summary>
/// 称重记录业务服务接口 - 简化版
/// </summary>
public interface IWeighingRecordService
{
    /// <summary>
    /// 创建称重记录
    /// </summary>
    Task<WeighingRecordResponse> CreateAsync(CreateWeighingRecordRequest request, string createdBy);

    /// <summary>
    /// 获取称重记录详情
    /// </summary>
    Task<WeighingRecordResponse?> GetByIdAsync(int id);

    /// <summary>
    /// 查询称重记录（分页）
    /// </summary>
    Task<(List<WeighingRecordResponse> Records, int TotalCount)> QueryAsync(WeighingRecordQueryRequest request);

    /// <summary>
    /// 删除称重记录
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// 获取今日记录汇总
    /// </summary>
    Task<TodaySummary> GetTodaySummaryAsync();

    /// <summary>
    /// 根据条码统计
    /// </summary>
    Task<List<BarcodeStatistics>> GetBarcodeStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);

    /// <summary>
    /// 获取用户操作统计
    /// </summary>
    Task<UserOperationSummary> GetUserSummaryAsync(string username);

    /// <summary>
    /// 根据条码查询记录（用于质量追溯）
    /// </summary>
    Task<List<WeighingRecordResponse>> GetByBarcodeAsync(string barcode);
}

/// <summary>
/// 今日汇总 - 简化版
/// </summary>
public class TodaySummary
{
    public int TotalRecords { get; set; }
    public decimal TotalWeight { get; set; }
    public int UniqueBarcodes { get; set; }
}

/// <summary>
/// 条码统计
/// </summary>
public class BarcodeStatistics
{
    public string Barcode { get; set; } = string.Empty;
    public int RecordCount { get; set; }
    public decimal TotalWeight { get; set; }
}

/// <summary>
/// 用户操作统计
/// </summary>
public class UserOperationSummary
{
    public int TodayRecords { get; set; }
    public decimal TodayWeight { get; set; }
    public int MonthRecords { get; set; }
    public decimal MonthWeight { get; set; }
    public int TotalRecords { get; set; }
    public decimal TotalWeight { get; set; }
}
