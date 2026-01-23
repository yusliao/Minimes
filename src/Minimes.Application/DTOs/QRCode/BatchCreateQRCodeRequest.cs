namespace Minimes.Application.DTOs.QRCode;

/// <summary>
/// 批量创建二维码请求DTO
/// </summary>
public class BatchCreateQRCodeRequest
{
    /// <summary>
    /// 肉类类型ID
    /// </summary>
    public int MeatTypeId { get; set; }

    /// <summary>
    /// 起始编号（如：1）
    /// </summary>
    public int StartNumber { get; set; }

    /// <summary>
    /// 结束编号（如：100）
    /// </summary>
    public int EndNumber { get; set; }

    /// <summary>
    /// 前缀（可选，如：A、B）
    /// 最终编号格式：前缀 + 补零后的数字（如：A001、B001）
    /// </summary>
    public string? Prefix { get; set; }

    /// <summary>
    /// 补零位数（默认3，如：001、002）
    /// </summary>
    public int PaddingLength { get; set; } = 3;

    /// <summary>
    /// 批次号（可选）
    /// 用于标识批量创建的批次
    /// </summary>
    public string? BatchNumber { get; set; }
}
