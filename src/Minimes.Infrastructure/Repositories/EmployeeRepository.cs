using Microsoft.EntityFrameworkCore;
using Minimes.Domain.Entities;
using Minimes.Domain.Interfaces;
using Minimes.Infrastructure.Persistence;

namespace Minimes.Infrastructure.Repositories;

/// <summary>
/// 员工仓储实现
/// </summary>
public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Employee?> GetByCodeAsync(string code)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.Code == code);
    }

    public async Task<IEnumerable<Employee>> SearchByNameAsync(string name)
    {
        return await _dbSet
            .Where(e => e.Name.Contains(name) || (e.ContactPerson != null && e.ContactPerson.Contains(name)))
            .ToListAsync();
    }

    public async Task<IEnumerable<Employee>> GetActiveEmployeesAsync()
    {
        return await _dbSet
            .Where(e => e.IsActive)
            .OrderBy(e => e.Code)
            .ToListAsync();
    }
}
