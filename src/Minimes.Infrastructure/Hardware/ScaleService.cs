using System.IO.Ports;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minimes.Application.Interfaces;

namespace Minimes.Infrastructure.Hardware;

/// <summary>
/// 电子秤串口通信服务 - 支持多种协议
/// </summary>
public class ScaleService : IScaleService
{
    private readonly ILogger<ScaleService> _logger;
    private readonly ScaleConfiguration _config;
    private SerialPort? _serialPort;
    private CancellationTokenSource? _readingCts;
    private Task? _readingTask;

    private decimal _currentWeight;
    private decimal _lastStableWeight;
    private DateTime _lastWeightChangeTime = DateTime.Now;

    public decimal CurrentWeight => _currentWeight;
    public bool IsConnected => _serialPort?.IsOpen ?? false;

    public event EventHandler<WeightChangedEventArgs>? WeightChanged;
    public event EventHandler<ScaleErrorEventArgs>? ErrorOccurred;

    public ScaleService(ILogger<ScaleService> logger, IOptions<ScaleConfiguration> config)
    {
        _logger = logger;
        _config = config.Value;
    }

    public async Task<bool> ConnectAsync()
    {
        try
        {
            if (IsConnected)
            {
                _logger.LogWarning("电子秤已经连接，无需重复连接");
                return true;
            }

            _serialPort = new SerialPort
            {
                PortName = _config.PortName,
                BaudRate = _config.BaudRate,
                DataBits = _config.DataBits,
                StopBits = _config.StopBits,
                Parity = _config.Parity,
                ReadTimeout = 500,
                WriteTimeout = 500
            };

            _serialPort.Open();
            _logger.LogInformation("电子秤连接成功: {Port} @ {BaudRate}", _config.PortName, _config.BaudRate);

            return await Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "电子秤连接失败: {Port}", _config.PortName);
            OnError($"连接失败: {ex.Message}", ex);
            return false;
        }
    }

    public async Task DisconnectAsync()
    {
        try
        {
            StopReading();

            if (_serialPort?.IsOpen == true)
            {
                _serialPort.Close();
                _logger.LogInformation("电子秤已断开连接");
            }

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "断开电子秤连接时发生错误");
            OnError($"断开连接失败: {ex.Message}", ex);
        }
    }

    public void StartReading()
    {
        if (!IsConnected)
        {
            _logger.LogWarning("电子秤未连接，无法开始读取");
            return;
        }

        if (_readingTask != null)
        {
            _logger.LogWarning("已经在读取数据中");
            return;
        }

        _readingCts = new CancellationTokenSource();
        _readingTask = Task.Run(() => ReadDataLoop(_readingCts.Token), _readingCts.Token);
        _logger.LogInformation("开始读取电子秤数据");
    }

    public void StopReading()
    {
        try
        {
            _readingCts?.Cancel();
            _readingTask?.Wait(TimeSpan.FromSeconds(2));
            _readingCts?.Dispose();
            _readingCts = null;
            _readingTask = null;
            _logger.LogInformation("停止读取电子秤数据");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "停止读取数据时发生错误");
        }
    }

    public async Task TareAsync()
    {
        try
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("电子秤未连接");
            }

            // 不同品牌的去皮命令不同，这里使用通用方式：记录当前重量作为皮重
            _logger.LogInformation("执行去皮操作，当前重量: {Weight}g", _currentWeight);

            // 简单实现：直接将当前读数清零
            _currentWeight = 0;
            _lastStableWeight = 0;

            OnWeightChanged(_currentWeight, true);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "去皮操作失败");
            OnError($"去皮失败: {ex.Message}", ex);
        }
    }

    private async Task ReadDataLoop(CancellationToken cancellationToken)
    {
        var buffer = new StringBuilder();

        while (!cancellationToken.IsCancellationRequested && IsConnected && _serialPort != null)
        {
            try
            {
                // 读取一行数据
                if (_serialPort.BytesToRead > 0)
                {
                    string data = _serialPort.ReadLine().Trim();

                    if (!string.IsNullOrWhiteSpace(data))
                    {
                        _logger.LogDebug("收到电子秤数据: {Data}", data);

                        // 解析重量数据
                        decimal? weight = ParseWeight(data);
                        if (weight.HasValue)
                        {
                            UpdateWeight(weight.Value);
                        }
                    }
                }

                await Task.Delay(_config.ReadIntervalMs, cancellationToken);
            }
            catch (TimeoutException)
            {
                // 读取超时是正常的，继续下一次读取
                continue;
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "读取电子秤数据时发生错误");
                OnError($"读取数据错误: {ex.Message}", ex);
                await Task.Delay(1000, cancellationToken); // 发生错误后延迟重试
            }
        }
    }

    private decimal? ParseWeight(string data)
    {
        try
        {
            // 通用格式解析：提取数字（支持小数点和负号）
            // 常见格式：
            // - "ST,GS,+00012.5kg"  (Toledo)
            // - "12.5 kg"           (通用)
            // - "+0012.5"           (通用数字)

            var match = Regex.Match(data, @"[+-]?\d+\.?\d*");
            if (match.Success && decimal.TryParse(match.Value, out decimal weight))
            {
                // 检查单位，如果是kg则转换为克
                if (data.ToUpper().Contains("KG"))
                {
                    weight *= 1000;
                }

                return weight;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "解析重量数据失败: {Data}", data);
            return null;
        }
    }

    private void UpdateWeight(decimal newWeight)
    {
        var oldWeight = _currentWeight;
        _currentWeight = newWeight;

        // 判断是否稳定：重量变化小于容差且持续一定时间
        bool isStable = false;
        if (Math.Abs(_currentWeight - _lastStableWeight) <= _config.StableWeightTolerance)
        {
            var elapsed = DateTime.Now - _lastWeightChangeTime;
            if (elapsed.TotalMilliseconds >= _config.StableThresholdMs)
            {
                isStable = true;
            }
        }
        else
        {
            _lastWeightChangeTime = DateTime.Now;
            _lastStableWeight = _currentWeight;
        }

        // 触发事件
        OnWeightChanged(_currentWeight, isStable);
    }

    private void OnWeightChanged(decimal weight, bool isStable)
    {
        try
        {
            WeightChanged?.Invoke(this, new WeightChangedEventArgs
            {
                Weight = weight,
                Unit = "g",
                IsStable = isStable,
                Timestamp = DateTime.Now
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "触发WeightChanged事件时发生错误");
        }
    }

    private void OnError(string message, Exception? exception = null)
    {
        try
        {
            ErrorOccurred?.Invoke(this, new ScaleErrorEventArgs
            {
                ErrorMessage = message,
                Exception = exception,
                Timestamp = DateTime.Now
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "触发ErrorOccurred事件时发生错误");
        }
    }

    public void Dispose()
    {
        StopReading();
        _serialPort?.Dispose();
        _readingCts?.Dispose();
    }
}
