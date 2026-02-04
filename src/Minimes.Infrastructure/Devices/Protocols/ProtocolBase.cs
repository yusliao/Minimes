namespace Minimes.Infrastructure.Devices.Protocols;

using Microsoft.Extensions.Logging;
using Minimes.Infrastructure.Devices.Models;

/// <summary>
/// 协议基类 - 实现IProtocol接口的通用逻辑
///
/// 设计原则：
/// 1. 模板方法模式：定义算法骨架，子类实现具体步骤
/// 2. 线程安全：使用SemaphoreSlim保护连接状态
/// 3. 资源管理：正确实现IDisposable模式
/// 4. 超时控制：支持连接和读写超时
/// </summary>
/// <typeparam name="TRaw">原始数据类型（byte[], string等）</typeparam>
/// <typeparam name="TData">业务数据类型（WeightData, DewPointData等）</typeparam>
public abstract class ProtocolBase<TRaw, TData> : IProtocol<TRaw, TData> where TData : class
{
    #region 字段和属性

    protected readonly ILogger Logger;
    protected readonly DeviceConfiguration Configuration;

    // 线程安全锁
    private readonly SemaphoreSlim _connectionLock = new(1, 1);

    // 连接状态
    private bool _isConnected;

    // 资源释放标志
    private bool _disposed;

    /// <inheritdoc/>
    public abstract string ProtocolName { get; }

    /// <inheritdoc/>
    public bool IsConnected => _isConnected;

    #endregion

    #region 构造函数

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志记录器</param>
    /// <param name="configuration">设备配置</param>
    protected ProtocolBase(ILogger logger, DeviceConfiguration configuration)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        Logger.LogInformation(
            "协议已创建: Protocol={Protocol}, DeviceId={DeviceId}",
            ProtocolName, Configuration.DeviceId);
    }

    #endregion

    #region 公共方法 - 连接管理

    /// <inheritdoc/>
    public async Task<bool> ConnectAsync(CancellationToken cancellationToken = default)
    {
        await _connectionLock.WaitAsync(cancellationToken);
        try
        {
            if (_isConnected)
            {
                Logger.LogWarning("协议已连接，无需重复连接: {Protocol}", ProtocolName);
                return true;
            }

            Logger.LogInformation("开始连接协议: {Protocol}", ProtocolName);

            try
            {
                using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                timeoutCts.CancelAfter(Configuration.ConnectionTimeoutMs);

                var connected = await OnConnectAsync(timeoutCts.Token);

                if (connected)
                {
                    _isConnected = true;
                    Logger.LogInformation("协议连接成功: {Protocol}", ProtocolName);
                    return true;
                }
                else
                {
                    Logger.LogWarning("协议连接失败: {Protocol}", ProtocolName);
                    return false;
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                Logger.LogWarning("协议连接被取消: {Protocol}", ProtocolName);
                throw;
            }
            catch (OperationCanceledException)
            {
                Logger.LogError("协议连接超时（{Timeout}ms）: {Protocol}",
                    Configuration.ConnectionTimeoutMs, ProtocolName);
                return false;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "协议连接异常: {Protocol}", ProtocolName);
                return false;
            }
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    /// <inheritdoc/>
    public async Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        await _connectionLock.WaitAsync(cancellationToken);
        try
        {
            if (!_isConnected)
            {
                Logger.LogWarning("协议未连接，无需断开: {Protocol}", ProtocolName);
                return;
            }

            Logger.LogInformation("开始断开协议: {Protocol}", ProtocolName);

            try
            {
                await OnDisconnectAsync(cancellationToken);
                _isConnected = false;
                Logger.LogInformation("协议已断开: {Protocol}", ProtocolName);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "断开协议时发生异常: {Protocol}", ProtocolName);
                _isConnected = false;
            }
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    #endregion

    #region 公共方法 - 读写操作

    /// <inheritdoc/>
    public async Task<TRaw?> ReadAsync(CancellationToken cancellationToken = default)
    {
        if (!_isConnected)
        {
            throw new InvalidOperationException($"协议未连接，无法读取数据: {ProtocolName}");
        }

        try
        {
            return await OnReadAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "读取数据时发生异常: {Protocol}", ProtocolName);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task WriteAsync(TRaw data, CancellationToken cancellationToken = default)
    {
        if (!_isConnected)
        {
            throw new InvalidOperationException($"协议未连接，无法写入数据: {ProtocolName}");
        }

        try
        {
            await OnWriteAsync(data, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "写入数据时发生异常: {Protocol}", ProtocolName);
            throw;
        }
    }

    /// <inheritdoc/>
    public TData? Parse(TRaw rawData)
    {
        try
        {
            return OnParse(rawData);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "解析数据时发生异常: {Protocol}", ProtocolName);
            return null;
        }
    }

    /// <inheritdoc/>
    public TRaw? Serialize(TData data)
    {
        try
        {
            return OnSerialize(data);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "序列化数据时发生异常: {Protocol}", ProtocolName);
            return default;
        }
    }

    #endregion

    #region 抽象方法 - 子类必须实现

    /// <summary>
    /// 建立连接（子类实现）
    /// </summary>
    protected abstract Task<bool> OnConnectAsync(CancellationToken cancellationToken);

    /// <summary>
    /// 断开连接（子类实现）
    /// </summary>
    protected abstract Task OnDisconnectAsync(CancellationToken cancellationToken);

    /// <summary>
    /// 读取原始数据（子类实现）
    /// </summary>
    protected abstract Task<TRaw?> OnReadAsync(CancellationToken cancellationToken);

    /// <summary>
    /// 写入原始数据（子类实现）
    /// </summary>
    protected abstract Task OnWriteAsync(TRaw data, CancellationToken cancellationToken);

    /// <summary>
    /// 解析原始数据为业务数据（子类实现）
    /// </summary>
    protected abstract TData? OnParse(TRaw rawData);

    /// <summary>
    /// 序列化业务数据为原始数据（子类实现）
    /// </summary>
    protected abstract TRaw? OnSerialize(TData data);

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
                if (_isConnected)
                {
                    DisconnectAsync().GetAwaiter().GetResult();
                }

                _connectionLock?.Dispose();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "释放协议资源时发生错误: {Protocol}", ProtocolName);
            }
        }

        _disposed = true;
    }

    #endregion
}
