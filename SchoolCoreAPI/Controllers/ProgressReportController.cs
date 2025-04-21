using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using SchoolCoreAPI.Services;

namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgressReportController : ControllerBase
    {
        // private readonly IReportCardService _reportCardService;
       // private readonly IReportSettingsService _reportSettingsService;
        private readonly IProgressReportService _progressReportService;
       

        public ProgressReportController(IProgressReportService progressReportService)
        {
            // _reportCardService = reportCardService;
            // _reportSettingsService = reportSettingsService;
            _progressReportService = progressReportService;
        }
        private UserParam GetUserParam()
        {
            return Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
        }


        [HttpGet("{rptcardId},{branchclassId},{studentId}")]
        [Authorize]
        public async Task<IActionResult> GetStudentReportCard(int rptcardId, int branchclassId,int studentId)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await _progressReportService.GetFn_ProgressReport(rptcardId, branchclassId, studentId, userParam.SessionYearId);


            // var res_performance = result["result_performance"].ToObject<List<VStudentScheduleResults>>();
            var res_performance = await _progressReportService.GetStudentScheduleVM(result["result_schedule"].ToObject<List<VStudentScheduleResults>>());
            var result_mark = result["result_mark"].ToObject<List<VStudentMarkResults>>();
            if(result_mark.Count > 0)
            {
                foreach(var item in res_performance.LstStudentScheduleResult)
                {
                    item.LstStudentMarkResult = result_mark.Where(x => x.ScheduleId == item.ScheduleId).OrderByDescending(x=> int.Parse(x.Marks)).ToList();
                    item.StudentMarkGraph = new StudentMarkGraphVM
                    {
                        labels = item.LstStudentMarkResult.Select(x => x.SubjectName).ToList(),
                        datasets = new List<StudentMarkChildVM>
                        {
                            new StudentMarkChildVM
                            {
                                label = "",
                                backgroundColor = item.LstStudentMarkResult.Select(x => x.GradeColor).ToList(),
                                borderWidth = 1,
                                fill = true,
                                data = item.LstStudentMarkResult.Select(x => int.Parse(x.Marks)).ToList()
                            }
                        }
                    };
                }
            }
            var result_report = result["result_report"]?.ToObject<List<StudentReport>>() ?? new List<StudentReport>();
            var result_attendence = result["result_attendence"]?.ToObject<List<AttendenceMetrics>>() ?? new List<AttendenceMetrics>();  
            


            StudentProgressReport studentProgressReport = new StudentProgressReport
            {
                SchedulePerformance = res_performance,
                StudentReport = result_report.FirstOrDefault(),
                LstAttendenceMetric = result_attendence
            };

            //var rptsettingsTask = _reportSettingsService.GetReportSettings(userParam.UserId);
            //var resultTask = _reportCardService.ViewReportCard(rptcardId);

            //await Task.WhenAll(rptsettingsTask, resultTask);

            //var rpt = new ProgressReport
            //{
            //    ReportCardId = rptcardId,
            //    StudentId = studentId,
            //    ReportSettings = rptsettingsTask.Result,
            //    ReportCard = resultTask.Result
            //};

            return Ok(studentProgressReport);
        }
    }
}
