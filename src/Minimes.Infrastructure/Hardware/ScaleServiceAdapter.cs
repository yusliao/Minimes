namespace Minimes.Infrastructure.Hardware;

using Microsoft.Extensions.Logging;
using Minimes.Application.Interfaces;
using Minimes.Infrastructure.Devices.Abstractions;
using Minimes.Infrastructure.Devices.Models;
using Minimes.Infrastructure.Devices.Models.Data;
using Minimes.Infrastructure.Devices.Models.EventArgs;

/// <summary>
/// 电子秤服务适配器 - 向后兼容旧接口
///
/// 设计说明：
/// 1. 实现IScaleService接口（保留旧接口）
/// 2. 内部使用IDevice<WeightData>（新框架）
/// 3. 将旧接口方法映射到新设备接口
/// 4. 确保不影响现有代码
/// </summary>
public class ScaleServiceAdapter : IScaleService
{
    private readonly ILogger<ScaleServiceAdapter> _logger;
    private readonly IDevice<WeightData> _device;
    private decimal _currentWeight;

    /// <inheritdoc/>
    public decimal CurrentWeight => _currentWeight;

    /// <inheritdoc/>
    public bool IsConnected => _device.IsConnected;

    /// <inheritdoc/>
    public event EventHandler<WeightChangedEventArgs>? WeightChanged;

    /// <inheritdoc/>
    public event EventHandler<ScaleErrorEventArgs>? ErrorOccurred;

    public ScaleServiceAdapter(ILogger<ScaleServiceAdapter> logger, IDevice<WeightData> device)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _device = device ?? throw new ArgumentNullException(nameof(device));

        // 订阅设备事件
        _device.DataReceived += OnDeviceDataReceived;
        _device.ErrorOccurred += OnDeviceErrorOccurred;

        _logger.LogInformation("电子秤服务适配器已创建");
    }

    /// <inheritdoc/>
    public async Task<bool> ConnectAsync()
    {
        return await _device.ConnectAsync();
    }

    /// <inheritdoc/>
    public async Task DisconnectAsync()
    {
        await _device.DisconnectAsync();
    }

    /// <inheritdoc/>
    public void StartReading()
    {
        _ = _device.StartAsync();
    }

    /// <inheritdoc/>
    public void StopReading()
    {
        _ = _device.StopAsync();
    }

    /// <inheritdoc/>
    public async Task TareAsync()
    {
        await _device.ExecuteCommandAsync<object>("Tare");
    }

    private void OnDeviceDataReceived(object? sender, DeviceDataEventArgs<WeightData> e)
    {
        _currentWeight = e.Data.ToGrams();

        WeightChanged?.Invoke(this, new WeightChangedEventArgs
        {
            Weight = _currentWeight,
            Unit = "g",
            Timestamp = e.Timestamp,
            IsStable = e.Data.IsStable
        });
    }

    private void OnDeviceErrorOccurred(object? sender, DeviceErrorEventArgs e)
    {
        ErrorOccurred?.Invoke(this, new ScaleErrorEventArgs
        {
            ErrorMessage = e.Message,
            Exception = e.Exception,
            Timestamp = e.Timestamp
        });
    }

    public void Dispose()
    {
        _device.DataReceived -= OnDeviceDataReceived;
        _device.ErrorOccurred -= OnDeviceErrorOccurred;
        _device.Dispose();
    }
}

