namespace Minimes.Domain.Entities;

/// <summary>
/// 基础实体类 - 所有实体的通用属性
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// 主键ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 最后更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
