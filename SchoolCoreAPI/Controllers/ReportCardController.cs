using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportCardController : ControllerBase
    {
        private readonly IReportCardService _reportCardService;

        public ReportCardController(IReportCardService reportCardService)
        {
            _reportCardService = reportCardService;
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
            var lst = await _reportCardService.GetAllReportCard(userParam.BranchId, userParam.SessionYearId);
            return Ok(lst);
        }

        [HttpGet("View/{id}")]
        [Authorize]
        public async Task<IActionResult> View(int id)
        {
            var branch = await _reportCardService.ViewReportCard(id);
            return Ok(branch);
        }

        [HttpGet("AddEdit/{id}")]
        [Authorize]
        public async Task<IActionResult> AddEdit(int id)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var branch = await _reportCardService.GetReportCard(id, userParam.BranchId, userParam.SessionYearId);
            return Ok(branch);
        }

        [HttpGet("ReportCardDetailView/{id}")]
        [Authorize]
        public async Task<IActionResult> GetReportCardDetail(int id)
        {
            var result = await _reportCardService.GetReportCardDetail(id);
            return Ok(result);
        }

        [HttpGet("IsExist/{name},{id}")]
        [Authorize]
        public async Task<IActionResult> IsExist(string name, int id)
        {
            var isExists = await _reportCardService.IsRecordExists(name, id);
            return Ok(isExists);
        }

        [HttpPost("Save")]
        [Authorize]
        public async Task<IActionResult> Save(ReportCard model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await _reportCardService.SaveReportCard(model, userParam.UserId);
            return Ok(result);
        }

        [HttpDelete("Delete/{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await _reportCardService.DeleteReportCard(id, userParam.UserId);
            return Ok(result);
        }

        [HttpGet("BranchClass/{id}")]
        [Authorize]
        public async Task<IActionResult> GetBranchClassByReportCard(int id)
        {
            var branch = await _reportCardService.GetBranchClassByReportCard(id);
            return Ok(branch);
        }

        [HttpGet("Schedules/{branchId},{reportcardId}")]
        [Authorize]
        public async Task<IActionResult> GetSchedulesByBranch(string branchId, string reportcardId)
        {
            var branch = await _reportCardService.GetSchedulesByBranch(branchId, reportcardId);
            return Ok(branch);
        }

        [HttpGet("SPReport/{reportCardId},{branchClassId}")]
        [Authorize]
        public async Task<IActionResult> GetSPReportCard(string reportCardId, string branchClassId)
        {
            var result = await _reportCardService.GetSPReportCard(reportCardId, branchClassId);
            var examResult = new SpReportCardResult
            {
                PreResult = result.Item1,
                PostResult = result.Item2
            };
            return Ok(examResult);
        }

        [HttpGet("RCCPStudent/{reportCardId},{studentId}")]
        [Authorize]
        public async Task<IActionResult> GetRCCP_Student(string reportCardId, string studentId)
        {
            var result = await _reportCardService.GetRCCP_Student(reportCardId, studentId);
            return Ok(result);
        }
    }
}
