using Minimes.Domain.Enums;

namespace Minimes.Application.DTOs.User;

/// <summary>
/// 用户登录响应DTO
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 消息（失败时返回错误信息）
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 用户ID
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// 姓名
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// 用户角色
    /// </summary>
    public UserRole? Role { get; set; }
}
