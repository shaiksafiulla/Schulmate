using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using SchoolCoreWEB.IServices;
using SchoolCoreWEB.Models;
using System.Reflection;
using static SchoolCoreWEB.Helpers.Shared;

namespace SchoolCoreWEB.Controllers
{
    
    public class StudentController : Controller
    {
        string _userparam;
        string _strtoken;
        private readonly ICallService _CallService;
        private LanguageService _localization;
        public StudentController(ICallService CallService, LanguageService localization)
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
            var httpResponseMessage = await _CallService.GetApi("api/student", _strtoken, _userparam);
            var model = await _CallService.FetchObjectListFromResponse<VStudent>(httpResponseMessage);
            if (model != null)
            {
                foreach (var admin in model)
                {
                    if (!string.IsNullOrEmpty(admin.FilePath))
                    {
                        admin.FilePath = _CallService.FetchApiUrl() + admin.FilePath;
                    }
                }
            }
            string strserialize = JsonConvert.SerializeObject(model);
            return Json(strserialize);
        }
        public async Task<ActionResult> GetAllPaymentByStudentId(int studentId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/student/Payments/" + studentId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<MbStudentPay>(httpResponseMessage);                        
            return Json(model);
        }
        public async Task<ActionResult> GetStudent(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/student/view/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<VStudent>(httpResponseMessage);
            model.FilePath = model !=null ? _CallService.FetchApiUrl() + model.FilePath : string.Empty;
            return Json(model);
        }
        public async Task<ActionResult> GetStudentDetail(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/student/StudentDetailView/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<StudentDetail>(httpResponseMessage);
            if (model != null)
            {
                model.FilePath = model != null ? _CallService.FetchApiUrl() + model.FilePath : string.Empty;

                model.ReportSettings.AFourHeaderPhotoPath = !string.IsNullOrEmpty(model.ReportSettings.AFourHeaderPhotoPath) ? _CallService.FetchApiUrl() + model.ReportSettings.AFourHeaderPhotoPath : string.Empty;
                model.ReportSettings.AFourFooterPhotoPath = !string.IsNullOrEmpty(model.ReportSettings.AFourFooterPhotoPath) ? _CallService.FetchApiUrl() + model.ReportSettings.AFourFooterPhotoPath : string.Empty;

                model.ReportSettings.AFiveHeaderPhotoPath = !string.IsNullOrEmpty(model.ReportSettings.AFiveHeaderPhotoPath) ? _CallService.FetchApiUrl() + model.ReportSettings.AFiveHeaderPhotoPath : string.Empty;
                model.ReportSettings.AFiveFooterPhotoPath = !string.IsNullOrEmpty(model.ReportSettings.AFiveFooterPhotoPath) ? _CallService.FetchApiUrl() + model.ReportSettings.AFiveFooterPhotoPath : string.Empty;
            }
            return PartialView("ViewStudent", model);
        }
        public async Task<ActionResult> GetClassesByBranch(string branchId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/student/Class/" + branchId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<List<SelectListItem>>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> GetSectionsByBranchAndClass(string branchId, string classId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/student/Section/" + branchId + "," + classId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<List<SelectListItem>>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> View(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/student/view/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<VStudent>(httpResponseMessage);
            model.FilePath = model != null ? _CallService.FetchApiUrl() + model.FilePath : string.Empty;
            return PartialView("ViewStudent", model);
        }
        public async Task<ActionResult> LoadAttachments(int referid)
        {
            int type = (int)AttachmentType.student;
            AttachModel model = new AttachModel { Type = type, ReferId = referid };            
            return PartialView("~/views/Attachment/index.cshtml", model);
        }
        public async Task<ActionResult> AddEdit(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/student/AddEdit/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<Student>(httpResponseMessage);
            model.FilePath = model != null ? _CallService.FetchApiUrl() + model.FilePath : string.Empty;            
            return PartialView("AddEditStudent", model);
        }
        //public async Task<ActionResult> Save()
        //{
        //    try
        //    {
        //        var files = Request.Form.Files;
        //        var strModel = Request.Form["model"];
        //        var objModel = JsonConvert.DeserializeObject<Student>(strModel);

        //        var userInfokey = HttpContext.Session.GetString("UserInfo");
        //        if (userInfokey != null)
        //        {
        //            _userInfo = JsonConvert.DeserializeObject<VUserInfo>(userInfokey);
        //        }
        //        _strtoken = HttpContext.Session.GetString("strtoken");
        //        var strjson = JsonConvert.SerializeObject(objModel);
        //        var response = await _CallService.PostFileApi(files, strjson, "api/student/save", _strtoken, _userparam);
        //        string data = await _CallService.FetchStringFromResponse(response);
        //        return Json(data);
        //    }
        //    catch (System.Exception ex)
        //    {

        //    }
        //    return Json(null);

        //}
        public async Task<ActionResult> GetStudentPay(int StudentId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/student/StudentPay/" + StudentId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<StudentPay>(httpResponseMessage);
            return PartialView("StudentPay", model);
        }
        public async Task<ActionResult> SaveStudentPay(StudentPay model)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            string jsonInput = JsonConvert.SerializeObject(model);
            var response = await _CallService.PostApi(jsonInput, "api/student/StudentPay/save", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
        }
        public async Task<ActionResult> Delete(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var response = await _CallService.DeleteApi("api/student/Delete/" + Id + "", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
        }
        public async Task<ActionResult> GetStudentSchedule(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/student/schedule/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<StudentScheduleVM>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> GetStudentScheduleResult(int Id,int reportCardId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/student/ScheduleResult/" + Id + ","+ reportCardId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<StudentScheduleResultVM>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> GetStudentMark(int Id, int scheduleId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/student/ScheduleMark/" + Id + "," + scheduleId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<StudentMarkVM>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> GetStudentMarkSubject(int Id, string Subject)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/student/SubjectMark/" + Id + "," + Subject + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<StudentMarkSubjectVM>(httpResponseMessage);
            return Json(model);
        }
    }
}
