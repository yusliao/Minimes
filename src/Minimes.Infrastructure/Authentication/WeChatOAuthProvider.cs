using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Minimes.Application.Authentication;

namespace Minimes.Infrastructure.Authentication;

/// <summary>
/// 微信OAuth提供商实现
/// </summary>
public class WeChatOAuthProvider : IOAuthProvider
{
    private readonly string _clientId;
    private readonly string _clientSecret;
    private const string AuthorizationUrl = "https://open.weixin.qq.com/connect/oauth2/authorize";
    private const string TokenUrl = "https://api.weixin.qq.com/sns/oauth2/access_token";
    private const string UserInfoUrl = "https://api.weixin.qq.com/sns/userinfo";

    public string ProviderName => "weixin";

    public WeChatOAuthProvider(IConfiguration configuration)
    {
        _clientId = configuration["OAuth:WeChat:ClientId"] ?? throw new InvalidOperationException("WeChat ClientId not configured");
        _clientSecret = configuration["OAuth:WeChat:ClientSecret"] ?? throw new InvalidOperationException("WeChat ClientSecret not configured");
    }

    public string GetAuthorizationUrl(string redirectUrl, string state)
    {
        return $"{AuthorizationUrl}?appid={_clientId}&redirect_uri={Uri.EscapeDataString(redirectUrl)}&response_type=code&scope=snsapi_userinfo&state={state}#wechat_redirect";
    }

    public async Task<OAuthUserInfo> GetUserInfoAsync(string code, string redirectUrl)
    {
        using var client = new HttpClient();

        // 第一步：用授权码交换access_token
        var tokenUrl = $"{TokenUrl}?appid={_clientId}&secret={_clientSecret}&code={code}&grant_type=authorization_code";
        var tokenResponse = await client.GetAsync(tokenUrl);
        var tokenContent = await tokenResponse.Content.ReadAsStringAsync();

        using var tokenDoc = JsonDocument.Parse(tokenContent);
        var accessToken = tokenDoc.RootElement.GetProperty("access_token").GetString();
        var openId = tokenDoc.RootElement.GetProperty("openid").GetString();

        if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(openId))
            throw new Exception("Failed to get WeChat access token");

        // 第二步：用access_token获取用户信息
        var userUrl = $"{UserInfoUrl}?access_token={accessToken}&openid={openId}&lang=zh_CN";
        var userResponse = await client.GetAsync(userUrl);
        var userContent = await userResponse.Content.ReadAsStringAsync();

        using var userDoc = JsonDocument.Parse(userContent);
        var nickName = userDoc.RootElement.GetProperty("nickname").GetString();
        var headImgUrl = userDoc.RootElement.GetProperty("headimgurl").GetString();

        return new OAuthUserInfo
        {
            ProviderUserId = openId ?? string.Empty,
            Name = nickName,
            Avatar = headImgUrl
        };
    }
}
