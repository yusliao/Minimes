using Minimes.Application.DTOs.User;
using Minimes.Domain.Enums;

namespace Minimes.Application.Interfaces;

/// <summary>
/// 用户服务接口
/// </summary>
public interface IUserService
{
    /// <summary>
    /// 创建用户（默认为操作员）
    /// </summary>
    Task<UserResponse> CreateAsync(string username, string password, string fullName);

    /// <summary>
    /// 创建用户（指定角色）- 仅管理员可调用
    /// </summary>
    Task<UserResponse> CreateAsync(string username, string password, string fullName, UserRole role);

    /// <summary>
    /// 根据ID获取用户
    /// </summary>
    Task<UserResponse?> GetByIdAsync(int id);

    /// <summary>
    /// 根据用户名获取用户
    /// </summary>
    Task<UserResponse?> GetByUsernameAsync(string username);

    /// <summary>
    /// 验证用户密码
    /// </summary>
    Task<bool> ValidatePasswordAsync(string username, string password);

    /// <summary>
    /// 修改密码
    /// </summary>
    Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);

    /// <summary>
    /// 重置密码（管理员）
    /// </summary>
    Task<bool> ResetPasswordAsync(int userId, string newPassword);

    /// <summary>
    /// 更新用户（不含角色）
    /// </summary>
    Task<UserResponse?> UpdateAsync(int id, string fullName, bool isActive);

    /// <summary>
    /// 更新用户角色 - 仅管理员可调用
    /// </summary>
    Task<bool> UpdateRoleAsync(int userId, UserRole newRole);

    /// <summary>
    /// 删除用户
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// 获取所有活跃用户
    /// </summary>
    Task<IEnumerable<UserResponse>> GetActiveUsersAsync();

    /// <summary>
    /// 检查用户名是否已存在
    /// </summary>
    Task<bool> UsernameExistsAsync(string username, int? excludeId = null);

    /// <summary>
    /// 获取所有用户
    /// </summary>
    Task<IEnumerable<UserResponse>> GetAllAsync();
}
