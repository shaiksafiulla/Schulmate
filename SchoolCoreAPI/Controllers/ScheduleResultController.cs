using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using System.Data;

namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleResultController : ControllerBase
    {
        private readonly IScheduleResultService _scheduleResultService;

        public ScheduleResultController(IScheduleResultService scheduleResultService)
        {
            _scheduleResultService = scheduleResultService;
        }
        private UserParam GetUserParam()
        {
            return Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var lst = await _scheduleResultService.GetAllScheduleResult(userParam.BranchId);
            return Ok(lst);
        }

        [HttpGet("View/{id}")]
        [Authorize]
        public async Task<IActionResult> View(int id)
        {
            var result = await _scheduleResultService.GetStudentResult(id);
            return Ok(result);
        }

        [HttpGet("ScheduleResultView/{id}")]
        [Authorize]
        public async Task<IActionResult> GetScheduleResultDetail(int id)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await _scheduleResultService.GetScheduleResultDetail(id, userParam.BranchId);
            return Ok(result);
        }

        [HttpGet("ExportData/{id}")]
        [Authorize]
        public async Task<IActionResult> ExportData(int id)
        {
            var ds = await _scheduleResultService.GetExportData(id);
            if (ds == null || ds.Tables.Count == 0)
            {
                return BadRequest("No data available to export.");
            }
            using (var workbook = new XLWorkbook())
            {
                foreach (DataTable table in ds.Tables)
                {
                    var worksheet = workbook.Worksheets.Add(table, table.TableName);
                    worksheet.Columns().AdjustToContents();
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Seek(0, SeekOrigin.Begin);

                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "MyExcelFile.xlsx");
                }
            }
        }

        [HttpGet("BranchClass/{id}")]
        [Authorize]
        public async Task<IActionResult> GetScheduleBranchClass(int id)
        {
            var result = await _scheduleResultService.GetStudentBranchClass(id);
            return Ok(result);
        }

        [HttpGet("ExamResult/{branchclassId},{schedId}")]
        [Authorize]
        public async Task<IActionResult> GetSPExamResult(string branchclassId, string schedId)
        {
            var result = await _scheduleResultService.GetSPExamResult(branchclassId, schedId);
            var examResult = new SpExamResult
            {
                PreResult = result.Item1,
                PostResult = result.Item2
            };
            return Ok(examResult);
        }

        [HttpPost("Upload")]
        [Authorize]
        public async Task<IActionResult> UploadStudentResult(StudentResultUpload model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var result = await _scheduleResultService.UploadStudentResult(model, userParam.UserId);
            return Ok(result);
        }

        [HttpPost("Student")]
        [Authorize]
        public async Task<IActionResult> SaveStudentResult(StudentResultVM model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var result = await _scheduleResultService.SaveStudentResult(model, userParam.UserId);
            return Ok(result);
        }

        [HttpGet("ResultStatus/{scheduleId},{branchclassId}")]
        [Authorize]
        public async Task<IActionResult> GetBranchClassResultStatus(string scheduleId, string branchclassId)
        {
            var result = await _scheduleResultService.GetBranchClassResultStatus(scheduleId, branchclassId);
            return Ok(result);
        }
    }
}
