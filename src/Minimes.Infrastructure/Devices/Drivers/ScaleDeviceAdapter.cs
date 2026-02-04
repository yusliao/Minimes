namespace Minimes.Infrastructure.Devices.Drivers;

using Microsoft.Extensions.Logging;
using Minimes.Infrastructure.Devices.Abstractions;
using Minimes.Infrastructure.Devices.Models;
using Minimes.Infrastructure.Devices.Models.Data;
using System.IO.Ports;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// 电子秤设备适配器
///
/// 设计说明：
/// 1. 继承DeviceAdapter<WeightData>
/// 2. 复用ScaleService的串口通信逻辑
/// 3. 支持去皮功能（通过ExecuteCommandAsync）
/// 4. 支持演示模式
/// </summary>
public class ScaleDeviceAdapter : DeviceAdapter<WeightData>
{
    #region 字段

    private SerialPort? _serialPort;

    // 稳定性检测
    private decimal _lastStableWeight;
    private DateTime _lastWeightChangeTime = DateTime.Now;

    // 串口配置
    private string _portName = "COM3";
    private int _baudRate = 9600;
    private int _dataBits = 8;
    private Parity _parity = Parity.None;
    private StopBits _stopBits = StopBits.One;

    // 协议类型
    private string _protocol = "Generic";

    #endregion

    #region 构造函数

    /// <summary>
    /// 构造函数
    /// </summary>
    public ScaleDeviceAdapter(ILogger<ScaleDeviceAdapter> logger, DeviceConfiguration configuration)
        : base(logger, configuration)
    {
        LoadSerialSettings(configuration);
        Logger.LogInformation("电子秤适配器已创建: DeviceId={DeviceId}", DeviceId);
    }

    /// <summary>
    /// 从配置中加载串口参数
    /// </summary>
    private void LoadSerialSettings(DeviceConfiguration configuration)
    {
        var settings = configuration.DeviceSettings;

        if (settings.TryGetValue("PortName", out var portName))
        {
            _portName = portName?.ToString() ?? "COM3";
        }

        if (settings.TryGetValue("BaudRate", out var baudRate))
        {
            _baudRate = Convert.ToInt32(baudRate);
        }

        if (settings.TryGetValue("DataBits", out var dataBits))
        {
            _dataBits = Convert.ToInt32(dataBits);
        }

        if (settings.TryGetValue("Parity", out var parity))
        {
            _parity = Enum.Parse<Parity>(parity?.ToString() ?? "None");
        }

        if (settings.TryGetValue("StopBits", out var stopBits))
        {
            _stopBits = Enum.Parse<StopBits>(stopBits?.ToString() ?? "One");
        }

        if (settings.TryGetValue("Protocol", out var protocol))
        {
            _protocol = protocol?.ToString() ?? "Generic";
        }

        Logger.LogInformation(
            "串口参数已加载: Port={Port}, BaudRate={BaudRate}, Protocol={Protocol}",
            _portName, _baudRate, _protocol);
    }

    #endregion

    #region 抽象方法实现 - 连接管理

    /// <inheritdoc/>
    protected override Task<bool> OnConnectAsync(CancellationToken cancellationToken)
    {
        try
        {
            _serialPort = new SerialPort
            {
                PortName = _portName,
                BaudRate = _baudRate,
                DataBits = _dataBits,
                StopBits = _stopBits,
                Parity = _parity,
                ReadTimeout = 500,
                WriteTimeout = 500
            };

            _serialPort.Open();
            Logger.LogInformation("电子秤串口已打开: {Port}", _portName);
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "打开电子秤串口失败: {Port}", _portName);
            _serialPort?.Dispose();
            _serialPort = null;
            return Task.FromResult(false);
        }
    }

    /// <inheritdoc/>
    protected override Task OnDisconnectAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (_serialPort?.IsOpen == true)
            {
                _serialPort.Close();
                Logger.LogInformation("电子秤串口已关闭: {Port}", _portName);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "关闭电子秤串口失败: {Port}", _portName);
        }
        finally
        {
            _serialPort?.Dispose();
            _serialPort = null;
        }

        return Task.CompletedTask;
    }

    #endregion

    #region 抽象方法实现 - 数据采集

    /// <inheritdoc/>
    protected override Task OnStartAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("电子秤启动数据采集: DeviceId={DeviceId}", DeviceId);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    protected override Task OnStopAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("电子秤停止数据采集: DeviceId={DeviceId}", DeviceId);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    protected override async Task<WeightData?> OnReadDataAsync(CancellationToken cancellationToken)
    {
        if (_serialPort == null || !_serialPort.IsOpen)
        {
            return null;
        }

        try
        {
            if (_serialPort.BytesToRead > 0)
            {
                var buffer = new byte[_serialPort.BytesToRead];
                await _serialPort.BaseStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                var rawData = Encoding.ASCII.GetString(buffer);
                return ParseWeightData(rawData);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "读取电子秤数据失败: DeviceId={DeviceId}", DeviceId);
        }

        return null;
    }

    /// <summary>
    /// 解析重量数据（简化实现）
    /// </summary>
    private WeightData? ParseWeightData(string rawData)
    {
        try
        {
            var match = Regex.Match(rawData, @"([\d.]+)\s*([a-zA-Z]+)");
            if (match.Success)
            {
                var weight = decimal.Parse(match.Groups[1].Value);
                var unit = match.Groups[2].Value.ToLower();

                var weightUnit = unit switch
                {
                    "g" => WeightUnit.Gram,
                    "kg" => WeightUnit.Kilogram,
                    "lb" => WeightUnit.Pound,
                    _ => WeightUnit.Gram
                };

                return new WeightData
                {
                    Weight = weight,
                    Unit = weightUnit,
                    IsStable = CheckStability(weight),
                    IsNet = true
                };
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "解析重量数据失败: {RawData}", rawData);
        }

        return null;
    }

    /// <summary>
    /// 检查重量稳定性
    /// </summary>
    private bool CheckStability(decimal weight)
    {
        var diff = Math.Abs(weight - _lastStableWeight);
        if (diff < 0.5m)
        {
            return true;
        }

        _lastStableWeight = weight;
        return false;
    }

    #endregion

    #region 抽象方法实现 - 元数据

    /// <inheritdoc/>
    protected override DeviceMetadata CreateMetadata()
    {
        return new DeviceMetadata
        {
            DeviceType = "Scale",
            DeviceName = Configuration.DeviceName,
            Manufacturer = "Generic",
            Model = "Scale-1000",
            ProtocolType = Configuration.ProtocolType,
            DataType = typeof(WeightData).Name,
            Capabilities = new List<string> { "Weight", "Tare", "Stability" }
        };
    }

    #endregion

    #region 抽象方法实现 - 命令执行

    /// <inheritdoc/>
    protected override Task<TResult?> OnExecuteCommandAsync<TResult>(string command, object? parameters, CancellationToken cancellationToken) where TResult : default
    {
        if (command.Equals("Tare", StringComparison.OrdinalIgnoreCase))
        {
            Logger.LogInformation("执行去皮命令: DeviceId={DeviceId}", DeviceId);
            _lastStableWeight = 0;
            return Task.FromResult<TResult?>(default);
        }

        return base.OnExecuteCommandAsync<TResult>(command, parameters, cancellationToken);
    }

    #endregion
}

