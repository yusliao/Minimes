using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Minimes.Web.Pages.Account;

/// <summary>
/// 登出页面 - 处理用户登出
/// </summary>
public class LogoutModel : PageModel
{
    public async Task<IActionResult> OnGetAsync()
    {
        // 执行登出
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // 重定向到登录页
        return RedirectToPage("/Account/Login");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // 执行登出
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // 重定向到登录页
        return RedirectToPage("/Account/Login");
    }
}
