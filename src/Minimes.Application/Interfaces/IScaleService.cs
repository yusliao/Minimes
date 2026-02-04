namespace Minimes.Application.Interfaces;

/// <summary>
/// 电子秤服务接口 - 硬件抽象层
/// </summary>
public interface IScaleService : IDisposable
{
    /// <summary>
    /// 当前重量值（克）
    /// </summary>
    decimal CurrentWeight { get; }

    /// <summary>
    /// 是否已连接
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// 重量数据更新事件
    /// </summary>
    event EventHandler<WeightChangedEventArgs>? WeightChanged;

    /// <summary>
    /// 连接错误事件
    /// </summary>
    event EventHandler<ScaleErrorEventArgs>? ErrorOccurred;

    /// <summary>
    /// 连接到电子秤
    /// </summary>
    Task<bool> ConnectAsync();

    /// <summary>
    /// 断开电子秤连接
    /// </summary>
    Task DisconnectAsync();

    /// <summary>
    /// 开始持续读取重量数据
    /// </summary>
    void StartReading();

    /// <summary>
    /// 停止读取重量数据
    /// </summary>
    void StopReading();

    /// <summary>
    /// 手动触发去皮（清零）
    /// </summary>
    Task TareAsync();
}

/// <summary>
/// 重量变化事件参数
/// </summary>
public class WeightChangedEventArgs : EventArgs
{
    public decimal Weight { get; set; }
    public string Unit { get; set; } = "g";
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public bool IsStable { get; set; }
}

/// <summary>
/// 电子秤错误事件参数
/// </summary>
public class ScaleErrorEventArgs : EventArgs
{
    public string ErrorMessage { get; set; } = string.Empty;
    public Exception? Exception { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
}
