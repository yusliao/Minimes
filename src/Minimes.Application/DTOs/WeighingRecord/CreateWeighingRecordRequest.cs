using Minimes.Domain.Enums;

namespace Minimes.Application.DTOs.WeighingRecord;

/// <summary>
/// 创建称重记录请求 - 简化版：条码+重量
/// </summary>
public class CreateWeighingRecordRequest
{
    /// <summary>
    /// 条码 - 扫码枪扫描的原始值
    /// </summary>
    public string Barcode { get; set; } = string.Empty;

    /// <summary>
    /// 重量（克）
    /// </summary>
    public decimal Weight { get; set; }

    /// <summary>
    /// 加工环节（入库/加工/出库）
    /// </summary>
    public ProcessStage ProcessStage { get; set; }

    /// <summary>
    /// 备注（如：去骨、分割、真空包装等）
    /// </summary>
    public string? Remarks { get; set; }
}
