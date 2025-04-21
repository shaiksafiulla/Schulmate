using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherTransferController : ControllerBase
    {
        private readonly ITeacherTransferService _teacherTransferService;

        public TeacherTransferController(ITeacherTransferService teacherTransferService)
        {
            _teacherTransferService = teacherTransferService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var branch = await _teacherTransferService.GetTeacherTransferVM();
            return Ok(branch);
        }

        [HttpGet("Teacher/{branchid}")]
        [Authorize]
        public async Task<IActionResult> GetTeachers(int branchid)
        {
            var teachers = await _teacherTransferService.GetTeachers(branchid);
            return Ok(teachers);
        }

        [HttpPost("Save")]
        [Authorize]
        public async Task<IActionResult> Save([FromBody] List<TeacherTransferBranch> model)
        {
            var userParam = Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var result = await _teacherTransferService.SaveTeacherTransfer(model, userParam.UserId);
            return Ok(result);
        }
    }
}
