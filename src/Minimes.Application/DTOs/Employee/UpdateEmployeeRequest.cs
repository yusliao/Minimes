namespace Minimes.Application.DTOs.Employee;

/// <summary>
/// 更新员工请求DTO
/// </summary>
public class UpdateEmployeeRequest
{
    /// <summary>
    /// 员工ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 员工代码
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

    /// <summary>
    /// 是否激活
    /// </summary>
    public bool IsActive { get; set; } = true;
}
