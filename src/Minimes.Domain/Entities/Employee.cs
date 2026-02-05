using System.ComponentModel.DataAnnotations.Schema;

namespace Minimes.Domain.Entities;

/// <summary>
/// 员工实体 - 企业员工信息管理
/// </summary>
public class Employee : BaseEntity
{
    /// <summary>
    /// 员工编码 - 唯一标识
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

    /// <summary>
    /// 关联的二维码数量 - 计算属性，不存储在数据库
    /// </summary>
    [NotMapped]
    public int QRCodeCount { get; set; }
}
