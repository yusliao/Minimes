namespace Minimes.Domain.Entities;

/// <summary>
/// 肉类类型实体 - 可配置的生肉分类管理
/// 用于管理不同种类的生肉分类（猪肉、牛肉、羊肉等）
/// </summary>
public class MeatType : BaseEntity
{
    /// <summary>
    /// 类型代码 - 唯一标识（如：PORK, BEEF, MUTTON）
    /// 用于系统内部识别，不可修改
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 类型名称 - 显示给用户的名称（如：猪肉、牛肉、羊肉）
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 显示顺序 - 用于前端下拉框排序
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// 是否激活 - 停用的类型不能用于新记录
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 描述说明 - 类型的详细说明
    /// </summary>
    public string? Description { get; set; }
}
