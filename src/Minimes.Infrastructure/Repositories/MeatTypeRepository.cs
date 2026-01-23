using Microsoft.EntityFrameworkCore;
using Minimes.Domain.Entities;
using Minimes.Domain.Interfaces;
using Minimes.Infrastructure.Persistence;

namespace Minimes.Infrastructure.Repositories;

/// <summary>
/// 肉类类型仓储实现
/// </summary>
public class MeatTypeRepository : Repository<MeatType>, IMeatTypeRepository
{
    public MeatTypeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<MeatType?> GetByCodeAsync(string code)
    {
        return await _dbSet.FirstOrDefaultAsync(m => m.Code == code);
    }

    public async Task<List<MeatType>> GetActiveTypesAsync()
    {
        return await _dbSet
            .Where(m => m.IsActive)
            .OrderBy(m => m.DisplayOrder)
            .ToListAsync();
    }

    public async Task<bool> CodeExistsAsync(string code, int? excludeId = null)
    {
        var query = _dbSet.Where(m => m.Code == code);

        if (excludeId.HasValue)
        {
            query = query.Where(m => m.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<bool> IsTypeInUseAsync(int typeId)
    {
        // 预留接口，当前返回false
        // 未来关联Product/WeighingRecord时，检查是否有记录使用该肉类类型
        // 例如：return await _context.Products.AnyAsync(p => p.MeatTypeId == typeId);
        return await Task.FromResult(false);
    }

    public async Task<int> GetActiveTypeCountAsync()
    {
        return await _dbSet.CountAsync(m => m.IsActive);
    }
}
