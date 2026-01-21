using Microsoft.Extensions.Configuration;
using Minimes.Application.Authentication;

namespace Minimes.Infrastructure.Authentication;

/// <summary>
/// OAuth提供商工厂 - 创建对应的OAuth提供商实例
/// </summary>
public class OAuthProviderFactory
{
    private readonly IConfiguration _configuration;

    public OAuthProviderFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// 根据提供商名称获取OAuth提供商实例
    /// </summary>
    public IOAuthProvider GetProvider(string providerName)
    {
        return providerName.ToLower() switch
        {
            "weixin" => new WeChatOAuthProvider(_configuration),
            "google" => new GoogleOAuthProvider(_configuration),
            _ => throw new NotSupportedException($"不支持的OAuth提供商: {providerName}")
        };
    }
}
