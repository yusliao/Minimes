using Microsoft.AspNetCore.SignalR;
using Minimes.Application.Interfaces;
using Minimes.Infrastructure.Devices.Abstractions;
using Minimes.Infrastructure.Devices.Models.EventArgs;
using Minimes.Web.Hubs;

namespace Minimes.Web.Services;

/// <summary>
/// 硬件后台服务 - 监听硬件事件并通过SignalR推送
/// </summary>
public class HardwareBackgroundService : BackgroundService
{
    private readonly ILogger<HardwareBackgroundService> _logger;
    private readonly IScaleService _scaleService;
    private readonly IBarcodeScannerService _scannerService;
    private readonly IHubContext<HardwareHub> _hubContext;
    private readonly IDeviceManager? _deviceManager;

    public HardwareBackgroundService(
        ILogger<HardwareBackgroundService> logger,
        IScaleService scaleService,
        IBarcodeScannerService scannerService,
        IHubContext<HardwareHub> hubContext,
        IDeviceManager? deviceManager = null)
    {
        _logger = logger;
        _scaleService = scaleService;
        _scannerService = scannerService;
        _hubContext = hubContext;
        _deviceManager = deviceManager;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("硬件后台服务启动中...");

        // 订阅硬件事件
        _scaleService.WeightChanged += OnWeightChanged;
        _scaleService.ErrorOccurred += OnScaleError;
        _scannerService.BarcodeScanned += OnBarcodeScanned;

        // 订阅设备管理器事件（艹，这个SB事件用于实时推送设备状态）
        if (_deviceManager != null)
        {
            _deviceManager.DeviceStatusChanged += OnDeviceStatusChanged;
            _deviceManager.DeviceErrorOccurred += OnDeviceErrorOccurred;
            _logger.LogInformation("设备管理器事件已订阅");
        }

        // 连接电子秤并开始读取
        var connected = await _scaleService.ConnectAsync();
        if (connected)
        {
            _scaleService.StartReading();
            _logger.LogInformation("电子秤已连接并开始读取数据");
        }
        else
        {
            _logger.LogWarning("电子秤连接失败，将在测试模式下运行");
        }

        // 启动扫码枪监听
        _scannerService.StartListening();
        _logger.LogInformation("扫码枪监听已启动");

        // 保持服务运行
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("硬件后台服务停止中...");

        // 取消订阅
        _scaleService.WeightChanged -= OnWeightChanged;
        _scaleService.ErrorOccurred -= OnScaleError;
        _scannerService.BarcodeScanned -= OnBarcodeScanned;

        // 取消订阅设备管理器事件
        if (_deviceManager != null)
        {
            _deviceManager.DeviceStatusChanged -= OnDeviceStatusChanged;
            _deviceManager.DeviceErrorOccurred -= OnDeviceErrorOccurred;
        }

        // 停止硬件服务
        _scaleService.StopReading();
        await _scaleService.DisconnectAsync();
        _scannerService.StopListening();

        await base.StopAsync(cancellationToken);
    }

    private async void OnWeightChanged(object? sender, WeightChangedEventArgs e)
    {
        try
        {
            await _hubContext.Clients.All.SendAsync("ReceiveWeight", new
            {
                weight = e.Weight,
                unit = e.Unit,
                isStable = e.IsStable,
                timestamp = e.Timestamp
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "推送重量数据失败");
        }
    }

    private async void OnBarcodeScanned(object? sender, BarcodeScannedEventArgs e)
    {
        try
        {
            await _hubContext.Clients.All.SendAsync("ReceiveBarcode", new
            {
                barcode = e.Barcode,
                scannerType = e.ScannerType,
                timestamp = e.Timestamp
            });

            _logger.LogInformation("条码已推送: {Barcode}", e.Barcode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "推送条码数据失败");
        }
    }

    private async void OnScaleError(object? sender, ScaleErrorEventArgs e)
    {
        try
        {
            await _hubContext.Clients.All.SendAsync("ReceiveError", new
            {
                errorMessage = e.ErrorMessage,
                source = "Scale",
                timestamp = e.Timestamp
            });

            _logger.LogError("电子秤错误已推送: {Message}", e.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "推送错误信息失败");
        }
    }

    private async void OnDeviceStatusChanged(object? sender, DeviceStatusEventArgs e)
    {
        try
        {
            // 获取设备类型（通过反射）
            string deviceType = "Unknown";
            if (sender != null)
            {
                var metadataProp = sender.GetType().GetProperty("Metadata");
                if (metadataProp != null)
                {
                    var metadata = metadataProp.GetValue(sender);
                    var deviceTypeProp = metadata?.GetType().GetProperty("DeviceType");
                    deviceType = deviceTypeProp?.GetValue(metadata)?.ToString() ?? "Unknown";
                }
            }

            await _hubContext.Clients.All.SendAsync("ReceiveDeviceStatusUpdate", new
            {
                deviceId = e.DeviceId,
                deviceType = deviceType,
                oldState = e.OldState.ToString(),
                newState = e.NewState.ToString(),
                timestamp = e.Timestamp
            });

            _logger.LogInformation("设备状态变化已推送: {DeviceId} {OldState} -> {NewState}",
                e.DeviceId, e.OldState, e.NewState);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "推送设备状态变化失败");
        }
    }

    private async void OnDeviceErrorOccurred(object? sender, DeviceErrorEventArgs e)
    {
        try
        {
            // 获取设备类型（通过反射）
            string deviceType = "Unknown";
            if (sender != null)
            {
                var metadataProp = sender.GetType().GetProperty("Metadata");
                if (metadataProp != null)
                {
                    var metadata = metadataProp.GetValue(sender);
                    var deviceTypeProp = metadata?.GetType().GetProperty("DeviceType");
                    deviceType = deviceTypeProp?.GetValue(metadata)?.ToString() ?? "Unknown";
                }
            }

            await _hubContext.Clients.All.SendAsync("ReceiveDeviceError", new
            {
                deviceId = e.DeviceId,
                deviceType = deviceType,
                errorMessage = e.Message,
                severity = e.Severity.ToString(),
                timestamp = e.Timestamp
            });

            _logger.LogError("设备错误已推送: {DeviceId} {Message}", e.DeviceId, e.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "推送设备错误失败");
        }
    }
}
