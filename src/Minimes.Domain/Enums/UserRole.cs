namespace Minimes.Domain.Enums;

/// <summary>
/// 用户角色枚举
/// </summary>
public enum UserRole
{
    /// <summary>
    /// 操作员 - 只能进行称重记录操作
    /// </summary>
    Operator = 1,

    /// <summary>
    /// 管理员 - 拥有所有权限
    /// </summary>
    Administrator = 2
}
