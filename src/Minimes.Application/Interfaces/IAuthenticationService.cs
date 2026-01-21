using Minimes.Application.DTOs.User;

namespace Minimes.Application.Interfaces;

/// <summary>
/// 身份认证服务接口
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// 用户登录
    /// </summary>
    Task<LoginResponse> LoginAsync(LoginRequest request);

    /// <summary>
    /// 用户注册
    /// </summary>
    Task<LoginResponse> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// 修改密码
    /// </summary>
    Task<bool> ChangePasswordAsync(ChangePasswordRequest request);
}
