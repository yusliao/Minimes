namespace Minimes.Application.DTOs.User;

/// <summary>
/// 用户注册请求DTO
/// </summary>
public class RegisterRequest
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
    /// 确认密码
    /// </summary>
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// 姓名
    /// </summary>
    public string FullName { get; set; } = string.Empty;
}
