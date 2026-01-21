using Minimes.Domain.Entities;
using Minimes.Domain.Enums;

namespace Minimes.Domain.Interfaces;

/// <summary>
/// 用户OAuth账号仓储接口
/// </summary>
public interface IUserOAuthAccountRepository : IRepository<UserOAuthAccount>
{
    /// <summary>
    /// 根据OAuth提供商和提供商用户ID获取账号
    /// </summary>
    Task<UserOAuthAccount?> GetByProviderAsync(OAuthProviderType provider, string providerUserId);

    /// <summary>
    /// 获取用户的所有OAuth账号
    /// </summary>
    Task<IEnumerable<UserOAuthAccount>> GetByUserIdAsync(int userId);

    /// <summary>
    /// 检查该提供商账号是否已被绑定
    /// </summary>
    Task<bool> ExistsAsync(OAuthProviderType provider, string providerUserId);
}
