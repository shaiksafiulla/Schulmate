using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using SchoolCoreWEB.IServices;
using SchoolCoreWEB.Models;

namespace SchoolCoreWEB.Controllers
{   
    public class ReportSettingsController : Controller
    {
        string _userparam;
        private string _strtoken;
        private readonly ICallService _CallService;
        private LanguageService _localization;
        public ReportSettingsController(ICallService CallService, LanguageService localization)
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
            var httpResponseMessage = await _CallService.GetApi("api/ReportSettings/AddEdit", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<ReportSettings>(httpResponseMessage);
            model.AFourHeaderPhotoPath = _CallService.FetchApiUrl() + model.AFourHeaderPhotoPath;
            model.AFourFooterPhotoPath = _CallService.FetchApiUrl() + model.AFourFooterPhotoPath;
            model.AFiveHeaderPhotoPath = _CallService.FetchApiUrl() + model.AFiveHeaderPhotoPath;
            model.AFiveFooterPhotoPath = _CallService.FetchApiUrl() + model.AFiveFooterPhotoPath;
            return View(model);            
        }

        //public async Task<ActionResult> Save()
        //{
        //    var files = Request.Form.Files;
        //    var strModel = Request.Form["model"];

        //    var userInfokey = HttpContext.Session.GetString("UserInfo");
        //    if (userInfokey != null)
        //    {
        //        _userInfo = JsonConvert.DeserializeObject<VUserInfo>(userInfokey);
        //    }
        //    _strtoken = HttpContext.Session.GetString("strtoken");

        //    var objModel = JsonConvert.DeserializeObject<ReportSettings>(strModel);
        //    var strjson = JsonConvert.SerializeObject(objModel);
        //    var response = await _CallService.PostFileApi(files, strjson, "api/ReportSettings/save", _strtoken, _userparam);
        //    string data = await _CallService.FetchStringFromResponse(response);
        //    return Json(data);
        //    return Json(null);
            
        //}
    }
}