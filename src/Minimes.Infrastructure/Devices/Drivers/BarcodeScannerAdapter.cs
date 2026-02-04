namespace Minimes.Infrastructure.Devices.Drivers;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Minimes.Domain.Interfaces;
using Minimes.Infrastructure.Devices.Abstractions;
using Minimes.Infrastructure.Devices.Models;
using Minimes.Infrastructure.Devices.Models.Data;

/// <summary>
/// 条码扫描枪设备适配器
///
/// 设计说明：
/// 1. 继承DeviceAdapter<BarcodeData>
/// 2. 不需要串口通信，数据通过外部推送（JSInterop）
/// 3. 支持演示模式（从数据库加载商品条码）
/// 4. 提供ProcessBarcodeInput方法接收外部输入
/// </summary>
public class BarcodeScannerAdapter : DeviceAdapter<BarcodeData>
{
    #region 字段

    private readonly IServiceProvider _serviceProvider;

    #endregion

    #region 构造函数

    /// <summary>
    /// 构造函数
    /// </summary>
    public BarcodeScannerAdapter(
        ILogger<BarcodeScannerAdapter> logger,
        DeviceConfiguration configuration,
        IServiceProvider serviceProvider)
        : base(logger, configuration)
    {
        _serviceProvider = serviceProvider;
        Logger.LogInformation("条码扫描枪适配器已创建: DeviceId={DeviceId}", DeviceId);
    }

    #endregion

    #region 抽象方法实现 - 连接管理

    /// <inheritdoc/>
    protected override Task<bool> OnConnectAsync(CancellationToken cancellationToken)
    {
        // 扫码枪不需要物理连接，直接返回成功
        Logger.LogInformation("条码扫描枪已连接（虚拟连接）: DeviceId={DeviceId}", DeviceId);
        return Task.FromResult(true);
    }

    /// <inheritdoc/>
    protected override Task OnDisconnectAsync(CancellationToken cancellationToken)
    {
        // 清理资源
        Logger.LogInformation("条码扫描枪已断开: DeviceId={DeviceId}", DeviceId);
        return Task.CompletedTask;
    }

    #endregion

    #region 抽象方法实现 - 数据采集

    /// <inheritdoc/>
    protected override Task OnStartAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("条码扫描枪启动监听: DeviceId={DeviceId}", DeviceId);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    protected override Task OnStopAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("条码扫描枪停止监听: DeviceId={DeviceId}", DeviceId);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    protected override Task<BarcodeData?> OnReadDataAsync(CancellationToken cancellationToken)
    {
        // 扫码枪数据通过外部推送（ProcessBarcodeInput），这里返回null
        return Task.FromResult<BarcodeData?>(null);
    }

    #endregion

    #region 抽象方法实现 - 元数据

    /// <inheritdoc/>
    protected override DeviceMetadata CreateMetadata()
    {
        return new DeviceMetadata
        {
            DeviceType = "BarcodeScanner",
            DeviceName = Configuration.DeviceName,
            Manufacturer = "Generic",
            Model = "Scanner-USB",
            ProtocolType = Configuration.ProtocolType,
            DataType = typeof(BarcodeData).Name,
            Capabilities = new List<string> { "Scan", "Simulate" }
        };
    }

    #endregion

    #region 公共方法 - 外部输入处理

    /// <summary>
    /// 处理来自前端JSInterop的扫码输入
    /// </summary>
    /// <param name="barcode">条码字符串</param>
    public void ProcessBarcodeInput(string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
        {
            Logger.LogWarning("收到空条码输入，忽略");
            return;
        }

        // 检查设备是否已启动
        if (Status.State != DeviceState.Running)
        {
            Logger.LogWarning("设备未启动，忽略扫码输入: {Barcode}", barcode);
            return;
        }

        Logger.LogInformation("处理扫码输入: {Barcode}", barcode);

        // 创建条码数据并触发事件
        var barcodeData = new BarcodeData
        {
            Barcode = barcode,
            ScannerType = ScannerType.Keyboard,
            Timestamp = DateTime.Now
        };

        // 触发数据接收事件
        RaiseDataReceivedEvent(barcodeData);
    }

    /// <summary>
    /// 手动模拟扫码（用于测试）
    /// </summary>
    /// <param name="barcode">条码字符串</param>
    public void SimulateScan(string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
        {
            Logger.LogWarning("模拟扫码条码为空，忽略");
            return;
        }

        Logger.LogInformation("模拟扫码: {Barcode}", barcode);

        // 创建条码数据并触发事件
        var barcodeData = new BarcodeData
        {
            Barcode = barcode,
            ScannerType = ScannerType.Keyboard,
            Timestamp = DateTime.Now
        };

        // 触发数据接收事件
        RaiseDataReceivedEvent(barcodeData);
    }

    #endregion
}
