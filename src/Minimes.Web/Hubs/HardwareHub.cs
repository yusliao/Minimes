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
