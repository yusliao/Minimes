namespace Minimes.Infrastructure.Devices.Models.Data;

/// <summary>
/// 条码类型枚举
/// </summary>
public enum BarcodeType
{
    /// <summary>未知</summary>
    Unknown = 0,

    /// <summary>EAN-13</summary>
    EAN13 = 1,

    /// <summary>EAN-8</summary>
    EAN8 = 2,

    /// <summary>UPC-A</summary>
    UPCA = 3,

    /// <summary>UPC-E</summary>
    UPCE = 4,

    /// <summary>Code 128</summary>
    Code128 = 5,

    /// <summary>Code 39</summary>
    Code39 = 6,

    /// <summary>QR码</summary>
    QRCode = 7
}

/// <summary>
/// 扫描方式枚举
/// </summary>
public enum ScannerType
{
    /// <summary>未知</summary>
    Unknown = 0,

    /// <summary>手持扫码枪</summary>
    Handheld = 1,

    /// <summary>固定式扫描器</summary>
    Fixed = 2,

    /// <summary>键盘输入模拟</summary>
    Keyboard = 3,

    /// <summary>摄像头扫描</summary>
    Camera = 4
}

/// <summary>
/// 条码数据
/// </summary>
public class BarcodeData
{
    /// <summary>条码内容</summary>
    public string Barcode { get; set; } = string.Empty;

    /// <summary>条码类型</summary>
    public BarcodeType BarcodeType { get; set; } = BarcodeType.Unknown;

    /// <summary>扫描方式</summary>
    public ScannerType ScannerType { get; set; } = ScannerType.Unknown;

    /// <summary>时间戳</summary>
    public DateTime Timestamp { get; set; } = DateTime.Now;

    public override string ToString()
    {
        return $"{Barcode} ({BarcodeType})";
    }
}
