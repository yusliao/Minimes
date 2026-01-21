using Minimes.Domain.Enums;

namespace Minimes.Domain.Entities;

/// <summary>
/// 用户实体 - 系统操作员和管理员
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// 用户名 - 登录账号（唯一）
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 密码哈希 - 加密存储
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// 全名 - 真实姓名
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// 角色 - 管理员或操作员
    /// </summary>
    public UserRole Role { get; set; }

    /// <summary>
    /// 是否激活
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 导航属性 - 该用户绑定的OAuth账号
    /// </summary>
    public ICollection<UserOAuthAccount> OAuthAccounts { get; set; } = new List<UserOAuthAccount>();
}
