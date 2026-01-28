using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minimes.Application.Configuration;
using Minimes.Application.DTOs.WeighingRecord;
using Minimes.Application.Interfaces;
using Minimes.Domain.Entities;
using Minimes.Domain.Interfaces;

namespace Minimes.Application.Services;

/// <summary>
/// 称重记录业务服务 - 简化版：条码+重量直接存
/// </summary>
public class WeighingRecordService : IWeighingRecordService
{
    private readonly IWeighingRecordRepository _recordRepository;
    private readonly ILogger<WeighingRecordService> _logger;
    private readonly IOptionsMonitor<WeightValidationConfig> _weightConfig;

    public WeighingRecordService(
        IWeighingRecordRepository recordRepository,
        ILogger<WeighingRecordService> logger,
        IOptionsMonitor<WeightValidationConfig> weightConfig)
    {
        _recordRepository = recordRepository;
        _logger = logger;
        _weightConfig = weightConfig;
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

        // 验证重量上下限（使用CurrentValue获取最新配置）
        if (request.Weight < _weightConfig.CurrentValue.MinWeightLb)
        {
            throw new InvalidOperationException($"重量过轻，最小重量为 {_weightConfig.CurrentValue.MinWeightLb:F3} 磅");
        }

        if (request.Weight > _weightConfig.CurrentValue.MaxWeightLb)
        {
            throw new InvalidOperationException($"重量过重，最大重量为 {_weightConfig.CurrentValue.MaxWeightLb:F1} 磅");
        }

        var record = new WeighingRecord
        {
            Barcode = request.Barcode.Trim(),
            Code = request.Code.Trim(),
            MeatTypeId = request.MeatTypeId,
            Weight = request.Weight,  // 直接存储英镑数值
            Remarks = request.Remarks,
            CreatedBy = createdBy
        };

        await _recordRepository.AddAsync(record);
        await _recordRepository.SaveChangesAsync();

        _logger.LogInformation("创建称重记录: 条码={Barcode}, 重量={Weight}lb",
            record.Barcode, record.Weight);

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
            Code = record.Code,
            MeatTypeId = record.MeatTypeId,
            MeatTypeName = record.MeatType.Name,
            Weight = record.Weight,
            Remarks = record.Remarks,
            CreatedAt = record.CreatedAt,
            CreatedBy = record.CreatedBy
        };
    }
}
