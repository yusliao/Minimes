using Microsoft.EntityFrameworkCore;
using Minimes.Domain.Entities;
using Minimes.Domain.Interfaces;
using Minimes.Infrastructure.Persistence;

namespace Minimes.Infrastructure.Repositories;

/// <summary>
/// 工序仓储实现
/// </summary>
public class ProcessStageRepository : Repository<ProcessStage>, IProcessStageRepository
{
    public ProcessStageRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<ProcessStage?> GetByCodeAsync(string code)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.Code == code);
    }

    public async Task<List<ProcessStage>> GetActiveStagesAsync()
    {
        return await _dbSet
            .Where(p => p.IsActive)
            .OrderBy(p => p.DisplayOrder)
            .ToListAsync();
    }

    public async Task<bool> CodeExistsAsync(string code, int? excludeId = null)
    {
        var query = _dbSet.Where(p => p.Code == code);

        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<bool> IsStageInUseAsync(int stageId)
    {
        // 检查是否有称重记录使用该工序
        return await _context.WeighingRecords.AnyAsync(r => r.ProcessStageId == stageId);
    }

    public async Task<int> GetActiveStageCountAsync()
    {
        return await _dbSet.CountAsync(p => p.IsActive);
    }
}
