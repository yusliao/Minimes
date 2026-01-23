namespace Minimes.Application.DTOs.ProcessStage;

/// <summary>
/// 创建工序请求DTO
/// </summary>
public class CreateProcessStageRequest
{
    /// <summary>
    /// 工序代码（唯一标识，如：RECEIVING, PROCESSING, SHIPPING）
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 工序名称（如：原料入库、加工过程、成品出库）
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 显示顺序
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// 是否计入损耗率计算
    /// </summary>
    public bool IncludeInLossRate { get; set; } = true;

    /// <summary>
    /// 工序类型（Start/Middle/End）
    /// </summary>
    public string StageType { get; set; } = "Middle";

    /// <summary>
    /// 描述说明
    /// </summary>
    public string? Description { get; set; }
}
