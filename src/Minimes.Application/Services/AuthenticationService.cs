using FluentValidation;
using Minimes.Application.DTOs.User;
using Minimes.Application.Interfaces;

namespace Minimes.Application.Services;

/// <summary>
/// 身份认证服务实现
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly IUserService _userService;
    private readonly IValidator<LoginRequest> _loginValidator;
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly IValidator<ChangePasswordRequest> _changePasswordValidator;

    public AuthenticationService(
        IUserService userService,
        IValidator<LoginRequest> loginValidator,
        IValidator<RegisterRequest> registerValidator,
        IValidator<ChangePasswordRequest> changePasswordValidator)
    {
        _userService = userService;
        _loginValidator = loginValidator;
        _registerValidator = registerValidator;
        _changePasswordValidator = changePasswordValidator;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        // 验证请求
        var validationResult = await _loginValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            return new LoginResponse
            {
                Success = false,
                Message = $"登录验证失败: {errors}"
            };
        }

        // 验证用户名和密码
        var isValid = await _userService.ValidatePasswordAsync(request.Username, request.Password);
        if (!isValid)
        {
            return new LoginResponse
            {
                Success = false,
                Message = "用户名或密码错误"
            };
        }

        // 获取用户信息
        var user = await _userService.GetByUsernameAsync(request.Username);
        if (user == null)
        {
            return new LoginResponse
            {
                Success = false,
                Message = "用户不存在"
            };
        }

        // 返回登录成功响应
        return new LoginResponse
        {
            Success = true,
            Message = "登录成功",
            UserId = user.Id,
            Username = user.Username,
            FullName = user.FullName,
            Role = user.Role
        };
    }

    public async Task<LoginResponse> RegisterAsync(RegisterRequest request)
    {
        // 验证请求
        var validationResult = await _registerValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            return new LoginResponse
            {
                Success = false,
                Message = $"注册验证失败: {errors}"
            };
        }

        try
        {
            // 创建用户
            var user = await _userService.CreateAsync(request.Username, request.Password, request.FullName);

            // 返回注册成功响应
            return new LoginResponse
            {
                Success = true,
                Message = "注册成功",
                UserId = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Role = user.Role
            };
        }
        catch (InvalidOperationException ex)
        {
            return new LoginResponse
            {
                Success = false,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new LoginResponse
            {
                Success = false,
                Message = $"注册失败: {ex.Message}"
            };
        }
    }

    public async Task<bool> ChangePasswordAsync(ChangePasswordRequest request)
    {
        // 验证请求
        var validationResult = await _changePasswordValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException($"密码修改验证失败: {errors}");
        }

        return await _userService.ChangePasswordAsync(request.UserId, request.OldPassword, request.NewPassword);
    }
}
