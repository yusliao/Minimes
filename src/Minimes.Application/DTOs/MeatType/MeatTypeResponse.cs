namespace Minimes.Application.DTOs.MeatType;

/// <summary>
/// 肉类类型响应DTO - 返回给前端的肉类类型数据
/// </summary>
public class MeatTypeResponse
{
    /// <summary>
    /// 类型ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 类型代码（唯一标识）
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 类型名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 显示顺序
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// 是否激活
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// 描述说明
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 该类型下的记录数量（预留，未来关联Product/WeighingRecord时启用）
    /// </summary>
    public int RecordCount { get; set; }
}
