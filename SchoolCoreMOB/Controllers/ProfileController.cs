using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolCoreMOB.IServices;
using SchoolCoreMOB.Models;
using Activity = System.Diagnostics.Activity;

namespace SchoolCoreMOB.Controllers
{
    public class ProfileController : Controller
    {

        string _userparam;
        string _strtoken;
        private readonly ICallService _CallService;
        private LanguageService _localization;
        public ProfileController(ICallService CallService, LanguageService localization)
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
        //public async Task<ActionResult> Index()
        //{
        //    var userInfokey = HttpContext.Session.GetString("UserInfo");
        //    if (userInfokey != null)
        //    {
        //        _userInfo = JsonConvert.DeserializeObject<VUserInfo>(userInfokey);
        //    }
        //    _strtoken = HttpContext.Session.GetString("strtoken");
        //    var httpResponseMessage = await _CallService.GetApi("api/MbPersonnel/PersonnelDetailView/" + _userInfo.PersonId + "," + _userInfo.SessionYearId + "," + _userInfo.UserType + "",_strtoken, _userparam);
        //    var model = await _CallService.FetchObjectFromResponse<MbPersonnelDetail>(httpResponseMessage);
        //    if (model != null)
        //    {
        //        model.FilePath = model != null ? _CallService.FetchApiUrl() + model.FilePath : string.Empty;
        //    }
        //    return View(model);
        //}
        //public async Task<ActionResult> GetProfileDetail()
        //{
        //    _userparam = HttpContext.Session.GetString("UserParam");
        //    _strtoken = HttpContext.Session.GetString("strtoken");
        //    var httpResponseMessage = await _CallService.GetApi("api/MbPersonnel/PersonnelDetailView", _strtoken, _userparam);
        //    var model = await _CallService.FetchObjectFromResponse<MbPersonnelDetail>(httpResponseMessage);
        //    if (model != null)
        //    {
        //        model.FilePath = model != null ? _CallService.FetchApiUrl() + model.FilePath : string.Empty;
        //    }
        //    return Json(model);
        //}
    }
}