using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolCoreMOB.IServices;
using SchoolCoreMOB.Models;

namespace SchoolCoreMOB.Controllers
{
    public class NotificationController : Controller
    {
        private readonly ICallService _CallService;
        private LanguageService _localization;
        string _userparam;  
        private string _strtoken;
        public NotificationController(ICallService CallService, LanguageService localization)
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
        [Route("Notification/SetUnReadCount")]
        public IActionResult SetUnReadCount(int newCount)
        {
            HttpContext.Session.SetString("noticount", newCount.ToString());
            return Json(new { success = true });
        }
        //public async Task<ActionResult> GetAll()
        //{
        //    _userparam = HttpContext.Session.GetString("UserParam");
        //    _strtoken = HttpContext.Session.GetString("strtoken");
        //    var httpResponseMessage = await _CallService.GetApi("api/notification", _strtoken, _userparam);
        //    var model = await _CallService.FetchObjectFromResponse<List<VNotification>>(httpResponseMessage);
        //    return Json(model);
        //}
    }
}