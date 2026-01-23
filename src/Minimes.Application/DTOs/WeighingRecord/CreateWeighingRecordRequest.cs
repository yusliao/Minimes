namespace Minimes.Application.DTOs.WeighingRecord;

/// <summary>
/// 创建称重记录请求 - 简化版：条码+重量
/// </summary>
public class CreateWeighingRecordRequest
{
    /// <summary>
    /// 条码 - 扫码枪扫描的原始值（完整二维码内容，如：PORK-001）
    /// </summary>
    public string Barcode { get; set; } = string.Empty;

    /// <summary>
    /// 用户编号 - 从二维码中拆分出的编号部分（如：001）
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 肉类类型ID - 从二维码中拆分出的肉类类型
    /// </summary>
    public int MeatTypeId { get; set; }

    /// <summary>
    /// 重量（磅）- 前端输入单位为磅，Service层会转换为千克存储
    /// </summary>
    public decimal Weight { get; set; }

    /// <summary>
    /// 工序ID - 关联ProcessStage表
    /// </summary>
    public int ProcessStageId { get; set; }

    /// <summary>
    /// 备注（如：去骨、分割、真空包装等）
    /// </summary>
    public string? Remarks { get; set; }
}
