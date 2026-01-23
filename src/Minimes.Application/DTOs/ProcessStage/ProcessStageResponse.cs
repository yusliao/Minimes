namespace Minimes.Application.DTOs.ProcessStage;

/// <summary>
/// 工序响应DTO - 返回给前端的工序数据
/// </summary>
public class ProcessStageResponse
{
    /// <summary>
    /// 工序ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 工序代码（唯一标识）
    /// </summary>
    public string Code { get; set; } = string.Empty;

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
    public string StageType { get; set; } = string.Empty;

    /// <summary>
    /// 描述说明
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 该工序下的记录数量
    /// </summary>
    public int RecordCount { get; set; }
}
