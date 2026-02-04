using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Minimes.Infrastructure.Devices.Management;

/// <summary>
/// 设备日志条目
/// </summary>
public class DeviceLogEntry
{
    /// <summary>日志ID（自增）</summary>
    public long Id { get; set; }

    /// <summary>设备ID</summary>
    public string DeviceId { get; set; } = string.Empty;

    /// <summary>日志级别</summary>
    public LogLevel Level { get; set; }

    /// <summary>日志消息</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>异常信息</summary>
    public string? Exception { get; set; }

    /// <summary>日志时间</summary>
    public DateTime Timestamp { get; set; }

    /// <summary>扩展数据</summary>
    public Dictionary<string, object>? Data { get; set; }
}

/// <summary>
/// 设备日志管理器
/// 艹，这个SB类管理所有设备的日志，每个设备1000条循环缓冲区
/// </summary>
public class DeviceLogManager
{
    private readonly ConcurrentDictionary<string, CircularBuffer<DeviceLogEntry>> _deviceLogs = new();
    private readonly ILogger<DeviceLogManager> _logger;
    private long _nextLogId = 1;
    private readonly object _idLock = new();

    /// <summary>
    /// 每个设备的日志容量
    /// </summary>
    public const int LogCapacityPerDevice = 1000;

    public DeviceLogManager(ILogger<DeviceLogManager> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogInformation("设备日志管理器已创建");
    }

    /// <summary>
    /// 添加设备日志
    /// </summary>
    public void AddLog(string deviceId, LogLevel level, string message, Exception? exception = null, Dictionary<string, object>? data = null)
    {
        if (string.IsNullOrEmpty(deviceId))
        {
            throw new ArgumentNullException(nameof(deviceId));
        }

        if (string.IsNullOrEmpty(message))
        {
            throw new ArgumentNullException(nameof(message));
        }

        // 获取或创建设备的日志缓冲区
        var buffer = _deviceLogs.GetOrAdd(deviceId, _ => new CircularBuffer<DeviceLogEntry>(LogCapacityPerDevice));

        // 生成日志ID
        long logId;
        lock (_idLock)
        {
            logId = _nextLogId++;
        }

        // 创建日志条目
        var logEntry = new DeviceLogEntry
        {
            Id = logId,
            DeviceId = deviceId,
            Level = level,
            Message = message,
            Exception = exception?.ToString(),
            Timestamp = DateTime.Now,
            Data = data
        };

        // 添加到缓冲区
        buffer.Add(logEntry);

        // 记录到系统日志
        _logger.Log(level, exception, "[{DeviceId}] {Message}", deviceId, message);
    }

    /// <summary>
    /// 获取设备日志（最近N条，从新到旧）
    /// </summary>
    public List<DeviceLogEntry> GetLogs(string deviceId, int limit = 100)
    {
        if (string.IsNullOrEmpty(deviceId))
        {
            throw new ArgumentNullException(nameof(deviceId));
        }

        if (!_deviceLogs.TryGetValue(deviceId, out var buffer))
        {
            return new List<DeviceLogEntry>();
        }

        return buffer.GetRecent(limit);
    }

    /// <summary>
    /// 获取所有设备日志（最近N条，从新到旧）
    /// </summary>
    public List<DeviceLogEntry> GetAllLogs(int limit = 100)
    {
        var allLogs = new List<DeviceLogEntry>();

        foreach (var kvp in _deviceLogs)
        {
            var logs = kvp.Value.GetRecent(limit);
            allLogs.AddRange(logs);
        }

        // 按时间倒序排序
        return allLogs.OrderByDescending(log => log.Timestamp).Take(limit).ToList();
    }

    /// <summary>
    /// 清空设备日志
    /// </summary>
    public void ClearLogs(string deviceId)
    {
        if (string.IsNullOrEmpty(deviceId))
        {
            throw new ArgumentNullException(nameof(deviceId));
        }

        if (_deviceLogs.TryGetValue(deviceId, out var buffer))
        {
            buffer.Clear();
            _logger.LogInformation("已清空设备日志: DeviceId={DeviceId}", deviceId);
        }
    }

    /// <summary>
    /// 清空所有设备日志
    /// </summary>
    public void ClearAllLogs()
    {
        foreach (var kvp in _deviceLogs)
        {
            kvp.Value.Clear();
        }

        _logger.LogInformation("已清空所有设备日志");
    }

    /// <summary>
    /// 获取设备日志统计信息
    /// </summary>
    public Dictionary<string, int> GetLogStatistics()
    {
        return _deviceLogs.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.Count
        );
    }
}

