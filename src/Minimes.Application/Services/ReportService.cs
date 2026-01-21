using Microsoft.Extensions.Logging;
using Minimes.Application.DTOs.Report;
using Minimes.Application.Interfaces;
using Minimes.Domain.Enums;
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

        // 加工环节过滤
        if (request.ProcessStage.HasValue)
        {
            filtered = filtered.Where(r => r.ProcessStage == request.ProcessStage.Value);
        }

        var filteredList = filtered.ToList();

        var response = new ProductionReportResponse
        {
            TotalRecords = filteredList.Count,
            ReceivingWeight = filteredList.Where(r => r.ProcessStage == ProcessStage.Receiving).Sum(r => r.Weight),
            ProcessingWeight = filteredList.Where(r => r.ProcessStage == ProcessStage.Processing).Sum(r => r.Weight),
            ShippingWeight = filteredList.Where(r => r.ProcessStage == ProcessStage.Shipping).Sum(r => r.Weight),
            UniqueBarcodes = filteredList.Select(r => r.Barcode).Distinct().Count()
        };

        _logger.LogInformation("生产报表查询: 记录数={TotalRecords}, 入库={ReceivingWeight}kg, 出库={ShippingWeight}kg",
            response.TotalRecords, response.ReceivingWeight, response.ShippingWeight);

        return response;
    }

    public async Task<List<ProductLossRateResponse>> GetBarcodeLossRateAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        // 性能优化：优先使用日期范围查询
        IEnumerable<Domain.Entities.WeighingRecord> records;
        if (startDate.HasValue && endDate.HasValue)
        {
            records = await _recordRepository.GetByDateRangeAsync(startDate.Value, endDate.Value);
        }
        else if (startDate.HasValue)
        {
            records = await _recordRepository.GetByDateRangeAsync(startDate.Value, DateTime.Now.AddYears(1));
        }
        else
        {
            records = await _recordRepository.GetAllAsync();
        }

        var filteredList = records.ToList();

        // 按条码分组统计
        var results = filteredList
            .GroupBy(r => r.Barcode)
            .Select(g => BuildLossRateResponse(g.Key, g.ToList()))
            .OrderByDescending(r => r.LossRate)
            .ToList();

        _logger.LogInformation("条码损耗率统计: 条码数={BarcodeCount}", results.Count);

        return results;
    }

    public async Task<ProductLossRateResponse?> GetBarcodeLossRateByCodeAsync(string barcode, DateTime? startDate = null, DateTime? endDate = null)
    {
        if (string.IsNullOrWhiteSpace(barcode))
        {
            return null;
        }

        // 使用条码查询替代全量加载（性能优化）
        var records = await _recordRepository.GetByBarcodeAsync(barcode);
        var filtered = records.AsEnumerable();

        if (startDate.HasValue)
        {
            filtered = filtered.Where(r => r.CreatedAt >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            var endOfDay = endDate.Value.Date.AddDays(1).AddTicks(-1);
            filtered = filtered.Where(r => r.CreatedAt <= endOfDay);
        }

        var filteredList = filtered.ToList();

        if (!filteredList.Any())
        {
            return null;
        }

        var response = BuildLossRateResponse(barcode, filteredList);

        _logger.LogInformation("条码损耗率查询: Barcode={Barcode}, 损耗率={LossRate}%",
            barcode, response.LossRate);

        return response;
    }

    private static ProductLossRateResponse BuildLossRateResponse(string barcode, List<Domain.Entities.WeighingRecord> records)
    {
        var receivingRecords = records.Where(r => r.ProcessStage == ProcessStage.Receiving).ToList();
        var processingRecords = records.Where(r => r.ProcessStage == ProcessStage.Processing).ToList();
        var shippingRecords = records.Where(r => r.ProcessStage == ProcessStage.Shipping).ToList();

        var receivingWeight = receivingRecords.Sum(r => r.Weight);
        var processingWeight = processingRecords.Sum(r => r.Weight);
        var shippingWeight = shippingRecords.Sum(r => r.Weight);

        var lossWeight = receivingWeight - shippingWeight;
        var lossRate = receivingWeight > 0 ? (lossWeight / receivingWeight) * 100 : 0;

        return new ProductLossRateResponse
        {
            Barcode = barcode,
            ReceivingWeight = receivingWeight,
            ProcessingWeight = processingWeight,
            ShippingWeight = shippingWeight,
            LossWeight = lossWeight,
            LossRate = Math.Round(lossRate, 2),
            ReceivingRecords = receivingRecords.Count,
            ProcessingRecords = processingRecords.Count,
            ShippingRecords = shippingRecords.Count
        };
    }
}
