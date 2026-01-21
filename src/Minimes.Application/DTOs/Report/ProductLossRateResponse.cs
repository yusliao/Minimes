namespace Minimes.Application.DTOs.Report;

/// <summary>
/// 条码损耗率统计响应 - 简化版
/// </summary>
public class ProductLossRateResponse
{
    /// <summary>
    /// 条码
    /// </summary>
    public string Barcode { get; set; } = string.Empty;

    /// <summary>
    /// 入库总重量（克）
    /// </summary>
    public decimal ReceivingWeight { get; set; }

    /// <summary>
    /// 加工过程总重量（克）
    /// </summary>
    public decimal ProcessingWeight { get; set; }

    /// <summary>
    /// 出库总重量（克）
    /// </summary>
    public decimal ShippingWeight { get; set; }

    /// <summary>
    /// 损耗重量（克）= 入库 - 出库
    /// </summary>
    public decimal LossWeight { get; set; }

    /// <summary>
    /// 损耗率（%）= (入库 - 出库) / 入库 * 100
    /// </summary>
    public decimal LossRate { get; set; }

    /// <summary>
    /// 入库记录数
    /// </summary>
    public int ReceivingRecords { get; set; }

    /// <summary>
    /// 加工记录数
    /// </summary>
    public int ProcessingRecords { get; set; }

    /// <summary>
    /// 出库记录数
    /// </summary>
    public int ShippingRecords { get; set; }
}
