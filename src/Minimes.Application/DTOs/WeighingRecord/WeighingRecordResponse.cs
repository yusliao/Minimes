namespace Minimes.Application.DTOs.WeighingRecord;

/// <summary>
/// 称重记录响应 - 简化版
/// </summary>
public class WeighingRecordResponse
{
    public int Id { get; set; }
    public string Barcode { get; set; } = string.Empty;

    /// <summary>
    /// 用户编号 - 从二维码中拆分出的编号部分
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 肉类类型ID
    /// </summary>
    public int MeatTypeId { get; set; }

    /// <summary>
    /// 肉类类型名称
    /// </summary>
    public string MeatTypeName { get; set; } = string.Empty;

    /// <summary>
    /// 重量（千克）- 数据库存储单位
    /// </summary>
    public decimal Weight { get; set; }

    /// <summary>
    /// 重量（磅）- 前端显示单位
    /// </summary>
    public decimal WeightInPounds => Weight / 0.45359237m;

    /// <summary>
    /// 工序ID
    /// </summary>
    public int ProcessStageId { get; set; }

    /// <summary>
    /// 工序代码
    /// </summary>
    public string ProcessStageCode { get; set; } = string.Empty;

    /// <summary>
    /// 工序名称
    /// </summary>
    public string ProcessStageName { get; set; } = string.Empty;

    public string? Remarks { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}
