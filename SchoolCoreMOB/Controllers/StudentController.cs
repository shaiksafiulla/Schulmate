using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolCoreMOB.IServices;
using SchoolCoreMOB.Models;

namespace SchoolCoreMOB.Controllers
{
    public class StudentController : Controller
    {
        private readonly ICallService _CallService;
        private LanguageService _localization;
        string _userparam;  
        private string _strtoken;
        public StudentController(ICallService CallService, LanguageService localization)
        {
            _CallService = CallService;
            _localization = localization;
        }
        public IActionResult Index()
        {
            if (!HttpContext.Session.Keys.Contains("UserInfo"))
            {
                return RedirectToAction("Logout", "Login");
            }
            return View();
        }
        //public async Task<IActionResult> SearchMbStudents(string query)
        //{
        //    _userparam = HttpContext.Session.GetString("UserParam");
        //    _strtoken = HttpContext.Session.GetString("strtoken");
        //    //var httpResponseMessage = await _CallService.GetApi("api/student/MbStudent/" + _userInfo.PersonId + "," + _userInfo.UserType + "," + _userInfo.SessionYearId + "," + query + "", _strtoken, _userparam);
        //    var httpResponseMessage = await _CallService.GetApi("api/student/MbStudent/" + query + "", _strtoken, _userparam);
        //    var model = await _CallService.FetchObjectFromResponse<List<MbStudent>>(httpResponseMessage);
        //    foreach (var student in model)
        //    {
        //        if (!string.IsNullOrEmpty(student.FilePath))
        //        {
        //            student.FilePath = _CallService.FetchApiUrl() + student.FilePath;
        //        }
        //    }
        //    return Json(model);
        //}
        //[Route("Student/Details")]
        //public async Task<ActionResult> GetStudentDetails(int Id)
        //{
        //    _userparam = HttpContext.Session.GetString("UserParam");
        //    _strtoken = HttpContext.Session.GetString("strtoken");
        //    var httpResponseMessage = await _CallService.GetApi("api/TeacherAnnouncement/AddEdit/" + Id + "", _strtoken, _userparam);
        //    var model = await _CallService.FetchObjectFromResponse<TeacherAnnouncement>(httpResponseMessage);
        //    if (model != null)
        //    {
        //        if (!string.IsNullOrEmpty(model.FilePath))
        //        {
        //            model.FilePath = _CallService.FetchApiUrl() + model.FilePath;
        //        }
        //    }
        //    return View("Details", model);
        //}
        [Route("Student/Profile")]
        public async Task<ActionResult> GetStudentProfile(int Id)
        {
            //_userparam = HttpContext.Session.GetString("UserParam");
            //_strtoken = HttpContext.Session.GetString("strtoken");           
            //var httpResponseMessage = await _CallService.GetApi("api/MbPersonnel/PersonnelDetailView/" + Id + "," + "4" + "", _strtoken, _userparam);
            //var model = await _CallService.FetchObjectFromResponse<MbPersonnelDetail>(httpResponseMessage);
            //if (model != null)
            //{
            //    model.FilePath = model != null ? _CallService.FetchApiUrl() + model.FilePath : string.Empty;
            //}
            var model = new MbPersonnelDetail();
            model.Id = Id;
            model.PersonnelType = "4";
            //var anonymousModel = new
            //{
            //    Id = Id,
            //    PersonnelType = "4"          
            //};
            //ViewData["ProfileModel"] = anonymousModel;
            return View("StudentProfile", model);
        }
        [Route("Student/Attendance")]
        public async Task<ActionResult> GetStudentAttendance(int Id)
        {
            //_userparam = HttpContext.Session.GetString("UserParam");
            //_strtoken = HttpContext.Session.GetString("strtoken");          
            //var httpResponseMessage = await _CallService.GetApi("api/Attendence/Personnel/" + Id + "," + "4" + "", _strtoken, _userparam);
            //var model = await _CallService.FetchObjectFromResponse<List<VPersonnelAttendenceBenchmark>>(httpResponseMessage);
            var model = new VPersonnelAttendenceBenchmark();           
            model.Id = Id;          
            return View("StudentAttendance", model);
        }
        
        [Route("Student/Payments")]
        public async Task<ActionResult> GetAllPaymentByStudentId(int Id)
        {
            //_userparam = HttpContext.Session.GetString("UserParam");
            //_strtoken = HttpContext.Session.GetString("strtoken");
            //var httpResponseMessage = await _CallService.GetApi("api/student/Payments/" + Id + "", _strtoken, _userparam);
            //var model = await _CallService.FetchObjectFromResponse<List<VStudentPay>>(httpResponseMessage);
            var model = new VStudentPay();
            model.Id = Id;
            return View("StudentPayments", model);
        }
        [Route("Student/ReportCard")]
        public async Task<ActionResult> GetStudentMbReportCardList(int Id)
        {
            //_userparam = HttpContext.Session.GetString("UserParam");
            //_strtoken = HttpContext.Session.GetString("strtoken");
            //var httpResponseMessage = await _CallService.GetApi("api/student/ReportCard/" + Id + "", _strtoken, _userparam);
            //var model = await _CallService.FetchObjectFromResponse<List<MbReportCard>>(httpResponseMessage);
            var model = new MbReportCard();
            model.Id = Id;
            return View("StudentReportCard", model);
        }
        [Route("Student/Performance")]
        public async Task<ActionResult> GetStudentPerformance(int Id)
        {
            //_userparam = HttpContext.Session.GetString("UserParam");
            //_strtoken = HttpContext.Session.GetString("strtoken");
            //var httpResponseMessage = await _CallService.GetApi("api/student/schedule/" + Id + "", _strtoken, _userparam);
            //var model = await _CallService.FetchObjectFromResponse<StudentScheduleVM>(httpResponseMessage);
            var model = new VStudentScheduleResult();
            model.Id = Id;
            return View("StudentPerformance", model);
        }
        //public async Task<ActionResult> GetStudentMark(int Id, int scheduleId)
        //{
        //    _userparam = HttpContext.Session.GetString("UserParam");
        //    _strtoken = HttpContext.Session.GetString("strtoken");
        //    var httpResponseMessage = await _CallService.GetApi("api/student/ScheduleMark/" + Id + "," + scheduleId + "", _strtoken, _userparam);
        //    var model = await _CallService.FetchObjectFromResponse<StudentMarkVM>(httpResponseMessage);
        //    return Json(model);
        //}
        //public async Task<ActionResult> GetStudentMarkSubject(int Id, string Subject)
        //{
        //    _userparam = HttpContext.Session.GetString("UserParam");
        //    _strtoken = HttpContext.Session.GetString("strtoken");
        //    var httpResponseMessage = await _CallService.GetApi("api/student/SubjectMark/" + Id + "," + Subject + "", _strtoken, _userparam);
        //    var model = await _CallService.FetchObjectFromResponse<StudentMarkSubjectVM>(httpResponseMessage);
        //    return Json(model);
        //}
        //public async Task<ActionResult> GetRCCP_Student(string reportCardId, string StudentId)
        //{
        //    _userparam = HttpContext.Session.GetString("UserParam");
        //    _strtoken = HttpContext.Session.GetString("strtoken");
        //    var httpResponseMessage = await _CallService.GetApi("api/ReportCard/RCCPStudent/" + reportCardId + "," + StudentId + "", _strtoken, _userparam);
        //    var model = await _CallService.FetchObjectFromResponse<RCCP_Student>(httpResponseMessage);
        //    return Json(model);
        //}
        //public async Task<ActionResult> GetStudentScheduleResult(int Id, int reportCardId)
        //{
        //    _userparam = HttpContext.Session.GetString("UserParam");
        //    _strtoken = HttpContext.Session.GetString("strtoken");
        //    var httpResponseMessage = await _CallService.GetApi("api/student/ScheduleResult/" + Id + "," + reportCardId + "", _strtoken, _userparam);
        //    var model = await _CallService.FetchObjectFromResponse<StudentScheduleResultVM>(httpResponseMessage);
        //    return Json(model);
        //}


    }
}