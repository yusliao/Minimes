namespace Minimes.Infrastructure.Devices.Protocols;

using Microsoft.Extensions.Logging;
using Minimes.Infrastructure.Devices.Models;
using System.IO.Ports;
using System.Text;

/// <summary>
/// 串口协议抽象基类
///
/// 设计原则：
/// 1. 单一职责：只负责串口通信，不负责数据解析
/// 2. 模板方法模式：定义串口通信骨架，子类实现数据解析
/// 3. 线程安全：使用锁保护串口操作
/// </summary>
/// <typeparam name="TData">业务数据类型</typeparam>
public abstract class SerialProtocol<TData> : ProtocolBase<byte[], TData> where TData : class
{
    #region 字段和属性

    private SerialPort? _serialPort;
    private readonly SemaphoreSlim _readLock = new(1, 1);
    private readonly SemaphoreSlim _writeLock = new(1, 1);

    /// <inheritdoc/>
    public override string ProtocolName => "Serial";

    /// <summary>
    /// 串口名称（如COM3）
    /// </summary>
    protected string PortName { get; private set; } = string.Empty;

    /// <summary>
    /// 波特率
    /// </summary>
    protected int BaudRate { get; private set; } = 9600;

    /// <summary>
    /// 数据位
    /// </summary>
    protected int DataBits { get; private set; } = 8;

    /// <summary>
    /// 校验位
    /// </summary>
    protected Parity Parity { get; private set; } = Parity.None;

    /// <summary>
    /// 停止位
    /// </summary>
    protected StopBits StopBits { get; private set; } = StopBits.One;

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
    protected SerialProtocol(ILogger logger, DeviceConfiguration configuration)
        : base(logger, configuration)
    {
        // 从配置中读取串口参数
        LoadSerialSettings(configuration);
    }

    /// <summary>
    /// 从配置中加载串口参数
    /// </summary>
    private void LoadSerialSettings(DeviceConfiguration configuration)
    {
        var settings = configuration.ProtocolSettings;

        if (settings.TryGetValue("PortName", out var portName))
        {
            PortName = portName?.ToString() ?? string.Empty;
        }

        if (settings.TryGetValue("BaudRate", out var baudRate))
        {
            BaudRate = Convert.ToInt32(baudRate);
        }

        if (settings.TryGetValue("DataBits", out var dataBits))
        {
            DataBits = Convert.ToInt32(dataBits);
        }

        if (settings.TryGetValue("Parity", out var parity))
        {
            Parity = Enum.Parse<Parity>(parity?.ToString() ?? "None");
        }

        if (settings.TryGetValue("StopBits", out var stopBits))
        {
            StopBits = Enum.Parse<StopBits>(stopBits?.ToString() ?? "One");
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
            "串口参数已加载: Port={Port}, BaudRate={BaudRate}, DataBits={DataBits}, Parity={Parity}, StopBits={StopBits}",
            PortName, BaudRate, DataBits, Parity, StopBits);
    }

    #endregion

    #region 连接管理

    /// <inheritdoc/>
    protected override async Task<bool> OnConnectAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(PortName))
        {
            Logger.LogError("串口名称为空，无法连接");
            return false;
        }

        try
        {
            _serialPort = new SerialPort(PortName, BaudRate, Parity, DataBits, StopBits)
            {
                ReadTimeout = ReadTimeout,
                WriteTimeout = WriteTimeout,
                Encoding = Encoding.UTF8
            };

            await Task.Run(() => _serialPort.Open(), cancellationToken);

            Logger.LogInformation("串口已打开: {Port}", PortName);
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "打开串口失败: {Port}", PortName);
            _serialPort?.Dispose();
            _serialPort = null;
            return false;
        }
    }

    /// <inheritdoc/>
    protected override async Task OnDisconnectAsync(CancellationToken cancellationToken)
    {
        if (_serialPort == null)
        {
            return;
        }

        try
        {
            await Task.Run(() =>
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                }
            }, cancellationToken);

            Logger.LogInformation("串口已关闭: {Port}", PortName);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "关闭串口失败: {Port}", PortName);
        }
        finally
        {
            _serialPort?.Dispose();
            _serialPort = null;
        }
    }

    #endregion

    #region 数据读写

    /// <inheritdoc/>
    protected override async Task<byte[]?> OnReadAsync(CancellationToken cancellationToken)
    {
        if (_serialPort == null || !_serialPort.IsOpen)
        {
            throw new InvalidOperationException("串口未打开");
        }

        await _readLock.WaitAsync(cancellationToken);
        try
        {
            if (_serialPort.BytesToRead > 0)
            {
                var buffer = new byte[_serialPort.BytesToRead];
                var bytesRead = await _serialPort.BaseStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);

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
        if (_serialPort == null || !_serialPort.IsOpen)
        {
            throw new InvalidOperationException("串口未打开");
        }

        if (data == null || data.Length == 0)
        {
            return;
        }

        await _writeLock.WaitAsync(cancellationToken);
        try
        {
            await _serialPort.BaseStream.WriteAsync(data, 0, data.Length, cancellationToken);
            await _serialPort.BaseStream.FlushAsync(cancellationToken);
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
            _serialPort?.Dispose();
            _readLock?.Dispose();
            _writeLock?.Dispose();
        }

        base.Dispose(disposing);
    }

    #endregion
}
