namespace Minimes.Application.DTOs.QRCode;

/// <summary>
/// 二维码响应DTO - 返回给前端的二维码数据
/// </summary>
public class QRCodeResponse
{
    /// <summary>
    /// 二维码ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 用户编号
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
    /// 肉类类型代码
    /// </summary>
    public string MeatTypeCode { get; set; } = string.Empty;

    /// <summary>
    /// 二维码内容（完整内容，如：PORK-001）
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 二维码图片（Base64编码的PNG图片）
    /// </summary>
    public string? ImageBase64 { get; set; }

    /// <summary>
    /// 批次号
    /// </summary>
    public string? BatchNumber { get; set; }

    /// <summary>
    /// 打印次数
    /// </summary>
    public int PrintCount { get; set; }

    /// <summary>
    /// 最后打印时间
    /// </summary>
    public DateTime? LastPrintedAt { get; set; }

    /// <summary>
    /// 是否激活
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// 备注说明
    /// </summary>
    public string? Remarks { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
