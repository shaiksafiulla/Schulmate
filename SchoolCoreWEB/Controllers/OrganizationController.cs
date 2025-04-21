using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using SchoolCoreWEB.Helpers;
using SchoolCoreWEB.IServices;
using SchoolCoreWEB.Models;

namespace SchoolCoreWEB.Controllers
{

    public class OrganizationController : Controller
    {
        string _userparam;
        private string _strtoken;
        private readonly ICallService _CallService;
        private LanguageService _localization;
        public OrganizationController(ICallService CallService, LanguageService localization) 
        {
            _CallService = CallService;
            _localization = localization;
        }
        //public OrganizationController(ICallService CallService, LanguageService localization) : base(localization)
        //{
        //    _CallService = CallService;
        //    _localization = localization;
        //}

        public async Task<IActionResult> Index()
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/Organization/AddEdit", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<Organization>(httpResponseMessage);
            if(model != null)
            {
                model.LogoPhotoPath = _CallService.FetchApiUrl() + model.LogoPhotoPath;
                model.HeaderPhotoPath = _CallService.FetchApiUrl() + model.HeaderPhotoPath;
            }           
            return View(model);
        }
        //[HttpPost]       
        //public async Task<IActionResult> Save(Organization model)
        //{
            
        //    try
        //    {               
        //        var userInfokey = HttpContext.Session.GetString("UserInfo");
        //        if (userInfokey != null)
        //        {
        //            _userInfo = JsonConvert.DeserializeObject<VUserInfo>(userInfokey);
        //        }
        //        _strtoken = HttpContext.Session.GetString("strtoken");
        //        // var objModel = JsonConvert.DeserializeObject<Organization>(strModel);
        //        var strjson = JsonConvert.SerializeObject(model);
        //        List<IFormFile> files = new List<IFormFile>();
        //        files.Add(model.LogoFile != null ? model.LogoFile : null);
        //        files.Add(model.HeaderFile != null ? model.HeaderFile : null);
        //        var response = await _CallService.PostFileApi(files, strjson, "api/organization/save", _strtoken, _userparam);
        //        string data = await _CallService.FetchStringFromResponse(response);
        //        return Json(data);
        //    }
        //    catch (Exception ex)
        //    {
                
        //    }
        //    return Json(null);
            
        //}
    }
}
