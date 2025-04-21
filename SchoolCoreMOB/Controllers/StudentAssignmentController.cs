using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using SchoolCoreMOB.IServices;
using SchoolCoreMOB.Models;

namespace SchoolCoreMOB.Controllers
{
    public class StudentAssignmentController : Controller
    {
        private readonly ICallService _CallService;
        private LanguageService _localization;
        string _userparam;  
        private string _strtoken;
        public StudentAssignmentController(ICallService CallService, LanguageService localization)
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
        [Route("StudentAssignment/AddStudentAssignment")]
        public async Task<ActionResult> AddEdit(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/StudentAssignment/AddEdit/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<StudentAssignment>(httpResponseMessage);
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.FilePath))
                {
                    model.FilePath = _CallService.FetchApiUrl() + model.FilePath;
                }
            }
            return View("AddStudentAssignment", model);
        }
        //[Route("StudentAssignment/AddStudentAssignment")]
        //public async Task<ActionResult> AddEdit(int Id)
        //{
        //    _userparam = HttpContext.Session.GetString("UserParam");
        //    _strtoken = HttpContext.Session.GetString("strtoken");
        //    var httpResponseMessage = await _CallService.GetApi("api/StudentAssignment/AddEdit/" + Id + "", _strtoken, _userparam);
        //    var model = await _CallService.FetchObjectFromResponse<StudentAssignment>(httpResponseMessage);
        //    if (model != null)
        //    {
        //        if (!string.IsNullOrEmpty(model.FilePath))
        //        {
        //            model.FilePath = _CallService.FetchApiUrl() + model.FilePath;
        //        }
        //    }
        //    return View("AddStudentAssignment", model);
        //}      

    }
}
