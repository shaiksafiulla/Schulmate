using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SchoolCoreMOB.IServices;
using SchoolCoreMOB.Models;

namespace SchoolCoreMOB.Controllers
{
    public class AnnouncementController : Controller
    {
        private readonly ICallService _CallService;
        private LanguageService _localization;
        string _userparam;  
        private string _strtoken;
        public AnnouncementController(ICallService CallService, LanguageService localization)
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
      
        [Route("Announcement/AddAnnouncement")]
        public async Task<ActionResult> AddEdit(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/TeacherAnnouncement/AddEdit/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<TeacherAnnouncement>(httpResponseMessage);
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.FilePath))
                {
                    model.FilePath = _CallService.FetchApiUrl() + model.FilePath;
                }
            }
            return View("AddAnnouncement", model);
        }
        [Route("Announcement/Download")]
        public async Task<FileResult> Download(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var response = await _CallService.GetApi("api/TeacherAnnouncement/download/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<Tuple<byte[], string>>(response);
            if(model != null)
            {
                return File(model.Item1, "application/octet-stream", model.Item2);
            }
            else
            {
                // Return an empty file or a placeholder file if the data is not available
                // You could also return a "not found" placeholder file here, for example:
                byte[] emptyFile = new byte[0]; // Empty byte array for an empty file
                string errorMessage = "No file data found.";

                // Optionally, you can return the error message as a file
                return File(emptyFile, "application/octet-stream", errorMessage);
            }
        }


    }
}
