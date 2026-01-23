namespace Minimes.Application.DTOs.QRCode;

/// <summary>
/// 更新二维码请求DTO
/// </summary>
public class UpdateQRCodeRequest
{
    /// <summary>
    /// 用户编号（只读显示，不可修改）
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 是否激活
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 备注说明（可选）
    /// </summary>
    public string? Remarks { get; set; }
}
