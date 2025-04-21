using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolCoreMOB.IServices;
using SchoolCoreMOB.Models;

namespace SchoolCoreMOB.Controllers
{    
    public class TeacherPerformanceController : Controller
    {
        private readonly ICallService _CallService;
        private LanguageService _localization;
        string _userparam;  
        private string _strtoken;
        public TeacherPerformanceController(ICallService CallService, LanguageService localization)
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
