namespace Minimes.Application.DTOs.User;

/// <summary>
/// 用户登录请求DTO
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 是否记住我（保持登录状态）
    /// </summary>
    public bool RememberMe { get; set; } = false;
}
