using Microsoft.EntityFrameworkCore;
using Minimes.Domain.Entities;
using Minimes.Domain.Interfaces;
using Minimes.Infrastructure.Persistence;

namespace Minimes.Infrastructure.Repositories;

/// <summary>
/// 客户仓储实现
/// </summary>
public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Customer?> GetByCodeAsync(string code)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Code == code);
    }

    public async Task<IEnumerable<Customer>> SearchByNameAsync(string name)
    {
        return await _dbSet
            .Where(c => c.Name.Contains(name) || (c.ContactPerson != null && c.ContactPerson.Contains(name)))
            .ToListAsync();
    }

    public async Task<IEnumerable<Customer>> GetActiveCustomersAsync()
    {
        return await _dbSet
            .Where(c => c.IsActive)
            .OrderBy(c => c.Code)
            .ToListAsync();
    }
}
