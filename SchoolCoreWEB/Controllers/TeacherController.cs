using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using SchoolCoreWEB.IServices;
using SchoolCoreWEB.Models;
using static SchoolCoreWEB.Helpers.Shared;

namespace SchoolCoreWEB.Controllers
{
   
    public class TeacherController : Controller
    {
        string _userparam;
        string _strtoken;
        private readonly ICallService _CallService;
        private LanguageService _localization;
        public TeacherController(ICallService CallService, LanguageService localization)
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
            var httpResponseMessage = await _CallService.GetApi("api/teacher", _strtoken, _userparam);
            var model = await _CallService.FetchObjectListFromResponse<VTeacher>(httpResponseMessage);
            if (model != null)
            {
                foreach (var teacher in model)
                {
                    if (!string.IsNullOrEmpty(teacher.FilePath))
                    {                        
                        teacher.FilePath = _CallService.FetchApiUrl() + teacher.FilePath;
                    }
                }
            }
            string strserialize = JsonConvert.SerializeObject(model);
            return Json(strserialize);
        }
        public async Task<ActionResult> GetTeacher(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/teacher/view/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<VTeacher>(httpResponseMessage);
            model.FilePath = !string.IsNullOrEmpty(model.FilePath) ? _CallService.FetchApiUrl()  + model.FilePath : string.Empty;
            return Json(model);
        }
        public async Task<ActionResult> GetTeacherDetail(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/teacher/TeacherDetailView/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<PersonnelDetail>(httpResponseMessage);
            if (model != null)
            {
                model.FilePath = model != null ? _CallService.FetchApiUrl() + model.FilePath : string.Empty;

                model.ReportSettings.AFourHeaderPhotoPath = !string.IsNullOrEmpty(model.ReportSettings.AFourHeaderPhotoPath) ? _CallService.FetchApiUrl() +  model.ReportSettings.AFourHeaderPhotoPath : string.Empty;
                model.ReportSettings.AFourFooterPhotoPath = !string.IsNullOrEmpty(model.ReportSettings.AFourFooterPhotoPath) ? _CallService.FetchApiUrl()  + model.ReportSettings.AFourFooterPhotoPath : string.Empty;

                model.ReportSettings.AFiveHeaderPhotoPath = !string.IsNullOrEmpty(model.ReportSettings.AFiveHeaderPhotoPath) ? _CallService.FetchApiUrl()  + model.ReportSettings.AFiveHeaderPhotoPath : string.Empty;
                model.ReportSettings.AFiveFooterPhotoPath = !string.IsNullOrEmpty(model.ReportSettings.AFiveFooterPhotoPath) ? _CallService.FetchApiUrl()  + model.ReportSettings.AFiveFooterPhotoPath : string.Empty;
            }
            return PartialView("ViewTeacher", model);
        }
        public async Task<ActionResult> View(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/teacher/view/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<VTeacher>(httpResponseMessage);
            model.FilePath = !string.IsNullOrEmpty(model.FilePath) ? _CallService.FetchApiUrl() + model.FilePath : string.Empty;
            return PartialView("ViewTeacher", model);
        }
        public async Task<ActionResult> LoadAttachments(int referid)
        {
            int type = (int)AttachmentType.teacher;
            AttachModel model = new AttachModel { Type = type, ReferId = referid };
            return PartialView("~/views/Attachment/index.cshtml", model);
        }
        public async Task<ActionResult> AddEdit(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/teacher/AddEdit/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<Personnel>(httpResponseMessage);
            model.FilePath = !string.IsNullOrEmpty(model.FilePath) ? _CallService.FetchApiUrl()  + model.FilePath : string.Empty;
            return PartialView("AddEditTeacher", model);
        }       
        public async Task<ActionResult> Delete(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var response = await _CallService.DeleteApi("api/teacher/Delete/" + Id + "", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
        }
        public async Task<ActionResult> AddEditTeacherTimeTable(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/teacher/WebTimeTable/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<TeacherTimeTableVM>(httpResponseMessage);
            return PartialView("AddEditTeacherTimeTable", model);
        }
        public async Task<ActionResult> GetTeacherDetailTimeTable(int teacherId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/teacher/WebTimeTable/" + teacherId + "", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(httpResponseMessage);
            return Json(data);
        }
        public async Task<ActionResult> GetTeacherTimeTableFromDB(int teacherId, int sessionyearId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            //var httpResponseMessage = await _CallService.GetApi("api/teacher/TimeTable/" + teacherId + "," + sessionyearId + "", _strtoken, _userparam);
            var httpResponseMessage = await _CallService.GetApi("api/teacher/WebTimeTableDB/" + teacherId + "", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(httpResponseMessage);
            return Json(data);
        }
        public async Task<ActionResult> AddEditTeacherClassSubject(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/teacher/ClassSubject/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<ClassSubjectVM>(httpResponseMessage);
            return PartialView("AddEditClassSubjectTeacher", model);
        }
        public async Task<ActionResult> UpdateTeacherClassSubject(ClassSubjectVM model)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            string jsonInput = JsonConvert.SerializeObject(model);
            var response = await _CallService.PostApi(jsonInput, "api/teacher/ClassSubject/Save", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
        }
        public async Task<ActionResult> GetTeacherSubjects(string Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/teacher/Subject/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<string>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> GetTeacherSchedules(string Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/teacher/Schedule/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<List<VSchedule>>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> GetPivotScheduleByTeacher(string scheduleId, string teacherId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/teacher/PivotSchedule/" + scheduleId + "," + teacherId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<ScheduleTeacherProcedure>(httpResponseMessage);
            return Json(model);
        }    


    }
}
