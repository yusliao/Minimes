using Microsoft.AspNetCore.SignalR;

namespace Minimes.Web.Hubs;

/// <summary>
/// 硬件设备SignalR Hub - 实时推送电子秤和扫码枪数据
/// </summary>
public class HardwareHub : Hub
{
    /// <summary>
    /// 推送重量数据到所有客户端
    /// </summary>
    public async Task BroadcastWeight(decimal weight, string unit, bool isStable)
    {
        await Clients.All.SendAsync("ReceiveWeight", new
        {
            weight,
            unit,
            isStable,
            timestamp = DateTime.Now
        });
    }

    /// <summary>
    /// 推送扫码数据到所有客户端
    /// </summary>
    public async Task BroadcastBarcode(string barcode, string scannerType)
    {
        await Clients.All.SendAsync("ReceiveBarcode", new
        {
            barcode,
            scannerType,
            timestamp = DateTime.Now
        });
    }

    /// <summary>
    /// 推送错误信息到所有客户端
    /// </summary>
    public async Task BroadcastError(string errorMessage, string source)
    {
        await Clients.All.SendAsync("ReceiveError", new
        {
            errorMessage,
            source,
            timestamp = DateTime.Now
        });
    }

    /// <summary>
    /// 推送设备状态更新到所有客户端
    /// 艹，这个SB方法用于实时推送设备状态变化
    /// </summary>
    public async Task BroadcastDeviceStatusUpdate(string deviceId, string deviceType, string oldState, string newState)
    {
        await Clients.All.SendAsync("ReceiveDeviceStatusUpdate", new
        {
            deviceId,
            deviceType,
            oldState,
            newState,
            timestamp = DateTime.Now
        });
    }

    /// <summary>
    /// 推送设备错误到所有客户端
    /// </summary>
    public async Task BroadcastDeviceError(string deviceId, string deviceType, string errorMessage, string severity)
    {
        await Clients.All.SendAsync("ReceiveDeviceError", new
        {
            deviceId,
            deviceType,
            errorMessage,
            severity,
            timestamp = DateTime.Now
        });
    }

    /// <summary>
    /// 推送设备列表更新到所有客户端
    /// </summary>
    public async Task BroadcastDeviceListUpdate()
    {
        await Clients.All.SendAsync("ReceiveDeviceListUpdate", new
        {
            timestamp = DateTime.Now
        });
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        Console.WriteLine($"客户端已连接: {Context.ConnectionId}");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
        Console.WriteLine($"客户端已断开: {Context.ConnectionId}");
    }
}
