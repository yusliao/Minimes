using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Minimes.Application.Interfaces;
using Minimes.Domain.Interfaces;

namespace Minimes.Infrastructure.Hardware;

/// <summary>
/// 扫码枪服务 - 键盘输入模拟监听
/// 注意：实际的键盘监听需要在Blazor前端通过JSInterop实现
/// 这里提供基础事件管理和手动触发功能
/// </summary>
public class BarcodeScannerService : IBarcodeScannerService
{
    private readonly ILogger<BarcodeScannerService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private bool _isListening;

    public bool IsListening => _isListening;

    public event EventHandler<BarcodeScannedEventArgs>? BarcodeScanned;

    public BarcodeScannerService(ILogger<BarcodeScannerService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public void StartListening()
    {
        if (_isListening)
        {
            _logger.LogWarning("扫码枪已在监听中");
            return;
        }

        _isListening = true;
        _logger.LogInformation("扫码枪监听已启动");
    }

    public void StopListening()
    {
        if (!_isListening)
        {
            _logger.LogWarning("扫码枪未在监听中");
            return;
        }

        _isListening = false;
        _logger.LogInformation("扫码枪监听已停止");
    }

    public void SimulateScan(string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
        {
            _logger.LogWarning("条码为空，忽略扫描");
            return;
        }

        _logger.LogInformation("模拟扫码: {Barcode}", barcode);
        OnBarcodeScanned(barcode, "Simulated");
    }

    /// <summary>
    /// 处理从前端JSInterop接收的扫码数据
    /// 这个方法会被Blazor组件调用
    /// </summary>
    public void ProcessBarcodeInput(string barcode)
    {
        if (!_isListening)
        {
            _logger.LogDebug("扫码枪未在监听状态，忽略输入: {Barcode}", barcode);
            return;
        }

        if (string.IsNullOrWhiteSpace(barcode))
        {
            _logger.LogWarning("收到空条码输入");
            return;
        }

        _logger.LogInformation("扫码成功: {Barcode}", barcode);
        OnBarcodeScanned(barcode, "Physical");
    }

    private void OnBarcodeScanned(string barcode, string scannerType)
    {
        try
        {
            BarcodeScanned?.Invoke(this, new BarcodeScannedEventArgs
            {
                Barcode = barcode,
                ScannerType = scannerType,
                Timestamp = DateTime.Now
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "触发BarcodeScanned事件时发生错误");
        }
    }
}
