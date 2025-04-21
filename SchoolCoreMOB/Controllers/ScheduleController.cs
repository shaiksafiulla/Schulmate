using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolCoreMOB.IServices;
using SchoolCoreMOB.Models;

namespace SchoolCoreMOB.Controllers
{
   
    public class ScheduleController : Controller
    {
        string _userparam;
        string _strtoken;
        private readonly ICallService _CallService;
        private LanguageService _localization;
        public ScheduleController(ICallService CallService, LanguageService localization)
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
       
        [Route("Schedule/ScheduleDetail")]
        public async Task<ActionResult> GetScheduleDetail(int scheduleId)
        {
            //_userparam = HttpContext.Session.GetString("UserParam");
            //_strtoken = HttpContext.Session.GetString("strtoken");
            //var httpResponseMessage = await _CallService.GetApi("api/teacher/TeacherInvigilation/"+ scheduleId + "", _strtoken, _userparam);
            //var data = await _CallService.FetchObjectFromResponse<List<VTeacherInvigilations>>(httpResponseMessage);
            var model = new ScheduleTitle();
            model.Id = scheduleId;            
            return View("ScheduleDetail", model);
        }
        //public async Task<ActionResult> GetExamTimeTableByScheduleAndClass(int scheduleId, int classId)
        //{
        //    _userparam = HttpContext.Session.GetString("UserParam");
        //    _strtoken = HttpContext.Session.GetString("strtoken");
        //    var httpResponseMessage = await _CallService.GetApi("api/teacher/ExamTimeTable/" + scheduleId + "," + classId + "", _strtoken, _userparam);
        //    var data = await _CallService.FetchObjectFromResponse<List<VExamTimeTable>>(httpResponseMessage);
        //    return View("Invigilations", data);
        //}

        [Route("Schedule/Invigilations")]
        public async Task<ActionResult> GetInvigilationsByTeacherId(int scheduleId)
        {
            var model = new ScheduleTitle();
            model.Id = scheduleId;
            return View("Invigilations", model);
        }
        [Route("Schedule/ExamTimeTable")]
        public async Task<ActionResult> GetTimeTableByScheduleId(int scheduleId)
        {
            var model = new ScheduleTitle();
            model.Id = scheduleId;
            return View("ExamTimeTable", model);
        }
        [Route("Schedule/ExamResult")]
        public async Task<ActionResult> GetResultByScheduleId(int scheduleId)
        {
            var model = new ScheduleTitle();
            model.Id = scheduleId;
            return View("ExamResult", model);
        }
        [Route("Schedule/Syllabus")]
        public async Task<ActionResult> GetSyllabusByScheduleId(int scheduleId)
        {
            var model = new ScheduleTitle();
            model.Id = scheduleId;
            return View("Syllabus", model);
        }



    }
}
