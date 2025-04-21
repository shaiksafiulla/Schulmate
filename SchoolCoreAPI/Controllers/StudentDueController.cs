using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentDueController : ControllerBase
    {
        private readonly IStudentDueService _studentDueService;

        public StudentDueController(IStudentDueService studentDueService)
        {
            _studentDueService = studentDueService;
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

            var lst = await _studentDueService.GetAllStudentDues(userParam.BranchId);
            return Ok(lst);
        }
        [HttpGet("FeeDue/{duePercent}")]
        [Authorize]
        public async Task<IActionResult> GetStudentDues(double duePercent)
        {
            if (duePercent < 0 || duePercent > 100)
            {
                return BadRequest("Due percentage must be between 0 and 100.");
            }
            var userParam = GetUserParam();
            var lst = await _studentDueService.GetAllStudentDuesByPercent(userParam.BranchId, userParam.SessionYearId, duePercent);
            return Ok(lst);
        }

        [HttpGet("View/{id}")]
        [Authorize]
        public async Task<IActionResult> View(int id)
        {
            var grade = await _studentDueService.ViewStudentDue(id);
            return Ok(grade);
        }
    }
}
