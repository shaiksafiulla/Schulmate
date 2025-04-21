using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminNotificationController : ControllerBase
    {
        private readonly IAdminNotificationService _adminNotificationService;

        public AdminNotificationController(IAdminNotificationService adminNotificationService)
        {
            _adminNotificationService = adminNotificationService;
        }

        private UserParam GetUserParam()
        {
            return Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
        }

        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var lst = await _adminNotificationService.GetAllAdminNotification(userParam.UserId, userParam.SessionYearId);
            return Ok(lst);
        }

        [HttpGet("View/{id}")]
        [Authorize]
        public async Task<IActionResult> View(int id)
        {
            var examtype = await _adminNotificationService.ViewAdminNotification(id);
            return Ok(examtype);
        }

        [HttpGet("UserTypes")]
        [Authorize]
        public async Task<IActionResult> GetUserTypes()
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var lst = await _adminNotificationService.GetUserTypes();
            return Ok(lst);
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
            var teacherAnnouncement = await _adminNotificationService.GetAdminNotification(id, userParam.BranchId, userParam.SessionYearId, userParam.UserId);
            return Ok(teacherAnnouncement);
        }

        [HttpGet("Personnel/{personneltype}")]
        [Authorize]
        public async Task<IActionResult> GetStudPersonNotification(int personnelType)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var sections = await _adminNotificationService.GetStudPersonNotification(userParam.BranchId, userParam.SessionYearId, personnelType);
            return Ok(sections);
        }

        [HttpPost("Save")]
        [Authorize]
        public async Task<IActionResult> Save([FromForm] AdminNotification model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await _adminNotificationService.SaveAdminNotification(model, userParam.UserId);
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
            var result = await _adminNotificationService.DeleteAdminNotification(id, userParam.UserId);
            return Ok(result);
        }

        [HttpGet("Download/{id}")]
        [Authorize]
        public async Task<IActionResult> DownloadAdminPost(int id)
        {
            var result = await _adminNotificationService.GetFileBytes(id);
            return Ok(result);
        }
    }
}
