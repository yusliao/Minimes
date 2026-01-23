using QRCoder;
using Minimes.Application.Interfaces;

namespace Minimes.Infrastructure.QRCodeGenerator;

/// <summary>
/// 二维码生成服务实现
/// </summary>
public class QRCodeGeneratorService : IQRCodeGeneratorService
{
    /// <summary>
    /// 生成单个二维码的Base64字符串（PNG格式）
    /// </summary>
    public string GenerateQRCodeBase64(string content, int pixelsPerModule = 20)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("二维码内容不能为空", nameof(content));
        }

        if (pixelsPerModule <= 0)
        {
            throw new ArgumentException("像素数必须大于0", nameof(pixelsPerModule));
        }

        try
        {
            var bytes = GenerateQRCodeBytes(content, pixelsPerModule);
            return Convert.ToBase64String(bytes);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"生成二维码失败: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// 生成单个二维码的字节数组（PNG格式）
    /// </summary>
    public byte[] GenerateQRCodeBytes(string content, int pixelsPerModule = 20)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("二维码内容不能为空", nameof(content));
        }

        if (pixelsPerModule <= 0)
        {
            throw new ArgumentException("像素数必须大于0", nameof(pixelsPerModule));
        }

        try
        {
            using var qrGenerator = new QRCoder.QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(content, QRCoder.QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new QRCoder.PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(pixelsPerModule);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"生成二维码失败: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// 批量生成二维码的Base64字符串列表
    /// </summary>
    public List<string> GenerateQRCodeBatchBase64(List<string> contents, int pixelsPerModule = 20)
    {
        if (contents == null || contents.Count == 0)
        {
            throw new ArgumentException("二维码内容列表不能为空", nameof(contents));
        }

        if (pixelsPerModule <= 0)
        {
            throw new ArgumentException("像素数必须大于0", nameof(pixelsPerModule));
        }

        var result = new List<string>();

        foreach (var content in contents)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException($"二维码内容不能为空: {content}");
            }

            try
            {
                var base64 = GenerateQRCodeBase64(content, pixelsPerModule);
                result.Add(base64);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"批量生成二维码失败，内容: {content}, 错误: {ex.Message}", ex);
            }
        }

        return result;
    }
}
