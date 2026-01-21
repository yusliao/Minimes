namespace Minimes.Application.DTOs.User;

/// <summary>
/// 修改密码请求DTO
/// </summary>
public class ChangePasswordRequest
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 旧密码
    /// </summary>
    public string OldPassword { get; set; } = string.Empty;

    /// <summary>
    /// 新密码
    /// </summary>
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// 确认新密码
    /// </summary>
    public string ConfirmNewPassword { get; set; } = string.Empty;
}
