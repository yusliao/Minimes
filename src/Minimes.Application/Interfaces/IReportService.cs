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

    /// <summary>
    /// 获取条码损耗率统计 - 按条码分组统计入库/加工/出库重量和损耗率
    /// </summary>
    Task<List<ProductLossRateResponse>> GetBarcodeLossRateAsync(DateTime? startDate = null, DateTime? endDate = null);

    /// <summary>
    /// 获取指定条码的损耗率详情
    /// </summary>
    Task<ProductLossRateResponse?> GetBarcodeLossRateByCodeAsync(string barcode, DateTime? startDate = null, DateTime? endDate = null);
}
