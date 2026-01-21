namespace Minimes.Application.DTOs.Customer;

/// <summary>
/// 创建客户请求DTO
/// </summary>
public class CreateCustomerRequest
{
    /// <summary>
    /// 客户代码（唯一）
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
}
