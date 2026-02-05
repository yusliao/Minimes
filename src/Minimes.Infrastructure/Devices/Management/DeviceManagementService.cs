using Microsoft.Extensions.Logging;
using Minimes.Application.DTOs.Device;
using Minimes.Application.Interfaces;
using Minimes.Infrastructure.Devices.Abstractions;
using Minimes.Infrastructure.Devices.Models;
using System.Reflection;

namespace Minimes.Infrastructure.Devices.Management;

/// <summary>
/// 设备管理服务实现
/// 艹，这个服务要处理所有设备的统一管理，反射调用泛型接口真TM麻烦
/// </summary>
public class DeviceManagementService : IDeviceManagementService
{
    private readonly IDeviceManager _deviceManager;
    private readonly ILogger<DeviceManagementService> _logger;
    private readonly IDeviceNotificationService? _notificationService;

    /// <summary>
    /// 构造函数
    /// 艹，通知服务是可选的，没有也能工作，只是不会推送实时更新
    /// </summary>
    public DeviceManagementService(
        IDeviceManager deviceManager,
        ILogger<DeviceManagementService> logger,
        IDeviceNotificationService? notificationService = null)
    {
        _deviceManager = deviceManager ?? throw new ArgumentNullException(nameof(deviceManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _notificationService = notificationService;
    }

    #region 查询操作（Operator可用）

    /// <summary>
    /// 获取所有设备信息
    /// 艹，要遍历所有设备并映射成DTO
    /// </summary>
    public async Task<IEnumerable<DeviceInfoDto>> GetAllDevicesAsync()
    {
        try
        {
            _logger.LogInformation("开始获取所有设备信息");

            var devices = _deviceManager.GetAllDevices();
            var deviceInfos = new List<DeviceInfoDto>();

            foreach (var device in devices)
            {
                try
                {
                    var deviceInfo = MapToDeviceInfo(device);
                    if (deviceInfo != null)
                    {
                        deviceInfos.Add(deviceInfo);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "映射设备信息失败，设备类型: {DeviceType}", device.GetType().Name);
                }
            }

            _logger.LogInformation("成功获取 {Count} 个设备信息", deviceInfos.Count);
            return await Task.FromResult(deviceInfos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取所有设备信息失败");
            throw;
        }
    }

    /// <summary>
    /// 根据设备ID获取设备详情
    /// </summary>
    public async Task<DeviceDetailDto?> GetDeviceDetailAsync(string deviceId)
    {
        try
        {
            _logger.LogInformation("获取设备详情: {DeviceId}", deviceId);

            var devices = _deviceManager.GetAllDevices();
            var device = devices.FirstOrDefault(d => GetDeviceId(d) == deviceId);

            if (device == null)
            {
                _logger.LogWarning("设备不存在: {DeviceId}", deviceId);
                return null;
            }

            var detail = MapToDeviceDetail(device);

            // 获取健康状态
            var health = await GetDeviceHealthInternal(device);
            if (health != null)
            {
                detail.Health = new DeviceHealthDto
                {
                    IsHealthy = health.Status == HealthStatus.Healthy,
                    HealthScore = CalculateHealthScore(health),
                    UptimeSeconds = CalculateUptime(device),
                    LastHeartbeat = DateTime.Now,
                    Diagnostics = health.CheckItems.ToDictionary(
                        item => item.Name,
                        item => item.Description
                    ),
                    Warnings = health.Recommendations
                };
            }

            _logger.LogInformation("成功获取设备详情: {DeviceId}", deviceId);
            return detail;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取设备详情失败: {DeviceId}", deviceId);
            throw;
        }
    }

    /// <summary>
    /// 获取设备健康状态
    /// </summary>
    public async Task<DeviceHealthDto?> GetDeviceHealthAsync(string deviceId)
    {
        try
        {
            _logger.LogInformation("获取设备健康状态: {DeviceId}", deviceId);

            var devices = _deviceManager.GetAllDevices();
            var device = devices.FirstOrDefault(d => GetDeviceId(d) == deviceId);

            if (device == null)
            {
                _logger.LogWarning("设备不存在: {DeviceId}", deviceId);
                return null;
            }

            var health = await GetDeviceHealthInternal(device);
            if (health == null)
            {
                return null;
            }

            var healthDto = new DeviceHealthDto
            {
                IsHealthy = health.Status == HealthStatus.Healthy,
                HealthScore = CalculateHealthScore(health),
                UptimeSeconds = CalculateUptime(device),
                LastHeartbeat = DateTime.Now,
                Diagnostics = health.CheckItems.ToDictionary(
                    item => item.Name,
                    item => item.Description
                ),
                Warnings = health.Recommendations
            };

            _logger.LogInformation("成功获取设备健康状态: {DeviceId}, 健康分数: {Score}",
                deviceId, healthDto.HealthScore);
            return healthDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取设备健康状态失败: {DeviceId}", deviceId);
            throw;
        }
    }

    /// <summary>
    /// 获取设备日志（最近N条）
    /// 艹，这个功能暂时返回空列表，因为设备框架还没实现日志存储
    /// TODO: 后续需要实现设备日志持久化
    /// </summary>
    public async Task<IEnumerable<DeviceLogDto>> GetDeviceLogsAsync(string deviceId, int limit = 100)
    {
        try
        {
            _logger.LogInformation("获取设备日志: {DeviceId}, 限制: {Limit}", deviceId, limit);

            // TODO: 实现设备日志查询
            // 目前返回空列表
            var logs = new List<DeviceLogDto>();

            _logger.LogInformation("成功获取设备日志: {DeviceId}, 数量: {Count}", deviceId, logs.Count);
            return await Task.FromResult(logs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取设备日志失败: {DeviceId}", deviceId);
            throw;
        }
    }

    /// <summary>
    /// 获取设备配置
    /// </summary>
    public async Task<DeviceConfigurationDto?> GetDeviceConfigurationAsync(string deviceId)
    {
        try
        {
            _logger.LogInformation("获取设备配置: {DeviceId}", deviceId);

            var devices = _deviceManager.GetAllDevices();
            var device = devices.FirstOrDefault(d => GetDeviceId(d) == deviceId);

            if (device == null)
            {
                _logger.LogWarning("设备不存在: {DeviceId}", deviceId);
                return null;
            }

            var configuration = MapToDeviceConfiguration(device);

            _logger.LogInformation("成功获取设备配置: {DeviceId}", deviceId);
            return await Task.FromResult(configuration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取设备配置失败: {DeviceId}", deviceId);
            throw;
        }
    }

    #endregion

    #region 控制操作（Admin专用）

    /// <summary>
    /// 连接设备
    /// 艹，反射调用泛型方法真TM麻烦
    /// </summary>
    public async Task<bool> ConnectDeviceAsync(string deviceId)
    {
        try
        {
            _logger.LogInformation("连接设备: {DeviceId}", deviceId);

            var devices = _deviceManager.GetAllDevices();
            var device = devices.FirstOrDefault(d => GetDeviceId(d) == deviceId);

            if (device == null)
            {
                _logger.LogWarning("设备不存在: {DeviceId}", deviceId);
                return false;
            }

            var result = await ConnectDeviceInternal(device);

            if (result)
            {
                _logger.LogInformation("成功连接设备: {DeviceId}", deviceId);
            }
            else
            {
                _logger.LogWarning("连接设备失败: {DeviceId}", deviceId);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "连接设备异常: {DeviceId}", deviceId);
            throw;
        }
    }

    /// <summary>
    /// 断开设备连接
    /// </summary>
    public async Task<bool> DisconnectDeviceAsync(string deviceId)
    {
        try
        {
            _logger.LogInformation("断开设备连接: {DeviceId}", deviceId);

            var devices = _deviceManager.GetAllDevices();
            var device = devices.FirstOrDefault(d => GetDeviceId(d) == deviceId);

            if (device == null)
            {
                _logger.LogWarning("设备不存在: {DeviceId}", deviceId);
                return false;
            }

            await DisconnectDeviceInternal(device);

            _logger.LogInformation("成功断开设备连接: {DeviceId}", deviceId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "断开设备连接异常: {DeviceId}", deviceId);
            throw;
        }
    }

    /// <summary>
    /// 启动设备
    /// </summary>
    public async Task<bool> StartDeviceAsync(string deviceId)
    {
        try
        {
            _logger.LogInformation("启动设备: {DeviceId}", deviceId);

            var devices = _deviceManager.GetAllDevices();
            var device = devices.FirstOrDefault(d => GetDeviceId(d) == deviceId);

            if (device == null)
            {
                _logger.LogWarning("设备不存在: {DeviceId}", deviceId);
                return false;
            }

            await StartDeviceInternal(device);

            _logger.LogInformation("成功启动设备: {DeviceId}", deviceId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "启动设备异常: {DeviceId}", deviceId);
            throw;
        }
    }

    /// <summary>
    /// 停止设备
    /// </summary>
    public async Task<bool> StopDeviceAsync(string deviceId)
    {
        try
        {
            _logger.LogInformation("停止设备: {DeviceId}", deviceId);

            var devices = _deviceManager.GetAllDevices();
            var device = devices.FirstOrDefault(d => GetDeviceId(d) == deviceId);

            if (device == null)
            {
                _logger.LogWarning("设备不存在: {DeviceId}", deviceId);
                return false;
            }

            await StopDeviceInternal(device);

            _logger.LogInformation("成功停止设备: {DeviceId}", deviceId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "停止设备异常: {DeviceId}", deviceId);
            throw;
        }
    }

    /// <summary>
    /// 重启设备（断开后重新连接并启动）
    /// 艹，这个操作要按顺序执行，不能并发
    /// </summary>
    public async Task<bool> RestartDeviceAsync(string deviceId)
    {
        try
        {
            _logger.LogInformation("重启设备: {DeviceId}", deviceId);

            var devices = _deviceManager.GetAllDevices();
            var device = devices.FirstOrDefault(d => GetDeviceId(d) == deviceId);

            if (device == null)
            {
                _logger.LogWarning("设备不存在: {DeviceId}", deviceId);
                return false;
            }

            // 1. 停止设备
            _logger.LogInformation("重启步骤1: 停止设备 {DeviceId}", deviceId);
            await StopDeviceInternal(device);

            // 2. 断开连接
            _logger.LogInformation("重启步骤2: 断开连接 {DeviceId}", deviceId);
            await DisconnectDeviceInternal(device);

            // 3. 等待一下，让设备完全释放资源
            await Task.Delay(1000);

            // 4. 重新连接
            _logger.LogInformation("重启步骤3: 重新连接 {DeviceId}", deviceId);
            var connected = await ConnectDeviceInternal(device);
            if (!connected)
            {
                _logger.LogWarning("重启失败: 无法重新连接设备 {DeviceId}", deviceId);
                return false;
            }

            // 5. 启动设备
            _logger.LogInformation("重启步骤4: 启动设备 {DeviceId}", deviceId);
            await StartDeviceInternal(device);

            _logger.LogInformation("成功重启设备: {DeviceId}", deviceId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "重启设备异常: {DeviceId}", deviceId);
            throw;
        }
    }

    #endregion

    #region 配置操作（Admin专用）

    /// <summary>
    /// 更新设备配置
    /// 艹，这个功能暂时不实现，因为设备配置更新比较复杂
    /// TODO: 后续需要实现设备配置动态更新
    /// </summary>
    public async Task<bool> UpdateDeviceConfigurationAsync(string deviceId, DeviceConfigurationDto configuration)
    {
        try
        {
            _logger.LogInformation("更新设备配置: {DeviceId}", deviceId);

            var devices = _deviceManager.GetAllDevices();
            var device = devices.FirstOrDefault(d => GetDeviceId(d) == deviceId);

            if (device == null)
            {
                _logger.LogWarning("设备不存在: {DeviceId}", deviceId);
                return false;
            }

            // TODO: 实现设备配置更新逻辑
            // 目前只记录日志
            _logger.LogWarning("设备配置更新功能暂未实现: {DeviceId}", deviceId);

            return await Task.FromResult(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新设备配置失败: {DeviceId}", deviceId);
            throw;
        }
    }

    #endregion

    #region 辅助方法 - 基础工具

    /// <summary>
    /// 获取设备ID
    /// 艹，反射获取属性值
    /// </summary>
    private string GetDeviceId(object device)
    {
        var deviceIdProperty = device.GetType().GetProperty("DeviceId");
        return deviceIdProperty?.GetValue(device)?.ToString() ?? string.Empty;
    }

    #endregion

    #region 辅助方法 - 映射方法

    /// <summary>
    /// 映射设备信息到DeviceInfoDto
    /// 艹，这个映射方法要处理各种设备类型
    /// </summary>
    private DeviceInfoDto? MapToDeviceInfo(object device)
    {
        try
        {
            var type = device.GetType();
            var metadataProperty = type.GetProperty("Metadata");
            var statusProperty = type.GetProperty("Status");
            var isConnectedProperty = type.GetProperty("IsConnected");
            var isRunningProperty = type.GetProperty("IsRunning");

            if (metadataProperty == null || statusProperty == null)
            {
                return null;
            }

            var metadata = metadataProperty.GetValue(device) as DeviceMetadata;
            var status = statusProperty.GetValue(device) as DeviceStatus;
            var isConnected = (bool)(isConnectedProperty?.GetValue(device) ?? false);
            var isRunning = (bool)(isRunningProperty?.GetValue(device) ?? false);

            if (metadata == null || status == null)
            {
                return null;
            }

            return new DeviceInfoDto
            {
                DeviceId = GetDeviceId(device),
                DeviceType = metadata.DeviceType,
                DeviceName = metadata.DeviceName,
                Manufacturer = metadata.Manufacturer,
                Model = metadata.Model,
                ProtocolType = metadata.ProtocolType,
                State = status.State.ToString(),
                StateDescription = status.Description,
                IsConnected = isConnected,
                IsRunning = isRunning,
                ConnectedAt = status.ConnectedAt,
                StartedAt = status.StartedAt,
                ErrorCount = status.ErrorCount,
                LastError = status.LastError,
                LastErrorAt = status.LastErrorAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "映射设备信息失败");
            return null;
        }
    }

    /// <summary>
    /// 映射设备详情到DeviceDetailDto
    /// </summary>
    private DeviceDetailDto MapToDeviceDetail(object device)
    {
        var type = device.GetType();
        var metadataProperty = type.GetProperty("Metadata");
        var statusProperty = type.GetProperty("Status");
        var isConnectedProperty = type.GetProperty("IsConnected");
        var isRunningProperty = type.GetProperty("IsRunning");

        var metadata = metadataProperty?.GetValue(device) as DeviceMetadata;
        var status = statusProperty?.GetValue(device) as DeviceStatus;
        var isConnected = (bool)(isConnectedProperty?.GetValue(device) ?? false);
        var isRunning = (bool)(isRunningProperty?.GetValue(device) ?? false);

        return new DeviceDetailDto
        {
            DeviceId = GetDeviceId(device),
            DeviceType = metadata?.DeviceType ?? string.Empty,
            DeviceName = metadata?.DeviceName ?? string.Empty,
            Manufacturer = metadata?.Manufacturer,
            Model = metadata?.Model,
            SerialNumber = metadata?.SerialNumber,
            ProtocolType = metadata?.ProtocolType ?? string.Empty,
            DataType = metadata?.DataType ?? string.Empty,
            Capabilities = metadata?.Capabilities ?? new List<string>(),
            ExtendedProperties = metadata?.ExtendedProperties ?? new Dictionary<string, object>(),
            State = status?.State.ToString() ?? string.Empty,
            StateDescription = status?.Description ?? string.Empty,
            IsConnected = isConnected,
            IsRunning = isRunning,
            ConnectedAt = status?.ConnectedAt,
            StartedAt = status?.StartedAt,
            TotalDataReceived = status?.TotalDataReceived ?? 0,
            ErrorCount = status?.ErrorCount ?? 0,
            LastError = status?.LastError,
            LastErrorAt = status?.LastErrorAt
        };
    }

    /// <summary>
    /// 映射设备配置到DeviceConfigurationDto
    /// 艹，这个配置映射比较简单，主要是元数据
    /// </summary>
    private DeviceConfigurationDto MapToDeviceConfiguration(object device)
    {
        var type = device.GetType();
        var metadataProperty = type.GetProperty("Metadata");

        var metadata = metadataProperty?.GetValue(device) as DeviceMetadata;

        return new DeviceConfigurationDto
        {
            DeviceId = GetDeviceId(device),
            DeviceType = metadata?.DeviceType ?? string.Empty,
            ConfigurationItems = metadata?.ExtendedProperties ?? new Dictionary<string, object>(),
            ConfigurationMode = metadata?.ProtocolType ?? string.Empty,
            LastUpdated = DateTime.Now
        };
    }

    #endregion

    #region 辅助方法 - 反射调用（第1部分）

    /// <summary>
    /// 获取设备健康状态（内部方法）
    /// 艹，反射调用GetHealthAsync方法
    /// </summary>
    private async Task<DeviceHealth?> GetDeviceHealthInternal(object device)
    {
        try
        {
            var type = device.GetType();
            var method = type.GetMethod("GetHealthAsync");
            if (method == null)
            {
                return null;
            }

            var task = method.Invoke(device, new object[] { CancellationToken.None }) as Task<DeviceHealth>;
            if (task == null)
            {
                return null;
            }

            return await task;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取设备健康状态失败");
            return null;
        }
    }

    /// <summary>
    /// 连接设备（内部方法）
    /// 艹，反射调用ConnectAsync方法
    /// </summary>
    private async Task<bool> ConnectDeviceInternal(object device)
    {
        try
        {
            var type = device.GetType();
            var method = type.GetMethod("ConnectAsync");
            if (method == null)
            {
                _logger.LogWarning("设备不支持ConnectAsync方法");
                return false;
            }

            var task = method.Invoke(device, new object[] { CancellationToken.None }) as Task<bool>;
            if (task == null)
            {
                return false;
            }

            return await task;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "连接设备失败");
            return false;
        }
    }

    /// <summary>
    /// 断开设备连接（内部方法）
    /// 艹，反射调用DisconnectAsync方法
    /// </summary>
    private async Task DisconnectDeviceInternal(object device)
    {
        try
        {
            var type = device.GetType();
            var method = type.GetMethod("DisconnectAsync");
            if (method == null)
            {
                _logger.LogWarning("设备不支持DisconnectAsync方法");
                return;
            }

            var task = method.Invoke(device, new object[] { CancellationToken.None }) as Task;
            if (task != null)
            {
                await task;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "断开设备连接失败");
            throw;
        }
    }

    /// <summary>
    /// 启动设备（内部方法）
    /// 艹，反射调用StartAsync方法
    /// </summary>
    private async Task StartDeviceInternal(object device)
    {
        try
        {
            var type = device.GetType();
            var method = type.GetMethod("StartAsync");
            if (method == null)
            {
                _logger.LogWarning("设备不支持StartAsync方法");
                return;
            }

            var task = method.Invoke(device, new object[] { CancellationToken.None }) as Task;
            if (task != null)
            {
                await task;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "启动设备失败");
            throw;
        }
    }

    /// <summary>
    /// 停止设备（内部方法）
    /// 艹，反射调用StopAsync方法
    /// </summary>
    private async Task StopDeviceInternal(object device)
    {
        try
        {
            var type = device.GetType();
            var method = type.GetMethod("StopAsync");
            if (method == null)
            {
                _logger.LogWarning("设备不支持StopAsync方法");
                return;
            }

            var task = method.Invoke(device, new object[] { CancellationToken.None }) as Task;
            if (task != null)
            {
                await task;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "停止设备失败");
            throw;
        }
    }

    /// <summary>
    /// 设置设备演示模式（内部方法）
    /// 艹，反射调用SetDemoMode方法
    /// </summary>
    #endregion

    #region 辅助方法 - 计算方法

    /// <summary>
    /// 计算健康分数
    /// 艹，根据健康状态计算0-100的分数
    /// </summary>
    private int CalculateHealthScore(DeviceHealth health)
    {
        // 基础分数根据健康状态
        int baseScore = health.Status switch
        {
            HealthStatus.Healthy => 100,
            HealthStatus.Degraded => 60,
            HealthStatus.Unhealthy => 20,
            _ => 0
        };

        // 根据错误率扣分（错误率越高扣分越多）
        int errorPenalty = (int)(health.ErrorRate * 30);

        // 根据响应时间扣分（响应时间超过1秒开始扣分）
        int responsePenalty = health.ResponseTimeMs > 1000 ? (int)((health.ResponseTimeMs - 1000) / 100) : 0;
        responsePenalty = Math.Min(responsePenalty, 20); // 最多扣20分

        // 计算最终分数
        int finalScore = baseScore - errorPenalty - responsePenalty;
        return Math.Max(0, Math.Min(100, finalScore)); // 确保在0-100范围内
    }

    /// <summary>
    /// 计算运行时长（秒）
    /// 艹，从StartedAt计算到现在的秒数
    /// </summary>
    private long CalculateUptime(object device)
    {
        try
        {
            var type = device.GetType();
            var statusProperty = type.GetProperty("Status");
            var status = statusProperty?.GetValue(device) as DeviceStatus;

            if (status?.StartedAt == null)
            {
                return 0;
            }

            var uptime = DateTime.Now - status.StartedAt.Value;
            return (long)uptime.TotalSeconds;
        }
        catch
        {
            return 0;
        }
    }

    #endregion
}
