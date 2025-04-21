using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolCoreMOB.IServices;
using SchoolCoreMOB.Models;

namespace SchoolCoreMOB.Controllers
{
   
    public class ChatBotController : Controller
    {
        string _userparam;
        string _strtoken;
        private readonly ICallService _CallService;
        private LanguageService _localization;
        public ChatBotController(ICallService CallService, LanguageService localization)
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
        //public async Task<ActionResult> GetTeacherTransTimeTable()
        //{
        //    _userparam = HttpContext.Session.GetString("UserParam");
        //    _strtoken = HttpContext.Session.GetString("strtoken");
        //    //var httpResponseMessage = await _CallService.GetApi("api/teacher/TransTimeTable/" + _userInfo.PersonId + "", _strtoken, _userparam);
        //    var httpResponseMessage = await _CallService.GetApi("api/teacher/TransTimeTable", _strtoken, _userparam);
        //    string data = await _CallService.FetchStringFromResponse(httpResponseMessage);
        //    return Json(data);
        //}
        //public async Task<ActionResult> GetTeacherTimeTableFromDB()
        //{
        //    _userparam = HttpContext.Session.GetString("UserParam");
        //    _strtoken = HttpContext.Session.GetString("strtoken");
        //    //var httpResponseMessage = await _CallService.GetApi("api/teacher/TimeTable/" + _userInfo.PersonId + "," + _userInfo.SessionYearId + "", _strtoken, _userparam);
        //    var httpResponseMessage = await _CallService.GetApi("api/teacher/TimeTable", _strtoken, _userparam);
        //    string data = await _CallService.FetchStringFromResponse(httpResponseMessage);
        //    return Json(data);
        //}
        //public async Task<ActionResult> GetSchedulesByTeacherId()
        //{
        //    _userparam = HttpContext.Session.GetString("UserParam");
        //    _strtoken = HttpContext.Session.GetString("strtoken");
        //    //var httpResponseMessage = await _CallService.GetApi("api/teacher/TeacherSchedule/" + _userInfo.PersonId + "", _strtoken, _userparam);
        //    var httpResponseMessage = await _CallService.GetApi("api/teacher/TeacherSchedule", _strtoken, _userparam);
        //    var data = await _CallService.FetchStringFromResponse(httpResponseMessage);           
        //    return RedirectToAction("TeacherSchedule", data);
        //    //return Json(data);

        //}


    }
}
