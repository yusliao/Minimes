using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Minimes.Application.DTOs.User;
using Minimes.Application.Interfaces;
using Minimes.Application.Resources;
using IAppAuthService = Minimes.Application.Interfaces.IAuthenticationService;

namespace Minimes.Web.Pages.Account;

/// <summary>
/// Login page - Razor Page handles HTTP POST requests
/// </summary>
public class LoginModel : PageModel
{
    private readonly IAppAuthService _authService;
    private readonly IScaleService _scaleService;
    private readonly IBarcodeScannerService _barcodeScannerService;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public LoginModel(
        IAppAuthService authService,
        IScaleService scaleService,
        IBarcodeScannerService barcodeScannerService,
        IStringLocalizer<SharedResource> localizer)
    {
        _authService = authService;
        _scaleService = scaleService;
        _barcodeScannerService = barcodeScannerService;
        _localizer = localizer;
    }

    [BindProperty]
    public string Username { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    public bool RememberMe { get; set; }

    [BindProperty]
    public string? ReturnUrl { get; set; }

    public string? ErrorMessage { get; set; }

    public void OnGet(string? returnUrl = null)
    {
        // If already logged in, redirect directly
        if (User.Identity?.IsAuthenticated == true)
        {
            Response.Redirect(returnUrl ?? "/");
            return;
        }

        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = _localizer["Login_ErrorEmpty"];
            return Page();
        }

        try
        {
            // Call authentication service
            var loginRequest = new LoginRequest
            {
                Username = Username,
                Password = Password,
                RememberMe = RememberMe
            };

            var response = await _authService.LoginAsync(loginRequest);

            if (response.Success && response.UserId.HasValue)
            {
                // Check if this is a demo account
                bool isDemoAccount = Username.Equals("demo", StringComparison.OrdinalIgnoreCase);

                // Create Claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, response.UserId.Value.ToString()),
                    new Claim(ClaimTypes.Name, response.Username ?? ""),
                    new Claim(ClaimTypes.Role, response.Role?.ToString() ?? ""),
                    new Claim("IsDemoMode", isDemoAccount.ToString())
                };

                // Create ClaimsIdentity
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Create ClaimsPrincipal
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                // Cookie authentication properties
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = RememberMe,
                    ExpiresUtc = RememberMe
                        ? DateTimeOffset.UtcNow.AddDays(30)
                        : DateTimeOffset.UtcNow.AddHours(8)
                };

                // Execute login
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    claimsPrincipal,
                    authProperties);

                // Redirect to target page
                return LocalRedirect(ReturnUrl ?? "/");
            }
            else
            {
                ErrorMessage = response.Message ?? _localizer["Login_ErrorFailed"];
                return Page();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"{_localizer["Login_ErrorException"]}{ex.Message}";
            return Page();
        }
    }
}
