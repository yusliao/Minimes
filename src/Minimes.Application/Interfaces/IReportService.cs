using Minimes.Application.DTOs.Report;

namespace Minimes.Application.Interfaces;

/// <summary>
/// 报表统计服务接口 - 简化版
/// </summary>
public interface IReportService
{
    /// <summary>
    /// 获取生产报表统计
    /// </summary>
    Task<ProductionReportResponse> GetProductionReportAsync(ProductionReportRequest request);
}
