namespace Minimes.Application.DTOs.ProcessStage;

/// <summary>
/// 更新工序请求DTO
/// </summary>
public class UpdateProcessStageRequest
{
    /// <summary>
    /// 工序名称
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
    /// 是否计入损耗率
    /// </summary>
    public bool IncludeInLossRate { get; set; }

    /// <summary>
    /// 工序类型（Start/Middle/End）
    /// </summary>
    public string StageType { get; set; } = "Middle";

    /// <summary>
    /// 描述说明
    /// </summary>
    public string? Description { get; set; }
}
