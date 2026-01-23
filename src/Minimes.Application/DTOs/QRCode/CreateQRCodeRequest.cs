namespace Minimes.Application.DTOs.QRCode;

/// <summary>
/// 创建二维码请求DTO
/// </summary>
public class CreateQRCodeRequest
{
    /// <summary>
    /// 用户编号（如：001、A01）
    /// 与肉类类型代码组合生成完整的二维码内容
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 肉类类型ID
    /// </summary>
    public int MeatTypeId { get; set; }

    /// <summary>
    /// 批次号（可选）
    /// 用于批量创建时的批次标识
    /// </summary>
    public string? BatchNumber { get; set; }

    /// <summary>
    /// 备注说明（可选）
    /// </summary>
    public string? Remarks { get; set; }
}
