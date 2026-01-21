namespace Minimes.Application.DTOs.Customer;

/// <summary>
/// 更新客户请求DTO
/// </summary>
public class UpdateCustomerRequest
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
    public bool IsActive { get; set; } = true;
}
