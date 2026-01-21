namespace Minimes.Application.Authentication;

/// <summary>
/// OAuth提供商接口 - 支持扩展不同的OAuth服务
/// </summary>
public interface IOAuthProvider
{
    /// <summary>
    /// 获取提供商标识
    /// </summary>
    string ProviderName { get; }

    /// <summary>
    /// 获取授权URL（用户跳转到这个URL进行授权）
    /// </summary>
    string GetAuthorizationUrl(string redirectUrl, string state);

    /// <summary>
    /// 交换授权码获取用户信息
    /// </summary>
    Task<OAuthUserInfo> GetUserInfoAsync(string code, string redirectUrl);
}

/// <summary>
/// OAuth用户信息
/// </summary>
public class OAuthUserInfo
{
    /// <summary>
    /// 第三方应用的用户唯一标识
    /// </summary>
    public string ProviderUserId { get; set; } = string.Empty;

    /// <summary>
    /// 用户昵称或邮箱
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 用户头像URL
    /// </summary>
    public string? Avatar { get; set; }

    /// <summary>
    /// 刷新令牌（可选）
    /// </summary>
    public string? RefreshToken { get; set; }
}
