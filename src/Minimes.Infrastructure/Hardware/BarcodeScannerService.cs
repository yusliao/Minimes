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

    // 演示模式相关字段
    private bool _isDemoMode;
    private CancellationTokenSource? _demoCts;
    private Task? _demoTask;
    private List<string> _demoBarcodes = new();
    private readonly Random _random = new();

    public bool IsListening => _isListening;
    public bool IsDemoMode => _isDemoMode;

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
    /// 设置演示模式 - 自动模拟扫码
    /// </summary>
    public void SetDemoMode(bool enabled)
    {
        if (_isDemoMode == enabled)
        {
            return;
        }

        _isDemoMode = enabled;

        if (enabled)
        {
            _logger.LogInformation("扫码枪进入演示模式");
            // 加载商品条码列表（异步调用，不等待）
            _ = LoadDemoBarcodesAsync();
            // 启动模拟扫码
            StartDemoScanLoop();
        }
        else
        {
            _logger.LogInformation("扫码枪退出演示模式");
            StopDemoScanLoop();
            _demoBarcodes.Clear();
        }
    }

    private async Task LoadDemoBarcodesAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var productRepo = scope.ServiceProvider.GetRequiredService<IProductRepository>();
            var products = await productRepo.GetAllAsync();  // 异步调用，避免死锁
            _demoBarcodes = products.Where(p => p.IsActive).Select(p => p.Barcode).ToList();
            _logger.LogInformation("演示模式加载了 {Count} 个商品条码", _demoBarcodes.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "加载演示条码失败");
            // 使用默认条码
            _demoBarcodes = new List<string> { "PORK2026010701", "BEEF2026010701", "CHICKEN2026010701" };
        }
    }

    private void StartDemoScanLoop()
    {
        if (_demoTask != null)
        {
            return;
        }

        _demoCts = new CancellationTokenSource();
        _demoTask = Task.Run(() => DemoScanLoop(_demoCts.Token), _demoCts.Token);
        _logger.LogInformation("开始模拟扫码推送");
    }

    private void StopDemoScanLoop()
    {
        try
        {
            _demoCts?.Cancel();
            _demoTask?.Wait(TimeSpan.FromSeconds(2));
            _demoCts?.Dispose();
            _demoCts = null;
            _demoTask = null;
            _logger.LogInformation("停止模拟扫码推送");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "停止模拟扫码推送时发生错误");
        }
    }

    /// <summary>
    /// 演示模式扫码循环 - 随机间隔推送商品条码
    /// </summary>
    private async Task DemoScanLoop(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                // 随机等待3-8秒后扫码（模拟操作员扫码间隔）
                var delay = _random.Next(3000, 8000);
                await Task.Delay(delay, cancellationToken);

                if (_demoBarcodes.Count > 0)
                {
                    // 随机选择一个条码
                    var barcode = _demoBarcodes[_random.Next(_demoBarcodes.Count)];
                    _logger.LogDebug("演示模式：模拟扫码 {Barcode}", barcode);
                    OnBarcodeScanned(barcode, "Demo");
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "演示模式扫码发生错误");
                await Task.Delay(1000, cancellationToken);
            }
        }
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
