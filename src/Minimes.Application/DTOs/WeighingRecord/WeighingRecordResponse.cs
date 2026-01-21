using Minimes.Domain.Enums;

namespace Minimes.Application.DTOs.WeighingRecord;

/// <summary>
/// 称重记录响应 - 简化版
/// </summary>
public class WeighingRecordResponse
{
    public int Id { get; set; }
    public string Barcode { get; set; } = string.Empty;
    public decimal Weight { get; set; }
    public ProcessStage ProcessStage { get; set; }
    public string ProcessStageName { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}
