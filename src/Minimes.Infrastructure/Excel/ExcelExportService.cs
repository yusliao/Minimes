using Microsoft.Extensions.Logging;
using Minimes.Application.DTOs.QRCode;
using Minimes.Application.DTOs.Report;
using Minimes.Application.DTOs.WeighingRecord;
using Minimes.Application.Interfaces;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace Minimes.Infrastructure.Excel;

/// <summary>
/// Excel导出服务 - 简化版
/// </summary>
public class ExcelExportService : IExcelExportService
{
    private readonly ILogger<ExcelExportService> _logger;

    public ExcelExportService(ILogger<ExcelExportService> logger)
    {
        _logger = logger;
    }

    public async Task<byte[]> ExportWeighingRecordsAsync(List<WeighingRecordResponse> records)
    {
        return await Task.Run(() =>
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("称重记录");

            var headers = new[] { "记录ID", "条码", "肉类类型", "编号", "加工环节", "重量(lb)", "备注", "创建时间", "创建人" };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
            }

            using (var range = worksheet.Cells[1, 1, 1, headers.Length])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            int row = 2;
            foreach (var record in records)
            {
                worksheet.Cells[row, 1].Value = record.Id;
                worksheet.Cells[row, 2].Value = record.Barcode;
                worksheet.Cells[row, 3].Value = record.MeatTypeName;
                worksheet.Cells[row, 4].Value = record.Code;
                worksheet.Cells[row, 5].Value = record.ProcessStageName;
                worksheet.Cells[row, 6].Value = record.WeightInPounds;
                worksheet.Cells[row, 7].Value = record.Remarks ?? "";
                worksheet.Cells[row, 8].Value = record.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
                worksheet.Cells[row, 9].Value = record.CreatedBy;
                row++;
            }

            worksheet.Cells.AutoFitColumns();
            _logger.LogInformation("导出称重记录: 记录数={RecordCount}", records.Count);
            return package.GetAsByteArray();
        });
    }

    public async Task<byte[]> ExportProductionReportAsync(ProductionReportResponse report, List<ProductLossRateResponse> lossRates)
    {
        return await Task.Run(() =>
        {
            using var package = new ExcelPackage();
            var summarySheet = package.Workbook.Worksheets.Add("生产报表汇总");

            summarySheet.Cells["A1"].Value = "生产报表汇总";
            summarySheet.Cells["A1:B1"].Merge = true;
            summarySheet.Cells["A1"].Style.Font.Size = 16;
            summarySheet.Cells["A1"].Style.Font.Bold = true;

            int row = 3;
            summarySheet.Cells[row, 1].Value = "总记录数";
            summarySheet.Cells[row++, 2].Value = report.TotalRecords;
            summarySheet.Cells[row, 1].Value = "入库总重量(lb)";
            summarySheet.Cells[row++, 2].Value = (report.ReceivingWeight / 0.45359237m).ToString("F3");
            summarySheet.Cells[row, 1].Value = "加工总重量(lb)";
            summarySheet.Cells[row++, 2].Value = (report.ProcessingWeight / 0.45359237m).ToString("F3");
            summarySheet.Cells[row, 1].Value = "出库总重量(lb)";
            summarySheet.Cells[row++, 2].Value = (report.ShippingWeight / 0.45359237m).ToString("F3");
            summarySheet.Cells[row, 1].Value = "涉及条码数";
            summarySheet.Cells[row, 2].Value = report.UniqueBarcodes;

            using (var range = summarySheet.Cells[$"A3:A{row}"])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            }
            summarySheet.Cells.AutoFitColumns();

            // 损耗率统计表
            if (lossRates != null && lossRates.Any())
            {
                AddLossRateSheet(package, lossRates);
            }

            _logger.LogInformation("导出生产报表成功");
            return package.GetAsByteArray();
        });
    }

    public async Task<byte[]> ExportProductLossRateAsync(List<ProductLossRateResponse> lossRates)
    {
        return await Task.Run(() =>
        {
            using var package = new ExcelPackage();
            AddLossRateSheet(package, lossRates);
            _logger.LogInformation("导出损耗率统计: 条码数={Count}", lossRates.Count);
            return package.GetAsByteArray();
        });
    }

    private void AddLossRateSheet(ExcelPackage package, List<ProductLossRateResponse> lossRates)
    {
        var worksheet = package.Workbook.Worksheets.Add("条码损耗率统计");

        var headers = new[] { "条码", "入库重量(lb)", "加工重量(lb)", "出库重量(lb)",
            "损耗重量(lb)", "损耗率(%)", "入库记录数", "加工记录数", "出库记录数" };

        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cells[1, i + 1].Value = headers[i];
        }

        using (var range = worksheet.Cells[1, 1, 1, headers.Length])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.Orange);
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        int row = 2;
        foreach (var item in lossRates.OrderByDescending(l => l.LossRate))
        {
            worksheet.Cells[row, 1].Value = item.Barcode;
            worksheet.Cells[row, 2].Value = (item.ReceivingWeight / 0.45359237m).ToString("F3");
            worksheet.Cells[row, 3].Value = (item.ProcessingWeight / 0.45359237m).ToString("F3");
            worksheet.Cells[row, 4].Value = (item.ShippingWeight / 0.45359237m).ToString("F3");
            worksheet.Cells[row, 5].Value = (item.LossWeight / 0.45359237m).ToString("F3");
            worksheet.Cells[row, 6].Value = item.LossRate;
            worksheet.Cells[row, 7].Value = item.ReceivingRecords;
            worksheet.Cells[row, 8].Value = item.ProcessingRecords;
            worksheet.Cells[row, 9].Value = item.ShippingRecords;

            // 损耗率颜色标记
            if (item.LossRate > 20)
            {
                worksheet.Cells[row, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, 6].Style.Fill.BackgroundColor.SetColor(Color.Red);
                worksheet.Cells[row, 6].Style.Font.Color.SetColor(Color.White);
            }
            else if (item.LossRate > 10)
            {
                worksheet.Cells[row, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row, 6].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
            }
            row++;
        }

        worksheet.Cells.AutoFitColumns();
    }

    public async Task<byte[]> ExportQRCodesAsync(List<QRCodeResponse> qrCodes)
    {
        return await Task.Run(() =>
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("二维码列表");

            var headers = new[] { "ID", "编号", "肉类类型", "二维码内容", "二维码图片", "批次号", "打印次数", "状态", "创建时间" };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
            }

            using (var range = worksheet.Cells[1, 1, 1, headers.Length])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            int row = 2;
            foreach (var qrCode in qrCodes)
            {
                worksheet.Cells[row, 1].Value = qrCode.Id;
                worksheet.Cells[row, 2].Value = qrCode.Code;
                worksheet.Cells[row, 3].Value = qrCode.MeatTypeName;
                worksheet.Cells[row, 4].Value = qrCode.Content;
                // 第5列留给二维码图片
                worksheet.Cells[row, 6].Value = qrCode.BatchNumber ?? "";
                worksheet.Cells[row, 7].Value = qrCode.PrintCount;
                worksheet.Cells[row, 8].Value = qrCode.IsActive ? "激活" : "未激活";
                worksheet.Cells[row, 9].Value = qrCode.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");

                // 嵌入二维码图片
                if (!string.IsNullOrEmpty(qrCode.ImageBase64))
                {
                    try
                    {
                        var imageBytes = Convert.FromBase64String(qrCode.ImageBase64);
                        using var ms = new MemoryStream(imageBytes);
                        var picture = worksheet.Drawings.AddPicture($"QRCode_{qrCode.Id}", ms);

                        // 设置图片位置（行索引从0开始，所以row-1）
                        picture.SetPosition(row - 1, 5, 4, 5);

                        // 设置图片大小（80x80像素）
                        picture.SetSize(80, 80);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("嵌入二维码图片失败: ID={Id}, Error={Error}", qrCode.Id, ex.Message);
                        worksheet.Cells[row, 5].Value = "图片加载失败";
                    }
                }

                // 设置行高以容纳图片（单位：磅，1磅≈1.33像素）
                worksheet.Row(row).Height = 60;

                row++;
            }

            // 设置列宽
            worksheet.Column(1).Width = 8;   // ID
            worksheet.Column(2).Width = 15;  // 编号
            worksheet.Column(3).Width = 12;  // 肉类类型
            worksheet.Column(4).Width = 20;  // 二维码内容
            worksheet.Column(5).Width = 12;  // 二维码图片
            worksheet.Column(6).Width = 15;  // 批次号
            worksheet.Column(7).Width = 10;  // 打印次数
            worksheet.Column(8).Width = 10;  // 状态
            worksheet.Column(9).Width = 20;  // 创建时间

            _logger.LogInformation("导出二维码列表: 记录数={Count}", qrCodes.Count);
            return package.GetAsByteArray();
        });
    }
}
