namespace Minimes.Application.Interfaces;

/// <summary>
/// 二维码生成服务接口
/// </summary>
public interface IQRCodeGeneratorService
{
    /// <summary>
    /// 生成单个二维码的Base64字符串（PNG格式）
    /// </summary>
    /// <param name="content">二维码内容</param>
    /// <param name="pixelsPerModule">每个模块的像素数（默认20）</param>
    /// <returns>Base64编码的PNG图片字符串</returns>
    string GenerateQRCodeBase64(string content, int pixelsPerModule = 20);

    /// <summary>
    /// 生成单个二维码的字节数组（PNG格式）
    /// </summary>
    /// <param name="content">二维码内容</param>
    /// <param name="pixelsPerModule">每个模块的像素数（默认20）</param>
    /// <returns>PNG图片字节数组</returns>
    byte[] GenerateQRCodeBytes(string content, int pixelsPerModule = 20);

    /// <summary>
    /// 批量生成二维码的Base64字符串列表
    /// </summary>
    /// <param name="contents">二维码内容列表</param>
    /// <param name="pixelsPerModule">每个模块的像素数（默认20）</param>
    /// <returns>Base64编码的PNG图片字符串列表</returns>
    List<string> GenerateQRCodeBatchBase64(List<string> contents, int pixelsPerModule = 20);
}
