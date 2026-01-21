using Microsoft.EntityFrameworkCore;
using Minimes.Domain.Entities;
using Minimes.Domain.Enums;
using Minimes.Domain.Interfaces;
using Minimes.Infrastructure.Persistence;

namespace Minimes.Infrastructure.Repositories;

/// <summary>
/// 用户OAuth账号仓储实现
/// </summary>
public class UserOAuthAccountRepository : Repository<UserOAuthAccount>, IUserOAuthAccountRepository
{
    public UserOAuthAccountRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<UserOAuthAccount?> GetByProviderAsync(OAuthProviderType provider, string providerUserId)
    {
        return await _dbSet.FirstOrDefaultAsync(o =>
            o.ProviderType == provider && o.ProviderUserId == providerUserId);
    }

    public async Task<IEnumerable<UserOAuthAccount>> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .Where(o => o.UserId == userId)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(OAuthProviderType provider, string providerUserId)
    {
        return await _dbSet.AnyAsync(o =>
            o.ProviderType == provider && o.ProviderUserId == providerUserId);
    }
}
