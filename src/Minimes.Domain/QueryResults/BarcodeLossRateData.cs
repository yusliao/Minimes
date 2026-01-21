namespace Minimes.Domain.QueryResults;

/// <summary>
/// 条码损耗率数据 - 用于数据库聚合查询
/// </summary>
public class BarcodeLossRateData
{
    /// <summary>
    /// 条码
    /// </summary>
    public string Barcode { get; set; } = string.Empty;

    /// <summary>
    /// 原料入库总重量（克）
    /// </summary>
    public decimal ReceivingWeight { get; set; }

    /// <summary>
    /// 加工过程总重量（克）
    /// </summary>
    public decimal ProcessingWeight { get; set; }

    /// <summary>
    /// 成品出库总重量（克）
    /// </summary>
    public decimal ShippingWeight { get; set; }

    /// <summary>
    /// 损耗重量（克）= 入库 - 出库
    /// </summary>
    public decimal LossWeight { get; set; }

    /// <summary>
    /// 损耗率（%）= (损耗重量 / 入库重量) * 100
    /// </summary>
    public decimal LossRate { get; set; }

    /// <summary>
    /// 原料入库记录数
    /// </summary>
    public int ReceivingRecords { get; set; }

    /// <summary>
    /// 加工过程记录数
    /// </summary>
    public int ProcessingRecords { get; set; }

    /// <summary>
    /// 成品出库记录数
    /// </summary>
    public int ShippingRecords { get; set; }
}
