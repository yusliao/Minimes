namespace Minimes.Domain.Entities;

/// <summary>
/// 称重记录实体 - 核心业务数据
/// 简化流程：扫码 → 称重 → 保存（条码+重量=一条记录）
/// </summary>
public class WeighingRecord : BaseEntity
{
    /// <summary>
    /// 条码 - 扫码枪扫描的原始条码值（完整二维码内容，如：PORK-001）
    /// </summary>
    public string Barcode { get; set; } = string.Empty;

    /// <summary>
    /// 用户编号 - 从二维码中拆分出的编号部分（如：001）
    /// 用于追溯和统计
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 肉类类型ID - 外键关联MeatType表
    /// 从二维码中拆分出的肉类类型
    /// </summary>
    public int MeatTypeId { get; set; }

    /// <summary>
    /// 重量（lb/磅）- 直接存储英镑数值，不做单位转换
    /// </summary>
    public decimal Weight { get; set; }

    /// <summary>
    /// 肉类类型导航属性 - 关联的肉类类型
    /// </summary>
    public MeatType MeatType { get; set; } = null!;

    /// <summary>
    /// 备注（如：去骨、分割、真空包装等）
    /// </summary>
    public string? Remarks { get; set; }

    /// <summary>
    /// 创建人
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;
}
