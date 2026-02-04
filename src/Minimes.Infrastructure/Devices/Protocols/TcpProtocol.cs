namespace Minimes.Infrastructure.Devices.Protocols;

using Microsoft.Extensions.Logging;
using Minimes.Infrastructure.Devices.Models;
using System.Net.Sockets;

/// <summary>
/// TCP协议抽象基类
///
/// 设计原则：
/// 1. 单一职责：只负责TCP通信，不负责数据解析
/// 2. 模板方法模式：定义TCP通信骨架，子类实现数据解析
/// 3. 线程安全：使用锁保护TCP操作
/// </summary>
/// <typeparam name="TData">业务数据类型</typeparam>
public abstract class TcpProtocol<TData> : ProtocolBase<byte[], TData> where TData : class
{
    #region 字段和属性

    private TcpClient? _tcpClient;
    private NetworkStream? _networkStream;
    private readonly SemaphoreSlim _readLock = new(1, 1);
    private readonly SemaphoreSlim _writeLock = new(1, 1);

    /// <inheritdoc/>
    public override string ProtocolName => "TCP";

    /// <summary>
    /// 主机地址
    /// </summary>
    protected string Host { get; private set; } = string.Empty;

    /// <summary>
    /// 端口号
    /// </summary>
    protected int Port { get; private set; } = 502;

    /// <summary>
    /// 连接超时（毫秒）
    /// </summary>
    protected int ConnectTimeout { get; private set; } = 5000;

    /// <summary>
    /// 读取超时（毫秒）
    /// </summary>
    protected int ReadTimeout { get; private set; } = 1000;

    /// <summary>
    /// 写入超时（毫秒）
    /// </summary>
    protected int WriteTimeout { get; private set; } = 1000;

    #endregion

    #region 构造函数

    /// <summary>
    /// 构造函数
    /// </summary>
    protected TcpProtocol(ILogger logger, DeviceConfiguration configuration)
        : base(logger, configuration)
    {
        LoadTcpSettings(configuration);
    }

    /// <summary>
    /// 从配置中加载TCP参数
    /// </summary>
    private void LoadTcpSettings(DeviceConfiguration configuration)
    {
        var settings = configuration.ProtocolSettings;

        if (settings.TryGetValue("Host", out var host))
        {
            Host = host?.ToString() ?? string.Empty;
        }

        if (settings.TryGetValue("Port", out var port))
        {
            Port = Convert.ToInt32(port);
        }

        if (settings.TryGetValue("ConnectTimeout", out var connectTimeout))
        {
            ConnectTimeout = Convert.ToInt32(connectTimeout);
        }

        if (settings.TryGetValue("ReadTimeout", out var readTimeout))
        {
            ReadTimeout = Convert.ToInt32(readTimeout);
        }

        if (settings.TryGetValue("WriteTimeout", out var writeTimeout))
        {
            WriteTimeout = Convert.ToInt32(writeTimeout);
        }

        Logger.LogInformation(
            "TCP参数已加载: Host={Host}, Port={Port}, ConnectTimeout={ConnectTimeout}ms",
            Host, Port, ConnectTimeout);
    }

    #endregion

    #region 连接管理

    /// <inheritdoc/>
    protected override async Task<bool> OnConnectAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(Host))
        {
            Logger.LogError("主机地址为空，无法连接");
            return false;
        }

        try
        {
            _tcpClient = new TcpClient();

            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(ConnectTimeout);

            await _tcpClient.ConnectAsync(Host, Port, timeoutCts.Token);

            _networkStream = _tcpClient.GetStream();
            _networkStream.ReadTimeout = ReadTimeout;
            _networkStream.WriteTimeout = WriteTimeout;

            Logger.LogInformation("TCP连接成功: {Host}:{Port}", Host, Port);
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "TCP连接失败: {Host}:{Port}", Host, Port);
            _networkStream?.Dispose();
            _tcpClient?.Dispose();
            _networkStream = null;
            _tcpClient = null;
            return false;
        }
    }

    /// <inheritdoc/>
    protected override async Task OnDisconnectAsync(CancellationToken cancellationToken)
    {
        if (_tcpClient == null)
        {
            return;
        }

        try
        {
            await Task.Run(() =>
            {
                _networkStream?.Close();
                _tcpClient?.Close();
            }, cancellationToken);

            Logger.LogInformation("TCP连接已断开: {Host}:{Port}", Host, Port);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "断开TCP连接失败: {Host}:{Port}", Host, Port);
        }
        finally
        {
            _networkStream?.Dispose();
            _tcpClient?.Dispose();
            _networkStream = null;
            _tcpClient = null;
        }
    }

    #endregion

    #region 数据读写

    /// <inheritdoc/>
    protected override async Task<byte[]?> OnReadAsync(CancellationToken cancellationToken)
    {
        if (_networkStream == null || !_networkStream.CanRead)
        {
            throw new InvalidOperationException("TCP连接未建立或不可读");
        }

        await _readLock.WaitAsync(cancellationToken);
        try
        {
            if (_networkStream.DataAvailable)
            {
                var buffer = new byte[1024];
                var bytesRead = await _networkStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);

                if (bytesRead > 0)
                {
                    var result = new byte[bytesRead];
                    Array.Copy(buffer, result, bytesRead);
                    return result;
                }
            }

            return null;
        }
        finally
        {
            _readLock.Release();
        }
    }

    /// <inheritdoc/>
    protected override async Task OnWriteAsync(byte[] data, CancellationToken cancellationToken)
    {
        if (_networkStream == null || !_networkStream.CanWrite)
        {
            throw new InvalidOperationException("TCP连接未建立或不可写");
        }

        if (data == null || data.Length == 0)
        {
            return;
        }

        await _writeLock.WaitAsync(cancellationToken);
        try
        {
            await _networkStream.WriteAsync(data, 0, data.Length, cancellationToken);
            await _networkStream.FlushAsync(cancellationToken);
        }
        finally
        {
            _writeLock.Release();
        }
    }

    #endregion

    #region 资源释放

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _networkStream?.Dispose();
            _tcpClient?.Dispose();
            _readLock?.Dispose();
            _writeLock?.Dispose();
        }

        base.Dispose(disposing);
    }

    #endregion
}

