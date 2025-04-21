using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using SchoolCoreAPI.Services;
using static SchoolCoreAPI.Helpers.Shared;


namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
        private UserParam GetUserParam()
        {
            return Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
        }

        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var lst = await Task.Run(() => _dashboardService.GetAllBranch());
            return Ok(lst);
        }

        [HttpGet("Performance/{id}")]
        [Authorize]
        public async Task<IActionResult> GetOverAllPerformanceByBranch(int id)
        {
            var lst = await Task.Run(() => _dashboardService.GetOverAllPerformanceVM(id));
            return Ok(lst);
        }
        [HttpGet("Metrics/{branchId},{sessionYearId}")]
        [Authorize]
        public async Task<IActionResult> GetDashboardMetrics(int branchId, int sessionYearId)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            try
            {               
                var currentsession = await _dashboardService.GetCurrentSessionYear();
                var result = await _dashboardService.GetDashboardMetrics(branchId, sessionYearId==0 ? currentsession : sessionYearId);
                var result_personnelcount = result["result_personnelcount"]?.ToObject<List<PersonnelCountMetric>>() ?? new List<PersonnelCountMetric>();                
                var result_holiday = result["result_holiday"]?.ToObject<List<HolidayMetric>>() ?? new List<HolidayMetric>();
                var result_event = result["result_event"]?.ToObject<List<EventMetric>>() ?? new List<EventMetric>();
                var result_attendence = result["result_attendence"]?.ToObject<List<AttendenceMetric>>() ?? new List<AttendenceMetric>();
                var result_feestatus = result["result_feestatus"]?.ToObject<List<FeeStatusMetric>>() ?? new List<FeeStatusMetric>();
                var result_expenses = result["result_expenses"]?.ToObject<List<ExpenseMetric>>() ?? new List<ExpenseMetric>();
                var result_gender = result["result_gender"]?.ToObject<List<GenderMetric>>() ?? new List<GenderMetric>();
                var result_leaverequest = result["result_leaverequest"]?.ToObject<List<LeaveRequestMetric>>() ?? new List<LeaveRequestMetric>();
                var result_birthday = result["result_birthday"]?.ToObject<List<BirthdayMetric>>() ?? new List<BirthdayMetric>();
                var result_classexam = result["result_classexam"]?.ToObject<List<ClassExamMetric>>() ?? new List<ClassExamMetric>();


                DashboardMetrics metrics = new DashboardMetrics
                {
                    PersonnelCountMetric = result_personnelcount.FirstOrDefault(),
                    LstHolidayMetric = result_holiday,
                    LstEventMetric = result_event,

                    LstAttendenceMetric = result_attendence,
                    LstFeeStatusMetric = result_feestatus,
                    LstClassExamMetric = result_classexam,
                    LstExpenseMetric = result_expenses,
                    LstGenderMetric = result_gender,
                    LstLeaveRequestMetric = result_leaverequest,
                    LstBirthdayMetric = result_birthday
                };
                return Ok(metrics);
            }
            catch (Exception ex)
            {                
            }
            return BadRequest("Error");


        }
    }
}