using Minimes.Application.DTOs.QRCode;
using Minimes.Application.DTOs.Report;
using Minimes.Application.DTOs.WeighingRecord;

namespace Minimes.Application.Interfaces;

/// <summary>
/// Excel导出服务接口 - 导出各类报表和记录
/// </summary>
public interface IExcelExportService
{
    /// <summary>
    /// 导出称重记录到Excel
    /// </summary>
    /// <param name="records">称重记录列表</param>
    /// <returns>Excel文件的字节数组</returns>
    Task<byte[]> ExportWeighingRecordsAsync(List<WeighingRecordResponse> records);

    /// <summary>
    /// 导出生产报表到Excel
    /// </summary>
    /// <param name="report">生产报表统计</param>
    /// <param name="lossRates">商品损耗率统计</param>
    /// <returns>Excel文件的字节数组</returns>
    Task<byte[]> ExportProductionReportAsync(ProductionReportResponse report, List<ProductLossRateResponse> lossRates);

    /// <summary>
    /// 导出商品损耗率统计到Excel
    /// </summary>
    /// <param name="lossRates">商品损耗率统计列表</param>
    /// <returns>Excel文件的字节数组</returns>
    Task<byte[]> ExportProductLossRateAsync(List<ProductLossRateResponse> lossRates);

    /// <summary>
    /// 导出二维码列表到Excel（包含二维码图片）
    /// </summary>
    /// <param name="qrCodes">二维码列表</param>
    /// <returns>Excel文件的字节数组</returns>
    Task<byte[]> ExportQRCodesAsync(List<QRCodeResponse> qrCodes);
}
