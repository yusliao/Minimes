using Minimes.Domain.Enums;

namespace Minimes.Application.DTOs.WeighingRecord;

/// <summary>
/// 称重记录查询请求 - 简化版
/// </summary>
public class WeighingRecordQueryRequest
{
    /// <summary>
    /// 条码（可选，模糊匹配）
    /// </summary>
    public string? Barcode { get; set; }

    /// <summary>
    /// 加工环节（可选）
    /// </summary>
    public ProcessStage? ProcessStage { get; set; }

    /// <summary>
    /// 开始日期（可选）
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// 结束日期（可选）
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// 操作员用户名（可选，用于权限过滤）
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 页码（从1开始）
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    public int PageSize { get; set; } = 20;
}
