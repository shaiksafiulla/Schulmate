using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolCoreMOB.IServices;
using SchoolCoreMOB.Models;

namespace SchoolCoreMOB.Controllers
{
    public class MbStudentController : Controller
    {
        private readonly ICallService _CallService;
        private LanguageService _localization;
        string _userparam;  
        private string _strtoken;
        public MbStudentController(ICallService CallService, LanguageService localization)
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
      
        
        [Route("MbStudent/Attendance")]
        public async Task<ActionResult> GetStudentAttendance(int Id)
        {           
            var model = new VPersonnelAttendenceBenchmark();           
            model.Id = Id;          
            return View("StudentAttendance", model);
        }
        
        [Route("MbStudent/Payments")]
        public async Task<ActionResult> GetAllPaymentByStudentId(int Id)
        {            
            var model = new VStudentPay();
            model.Id = Id;
            return View("StudentPayments", model);
        }
        [Route("MbStudent/ReportCard")]
        public async Task<ActionResult> GetStudentMbReportCardList(int Id)
        {          
            var model = new MbReportCard();
            model.Id = Id;
            return View("StudentReportCard", model);
        }
        [Route("MbStudent/Performance")]
        public async Task<ActionResult> GetStudentPerformance(int Id)
        {           
            var model = new VStudentScheduleResult();
            model.Id = Id;
            return View("StudentPerformance", model);
        }
        [Route("MbStudent/Schedule")]
        public async Task<ActionResult> GetStudentSchedule(int Id)
        {
            var model = new VPersonnelAttendenceBenchmark();
            model.Id = Id;
            return View("StudentSchedule", model);
        }
        [Route("MbStudent/HomeWork")]
        public async Task<ActionResult> GetStudentHomework(int Id)
        {
            var model = new VPersonnelAttendenceBenchmark();
            model.Id = Id;
            return View("StudentHomeWork", model);
        }
        [Route("MbStudent/TimeTable")]
        public async Task<ActionResult> GetStudentTimeTable(int Id)
        {
            var model = new VPersonnelAttendenceBenchmark();
            model.Id = Id;
            return View("StudentTimeTable", model);
        }
    }
}