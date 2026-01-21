using Minimes.Domain.Enums;

namespace Minimes.Domain.Entities;

/// <summary>
/// 用户OAuth账号映射
/// 一个本地用户可以绑定多个第三方账号
/// </summary>
public class UserOAuthAccount : BaseEntity
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// OAuth提供商类型（微信、Google等）
    /// </summary>
    public OAuthProviderType ProviderType { get; set; }

    /// <summary>
    /// 第三方应用的用户ID或唯一标识（如微信openid、Google sub）
    /// </summary>
    public string ProviderUserId { get; set; } = string.Empty;

    /// <summary>
    /// 第三方应用返回的昵称或邮箱
    /// </summary>
    public string? ProviderName { get; set; }

    /// <summary>
    /// 第三方应用返回的头像URL
    /// </summary>
    public string? ProviderAvatar { get; set; }

    /// <summary>
    /// 刷新令牌（用于续期访问令牌）
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// 导航属性
    /// </summary>
    public User User { get; set; } = null!;
}
