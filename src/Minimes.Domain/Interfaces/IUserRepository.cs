using Minimes.Domain.Entities;

namespace Minimes.Domain.Interfaces;

/// <summary>
/// 用户仓储接口
/// </summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// 根据用户名获取用户
    /// </summary>
    Task<User?> GetByUsernameAsync(string username);

    /// <summary>
    /// 获取所有激活的用户
    /// </summary>
    Task<IEnumerable<User>> GetActiveUsersAsync();

    /// <summary>
    /// 检查用户名是否已存在
    /// </summary>
    Task<bool> UsernameExistsAsync(string username, int? excludeId = null);
}
