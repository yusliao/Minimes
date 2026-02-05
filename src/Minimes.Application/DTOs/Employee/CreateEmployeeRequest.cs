namespace Minimes.Application.DTOs.Employee;

/// <summary>
/// 创建员工请求DTO
/// </summary>
public class CreateEmployeeRequest
{
    /// <summary>
    /// 员工代码（唯一）
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 员工姓名
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
