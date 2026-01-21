namespace Minimes.Domain.Entities;

/// <summary>
/// 客户实体 - 业务往来客户
/// </summary>
public class Customer : BaseEntity
{
    /// <summary>
    /// 客户编码 - 唯一标识
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 客户名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 联系人
    /// </summary>
    public string? ContactPerson { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// 是否激活
    /// </summary>
    public bool IsActive { get; set; } = true;
}
