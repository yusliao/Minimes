using Minimes.Domain.Enums;

namespace Minimes.Domain.Entities;

/// <summary>
/// 称重记录实体 - 核心业务数据
/// 简化流程：扫码 → 称重 → 保存（条码+重量=一条记录）
/// </summary>
public class WeighingRecord : BaseEntity
{
    /// <summary>
    /// 条码 - 扫码枪扫描的原始条码值
    /// </summary>
    public string Barcode { get; set; } = string.Empty;

    /// <summary>
    /// 重量（kg）
    /// </summary>
    public decimal Weight { get; set; }

    /// <summary>
    /// 加工环节 - 区分称重发生在哪个生产阶段（入库/加工/出库）
    /// </summary>
    public ProcessStage ProcessStage { get; set; }

    /// <summary>
    /// 备注（如：去骨、分割、真空包装等）
    /// </summary>
    public string? Remarks { get; set; }

    /// <summary>
    /// 创建人
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;
}
