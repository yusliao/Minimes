using Microsoft.Extensions.Logging;
using Minimes.Application.DTOs.Report;
using Minimes.Application.Interfaces;
using Minimes.Domain.Entities;
using Minimes.Domain.Interfaces;

namespace Minimes.Application.Services;

/// <summary>
/// 报表统计服务 - 简化版：按条码统计
/// </summary>
public class ReportService : IReportService
{
    private readonly IWeighingRecordRepository _recordRepository;
    private readonly ILogger<ReportService> _logger;

    public ReportService(
        IWeighingRecordRepository recordRepository,
        ILogger<ReportService> logger)
    {
        _recordRepository = recordRepository;
        _logger = logger;
    }

    public async Task<ProductionReportResponse> GetProductionReportAsync(ProductionReportRequest request)
    {
        // 性能优化：优先使用日期范围查询
        IEnumerable<Domain.Entities.WeighingRecord> records;
        if (request.StartDate.HasValue && request.EndDate.HasValue)
        {
            records = await _recordRepository.GetByDateRangeAsync(request.StartDate.Value, request.EndDate.Value);
        }
        else if (request.StartDate.HasValue)
        {
            records = await _recordRepository.GetByDateRangeAsync(request.StartDate.Value, DateTime.Now.AddYears(1));
        }
        else
        {
            records = await _recordRepository.GetAllAsync();
        }

        var filtered = records.AsEnumerable();

        // 条码过滤
        if (!string.IsNullOrWhiteSpace(request.Barcode))
        {
            filtered = filtered.Where(r => r.Barcode.Contains(request.Barcode, StringComparison.OrdinalIgnoreCase));
        }

        var filteredList = filtered.ToList();

        // 统计总重量
        var totalWeight = filteredList.Sum(r => r.Weight);

        var response = new ProductionReportResponse
        {
            TotalRecords = filteredList.Count,
            TotalWeight = totalWeight,
            UniqueBarcodes = filteredList.Select(r => r.Barcode).Distinct().Count()
        };

        _logger.LogInformation("生产报表查询: 记录数={TotalRecords}, 总重量={TotalWeight}lb, 唯一条码={UniqueBarcodes}",
            response.TotalRecords, response.TotalWeight, response.UniqueBarcodes);

        return response;
    }

}
