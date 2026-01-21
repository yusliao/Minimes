namespace Minimes.Application.Authentication;

/// <summary>
/// 认证服务接口
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// 本地账号登录
    /// </summary>
    Task<LoginResponse> LoginAsync(LoginRequest request);

    /// <summary>
    /// 获取OAuth授权URL
    /// </summary>
    Task<string> GetOAuthUrlAsync(string provider);

    /// <summary>
    /// 处理OAuth回调
    /// </summary>
    Task<LoginResponse> HandleOAuthCallbackAsync(OAuthCallbackRequest request);

    /// <summary>
    /// 注册新用户（本地账号）
    /// </summary>
    Task<LoginResponse> RegisterAsync(string username, string password, string fullName);

    /// <summary>
    /// 验证密码
    /// </summary>
    Task<bool> ValidatePasswordAsync(string username, string password);
}
