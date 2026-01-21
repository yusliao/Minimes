using Microsoft.Extensions.Logging;
using Minimes.Application.DTOs.WeighingRecord;
using Minimes.Application.Interfaces;
using Minimes.Domain.Entities;
using Minimes.Domain.Enums;
using Minimes.Domain.Interfaces;

namespace Minimes.Application.Services;

/// <summary>
/// 称重记录业务服务 - 简化版：条码+重量直接存
/// </summary>
public class WeighingRecordService : IWeighingRecordService
{
    private readonly IWeighingRecordRepository _recordRepository;
    private readonly ILogger<WeighingRecordService> _logger;

    public WeighingRecordService(
        IWeighingRecordRepository recordRepository,
        ILogger<WeighingRecordService> logger)
    {
        _recordRepository = recordRepository;
        _logger = logger;
    }

    public async Task<WeighingRecordResponse> CreateAsync(CreateWeighingRecordRequest request, string createdBy)
    {
        if (string.IsNullOrWhiteSpace(request.Barcode))
        {
            throw new InvalidOperationException("条码不能为空");
        }

        if (request.Weight <= 0)
        {
            throw new InvalidOperationException("重量必须大于0");
        }

        // 检查同一条码+同一加工环节是否已存在记录
        var existingRecords = await _recordRepository.GetByBarcodeAsync(request.Barcode.Trim());
        if (existingRecords.Any(r => r.ProcessStage == request.ProcessStage))
        {
            var stageName = GetProcessStageName(request.ProcessStage);
            throw new InvalidOperationException($"条码 [{request.Barcode}] 在 [{stageName}] 环节已有记录，不能重复录入");
        }

        var record = new WeighingRecord
        {
            Barcode = request.Barcode.Trim(),
            Weight = request.Weight,
            ProcessStage = request.ProcessStage,
            Remarks = request.Remarks,
            CreatedBy = createdBy
        };

        await _recordRepository.AddAsync(record);
        await _recordRepository.SaveChangesAsync();

        _logger.LogInformation("创建称重记录: 条码={Barcode}, 重量={Weight}g, 环节={Stage}",
            record.Barcode, record.Weight, record.ProcessStage);

        return ToResponse(record);
    }

    public async Task<WeighingRecordResponse?> GetByIdAsync(int id)
    {
        var record = await _recordRepository.GetByIdAsync(id);
        return record == null ? null : ToResponse(record);
    }

    public async Task<(List<WeighingRecordResponse> Records, int TotalCount)> QueryAsync(WeighingRecordQueryRequest request)
    {
        // 使用新的Repository方法 - 数据库层面过滤和分页（性能优化）
        var (records, totalCount) = await _recordRepository.QueryPagedAsync(
            request.Barcode,
            request.ProcessStage,
            request.StartDate,
            request.EndDate,
            request.CreatedBy,
            request.PageNumber,
            request.PageSize);

        var responses = records.Select(ToResponse).ToList();
        return (responses, totalCount);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var record = await _recordRepository.GetByIdAsync(id);
        if (record == null)
        {
            return false;
        }

        await _recordRepository.DeleteAsync(record);
        await _recordRepository.SaveChangesAsync();

        _logger.LogInformation("删除称重记录: Id={Id}", id);
        return true;
    }

    public async Task<TodaySummary> GetTodaySummaryAsync()
    {
        // 使用数据库聚合查询（性能优化）
        var data = await _recordRepository.GetTodayStatisticsAsync();

        return new TodaySummary
        {
            TotalRecords = data.TotalRecords,
            TotalWeight = data.TotalWeight,
            UniqueBarcodes = data.UniqueBarcodes
        };
    }

    public async Task<List<BarcodeStatistics>> GetBarcodeStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        // 使用数据库聚合查询（性能优化）
        var data = await _recordRepository.GetBarcodeStatisticsAsync(startDate, endDate);

        return data.Select(d => new BarcodeStatistics
        {
            Barcode = d.Barcode,
            RecordCount = d.RecordCount,
            TotalWeight = d.TotalWeight
        }).ToList();
    }

    public async Task<UserOperationSummary> GetUserSummaryAsync(string username)
    {
        // 使用数据库聚合查询（性能优化）
        var data = await _recordRepository.GetUserOperationStatisticsAsync(username);

        return new UserOperationSummary
        {
            TodayRecords = data.TodayRecords,
            TodayWeight = data.TodayWeight,
            MonthRecords = data.MonthRecords,
            MonthWeight = data.MonthWeight,
            TotalRecords = data.TotalRecords,
            TotalWeight = data.TotalWeight
        };
    }

    public async Task<List<WeighingRecordResponse>> GetByBarcodeAsync(string barcode)
    {
        var records = await _recordRepository.GetByBarcodeAsync(barcode);
        return records.OrderBy(r => r.CreatedAt).Select(ToResponse).ToList();
    }

    private static WeighingRecordResponse ToResponse(WeighingRecord record)
    {
        return new WeighingRecordResponse
        {
            Id = record.Id,
            Barcode = record.Barcode,
            Weight = record.Weight,
            ProcessStage = record.ProcessStage,
            ProcessStageName = GetProcessStageName(record.ProcessStage),
            Remarks = record.Remarks,
            CreatedAt = record.CreatedAt,
            CreatedBy = record.CreatedBy
        };
    }

    private static string GetProcessStageName(ProcessStage stage)
    {
        return stage switch
        {
            ProcessStage.Receiving => "原料入库",
            ProcessStage.Processing => "加工过程",
            ProcessStage.Shipping => "成品出库",
            _ => "未知"
        };
    }
}
