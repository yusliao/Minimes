namespace Minimes.Infrastructure.Devices.Management;

using Microsoft.Extensions.Logging;
using Minimes.Application.Interfaces;
using Minimes.Infrastructure.Devices.Abstractions;
using Minimes.Infrastructure.Devices.Models.EventArgs;
using System.Collections.Concurrent;

/// <summary>
/// 设备管理器实现
/// 艹，这个SB管理器负责所有设备的注册和事件聚合
/// </summary>
public class DeviceManager : IDeviceManager
{
    private readonly ILogger<DeviceManager> _logger;
    private readonly DeviceLogManager? _deviceLogManager;
    private readonly IDeviceNotificationService? _notificationService;
    private readonly ConcurrentDictionary<string, object> _devices = new();
    private bool _disposed;

    /// <inheritdoc/>
    public event EventHandler<DeviceStatusEventArgs>? DeviceStatusChanged;

    /// <inheritdoc/>
    public event EventHandler<DeviceErrorEventArgs>? DeviceErrorOccurred;

    public DeviceManager(
        ILogger<DeviceManager> logger,
        DeviceLogManager? deviceLogManager = null,
        IDeviceNotificationService? notificationService = null)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _deviceLogManager = deviceLogManager;
        _notificationService = notificationService;
        _logger.LogInformation("设备管理器已创建");
    }

    #region 设备注册

    /// <inheritdoc/>
    public void RegisterDevice<TData>(IDevice<TData> device) where TData : class
    {
        if (device == null)
        {
            throw new ArgumentNullException(nameof(device));
        }

        if (_devices.TryAdd(device.DeviceId, device))
        {
            // 订阅设备事件
            device.StatusChanged += OnDeviceStatusChanged;
            device.ErrorOccurred += OnDeviceErrorOccurred;

            _logger.LogInformation("设备已注册: DeviceId={DeviceId}, Type={DeviceType}",
                device.DeviceId, device.Metadata.DeviceType);

            // 推送设备列表更新到前端（艹，有新设备注册了）
            _ = _notificationService?.NotifyDeviceListUpdateAsync();
        }
        else
        {
            throw new InvalidOperationException($"设备ID已存在: {device.DeviceId}");
        }
    }

    /// <inheritdoc/>
    public void UnregisterDevice(string deviceId)
    {
        if (string.IsNullOrEmpty(deviceId))
        {
            throw new ArgumentNullException(nameof(deviceId));
        }

        if (_devices.TryRemove(deviceId, out var deviceObj))
        {
            // 取消订阅设备事件
            if (deviceObj is IDevice<object> device)
            {
                device.StatusChanged -= OnDeviceStatusChanged;
                device.ErrorOccurred -= OnDeviceErrorOccurred;
            }

            // 释放设备资源
            if (deviceObj is IDisposable disposable)
            {
                disposable.Dispose();
            }

            _logger.LogInformation("设备已注销: DeviceId={DeviceId}", deviceId);

            // 推送设备列表更新到前端（艹，有设备被注销了）
            _ = _notificationService?.NotifyDeviceListUpdateAsync();
        }
        else
        {
            _logger.LogWarning("设备不存在: DeviceId={DeviceId}", deviceId);
        }
    }

    #endregion

    #region 设备查询

    /// <inheritdoc/>
    public IDevice<TData>? GetDevice<TData>(string deviceId) where TData : class
    {
        if (string.IsNullOrEmpty(deviceId))
        {
            throw new ArgumentNullException(nameof(deviceId));
        }

        if (_devices.TryGetValue(deviceId, out var deviceObj))
        {
            return deviceObj as IDevice<TData>;
        }

        return null;
    }

    /// <inheritdoc/>
    public IReadOnlyList<object> GetAllDevices()
    {
        return _devices.Values.ToList();
    }

    /// <inheritdoc/>
    public bool DeviceExists(string deviceId)
    {
        return !string.IsNullOrEmpty(deviceId) && _devices.ContainsKey(deviceId);
    }

    #endregion

    #region 批量操作

    /// <inheritdoc/>
    public async Task ConnectAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("开始连接所有设备，共 {Count} 个", _devices.Count);

        var tasks = _devices.Values
            .Select(async deviceObj =>
            {
                try
                {
                    if (deviceObj is IDevice<object> device)
                    {
                        await device.ConnectAsync(cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "连接设备失败");
                }
            });

        await Task.WhenAll(tasks);
        _logger.LogInformation("所有设备连接操作已完成");
    }

    /// <inheritdoc/>
    public async Task DisconnectAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("开始断开所有设备，共 {Count} 个", _devices.Count);

        var tasks = _devices.Values
            .Select(async deviceObj =>
            {
                try
                {
                    if (deviceObj is IDevice<object> device)
                    {
                        await device.DisconnectAsync(cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "断开设备失败");
                }
            });

        await Task.WhenAll(tasks);
        _logger.LogInformation("所有设备断开操作已完成");
    }

    /// <inheritdoc/>
    public async Task StartAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("开始启动所有设备，共 {Count} 个", _devices.Count);

        var tasks = _devices.Values
            .Select(async deviceObj =>
            {
                try
                {
                    if (deviceObj is IDevice<object> device)
                    {
                        await device.StartAsync(cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "启动设备失败");
                }
            });

        await Task.WhenAll(tasks);
        _logger.LogInformation("所有设备启动操作已完成");
    }

    /// <inheritdoc/>
    public async Task StopAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("开始停止所有设备，共 {Count} 个", _devices.Count);

        var tasks = _devices.Values
            .Select(async deviceObj =>
            {
                try
                {
                    if (deviceObj is IDevice<object> device)
                    {
                        await device.StopAsync(cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "停止设备失败");
                }
            });

        await Task.WhenAll(tasks);
        _logger.LogInformation("所有设备停止操作已完成");
    }

    #endregion

    #region 事件处理

    private void OnDeviceStatusChanged(object? sender, DeviceStatusEventArgs e)
    {
        // 记录状态变化日志
        _deviceLogManager?.AddLog(
            e.DeviceId,
            LogLevel.Information,
            $"设备状态变化: {e.OldState} -> {e.NewState}",
            null,
            new Dictionary<string, object>
            {
                { "OldState", e.OldState.ToString() },
                { "NewState", e.NewState.ToString() },
                { "Timestamp", e.Timestamp }
            }
        );

        DeviceStatusChanged?.Invoke(this, e);

        // 推送设备状态更新到前端（艹，fire-and-forget模式，不阻塞主流程）
        if (_notificationService != null && _devices.TryGetValue(e.DeviceId, out var device))
        {
            var deviceType = GetDeviceType(device);
            _ = _notificationService.NotifyDeviceStatusUpdateAsync(
                e.DeviceId,
                deviceType,
                e.OldState.ToString(),
                e.NewState.ToString()
            );
        }
    }

    private void OnDeviceErrorOccurred(object? sender, DeviceErrorEventArgs e)
    {
        // 记录错误日志
        _deviceLogManager?.AddLog(
            e.DeviceId,
            LogLevel.Error,
            $"设备错误: {e.Message}",
            e.Exception,
            new Dictionary<string, object>
            {
                { "Severity", e.Severity.ToString() },
                { "IsRecoverable", e.IsRecoverable },
                { "Timestamp", e.Timestamp }
            }
        );

        DeviceErrorOccurred?.Invoke(this, e);

        // 推送设备错误到前端（艹，fire-and-forget模式）
        if (_notificationService != null && _devices.TryGetValue(e.DeviceId, out var device))
        {
            var deviceType = GetDeviceType(device);
            _ = _notificationService.NotifyDeviceErrorAsync(
                e.DeviceId,
                deviceType,
                e.Message,
                e.Severity.ToString()
            );
        }
    }

    #endregion

    #region 辅助方法

    /// <summary>
    /// 获取设备类型
    /// 艹，通过反射获取设备的Metadata.DeviceType属性
    /// </summary>
    private string GetDeviceType(object device)
    {
        try
        {
            var metadataProperty = device.GetType().GetProperty("Metadata");
            if (metadataProperty != null)
            {
                var metadata = metadataProperty.GetValue(device);
                if (metadata != null)
                {
                    var deviceTypeProperty = metadata.GetType().GetProperty("DeviceType");
                    return deviceTypeProperty?.GetValue(metadata)?.ToString() ?? "Unknown";
                }
            }
            return "Unknown";
        }
        catch
        {
            return "Unknown";
        }
    }

    #endregion

    #region IDisposable实现

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _logger.LogInformation("开始释放设备管理器资源");

        foreach (var deviceId in _devices.Keys.ToList())
        {
            UnregisterDevice(deviceId);
        }

        _disposed = true;
        _logger.LogInformation("设备管理器资源已释放");
    }

    #endregion
}
