using Localization.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreMOB.Models;
using SchoolCoreMOB.IServices;
using Activity = System.Diagnostics.Activity;

namespace SchoolCoreMOB.Controllers
{
    public class SyllabusController : Controller
    {
        private readonly ICallService _CallService;
        private LanguageService _localization;       
        public SyllabusController(ICallService CallService, LanguageService localization)
        {
            _CallService = CallService;
            _localization = localization;
        }
        public IActionResult Index()
        {
            if (!HttpContext.Session.Keys.Contains("UserInfo"))
            {
                return RedirectToAction("Logout", "Login");
            }
            return View();
        }

       
    }
}