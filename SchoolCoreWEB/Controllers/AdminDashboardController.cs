using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreWEB.IServices;

namespace SchoolCoreWEB.Controllers
{

    public class AdminDashboardController : Controller
    {
        private readonly ICallService _CallService;
        private LanguageService _localization;
        string _userparam;
        private string _strtoken;
        public AdminDashboardController(ICallService CallService, LanguageService localization)
        {
            _CallService = CallService;
            _localization = localization;
        }
        public async Task<ActionResult> Index()
        {
            if (!HttpContext.Session.Keys.Contains("UserInfo"))
            {
                return RedirectToAction("Logout", "Login");
            }
            return View();
        }     
       
    }
}