using Microsoft.EntityFrameworkCore;
using Minimes.Domain.Entities;
using Minimes.Domain.Enums;
using Minimes.Domain.Interfaces;
using Minimes.Domain.QueryResults;
using Minimes.Infrastructure.Persistence;

namespace Minimes.Infrastructure.Repositories;

/// <summary>
/// 称重记录仓储实现 - 简化版
/// </summary>
public class WeighingRecordRepository : Repository<WeighingRecord>, IWeighingRecordRepository
{
    public WeighingRecordRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<WeighingRecord>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(w => w.CreatedAt >= startDate && w.CreatedAt <= endDate)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<WeighingRecord>> GetByBarcodeAsync(string barcode)
    {
        return await _dbSet
            .Where(w => w.Barcode == barcode)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<WeighingRecord>> GetLatestAsync(int count)
    {
        return await _dbSet
            .OrderByDescending(w => w.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<(List<WeighingRecord> Records, int TotalCount)> QueryPagedAsync(
        string? barcode,
        ProcessStage? processStage,
        DateTime? startDate,
        DateTime? endDate,
        string? createdBy,
        int pageNumber,
        int pageSize)
    {
        var query = _dbSet.AsQueryable();

        // 条码模糊匹配
        if (!string.IsNullOrWhiteSpace(barcode))
        {
            query = query.Where(r => r.Barcode.Contains(barcode));
        }

        // 加工环节过滤
        if (processStage.HasValue)
        {
            query = query.Where(r => r.ProcessStage == processStage.Value);
        }

        // 日期范围过滤
        if (startDate.HasValue)
        {
            query = query.Where(r => r.CreatedAt >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            var endOfDay = endDate.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(r => r.CreatedAt <= endOfDay);
        }

        // 操作员过滤
        if (!string.IsNullOrWhiteSpace(createdBy))
        {
            query = query.Where(r => r.CreatedBy == createdBy);
        }

        // 获取总数（在过滤后计算）
        var totalCount = await query.CountAsync();

        // 排序和分页
        var records = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (records, totalCount);
    }

    public async Task<List<BarcodeStatistic>> GetBarcodeStatisticsAsync(
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var query = _dbSet.AsQueryable();

        if (startDate.HasValue)
        {
            query = query.Where(r => r.CreatedAt >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            var endOfDay = endDate.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(r => r.CreatedAt <= endOfDay);
        }

        return await query
            .GroupBy(r => r.Barcode)
            .Select(g => new BarcodeStatistic
            {
                Barcode = g.Key,
                RecordCount = g.Count(),
                TotalWeight = g.Sum(r => r.Weight)
            })
            .OrderByDescending(s => s.TotalWeight)
            .ToListAsync();
    }

    public async Task<TodayStatistic> GetTodayStatisticsAsync()
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        var records = await _dbSet
            .Where(r => r.CreatedAt >= today && r.CreatedAt < tomorrow)
            .ToListAsync();

        return new TodayStatistic
        {
            TotalRecords = records.Count,
            TotalWeight = records.Sum(r => r.Weight),
            UniqueBarcodes = records.Select(r => r.Barcode).Distinct().Count()
        };
    }

    public async Task<UserOperationStatistic> GetUserOperationStatisticsAsync(string username)
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);
        var monthStart = new DateTime(today.Year, today.Month, 1);

        var query = _dbSet.Where(r => r.CreatedBy == username);

        var todayRecords = await query
            .Where(r => r.CreatedAt >= today && r.CreatedAt < tomorrow)
            .ToListAsync();

        var monthRecords = await query
            .Where(r => r.CreatedAt >= monthStart)
            .ToListAsync();

        var allRecords = await query.ToListAsync();

        return new UserOperationStatistic
        {
            TodayRecords = todayRecords.Count,
            TodayWeight = todayRecords.Sum(r => r.Weight),
            MonthRecords = monthRecords.Count,
            MonthWeight = monthRecords.Sum(r => r.Weight),
            TotalRecords = allRecords.Count,
            TotalWeight = allRecords.Sum(r => r.Weight)
        };
    }
}
