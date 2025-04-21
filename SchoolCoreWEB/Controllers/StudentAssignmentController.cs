using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using SchoolCoreWEB.IServices;
using SchoolCoreWEB.Models;

namespace SchoolCoreWEB.Controllers
{
    public class StudentAssignmentController : Controller
    {
        private readonly ICallService _CallService;
        private LanguageService _localization;
        private string _userparam;
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
        public async Task<ActionResult> GetAll()
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/StudentAssignment/", _strtoken, _userparam);
            var model = await _CallService.FetchObjectListFromResponse<VStudentAssignment>(httpResponseMessage);
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
            //string data = await _CallService.FetchStringFromResponse(httpResponseMessage);
            //return Json(data);
        }
        public async Task<ActionResult> GetStudentAssignment(int Id)
        {

            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/StudentAssignment/view/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<VStudentAssignment>(httpResponseMessage);
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
            var httpResponseMessage = await _CallService.GetApi("api/StudentAssignment/view/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<VStudentAssignment>(httpResponseMessage);
            return PartialView("ViewStudentAssignment", model);
        }
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
            return PartialView("AddEditStudentAssignment", model);
        }
        public async Task<IActionResult> GetClassByBranch(int BranchId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/StudentAssignment/Class/" + BranchId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<List<SelectListItem>>(httpResponseMessage);
            return Json(model);
        }
        public async Task<IActionResult> GetSectionByClass(int BranchId, int ClassId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/StudentAssignment/Section/" + BranchId + "," + ClassId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<List<SelectListItem>>(httpResponseMessage);
            return Json(model);
        }
        public async Task<IActionResult> GetSectionIdsByStudentAssignment(int StudentAssignId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/StudentAssignment/Section/" + StudentAssignId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<List<int>>(httpResponseMessage);
            return Json(model);
        }
        public async Task<IActionResult> GetSubjectByClass(int ClassId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/StudentAssignment/Subject/" + ClassId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<List<SelectListItem>>(httpResponseMessage);
            return Json(model);
        }     
        

    }
}
