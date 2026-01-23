namespace Minimes.Application.DTOs.MeatType;

/// <summary>
/// 创建肉类类型请求DTO
/// </summary>
public class CreateMeatTypeRequest
{
    /// <summary>
    /// 类型代码（唯一标识，如：PORK, BEEF, MUTTON）
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 类型名称（如：猪肉、牛肉、羊肉）
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 显示顺序
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// 是否激活
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 描述说明
    /// </summary>
    public string? Description { get; set; }
}
