namespace Minimes.Infrastructure.Devices.Models.EventArgs;

/// <summary>
/// 错误严重程度
/// </summary>
public enum ErrorSeverity
{
    /// <summary>信息</summary>
    Info = 0,

    /// <summary>警告</summary>
    Warning = 1,

    /// <summary>错误</summary>
    Error = 2,

    /// <summary>致命错误</summary>
    Critical = 3
}

/// <summary>
/// 设备错误事件参数
/// </summary>
public class DeviceErrorEventArgs : System.EventArgs
{
    /// <summary>设备ID</summary>
    public string DeviceId { get; set; } = string.Empty;

    /// <summary>错误消息</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>错误严重程度</summary>
    public ErrorSeverity Severity { get; set; }

    /// <summary>异常对象</summary>
    public Exception? Exception { get; set; }

    /// <summary>是否可恢复</summary>
    public bool IsRecoverable { get; set; }

    /// <summary>时间戳</summary>
    public DateTime Timestamp { get; set; } = DateTime.Now;

    /// <summary>
    /// 构造函数
    /// </summary>
    public DeviceErrorEventArgs(string deviceId, string message, ErrorSeverity severity, Exception? exception = null, bool isRecoverable = true)
    {
        DeviceId = deviceId;
        Message = message;
        Severity = severity;
        Exception = exception;
        IsRecoverable = isRecoverable;
    }
}
