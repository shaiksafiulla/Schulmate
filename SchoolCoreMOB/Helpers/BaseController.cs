using Localization.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

public class BaseController : Controller
{
    private LanguageService _localization;

    public BaseController(LanguageService localization)
    {
        _localization = localization;
    }

    public IActionResult ChangeLanguage(string culture)
    {
        Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions() { Expires = DateTimeOffset.UtcNow.AddYears(1) });

        return Redirect(Request.Headers["Referer"].ToString());
    }

    //public IActionResult ChangeLanguage(string culture)
    //{
    //    Response.Cookies.Append(
    //        CookieRequestCultureProvider.DefaultCookieName,
    //        CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
    //        new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
    //    );

    //    return RedirectToAction(nameof(Index));
    //}
}