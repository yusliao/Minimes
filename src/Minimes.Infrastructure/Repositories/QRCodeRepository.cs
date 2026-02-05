using Microsoft.EntityFrameworkCore;
using Minimes.Domain.Entities;
using Minimes.Domain.Interfaces;
using Minimes.Infrastructure.Persistence;

namespace Minimes.Infrastructure.Repositories;

/// <summary>
/// 二维码仓储实现
/// </summary>
public class QRCodeRepository : Repository<QRCode>, IQRCodeRepository
{
    public QRCodeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<QRCode?> GetByContentAsync(string content)
    {
        return await _dbSet
            .Include(q => q.MeatType)
            .FirstOrDefaultAsync(q => q.Content == content);
    }

    public async Task<List<QRCode>> GetByMeatTypeIdAsync(int meatTypeId)
    {
        return await _dbSet
            .Include(q => q.MeatType)
            .Where(q => q.MeatTypeId == meatTypeId)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<QRCode>> GetByBatchNumberAsync(string batchNumber)
    {
        return await _dbSet
            .Include(q => q.MeatType)
            .Where(q => q.BatchNumber == batchNumber)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<QRCode>> GetActiveAsync()
    {
        return await _dbSet
            .Include(q => q.MeatType)
            .Where(q => q.IsActive)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync();
    }

    public async Task UpdatePrintCountAsync(int id)
    {
        var qrCode = await _dbSet.FindAsync(id);
        if (qrCode != null)
        {
            qrCode.PrintCount++;
            qrCode.LastPrintedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ContentExistsAsync(string content, int? excludeId = null)
    {
        var query = _dbSet.Where(q => q.Content == content);

        if (excludeId.HasValue)
        {
            query = query.Where(q => q.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<int> GetQRCodeCountAsync()
    {
        return await _dbSet.CountAsync();
    }

    public async Task<int> GetActiveQRCodeCountAsync()
    {
        return await _dbSet.CountAsync(q => q.IsActive);
    }

    public async Task<List<QRCode>> GetByEmployeeCodeAsync(string employeeCode)
    {
        return await _dbSet
            .Include(q => q.MeatType)
            .Where(q => q.EmployeeCode == employeeCode)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync();
    }

    public async Task DeactivateByEmployeeCodeAsync(string employeeCode)
    {
        var qrCodes = await _dbSet
            .Where(q => q.EmployeeCode == employeeCode && q.IsActive)
            .ToListAsync();

        foreach (var qrCode in qrCodes)
        {
            qrCode.IsActive = false;
        }

        if (qrCodes.Any())
        {
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeactivateByMeatTypeIdAsync(int meatTypeId)
    {
        var qrCodes = await _dbSet
            .Where(q => q.MeatTypeId == meatTypeId && q.IsActive)
            .ToListAsync();

        foreach (var qrCode in qrCodes)
        {
            qrCode.IsActive = false;
        }

        if (qrCodes.Any())
        {
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> GetCountByEmployeeCodeAsync(string employeeCode)
    {
        return await _dbSet.CountAsync(q => q.EmployeeCode == employeeCode);
    }
}
