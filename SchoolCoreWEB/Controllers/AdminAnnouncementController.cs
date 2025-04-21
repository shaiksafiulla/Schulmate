using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using SchoolCoreWEB.IServices;
using SchoolCoreWEB.Models;

namespace SchoolCoreWEB.Controllers
{
    public class AdminAnnouncementController : Controller
    {
        private readonly ICallService _CallService;
        private LanguageService _localization;
        private string _userparam;
        private string _strtoken;
        public AdminAnnouncementController(ICallService CallService, LanguageService localization)
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
        public async Task<ActionResult> GetAll()
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/AdminAnnouncement/", _strtoken, _userparam);
            var model = await _CallService.FetchObjectListFromResponse<VAdminAnnouncement>(httpResponseMessage);
            if (model != null)
            {
                foreach (var announcement in model)
                {
                    if (!string.IsNullOrEmpty(announcement.FilePath))
                    {
                        announcement.FilePath = _CallService.FetchApiUrl() + announcement.FilePath;
                    }
                }
                
            }
            string strserialize = JsonConvert.SerializeObject(model);
            return Json(strserialize);
            //string data = await _CallService.FetchStringFromResponse(httpResponseMessage);
            //return Json(data);
        }
        public async Task<ActionResult> GetAdminAnnouncement(int Id)
        {

            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/AdminAnnouncement/view/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<VAdminAnnouncement>(httpResponseMessage);
            if(model != null)
            {
                if (!string.IsNullOrEmpty(model.FilePath))
                {
                    model.FilePath = _CallService.FetchApiUrl() + model.FilePath;
                }
            }
            
            return Json(model);
        }
        public async Task<ActionResult> View(int Id)
        {

            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/AdminAnnouncement/view/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<VAdminAnnouncement>(httpResponseMessage);
            return PartialView("ViewAdminAnnouncement", model);
        }
        public async Task<ActionResult> AddEdit(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/AdminAnnouncement/AddEdit/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<AdminAnnouncement>(httpResponseMessage);
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.FilePath))
                {
                    model.FilePath = _CallService.FetchApiUrl() + model.FilePath;
                }
            }
            return PartialView("AddEditAdminAnnouncement", model);
        }
       
        public async Task<IActionResult> GetSectionIdsByAdminAnnouncement(int AdminAnnounceId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/AdminAnnouncement/Section/" + AdminAnnounceId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<List<int>>(httpResponseMessage);
            return Json(model);
        }       

    }
}
