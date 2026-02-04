namespace Minimes.Infrastructure.Devices.Abstractions;

using Microsoft.Extensions.Logging;
using Minimes.Infrastructure.Devices.Models;
using Minimes.Infrastructure.Devices.Models.EventArgs;
using System.Collections.Concurrent;
using System.Diagnostics;

/// <summary>
/// 设备适配器抽象基类 - 实现IDevice接口的通用逻辑
///
/// 设计原则：
/// 1. 模板方法模式：定义算法骨架，子类实现具体步骤
/// 2. 线程安全：使用SemaphoreSlim保护状态变更
/// 3. 资源管理：正确实现IDisposable模式
/// 4. 事件驱动：提供统一的事件触发机制
/// 5. 演示模式：内置模拟数据生成支持
/// </summary>
/// <typeparam name="TData">设备数据类型</typeparam>
public abstract class DeviceAdapter<TData> : IDevice<TData> where TData : class
{
    #region 字段和属性

    protected readonly ILogger Logger;
    protected readonly DeviceConfiguration Configuration;

    // 线程安全锁
    private readonly SemaphoreSlim _stateLock = new(1, 1);
    private readonly SemaphoreSlim _operationLock = new(1, 1);

    // 设备状态
    private DeviceStatus _status;

    // 数据采集任务
    private CancellationTokenSource? _dataLoopCts;
    private Task? _dataLoopTask;

    // 健康监控
    private readonly ConcurrentQueue<DateTime> _dataTimestamps = new();
    private readonly Stopwatch _uptimeStopwatch = new();
    private long _sequenceNumber;

    // 重连相关
    private int _reconnectAttempts;
    private DateTime? _lastReconnectAttempt;

    // 数据过滤
    private TData? _lastData;
    private DateTime? _lastDataTime;

    // 资源释放标志
    private bool _disposed;

    /// <inheritdoc/>
    public string DeviceId => Configuration.DeviceId;

    /// <inheritdoc/>
    public DeviceMetadata Metadata { get; protected set; }

    /// <inheritdoc/>
    public DeviceStatus Status
    {
        get
        {
            _stateLock.Wait();
            try
            {
                return _status.Clone();
            }
            finally
            {
                _stateLock.Release();
            }
        }
    }

    /// <inheritdoc/>
    public bool IsConnected => _status.State == DeviceState.Connected
                                || _status.State == DeviceState.Running;

    /// <inheritdoc/>
    public bool IsRunning => _status.State == DeviceState.Running;

    #endregion

    #region 事件

    /// <inheritdoc/>
    public event EventHandler<DeviceDataEventArgs<TData>>? DataReceived;

    /// <inheritdoc/>
    public event EventHandler<DeviceStatusEventArgs>? StatusChanged;

    /// <inheritdoc/>
    public event EventHandler<DeviceErrorEventArgs>? ErrorOccurred;

    #endregion

    #region 构造函数

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志记录器</param>
    /// <param name="configuration">设备配置</param>
    protected DeviceAdapter(ILogger logger, DeviceConfiguration configuration)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        _status = new DeviceStatus
        {
            State = DeviceState.Uninitialized,
            Description = "设备已创建，等待连接"
        };

        // 初始化元数据（调用子类实现）
        Metadata = CreateMetadata();

        Logger.LogInformation(
            "设备适配器已创建: DeviceId={DeviceId}, Type={DeviceType}, Protocol={Protocol}",
            DeviceId, Configuration.DeviceType, Configuration.ProtocolType);
    }

    #endregion

    #region 公共方法 - 连接管理

    /// <inheritdoc/>
    public async Task<bool> ConnectAsync(CancellationToken cancellationToken = default)
    {
        await _operationLock.WaitAsync(cancellationToken);
        try
        {
            if (IsConnected)
            {
                Logger.LogWarning("设备已连接，无需重复连接: {DeviceId}", DeviceId);
                return true;
            }

            Logger.LogInformation("开始连接设备: {DeviceId}", DeviceId);
            await UpdateStatusAsync(DeviceState.Connecting, "正在连接设备...");

            try
            {
                using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                timeoutCts.CancelAfter(Configuration.ConnectionTimeoutMs);

                var connected = await OnConnectAsync(timeoutCts.Token);

                if (connected)
                {
                    await UpdateStatusAsync(DeviceState.Connected, "设备连接成功");
                    _reconnectAttempts = 0;
                    Logger.LogInformation("设备连接成功: {DeviceId}", DeviceId);
                    return true;
                }
                else
                {
                    await UpdateStatusAsync(DeviceState.Disconnected, "设备连接失败");
                    Logger.LogWarning("设备连接失败: {DeviceId}", DeviceId);
                    await TryAutoReconnectAsync(cancellationToken);
                    return false;
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                Logger.LogWarning("设备连接被取消: {DeviceId}", DeviceId);
                await UpdateStatusAsync(DeviceState.Disconnected, "连接操作被取消");
                throw;
            }
            catch (OperationCanceledException)
            {
                var errorMsg = $"设备连接超时（{Configuration.ConnectionTimeoutMs}ms）";
                Logger.LogError("{ErrorMsg}: {DeviceId}", errorMsg, DeviceId);
                await UpdateStatusAsync(DeviceState.Error, errorMsg);
                RaiseErrorEvent(errorMsg, ErrorSeverity.Error, isRecoverable: true);
                await TryAutoReconnectAsync(cancellationToken);
                return false;
            }
            catch (Exception ex)
            {
                var errorMsg = $"设备连接异常: {ex.Message}";
                Logger.LogError(ex, "{ErrorMsg}: {DeviceId}", errorMsg, DeviceId);
                await UpdateStatusAsync(DeviceState.Error, errorMsg);
                RaiseErrorEvent(errorMsg, ErrorSeverity.Error, ex, isRecoverable: true);
                await TryAutoReconnectAsync(cancellationToken);
                return false;
            }
        }
        finally
        {
            _operationLock.Release();
        }
    }

    /// <inheritdoc/>
    public async Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        await _operationLock.WaitAsync(cancellationToken);
        try
        {
            if (!IsConnected)
            {
                Logger.LogWarning("设备未连接，无需断开: {DeviceId}", DeviceId);
                return;
            }

            Logger.LogInformation("开始断开设备: {DeviceId}", DeviceId);

            if (IsRunning)
            {
                await StopAsync(cancellationToken);
            }

            try
            {
                await OnDisconnectAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "断开设备时发生异常: {DeviceId}", DeviceId);
                RaiseErrorEvent($"断开设备异常: {ex.Message}", ErrorSeverity.Warning, ex);
            }

            await UpdateStatusAsync(DeviceState.Disconnected, "设备已断开连接");
            _uptimeStopwatch.Stop();
            Logger.LogInformation("设备已断开: {DeviceId}", DeviceId);
        }
        finally
        {
            _operationLock.Release();
        }
    }

    #endregion

    #region 公共方法 - 数据采集

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        await _operationLock.WaitAsync(cancellationToken);
        try
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException($"设备未连接，无法启动数据采集: {DeviceId}");
            }

            if (IsRunning)
            {
                Logger.LogWarning("设备已在运行中: {DeviceId}", DeviceId);
                return;
            }

            Logger.LogInformation("启动设备数据采集: {DeviceId}", DeviceId);

            await OnStartAsync(cancellationToken);
            StartDataLoop();

            await UpdateStatusAsync(DeviceState.Running, "设备正在采集数据");
            _uptimeStopwatch.Restart();

            Logger.LogInformation("设备数据采集已启动: {DeviceId}", DeviceId);
        }
        finally
        {
            _operationLock.Release();
        }
    }

    /// <inheritdoc/>
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        await _operationLock.WaitAsync(cancellationToken);
        try
        {
            if (!IsRunning)
            {
                Logger.LogWarning("设备未在运行中: {DeviceId}", DeviceId);
                return;
            }

            Logger.LogInformation("停止设备数据采集: {DeviceId}", DeviceId);

            StopDataLoop();
            try
            {
                await OnStopAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "停止设备时发生异常: {DeviceId}", DeviceId);
                RaiseErrorEvent($"停止设备异常: {ex.Message}", ErrorSeverity.Warning, ex);
            }

            await UpdateStatusAsync(DeviceState.Connected, "设备已停止数据采集");
            _uptimeStopwatch.Stop();

            Logger.LogInformation("设备数据采集已停止: {DeviceId}", DeviceId);
        }
        finally
        {
            _operationLock.Release();
        }
    }

    #endregion

    #region 公共方法 - 命令执行和健康检查

    /// <inheritdoc/>
    public async Task<TResult?> ExecuteCommandAsync<TResult>(string command, object? parameters = null, CancellationToken cancellationToken = default)
    {
        if (!IsConnected)
        {
            throw new InvalidOperationException($"设备未连接，无法执行命令: {DeviceId}");
        }

        Logger.LogInformation("执行设备命令: {DeviceId}, Command={Command}", DeviceId, command);

        try
        {
            return await OnExecuteCommandAsync<TResult>(command, parameters, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "执行设备命令失败: {DeviceId}, Command={Command}", DeviceId, command);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<DeviceHealth> GetHealthAsync(CancellationToken cancellationToken = default)
    {
        var health = new DeviceHealth
        {
            Status = HealthStatus.Unknown,
            Description = "正在检查设备健康状态...",
            CheckedAt = DateTime.Now
        };

        try
        {
            var checkItems = new List<HealthCheckItem>();

            checkItems.Add(new HealthCheckItem
            {
                Name = "连接状态",
                Passed = IsConnected,
                Description = IsConnected ? "设备已连接" : "设备未连接",
                Value = _status.State.ToString()
            });

            checkItems.Add(new HealthCheckItem
            {
                Name = "运行状态",
                Passed = IsRunning,
                Description = IsRunning ? "设备正在运行" : "设备未运行",
                Value = IsRunning
            });

            var dataRate = CalculateDataRate();
            checkItems.Add(new HealthCheckItem
            {
                Name = "数据速率",
                Passed = dataRate > 0,
                Description = $"每秒接收 {dataRate:F2} 条数据",
                Value = dataRate
            });

            var errorRate = CalculateErrorRate();
            checkItems.Add(new HealthCheckItem
            {
                Name = "错误率",
                Passed = errorRate < 0.1,
                Description = $"错误率: {errorRate:P2}",
                Value = errorRate
            });

            var customCheckItems = await OnHealthCheckAsync(cancellationToken);
            if (customCheckItems != null)
            {
                checkItems.AddRange(customCheckItems);
            }

            health.CheckItems = checkItems;
            health.DataRatePerSecond = dataRate;
            health.ErrorRate = errorRate;

            var failedCount = checkItems.Count(c => !c.Passed);
            if (failedCount == 0)
            {
                health.Status = HealthStatus.Healthy;
                health.Description = "设备运行正常";
            }
            else if (failedCount <= 2)
            {
                health.Status = HealthStatus.Degraded;
                health.Description = $"设备部分功能受限（{failedCount}项检查失败）";
            }
            else
            {
                health.Status = HealthStatus.Unhealthy;
                health.Description = $"设备运行异常（{failedCount}项检查失败）";
            }

            return health;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "健康检查失败: {DeviceId}", DeviceId);
            health.Status = HealthStatus.Unknown;
            health.Description = $"健康检查失败: {ex.Message}";
            return health;
        }
    }

    #endregion

    #region 私有方法 - 状态管理

    /// <summary>
    /// 更新设备状态
    /// </summary>
    private async Task UpdateStatusAsync(DeviceState newState, string description)
    {
        await _stateLock.WaitAsync();
        try
        {
            var oldState = _status.State;
            _status.State = newState;
            _status.Description = description;

            if (newState == DeviceState.Connected)
            {
                _status.ConnectedAt = DateTime.Now;
            }
            else if (newState == DeviceState.Running)
            {
                _status.StartedAt = DateTime.Now;
            }

            if (oldState != newState)
            {
                RaiseStatusChangedEvent(oldState, newState, description);
            }
        }
        finally
        {
            _stateLock.Release();
        }
    }

    /// <summary>
    /// 增加错误计数
    /// </summary>
    private async Task IncrementErrorCountAsync()
    {
        await _stateLock.WaitAsync();
        try
        {
            _status.ErrorCount++;
            _status.LastErrorAt = DateTime.Now;
        }
        finally
        {
            _stateLock.Release();
        }
    }

    /// <summary>
    /// 更新数据统计
    /// </summary>
    private async Task UpdateDataStatisticsAsync()
    {
        await _stateLock.WaitAsync();
        try
        {
            _status.TotalDataReceived++;
            _dataTimestamps.Enqueue(DateTime.Now);

            while (_dataTimestamps.TryPeek(out var timestamp) && (DateTime.Now - timestamp).TotalSeconds > 10)
            {
                _dataTimestamps.TryDequeue(out _);
            }
        }
        finally
        {
            _stateLock.Release();
        }
    }

    #endregion

    #region 私有方法 - 事件触发

    /// <summary>
    /// 触发数据接收事件
    /// </summary>
    protected void RaiseDataReceivedEvent(TData data, bool isDemo = false)
    {
        var eventArgs = new DeviceDataEventArgs<TData>(DeviceId, data, isDemo, Interlocked.Increment(ref _sequenceNumber));
        DataReceived?.Invoke(this, eventArgs);
    }

    /// <summary>
    /// 触发状态变化事件
    /// </summary>
    private void RaiseStatusChangedEvent(DeviceState oldState, DeviceState newState, string description)
    {
        var eventArgs = new DeviceStatusEventArgs(DeviceId, oldState, newState, description);
        StatusChanged?.Invoke(this, eventArgs);
    }

    /// <summary>
    /// 触发错误事件
    /// </summary>
    protected void RaiseErrorEvent(string message, ErrorSeverity severity, Exception? exception = null, bool isRecoverable = true)
    {
        var eventArgs = new DeviceErrorEventArgs(DeviceId, message, severity, exception, isRecoverable);
        ErrorOccurred?.Invoke(this, eventArgs);
    }

    #endregion

    #region 私有方法 - 数据循环

    /// <summary>
    /// 启动真实数据读取循环
    /// </summary>
    private void StartDataLoop()
    {
        if (_dataLoopTask != null)
        {
            Logger.LogWarning("数据读取循环已在运行中: {DeviceId}", DeviceId);
            return;
        }

        _dataLoopCts = new CancellationTokenSource();
        _dataLoopTask = Task.Run(() => DataReadLoop(_dataLoopCts.Token), _dataLoopCts.Token);
        Logger.LogDebug("数据读取循环已启动: {DeviceId}", DeviceId);
    }

    /// <summary>
    /// 停止数据读取循环
    /// </summary>
    private void StopDataLoop()
    {
        try
        {
            _dataLoopCts?.Cancel();
            _dataLoopTask?.Wait(TimeSpan.FromSeconds(5));
            _dataLoopCts?.Dispose();
            _dataLoopCts = null;
            _dataLoopTask = null;
            Logger.LogDebug("数据读取循环已停止: {DeviceId}", DeviceId);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "停止数据读取循环时发生错误: {DeviceId}", DeviceId);
        }
    }

    #endregion

    /// <summary>
    /// 数据读取循环
    /// </summary>
    private async Task DataReadLoop(CancellationToken cancellationToken)
    {
        Logger.LogInformation("进入数据读取循环: {DeviceId}", DeviceId);

        while (!cancellationToken.IsCancellationRequested && IsConnected)
        {
            try
            {
                var data = await OnReadDataAsync(cancellationToken);

                if (data != null && ShouldAcceptData(data))
                {
                    RaiseDataReceivedEvent(data);
                    await UpdateDataStatisticsAsync();
                }

                await Task.Delay(Configuration.ReadIntervalMs, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "数据读取循环发生异常: {DeviceId}", DeviceId);
                RaiseErrorEvent($"数据读取异常: {ex.Message}", ErrorSeverity.Error, ex);
                await IncrementErrorCountAsync();

                try
                {
                    await Task.Delay(1000, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        Logger.LogInformation("退出数据读取循环: {DeviceId}", DeviceId);
    }

    #region 私有方法 - 重连逻辑

    /// <summary>
    /// 尝试自动重连
    /// </summary>
    private async Task TryAutoReconnectAsync(CancellationToken cancellationToken)
    {
        var policy = Configuration.ReconnectionPolicy;
        if (policy == null || !policy.Enabled)
        {
            return;
        }

        if (_reconnectAttempts >= policy.MaxRetries)
        {
            Logger.LogWarning("已达到最大重连次数: {DeviceId}, Attempts={Attempts}", DeviceId, _reconnectAttempts);
            return;
        }

        _reconnectAttempts++;
        _lastReconnectAttempt = DateTime.Now;

        var delay = policy.RetryIntervalMs;
        if (policy.UseExponentialBackoff)
        {
            delay = Math.Min(policy.RetryIntervalMs * (int)Math.Pow(2, _reconnectAttempts - 1), policy.MaxBackoffMs);
        }

        Logger.LogInformation("将在 {Delay}ms 后尝试第 {Attempt} 次重连: {DeviceId}", delay, _reconnectAttempts, DeviceId);

        try
        {
            await Task.Delay(delay, cancellationToken);
            _ = ConnectAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            Logger.LogWarning("重连操作被取消: {DeviceId}", DeviceId);
        }
    }

    #endregion

    #region 私有方法 - 数据过滤和统计

    /// <summary>
    /// 判断是否接受数据
    /// </summary>
    private bool ShouldAcceptData(TData data)
    {
        var filter = Configuration.DataFilter;
        if (filter == null || !filter.Enabled)
        {
            return true;
        }

        // 时间间隔过滤
        if (filter.MinIntervalMs.HasValue && _lastDataTime.HasValue)
        {
            var elapsed = (DateTime.Now - _lastDataTime.Value).TotalMilliseconds;
            if (elapsed < filter.MinIntervalMs.Value)
            {
                return false;
            }
        }

        // 调用子类自定义过滤逻辑
        if (!OnFilterData(data, filter))
        {
            return false;
        }

        _lastData = data;
        _lastDataTime = DateTime.Now;
        return true;
    }

    /// <summary>
    /// 计算数据速率
    /// </summary>
    private double CalculateDataRate()
    {
        if (_dataTimestamps.IsEmpty)
        {
            return 0;
        }

        var count = _dataTimestamps.Count;
        if (count < 2)
        {
            return 0;
        }

        var timestamps = _dataTimestamps.ToArray();
        var duration = (timestamps[^1] - timestamps[0]).TotalSeconds;
        return duration > 0 ? count / duration : 0;
    }

    /// <summary>
    /// 计算错误率
    /// </summary>
    private double CalculateErrorRate()
    {
        var total = _status.TotalDataReceived + _status.ErrorCount;
        return total > 0 ? (double)_status.ErrorCount / total : 0;
    }

    #endregion

    #region 抽象方法 - 子类必须实现

    /// <summary>
    /// 连接设备（子类实现）
    /// </summary>
    protected abstract Task<bool> OnConnectAsync(CancellationToken cancellationToken);

    /// <summary>
    /// 断开设备连接（子类实现）
    /// </summary>
    protected abstract Task OnDisconnectAsync(CancellationToken cancellationToken);

    /// <summary>
    /// 启动数据采集（子类实现）
    /// </summary>
    protected abstract Task OnStartAsync(CancellationToken cancellationToken);

    /// <summary>
    /// 停止数据采集（子类实现）
    /// </summary>
    protected abstract Task OnStopAsync(CancellationToken cancellationToken);

    /// <summary>
    /// 读取设备数据（子类实现）
    /// </summary>
    protected abstract Task<TData?> OnReadDataAsync(CancellationToken cancellationToken);

    /// <summary>
    /// 创建设备元数据（子类实现）
    /// </summary>
    protected abstract DeviceMetadata CreateMetadata();

    /// <summary>
    /// 执行设备命令（子类可选实现）
    /// </summary>
    protected virtual Task<TResult?> OnExecuteCommandAsync<TResult>(string command, object? parameters, CancellationToken cancellationToken)
    {
        throw new NotSupportedException($"设备不支持命令: {command}");
    }

    /// <summary>
    /// 自定义健康检查（子类可选实现）
    /// </summary>
    protected virtual Task<List<HealthCheckItem>?> OnHealthCheckAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<List<HealthCheckItem>?>(null);
    }

    /// <summary>
    /// 数据过滤逻辑（子类可选实现）
    /// </summary>
    protected virtual bool OnFilterData(TData data, DataFilterConfiguration filter)
    {
        return true;
    }

    #endregion

    #region IDisposable实现

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 释放资源（受保护方法）
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            try
            {
                if (IsRunning)
                {
                    StopAsync().GetAwaiter().GetResult();
                }

                if (IsConnected)
                {
                    DisconnectAsync().GetAwaiter().GetResult();
                }

                _stateLock?.Dispose();
                _operationLock?.Dispose();
                _dataLoopCts?.Dispose();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "释放设备资源时发生错误: {DeviceId}", DeviceId);
            }
        }

        _disposed = true;
    }

    #endregion
}
