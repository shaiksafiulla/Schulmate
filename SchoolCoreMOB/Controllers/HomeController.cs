using Localization.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreMOB.Models;
using SchoolCoreMOB.IServices;
using Activity = System.Diagnostics.Activity;

namespace SchoolCoreMOB.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICallService _CallService;
        private LanguageService _localization;
        private readonly IConfiguration _configuration;
        public HomeController(ICallService CallService, LanguageService localization, IConfiguration configuration)
        {
            _CallService = CallService;
            _localization = localization;
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            if (!HttpContext.Session.Keys.Contains("UserInfo"))
            {
                return RedirectToAction("Logout", "Login");
            }
            //ViewData["apiUrl"] = _configuration["ApiUrl"];
            //ViewData["strtoken"] = HttpContext.Session.GetString("strtoken");
            //ViewData["userParam"] = HttpContext.Session.GetString("UserParam");
            return View();
        }

        public IActionResult ChangeLanguage()
        {
            bool isright = System.Threading.Thread.CurrentThread.CurrentUICulture.TextInfo.IsRightToLeft;
            var culture = string.Empty;
            if (isright)
            {
                culture = "en-US";
            }
            else
            {
                culture = "ar-SA";
            }
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions() { Expires = DateTimeOffset.UtcNow.AddYears(1) });

            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}