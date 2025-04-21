using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolCoreMOB.IServices;
using SchoolCoreMOB.Models;

namespace SchoolCoreMOB.Controllers
{
   
    public class SectionController : Controller
    {
        string _userparam;
        string _strtoken;
        private readonly ICallService _CallService;
        private LanguageService _localization;
        public SectionController(ICallService CallService, LanguageService localization)
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
        //public async Task<ActionResult> GetSectionTransTimeTable(int Id)
        //{
        //    _userparam = HttpContext.Session.GetString("UserParam");
        //    _strtoken = HttpContext.Session.GetString("strtoken");
        //    var httpResponseMessage = await _CallService.GetApi("api/section/TransTimeTable/" + Id + "", _strtoken, _userparam);
        //    string data = await _CallService.FetchStringFromResponse(httpResponseMessage);
        //    return Json(data);
        //}
        //public async Task<ActionResult> GetSectionTimeTableFromDB(int Id)
        //{
        //    _userparam = HttpContext.Session.GetString("UserParam");
        //    _strtoken = HttpContext.Session.GetString("strtoken");
        //    var httpResponseMessage = await _CallService.GetApi("api/section/MbTimeTable/" + Id + "", _strtoken, _userparam);
        //    string data = await _CallService.FetchStringFromResponse(httpResponseMessage);
        //    return Json(data);
        //}
        //public async Task<IActionResult> GetSectionsByPersonnelId()
        //{
        //    _userparam = HttpContext.Session.GetString("UserParam");
        //    _strtoken = HttpContext.Session.GetString("strtoken");
        //    var httpResponseMessage = await _CallService.GetApi("api/Section/MbSection", _strtoken, _userparam);
        //    var model = await _CallService.FetchObjectFromResponse<List<MbSection>>(httpResponseMessage);
        //    return Json(model);
        //}
        //public async Task<IActionResult> GetClassesByPersonnelId()
        //{
        //    _userparam = HttpContext.Session.GetString("UserParam");
        //    _strtoken = HttpContext.Session.GetString("strtoken");
        //    var httpResponseMessage = await _CallService.GetApi("api/Section/MbClass", _strtoken, _userparam);
        //    var model = await _CallService.FetchObjectFromResponse<List<MbClass>>(httpResponseMessage);
        //    return Json(model);
        //}
        //public async Task<IActionResult> GetSubjectLessonsByClassId(int ClassId)
        //{
        //    _userparam = HttpContext.Session.GetString("UserParam");
        //    _strtoken = HttpContext.Session.GetString("strtoken");
        //    var httpResponseMessage = await _CallService.GetApi("api/Section/SubjectLession/" + ClassId + "", _strtoken, _userparam);
        //    var model = await _CallService.FetchObjectFromResponse<List<MbSyllabus>>(httpResponseMessage);
        //    return Json(model);
        //}

    }
}
