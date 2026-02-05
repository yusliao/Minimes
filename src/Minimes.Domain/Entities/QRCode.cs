namespace Minimes.Domain.Entities;

/// <summary>
/// 二维码实体 - 肉类产品二维码标签管理
/// 用于生成和管理肉类产品的二维码标签，支持批量创建和打印
/// </summary>
public class QRCode : BaseEntity
{
    /// <summary>
    /// 二维码唯一标识 - 完整的唯一编码（如：PORK-E001）
    /// 格式：MeatType.Code + "-" + Employee.Code
    /// 全局唯一，用于快速查找和关联
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 员工编码 - 关联的员工编码（如：E001）
    /// 用于级联查询和停用操作
    /// </summary>
    public string? EmployeeCode { get; set; }

    /// <summary>
    /// 肉类类型ID - 关联的肉类类型
    /// </summary>
    public int MeatTypeId { get; set; }

    /// <summary>
    /// 二维码内容 - 完整的二维码内容（如：PORK-001）
    /// 格式：MeatType.Code + "-" + Code
    /// 全局唯一，不可重复
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 二维码图片 - Base64编码的PNG图片
    /// 用于前端显示和打印
    /// </summary>
    public string? ImageBase64 { get; set; }

    /// <summary>
    /// 批次号 - 批量创建时的批次标识
    /// 用于追溯和管理批量创建的二维码
    /// </summary>
    public string? BatchNumber { get; set; }

    /// <summary>
    /// 打印次数 - 记录该二维码被打印的次数
    /// 用于统计和追溯
    /// </summary>
    public int PrintCount { get; set; } = 0;

    /// <summary>
    /// 最后打印时间 - 记录最后一次打印的时间
    /// </summary>
    public DateTime? LastPrintedAt { get; set; }

    /// <summary>
    /// 是否激活 - 停用的二维码不能用于新记录
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 备注说明 - 二维码的备注信息
    /// </summary>
    public string? Remarks { get; set; }

    /// <summary>
    /// 导航属性 - 关联的肉类类型
    /// </summary>
    public virtual MeatType? MeatType { get; set; }
}
