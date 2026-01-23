namespace Minimes.Application.DTOs.MeatType;

/// <summary>
/// 更新肉类类型请求DTO
/// </summary>
public class UpdateMeatTypeRequest
{
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
}
