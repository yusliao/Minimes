using Microsoft.AspNetCore.SignalR;
using Minimes.Application.Interfaces;
using Minimes.Web.Hubs;

namespace Minimes.Web.Services;

/// <summary>
/// 设备通知服务实现（基于SignalR）
/// 艹，这个SB服务负责把设备事件推送到前端，内部使用HardwareHub
/// </summary>
public class DeviceNotificationService : IDeviceNotificationService
{
    private readonly IHubContext<HardwareHub> _hubContext;
    private readonly ILogger<DeviceNotificationService> _logger;

    public DeviceNotificationService(
        IHubContext<HardwareHub> hubContext,
        ILogger<DeviceNotificationService> logger)
    {
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task NotifyDeviceStatusUpdateAsync(string deviceId, string deviceType, string oldState, string newState)
    {
        try
        {
            _logger.LogInformation(
                "推送设备状态更新: DeviceId={DeviceId}, {OldState} -> {NewState}",
                deviceId, oldState, newState);

            await _hubContext.Clients.All.SendAsync("ReceiveDeviceStatusUpdate", new
            {
                deviceId,
                deviceType,
                oldState,
                newState,
                timestamp = DateTime.Now
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "推送设备状态更新失败: DeviceId={DeviceId}", deviceId);
        }
    }

    /// <inheritdoc/>
    public async Task NotifyDeviceErrorAsync(string deviceId, string deviceType, string errorMessage, string severity)
    {
        try
        {
            _logger.LogWarning(
                "推送设备错误: DeviceId={DeviceId}, Severity={Severity}, Message={Message}",
                deviceId, severity, errorMessage);

            await _hubContext.Clients.All.SendAsync("ReceiveDeviceError", new
            {
                deviceId,
                deviceType,
                errorMessage,
                severity,
                timestamp = DateTime.Now
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "推送设备错误失败: DeviceId={DeviceId}", deviceId);
        }
    }

    /// <inheritdoc/>
    public async Task NotifyDeviceListUpdateAsync()
    {
        try
        {
            _logger.LogInformation("推送设备列表更新");

            await _hubContext.Clients.All.SendAsync("ReceiveDeviceListUpdate", new
            {
                timestamp = DateTime.Now
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "推送设备列表更新失败");
        }
    }
}
