using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using SchoolCoreWEB.IServices;
using SchoolCoreWEB.Models;


namespace SchoolCoreWEB.Controllers
{    
    public class LeaveReportController : Controller
    {
        private readonly ICallService _CallService;
        private LanguageService _localization;
        string _userparam;
        private string _strtoken;
        public LeaveReportController(ICallService CallService, LanguageService localization)
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
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/leaverequest/LeaveReport", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<LeaveReport>(httpResponseMessage);
            return View(model);
        }
       
        public async Task<ActionResult> GetSPLeaveReport(int sessionYearId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/leaverequest/LeaveReport/" + sessionYearId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<SpLeaveReportResult>(httpResponseMessage);
            return Json(model);

        }
   
    }
}
