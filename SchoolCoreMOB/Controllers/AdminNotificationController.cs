using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using SchoolCoreMOB.IServices;
using SchoolCoreMOB.Models;

namespace SchoolCoreMOB.Controllers
{
    public class AdminNotificationController : Controller
    {
        private readonly ICallService _CallService;
        private LanguageService _localization;
        private string _userparam;
        private string _strtoken;
        public AdminNotificationController(ICallService CallService, LanguageService localization)
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
        //public async Task<ActionResult> GetAll()
        //{
        //    _userparam = HttpContext.Session.GetString("UserParam");
        //    _strtoken = HttpContext.Session.GetString("strtoken");
        //    var httpResponseMessage = await _CallService.GetApi("api/AdminNotification/", _strtoken, _userparam);
        //    var model = await _CallService.FetchObjectListFromResponse<AdminNotification>(httpResponseMessage);
        //    if (model != null)
        //    {
        //        foreach (var announcement in model)
        //        {
        //            if (!string.IsNullOrEmpty(announcement.FilePath))
        //            {
        //                announcement.FilePath = _CallService.FetchApiUrl() + announcement.FilePath;
        //            }
        //        }                
        //    }
        //    string strserialize = JsonConvert.SerializeObject(model);
        //    return Json(strserialize);
            
        //}
        //public async Task<ActionResult> GetAdminNotification(int Id)
        //{

        //    _userparam = HttpContext.Session.GetString("UserParam");
        //    _strtoken = HttpContext.Session.GetString("strtoken");
        //    var httpResponseMessage = await _CallService.GetApi("api/AdminNotification/AddEdit/" + Id + "", _strtoken, _userparam);
        //    var model = await _CallService.FetchObjectFromResponse<AdminNotification>(httpResponseMessage);
        //    if(model != null)
        //    {
        //        if (!string.IsNullOrEmpty(model.FilePath))
        //        {
        //            model.FilePath = _CallService.FetchApiUrl() + model.FilePath;
        //        }
        //    }
            
        //    return Json(model);
        //}
        [Route("AdminNotification/AddAdminNotification")]
        public async Task<ActionResult> AddEdit(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/AdminNotification/AddEdit/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<AdminNotification>(httpResponseMessage);
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.FilePath))
                {
                    model.FilePath = _CallService.FetchApiUrl() + model.FilePath;
                }
            }
            return View("AddAdminNotification", model);
        }
       
        public async Task<IActionResult> GetStudPersonNotification(int PersonnelType)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/AdminNotification/Personnel/" + PersonnelType + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<List<CastStudPersonNotification>>(httpResponseMessage);
            return Json(model);
        }
        [Route("AdminNotification/Download")]
        public async Task<FileResult> Download(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var response = await _CallService.GetApi("api/AdminNotification/download/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<Tuple<byte[], string>>(response);
            if (model != null)
            {
                return File(model.Item1, "application/octet-stream", model.Item2);
            }
            else
            {               
                byte[] emptyFile = new byte[0]; // Empty byte array for an empty file
                string errorMessage = "No file data found.";
                // Optionally, you can return the error message as a file
                return File(emptyFile, "application/octet-stream", errorMessage);
            }
        }

    }
}
