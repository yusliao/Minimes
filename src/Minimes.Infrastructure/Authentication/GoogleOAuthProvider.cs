using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Minimes.Application.Authentication;

namespace Minimes.Infrastructure.Authentication;

/// <summary>
/// Google OAuth提供商实现
/// </summary>
public class GoogleOAuthProvider : IOAuthProvider
{
    private readonly string _clientId;
    private readonly string _clientSecret;
    private const string AuthorizationUrl = "https://accounts.google.com/o/oauth2/v2/auth";
    private const string TokenUrl = "https://oauth2.googleapis.com/token";
    private const string UserInfoUrl = "https://www.googleapis.com/oauth2/v2/userinfo";

    public string ProviderName => "google";

    public GoogleOAuthProvider(IConfiguration configuration)
    {
        _clientId = configuration["OAuth:Google:ClientId"] ?? throw new InvalidOperationException("Google ClientId not configured");
        _clientSecret = configuration["OAuth:Google:ClientSecret"] ?? throw new InvalidOperationException("Google ClientSecret not configured");
    }

    public string GetAuthorizationUrl(string redirectUrl, string state)
    {
        var scopes = Uri.EscapeDataString("openid profile email");
        return $"{AuthorizationUrl}?client_id={_clientId}&redirect_uri={Uri.EscapeDataString(redirectUrl)}&response_type=code&scope={scopes}&state={state}";
    }

    public async Task<OAuthUserInfo> GetUserInfoAsync(string code, string redirectUrl)
    {
        using var client = new HttpClient();

        // 用授权码交换access_token
        var request = new Dictionary<string, string>
        {
            { "client_id", _clientId },
            { "client_secret", _clientSecret },
            { "code", code },
            { "grant_type", "authorization_code" },
            { "redirect_uri", redirectUrl }
        };

        var tokenResponse = await client.PostAsync(TokenUrl, new FormUrlEncodedContent(request));
        var tokenContent = await tokenResponse.Content.ReadAsStringAsync();

        using var tokenDoc = JsonDocument.Parse(tokenContent);
        var accessToken = tokenDoc.RootElement.GetProperty("access_token").GetString();

        if (string.IsNullOrEmpty(accessToken))
            throw new Exception("Failed to get Google access token");

        // 用access_token获取用户信息
        var userRequest = new HttpRequestMessage(HttpMethod.Get, UserInfoUrl);
        userRequest.Headers.Add("Authorization", $"Bearer {accessToken}");

        var userResponse = await client.SendAsync(userRequest);
        var userContent = await userResponse.Content.ReadAsStringAsync();

        using var userDoc = JsonDocument.Parse(userContent);
        var sub = userDoc.RootElement.GetProperty("id").GetString();
        var name = userDoc.RootElement.TryGetProperty("name", out var nameProp) ? nameProp.GetString() : null;
        var picture = userDoc.RootElement.TryGetProperty("picture", out var pictureProp) ? pictureProp.GetString() : null;

        return new OAuthUserInfo
        {
            ProviderUserId = sub ?? string.Empty,
            Name = name,
            Avatar = picture
        };
    }
}
