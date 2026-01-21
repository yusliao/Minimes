namespace Minimes.Application.Authentication;

/// <summary>
/// OAuth回调请求 - 第三方授权后的回调
/// </summary>
public class OAuthCallbackRequest
{
    /// <summary>
    /// 授权码
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 状态（CSRF防护）
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// OAuth提供商（weixin/google）
    /// </summary>
    public string Provider { get; set; } = string.Empty;
}
