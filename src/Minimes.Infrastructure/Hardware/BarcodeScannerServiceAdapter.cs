namespace Minimes.Infrastructure.Hardware;

using Microsoft.Extensions.Logging;
using Minimes.Application.Interfaces;
using Minimes.Infrastructure.Devices.Abstractions;
using Minimes.Infrastructure.Devices.Drivers;
using Minimes.Infrastructure.Devices.Models.Data;
using Minimes.Infrastructure.Devices.Models.EventArgs;

/// <summary>
/// 条码扫描枪服务适配器 - 向后兼容旧接口
///
/// 设计说明：
/// 1. 实现IBarcodeScannerService接口（保留旧接口）
/// 2. 内部使用BarcodeScannerAdapter（新框架）
/// 3. 将旧接口方法映射到新设备接口
/// 4. 确保不影响现有代码
/// </summary>
public class BarcodeScannerServiceAdapter : IBarcodeScannerService
{
    private readonly ILogger<BarcodeScannerServiceAdapter> _logger;
    private readonly BarcodeScannerAdapter _adapter;

    /// <inheritdoc/>
    public bool IsListening => _adapter.IsRunning;

    /// <inheritdoc/>
    public event EventHandler<BarcodeScannedEventArgs>? BarcodeScanned;

    public BarcodeScannerServiceAdapter(
        ILogger<BarcodeScannerServiceAdapter> logger,
        BarcodeScannerAdapter adapter)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));

        // 订阅设备事件
        _adapter.DataReceived += OnDeviceDataReceived;
        _adapter.ErrorOccurred += OnDeviceErrorOccurred;

        _logger.LogInformation("条码扫描枪服务适配器已创建");
    }

    /// <inheritdoc/>
    public void StartListening()
    {
        _logger.LogInformation("启动扫码枪监听");

        // 确保设备已连接
        if (!_adapter.IsConnected)
        {
            _ = _adapter.ConnectAsync();
        }

        // 启动数据采集
        _ = _adapter.StartAsync();
    }

    /// <inheritdoc/>
    public void StopListening()
    {
        _logger.LogInformation("停止扫码枪监听");
        _ = _adapter.StopAsync();
    }

    /// <inheritdoc/>
    public void SimulateScan(string barcode)
    {
        _logger.LogInformation("模拟扫码: {Barcode}", barcode);
        _adapter.SimulateScan(barcode);
    }

    /// <summary>
    /// 处理来自前端JSInterop的扫码输入
    /// 注意：这个方法不在IBarcodeScannerService接口中，但现有代码可能会调用
    /// </summary>
    public void ProcessBarcodeInput(string barcode)
    {
        _logger.LogInformation("处理扫码输入: {Barcode}", barcode);
        _adapter.ProcessBarcodeInput(barcode);
    }

    private void OnDeviceDataReceived(object? sender, DeviceDataEventArgs<BarcodeData> e)
    {
        // 转换为旧事件格式并触发
        BarcodeScanned?.Invoke(this, new BarcodeScannedEventArgs
        {
            Barcode = e.Data.Barcode,
            ScannerType = ConvertScannerTypeToString(e.Data.ScannerType),
            Timestamp = e.Timestamp
        });
    }

    /// <summary>
    /// 将枚举类型的ScannerType转换为字符串（向后兼容）
    /// </summary>
    private string ConvertScannerTypeToString(Devices.Models.Data.ScannerType scannerType)
    {
        return scannerType switch
        {
            Devices.Models.Data.ScannerType.Handheld => "Physical",
            Devices.Models.Data.ScannerType.Keyboard => "Physical",
            Devices.Models.Data.ScannerType.Fixed => "Physical",
            Devices.Models.Data.ScannerType.Camera => "Physical",
            _ => "Simulated"
        };
    }

    private void OnDeviceErrorOccurred(object? sender, DeviceErrorEventArgs e)
    {
        _logger.LogError(e.Exception, "扫码枪发生错误: {Message}", e.Message);
    }
}
