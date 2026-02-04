namespace Minimes.Application.Interfaces;

/// <summary>
/// 扫码枪服务接口 - 键盘输入模拟监听
/// </summary>
public interface IBarcodeScannerService
{
    /// <summary>
    /// 是否正在监听
    /// </summary>
    bool IsListening { get; }

    /// <summary>
    /// 扫码完成事件（接收到完整条码）
    /// </summary>
    event EventHandler<BarcodeScannedEventArgs>? BarcodeScanned;

    /// <summary>
    /// 开始监听扫码枪输入
    /// </summary>
    void StartListening();

    /// <summary>
    /// 停止监听扫码枪输入
    /// </summary>
    void StopListening();

    /// <summary>
    /// 手动触发扫码（用于测试）
    /// </summary>
    void SimulateScan(string barcode);
}

/// <summary>
/// 条码扫描事件参数
/// </summary>
public class BarcodeScannedEventArgs : EventArgs
{
    public string Barcode { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string ScannerType { get; set; } = "Physical"; // Physical or Simulated
}
