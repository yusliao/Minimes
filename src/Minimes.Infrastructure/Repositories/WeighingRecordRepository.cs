using Microsoft.EntityFrameworkCore;
using Minimes.Domain.Entities;
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
            .Include(w => w.MeatType)
            .Where(w => w.CreatedAt >= startDate && w.CreatedAt <= endDate)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<WeighingRecord>> GetByBarcodeAsync(string barcode)
    {
        return await _dbSet
            .Include(w => w.MeatType)
            .Where(w => w.Barcode == barcode)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<WeighingRecord>> GetLatestAsync(int count)
    {
        return await _dbSet
            .Include(w => w.MeatType)
            .OrderByDescending(w => w.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<(List<WeighingRecord> Records, int TotalCount)> QueryPagedAsync(
        string? barcode,
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

        // 排序和分页（加载肉类类型导航属性）
        var records = await query
            .Include(r => r.MeatType)
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
                TotalWeight = (decimal)g.Sum(r => (double)r.Weight) // SQLite不支持decimal聚合，转为double
            })
            .OrderByDescending(s => s.TotalWeight)
            .ToListAsync();
    }

    public async Task<TodayStatistic> GetTodayStatisticsAsync()
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        var query = _dbSet.Where(r => r.CreatedAt >= today && r.CreatedAt < tomorrow);

        return new TodayStatistic
        {
            TotalRecords = await query.CountAsync(),
            TotalWeight = (decimal)await query.SumAsync(r => (double)r.Weight), // SQLite不支持decimal聚合，转为double
            UniqueBarcodes = await query.Select(r => r.Barcode).Distinct().CountAsync()
        };
    }

    public async Task<UserOperationStatistic> GetUserOperationStatisticsAsync(string username)
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);
        var monthStart = new DateTime(today.Year, today.Month, 1);

        // 使用数据库聚合查询，避免加载所有记录到内存（性能优化）
        var baseQuery = _dbSet.Where(r => r.CreatedBy == username);

        // 并行执行6个数据库聚合查询
        var todayRecordsTask = baseQuery.CountAsync(r => r.CreatedAt >= today && r.CreatedAt < tomorrow);
        var todayWeightTask = baseQuery
            .Where(r => r.CreatedAt >= today && r.CreatedAt < tomorrow)
            .SumAsync(r => (double?)r.Weight);

        var monthRecordsTask = baseQuery.CountAsync(r => r.CreatedAt >= monthStart);
        var monthWeightTask = baseQuery
            .Where(r => r.CreatedAt >= monthStart)
            .SumAsync(r => (double?)r.Weight);

        var totalRecordsTask = baseQuery.CountAsync();
        var totalWeightTask = baseQuery.SumAsync(r => (double?)r.Weight);

        await Task.WhenAll(todayRecordsTask, todayWeightTask, monthRecordsTask,
                          monthWeightTask, totalRecordsTask, totalWeightTask);

        return new UserOperationStatistic
        {
            TodayRecords = await todayRecordsTask,
            TodayWeight = (decimal)(await todayWeightTask ?? 0),
            MonthRecords = await monthRecordsTask,
            MonthWeight = (decimal)(await monthWeightTask ?? 0),
            TotalRecords = await totalRecordsTask,
            TotalWeight = (decimal)(await totalWeightTask ?? 0)
        };
    }
}
