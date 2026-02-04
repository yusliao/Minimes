using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minimes.Application.Interfaces;

namespace Minimes.Infrastructure.Hardware;

/// <summary>
/// WiFi电子秤服务 - 支持HTTP API、WebSocket、TCP协议
/// 支持A&D、Mettler、凯丰等品牌的WiFi电子秤
/// </summary>
public class WiFiScaleService : IScaleService
{
    private readonly ILogger<WiFiScaleService> _logger;
    private readonly WiFiScaleConfiguration _config;
    private readonly HttpClient _httpClient;

    private bool _isConnected = false;
    private decimal _currentWeight = 0;
    private decimal _lastStableWeight = 0;
    private DateTime _lastWeightChangeTime = DateTime.Now;

    private CancellationTokenSource? _readingCts;
    private Task? _readingTask;

    public decimal CurrentWeight => _currentWeight;
    public bool IsConnected => _isConnected;

    public event EventHandler<WeightChangedEventArgs>? WeightChanged;
    public event EventHandler<ScaleErrorEventArgs>? ErrorOccurred;

    public WiFiScaleService(
        ILogger<WiFiScaleService> logger,
        IOptions<WiFiScaleConfiguration> config,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _config = config.Value;

        // 从HttpClientFactory创建HttpClient
        _httpClient = httpClientFactory.CreateClient();

        // 配置HttpClient
        var timeout = TimeSpan.FromMilliseconds(_config.RequestTimeoutMs);
        _httpClient.Timeout = timeout;
    }

    /// <summary>
    /// 连接到WiFi电子秤
    /// </summary>
    public async Task<bool> ConnectAsync()
    {
        try
        {
            if (_isConnected)
            {
                _logger.LogWarning("WiFi电子秤已经连接，无需重复连接");
                return true;
            }

            _logger.LogInformation("正在连接WiFi电子秤: {IpAddress}:{Port}", _config.IpAddress, _config.Port);

            // 测试连接：发起一个轻量级HTTP请求
            var testUrl = BuildUrl("/");
            var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(_config.ConnectionTimeoutMs));

            try
            {
                var response = await _httpClient.GetAsync(testUrl, HttpCompletionOption.ResponseHeadersRead, cts.Token);

                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound)
                {
                    _isConnected = true;
                    _logger.LogInformation("WiFi电子秤连接成功: {IpAddress}:{Port}", _config.IpAddress, _config.Port);
                    return true;
                }
                else
                {
                    var errorMsg = $"WiFi电子秤连接失败，HTTP状态码: {response.StatusCode}";
                    _logger.LogError(errorMsg);
                    OnError(errorMsg);
                    return false;
                }
            }
            catch (OperationCanceledException)
            {
                var timeoutMsg = $"连接超时（{_config.ConnectionTimeoutMs}ms）- 请检查IP地址和网络连接";
                _logger.LogError(timeoutMsg);
                OnError(timeoutMsg);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WiFi电子秤连接异常");
            OnError($"连接失败: {ex.Message}", ex);
            return false;
        }
    }

    /// <summary>
    /// 断开连接
    /// </summary>
    public async Task DisconnectAsync()
    {
        try
        {
            StopReading();
            _isConnected = false;
            _logger.LogInformation("WiFi电子秤已断开连接");
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "断开电子秤连接时发生错误");
            OnError($"断开连接失败: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// 开始读取重量数据
    /// </summary>
    public void StartReading()
    {
        if (!_isConnected)
        {
            _logger.LogWarning("WiFi电子秤未连接，无法开始读取");
            return;
        }

        if (_readingTask != null)
        {
            _logger.LogWarning("已经在读取数据中");
            return;
        }

        _readingCts = new CancellationTokenSource();
        _readingTask = Task.Run(() => ReadDataLoop(_readingCts.Token), _readingCts.Token);
        _logger.LogInformation("开始读取WiFi电子秤数据");
    }

    /// <summary>
    /// 停止读取数据
    /// </summary>
    public void StopReading()
    {
        try
        {
            _readingCts?.Cancel();
            _readingTask?.Wait(TimeSpan.FromSeconds(2));
            _readingCts?.Dispose();
            _readingCts = null;
            _readingTask = null;
            _logger.LogInformation("停止读取WiFi电子秤数据");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "停止读取数据时发生错误");
        }
    }

    /// <summary>
    /// 去皮（清零）
    /// </summary>
    public async Task TareAsync()
    {
        try
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("WiFi电子秤未连接");
            }

            _logger.LogInformation("执行去皮操作，当前重量: {Weight}g", _currentWeight);

            // 发送去皮命令到电子秤
            var tareUrl = BuildUrl(_config.TareApiPath);
            var response = await _httpClient.PostAsJsonAsync(tareUrl, new { });

            if (response.IsSuccessStatusCode)
            {
                // 清零本地数据
                _currentWeight = 0;
                _lastStableWeight = 0;
                OnWeightChanged(0, true);
                _logger.LogInformation("去皮成功");
            }
            else
            {
                _logger.LogWarning("去皮失败，电子秤返回状态码: {StatusCode}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "去皮操作失败");
            OnError($"去皮失败: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// 数据读取循环
    /// </summary>
    private async Task ReadDataLoop(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && _isConnected)
        {
            try
            {
                // 向电子秤发起HTTP请求获取重量
                var weightUrl = BuildUrl(_config.WeightApiPath);
                var response = await _httpClient.GetAsync(weightUrl, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var weightData = await response.Content.ReadFromJsonAsync<WeightResponse>(cancellationToken: cancellationToken);

                    if (weightData != null)
                    {
                        // 使用兼容方法获取实际数据（支持多种格式）
                        var weightValue = weightData.GetActualWeight();

                        if (weightValue.HasValue)
                        {
                            decimal weight = weightValue.Value;

                            // 单位转换：如果是kg则转换为克
                            string? unit = weightData.GetActualUnit();
                            if (!string.IsNullOrEmpty(unit) && unit.ToUpper() == "KG")
                            {
                                weight *= 1000;
                            }

                            // 检查是否稳定
                            bool isStable = weightData.GetActualStable();

                            UpdateWeight(weight, isStable);

                            _logger.LogDebug("收到电子秤数据: {Weight}g, 稳定: {Stable}", weight, isStable);
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("获取重量失败，HTTP状态码: {StatusCode}", response.StatusCode);
                    OnError($"获取重量失败: HTTP {response.StatusCode}");
                }

                // 等待一段时间后进行下一次读取
                await Task.Delay(_config.ReadIntervalMs, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // 正常取消，不记录错误
                break;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "网络请求异常");
                OnError($"网络请求异常: {ex.Message}", ex);

                // 网络异常后延迟重试
                await Task.Delay(1000, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "读取电子秤数据时发生异常");
                OnError($"读取数据异常: {ex.Message}", ex);

                // 发生错误后延迟重试
                await Task.Delay(1000, cancellationToken);
            }
        }

        _logger.LogInformation("WiFi电子秤数据读取循环已结束");
    }

    /// <summary>
    /// 更新重量数据
    /// </summary>
    private void UpdateWeight(decimal newWeight, bool hardwareStable = true)
    {
        var oldWeight = _currentWeight;
        _currentWeight = newWeight;

        // 判断是否稳定：软件级稳定性检测
        bool isStable = hardwareStable;

        if (hardwareStable)
        {
            // 如果硬件已标记为稳定，直接信任
            _lastStableWeight = _currentWeight;
            _lastWeightChangeTime = DateTime.Now;
        }
        else
        {
            // 如果硬件未标记为稳定，进行软件稳定性检测
            if (Math.Abs(_currentWeight - _lastStableWeight) <= _config.StableWeightTolerance)
            {
                var elapsed = DateTime.Now - _lastWeightChangeTime;
                if (elapsed.TotalMilliseconds >= _config.StableThresholdMs)
                {
                    isStable = true;
                    _lastStableWeight = _currentWeight;
                }
            }
            else
            {
                _lastWeightChangeTime = DateTime.Now;
                _lastStableWeight = _currentWeight;
                isStable = false;
            }
        }

        // 触发事件
        OnWeightChanged(_currentWeight, isStable);
    }

    /// <summary>
    /// 构建完整的URL
    /// </summary>
    private string BuildUrl(string path)
    {
        var scheme = "http"; // WiFi电子秤通常使用HTTP
        var host = _config.IpAddress;
        var port = _config.Port;

        if (port == 80)
        {
            return $"{scheme}://{host}{path}";
        }
        else
        {
            return $"{scheme}://{host}:{port}{path}";
        }
    }

    /// <summary>
    /// 触发重量变化事件
    /// </summary>
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

    /// <summary>
    /// 触发错误事件
    /// </summary>
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
        _httpClient?.Dispose();
    }

    /// <summary>
    /// HTTP API返回的重量数据格式
    /// 支持多种格式：
    /// 1. A&D格式: { "weight": 12.5, "unit": "kg", "stable": true }
    /// 2. 凯丰格式: { "data": { "value": 12.5, "unit": "kg", "stable": 1 } }
    /// 3. 通用格式: { "w": 12.5, "u": "kg", "s": true }
    /// </summary>
    private class WeightResponse
    {
        /// <summary>
        /// 重量值
        /// </summary>
        [JsonPropertyName("weight")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? Weight { get; set; }

        /// <summary>
        /// 简写：w
        /// </summary>
        [JsonPropertyName("w")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? W { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        [JsonPropertyName("unit")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Unit { get; set; }

        /// <summary>
        /// 简写：u
        /// </summary>
        [JsonPropertyName("u")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? U { get; set; }

        /// <summary>
        /// 是否稳定
        /// </summary>
        [JsonPropertyName("stable")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Stable { get; set; }

        /// <summary>
        /// 简写：s
        /// </summary>
        [JsonPropertyName("s")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? S { get; set; }

        /// <summary>
        /// 嵌套数据结构（某些电子秤使用）
        /// </summary>
        [JsonPropertyName("data")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public WeightData? Data { get; set; }

        /// <summary>
        /// 获取实际的重量值（兼容多种格式）
        /// </summary>
        public decimal? GetActualWeight()
        {
            return Weight ?? W ?? Data?.GetActualWeight();
        }

        /// <summary>
        /// 获取实际的单位（兼容多种格式）
        /// </summary>
        public string? GetActualUnit()
        {
            return Unit ?? U ?? Data?.Unit ?? Data?.U;
        }

        /// <summary>
        /// 获取实际的稳定状态（兼容多种格式）
        /// </summary>
        public bool GetActualStable()
        {
            if (Stable.HasValue) return Stable.Value;
            if (S.HasValue) return S.Value;
            if (Data?.Stable.HasValue == true) return Data.Stable!.Value;
            if (Data?.S.HasValue == true) return Data.S!.Value;
            return true; // 默认为稳定
        }

        /// <summary>
        /// 嵌套数据结构
        /// </summary>
        public class WeightData
        {
            [JsonPropertyName("value")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public decimal? Value { get; set; }

            [JsonPropertyName("v")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public decimal? V { get; set; }

            [JsonPropertyName("unit")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string? Unit { get; set; }

            [JsonPropertyName("u")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string? U { get; set; }

            [JsonPropertyName("stable")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public bool? Stable { get; set; }

            [JsonPropertyName("s")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public bool? S { get; set; }

            public decimal? GetActualWeight()
            {
                return Value ?? V;
            }
        }
    }
}
