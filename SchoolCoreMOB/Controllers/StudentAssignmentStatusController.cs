using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using SchoolCoreMOB.IServices;
using SchoolCoreMOB.Models;

namespace SchoolCoreMOB.Controllers
{
    public class StudentAssignmentStatusController : Controller
    {
        private readonly ICallService _CallService;
        private LanguageService _localization;
        string _userparam;  
        private string _strtoken;
        public StudentAssignmentStatusController(ICallService CallService, LanguageService localization)
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
            var httpResponseMessage = await _CallService.GetApi("api/StudentAssignment/Student", _strtoken, _userparam);
            var model = await _CallService.FetchObjectListFromResponse<VStudentAssignmentStatus>(httpResponseMessage);
            if (model != null)
            {
                foreach (var studentAssignment in model)
                {
                    if (!string.IsNullOrEmpty(studentAssignment.FilePath))
                    {
                        studentAssignment.FilePath = _CallService.FetchApiUrl() + studentAssignment.FilePath;
                    }
                }
            }
            string strserialize = JsonConvert.SerializeObject(model);
            return Json(strserialize);
        }       
        public async Task<ActionResult> GetStudentAssignStatus(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/StudentAssignment/Student/view/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<VStudentAssignmentStatus>(httpResponseMessage);
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.FilePath))
                {
                    model.FilePath = _CallService.FetchApiUrl() + model.FilePath;
                }
            }
            return Json(model);
        }
        
        public async Task<ActionResult> AddEdit(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/StudentAssignment/Student/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<StudentAssignStatus>(httpResponseMessage);
            return PartialView("AddEditStudentAssignStatus", model);
        }
        public async Task<ActionResult> Save(StudentAssignStatus model)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            string jsonInput = JsonConvert.SerializeObject(model);
            var response = await _CallService.PostApi(jsonInput, "api/StudentAssignment/Student/save", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
        }
       
       

    }
}
