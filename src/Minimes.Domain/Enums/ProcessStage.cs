namespace Minimes.Domain.Enums;

/// <summary>
/// 加工环节 - 用于区分称重发生在哪个生产阶段
/// </summary>
public enum ProcessStage
{
    /// <summary>
    /// 原料入库 - 供应商送货，原料入库称重
    /// </summary>
    Receiving = 1,

    /// <summary>
    /// 加工过程 - 分割、去骨、腌制等加工环节称重
    /// </summary>
    Processing = 2,

    /// <summary>
    /// 成品出库 - 最终成品包装后出库称重
    /// </summary>
    Shipping = 3
}
