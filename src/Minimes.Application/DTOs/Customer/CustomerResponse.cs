namespace Minimes.Application.DTOs.Customer;

/// <summary>
/// 客户响应DTO
/// </summary>
public class CustomerResponse
{
    /// <summary>
    /// 客户ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 客户代码
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
    public bool IsActive { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
