using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Minimes.Web.Controllers;

/// <summary>
/// Language switching controller - handles culture cookie setting
/// </summary>
[Route("[controller]/[action]")]
public class CultureController : Controller
{
    private readonly ILogger<CultureController> _logger;

    public CultureController(ILogger<CultureController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Set user's preferred culture and redirect back
    /// </summary>
    [HttpGet]
    public IActionResult Set(string culture, string returnUrl)
    {
        _logger.LogInformation("CultureController.Set called: culture={Culture}, returnUrl={ReturnUrl}", culture, returnUrl);

        // Validate culture
        if (string.IsNullOrEmpty(culture))
        {
            culture = "zh-CN";
        }

        var cookieValue = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture));
        _logger.LogInformation("Setting cookie: {CookieName}={CookieValue}", CookieRequestCultureProvider.DefaultCookieName, cookieValue);

        // Set culture cookie
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            cookieValue,
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                SameSite = SameSiteMode.Lax,
                Path = "/"
            }
        );

        // Redirect back to the original page
        if (string.IsNullOrEmpty(returnUrl))
        {
            returnUrl = "/";
        }

        _logger.LogInformation("Redirecting to: {ReturnUrl}", returnUrl);
        return LocalRedirect(returnUrl);
    }
}
