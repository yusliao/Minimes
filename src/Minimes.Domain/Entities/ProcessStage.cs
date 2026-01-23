namespace Minimes.Domain.Entities;

/// <summary>
/// 工序实体 - 可配置的生产工序管理
/// 替代原有的ProcessStage枚举，支持动态增删改查
/// </summary>
public class ProcessStage : BaseEntity
{
    /// <summary>
    /// 工序代码 - 唯一标识（如：RECEIVING, PROCESSING, SHIPPING）
    /// 用于系统内部识别，不可修改
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 工序名称 - 显示给用户的名称（如：原料入库、加工过程、成品出库）
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 显示顺序 - 用于前端下拉框排序
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// 是否激活 - 停用的工序不能用于新记录
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 工序类型 - 用于损耗率计算（Start=入库，End=出库，Middle=中间环节）
    /// </summary>
    public StageType StageType { get; set; }

    /// <summary>
    /// 是否计入损耗率计算
    /// </summary>
    public bool IncludeInLossRate { get; set; } = true;

    /// <summary>
    /// 描述说明 - 工序的详细说明
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 导航属性：该工序下的所有称重记录
    /// </summary>
    public ICollection<WeighingRecord> WeighingRecords { get; set; } = new List<WeighingRecord>();
}

/// <summary>
/// 工序类型枚举 - 用于损耗率计算逻辑
/// </summary>
public enum StageType
{
    /// <summary>
    /// 起始环节 - 如：原料入库（损耗率计算的分子）
    /// </summary>
    Start = 1,

    /// <summary>
    /// 中间环节 - 如：加工过程（不参与损耗率计算）
    /// </summary>
    Middle = 2,

    /// <summary>
    /// 结束环节 - 如：成品出库（损耗率计算的分母）
    /// </summary>
    End = 3
}
