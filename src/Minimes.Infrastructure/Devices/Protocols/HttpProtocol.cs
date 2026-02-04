namespace Minimes.Infrastructure.Devices.Protocols;

using Microsoft.Extensions.Logging;
using Minimes.Infrastructure.Devices.Models;
using System.Net.Http;
using System.Text;

/// <summary>
/// HTTP协议抽象基类
///
/// 设计原则：
/// 1. 单一职责：只负责HTTP通信，不负责数据解析
/// 2. 模板方法模式：定义HTTP通信骨架，子类实现数据解析
/// 3. 无状态：HTTP是无状态协议，"连接"只是验证URL可达性
/// </summary>
/// <typeparam name="TData">业务数据类型</typeparam>
public abstract class HttpProtocol<TData> : ProtocolBase<string, TData> where TData : class
{
    #region 字段和属性

    private readonly HttpClient _httpClient;

    /// <inheritdoc/>
    public override string ProtocolName => "HTTP";

    /// <summary>
    /// 基础URL
    /// </summary>
    protected string BaseUrl { get; private set; } = string.Empty;

    /// <summary>
    /// 读取端点（GET请求）
    /// </summary>
    protected string ReadEndpoint { get; private set; } = string.Empty;

    /// <summary>
    /// 写入端点（POST请求）
    /// </summary>
    protected string WriteEndpoint { get; private set; } = string.Empty;

    /// <summary>
    /// 请求超时（毫秒）
    /// </summary>
    protected int RequestTimeout { get; private set; } = 5000;

    #endregion

    #region 构造函数

    /// <summary>
    /// 构造函数
    /// </summary>
    protected HttpProtocol(ILogger logger, DeviceConfiguration configuration)
        : base(logger, configuration)
    {
        _httpClient = new HttpClient();
        LoadHttpSettings(configuration);
    }

    /// <summary>
    /// 从配置中加载HTTP参数
    /// </summary>
    private void LoadHttpSettings(DeviceConfiguration configuration)
    {
        var settings = configuration.ProtocolSettings;

        if (settings.TryGetValue("BaseUrl", out var baseUrl))
        {
            BaseUrl = baseUrl?.ToString() ?? string.Empty;
        }

        if (settings.TryGetValue("ReadEndpoint", out var readEndpoint))
        {
            ReadEndpoint = readEndpoint?.ToString() ?? "/data";
        }

        if (settings.TryGetValue("WriteEndpoint", out var writeEndpoint))
        {
            WriteEndpoint = writeEndpoint?.ToString() ?? "/command";
        }

        if (settings.TryGetValue("Timeout", out var timeout))
        {
            RequestTimeout = Convert.ToInt32(timeout);
        }

        _httpClient.Timeout = TimeSpan.FromMilliseconds(RequestTimeout);

        Logger.LogInformation(
            "HTTP参数已加载: BaseUrl={BaseUrl}, ReadEndpoint={ReadEndpoint}, WriteEndpoint={WriteEndpoint}, Timeout={Timeout}ms",
            BaseUrl, ReadEndpoint, WriteEndpoint, RequestTimeout);
    }

    #endregion

    #region 连接管理

    /// <inheritdoc/>
    protected override async Task<bool> OnConnectAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(BaseUrl))
        {
            Logger.LogError("BaseUrl为空，无法连接");
            return false;
        }

        try
        {
            var url = $"{BaseUrl}{ReadEndpoint}";
            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                Logger.LogInformation("HTTP连接验证成功: {Url}", url);
                return true;
            }
            else
            {
                Logger.LogWarning("HTTP连接验证失败: {Url}, StatusCode={StatusCode}", url, response.StatusCode);
                return false;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "HTTP连接验证异常: {BaseUrl}", BaseUrl);
            return false;
        }
    }

    /// <inheritdoc/>
    protected override Task OnDisconnectAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("HTTP协议断开连接（无需操作）");
        return Task.CompletedTask;
    }

    #endregion

    #region 数据读写

    /// <inheritdoc/>
    protected override async Task<string?> OnReadAsync(CancellationToken cancellationToken)
    {
        try
        {
            var url = $"{BaseUrl}{ReadEndpoint}";
            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                return content;
            }
            else
            {
                Logger.LogWarning("HTTP GET请求失败: {Url}, StatusCode={StatusCode}", url, response.StatusCode);
                return null;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "HTTP GET请求异常: {BaseUrl}{ReadEndpoint}", BaseUrl, ReadEndpoint);
            throw;
        }
    }

    /// <inheritdoc/>
    protected override async Task OnWriteAsync(string data, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(data))
        {
            return;
        }

        try
        {
            var url = $"{BaseUrl}{WriteEndpoint}";
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                Logger.LogWarning("HTTP POST请求失败: {Url}, StatusCode={StatusCode}", url, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "HTTP POST请求异常: {BaseUrl}{WriteEndpoint}", BaseUrl, WriteEndpoint);
            throw;
        }
    }

    #endregion

    #region 资源释放

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _httpClient?.Dispose();
        }

        base.Dispose(disposing);
    }

    #endregion
}
