using FluentValidation;
using Minimes.Application.DTOs.User;
using Minimes.Application.Interfaces;
using Minimes.Domain.Entities;
using Minimes.Domain.Enums;
using Minimes.Domain.Interfaces;
using Minimes.Domain.Security;

namespace Minimes.Application.Services;

/// <summary>
/// 用户服务实现
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly IValidator<ChangePasswordRequest> _changePasswordValidator;

    public UserService(
        IUserRepository repository,
        IValidator<RegisterRequest> registerValidator,
        IValidator<ChangePasswordRequest> changePasswordValidator)
    {
        _repository = repository;
        _registerValidator = registerValidator;
        _changePasswordValidator = changePasswordValidator;
    }

    public async Task<UserResponse> CreateAsync(string username, string password, string fullName)
    {
        // 默认创建操作员角色
        return await CreateAsync(username, password, fullName, UserRole.Operator);
    }

    public async Task<UserResponse> CreateAsync(string username, string password, string fullName, UserRole role)
    {
        // 验证输入
        var request = new RegisterRequest
        {
            Username = username,
            Password = password,
            ConfirmPassword = password,
            FullName = fullName
        };

        var validationResult = await _registerValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException($"用户创建验证失败: {errors}");
        }

        // 检查用户名是否已存在
        if (await UsernameExistsAsync(username))
        {
            throw new InvalidOperationException($"用户名 '{username}' 已存在");
        }

        // 哈希密码并创建用户
        string passwordHash = PasswordHashService.HashPassword(password);

        var user = new User
        {
            Username = username,
            PasswordHash = passwordHash,
            FullName = fullName,
            Role = role,  // 使用传入的角色
            IsActive = true
        };

        await _repository.AddAsync(user);
        await _repository.SaveChangesAsync();

        return MapToResponse(user);
    }

    public async Task<UserResponse?> GetByIdAsync(int id)
    {
        var user = await _repository.GetByIdAsync(id);
        return user == null ? null : MapToResponse(user);
    }

    public async Task<UserResponse?> GetByUsernameAsync(string username)
    {
        var user = await _repository.GetByUsernameAsync(username);
        return user == null ? null : MapToResponse(user);
    }

    public async Task<bool> ValidatePasswordAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return false;
        }

        var user = await _repository.GetByUsernameAsync(username);
        if (user == null || !user.IsActive)
        {
            return false;
        }

        return PasswordHashService.VerifyPassword(password, user.PasswordHash);
    }

    public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
    {
        // 验证请求
        var request = new ChangePasswordRequest
        {
            UserId = userId,
            OldPassword = oldPassword,
            NewPassword = newPassword,
            ConfirmNewPassword = newPassword
        };

        var validationResult = await _changePasswordValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException($"密码修改验证失败: {errors}");
        }

        var user = await _repository.GetByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        // 验证旧密码
        if (!PasswordHashService.VerifyPassword(oldPassword, user.PasswordHash))
        {
            throw new InvalidOperationException("旧密码不正确");
        }

        // 更新密码
        user.PasswordHash = PasswordHashService.HashPassword(newPassword);
        await _repository.UpdateAsync(user);
        await _repository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ResetPasswordAsync(int userId, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
        {
            throw new ArgumentException("新密码不符合要求");
        }

        var user = await _repository.GetByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        user.PasswordHash = PasswordHashService.HashPassword(newPassword);
        await _repository.UpdateAsync(user);
        await _repository.SaveChangesAsync();

        return true;
    }

    public async Task<UserResponse?> UpdateAsync(int id, string fullName, bool isActive)
    {
        if (string.IsNullOrWhiteSpace(fullName) || fullName.Length < 2 || fullName.Length > 50)
        {
            throw new ArgumentException("姓名长度必须在2-50个字符之间");
        }

        var user = await _repository.GetByIdAsync(id);
        if (user == null)
        {
            return null;
        }

        user.FullName = fullName;
        user.IsActive = isActive;

        await _repository.UpdateAsync(user);
        await _repository.SaveChangesAsync();

        return MapToResponse(user);
    }

    public async Task<bool> UpdateRoleAsync(int userId, UserRole newRole)
    {
        var user = await _repository.GetByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        // 如果是从管理员降级为操作员，检查是否是最后一个管理员
        if (user.Role == UserRole.Administrator && newRole == UserRole.Operator)
        {
            var allUsers = await _repository.GetAllAsync();
            var adminCount = allUsers.Count(u => u.Role == UserRole.Administrator && u.IsActive);

            if (adminCount <= 1)
            {
                throw new InvalidOperationException("不能降级最后一个管理员！系统至少需要保留一个管理员账号。");
            }
        }

        // 更新角色
        user.Role = newRole;
        await _repository.UpdateAsync(user);
        await _repository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user == null)
        {
            return false;
        }

        // 防止删除最后一个管理员账号
        if (user.Role == UserRole.Administrator && user.IsActive)
        {
            var allUsers = await _repository.GetAllAsync();
            var adminCount = allUsers.Count(u => u.Role == UserRole.Administrator && u.IsActive);

            if (adminCount <= 1)
            {
                throw new InvalidOperationException("不能删除最后一个管理员！系统至少需要保留一个管理员账号。");
            }
        }

        await _repository.DeleteAsync(user);
        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<UserResponse>> GetActiveUsersAsync()
    {
        var users = await _repository.GetActiveUsersAsync();
        return users.Select(MapToResponse);
    }

    public async Task<bool> UsernameExistsAsync(string username, int? excludeId = null)
    {
        return await _repository.UsernameExistsAsync(username, excludeId);
    }

    public async Task<IEnumerable<UserResponse>> GetAllAsync()
    {
        var users = await _repository.GetAllAsync();
        return users.Select(MapToResponse);
    }

    /// <summary>
    /// 映射实体到响应DTO
    /// </summary>
    private static UserResponse MapToResponse(User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            FullName = user.FullName,
            Role = user.Role,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}
