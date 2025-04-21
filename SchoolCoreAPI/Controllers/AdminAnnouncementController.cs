using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminAnnouncementController : ControllerBase
    {
        private readonly IAdminAnnouncementService _adminAnnouncementService;

        public AdminAnnouncementController(IAdminAnnouncementService adminAnnouncementService)
        {
            _adminAnnouncementService = adminAnnouncementService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var userParam = Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var lst = await Task.Run(() => _adminAnnouncementService.GetAllAdminAnnouncement(userParam.UserId));
            return Ok(lst);
        }

        [HttpGet("View/{id}")]
        [Authorize]
        public async Task<IActionResult> View(int id)
        {
            var examtype = await Task.Run(() => _adminAnnouncementService.ViewAdminAnnouncement(id));
            return Ok(examtype);
        }

        [HttpGet("AddEdit/{id}")]
        [Authorize]
        public async Task<IActionResult> AddEdit(int id)
        {
            var userParam = Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var teacherAnnouncement = await Task.Run(() => _adminAnnouncementService.GetAdminAnnouncement(id, userParam.UserId));
            return Ok(teacherAnnouncement);
        }

        [HttpGet("Section/{adminannounceid}")]
        [Authorize]
        public async Task<IActionResult> GetSectionIdsByTeacherAnnouncement(int adminAnnounceId)
        {
            var sections = await Task.Run(() => _adminAnnouncementService.GetSectionIdsByAdminAnnouncement(adminAnnounceId));
            return Ok(sections);
        }

        [HttpPost("Save")]
        [Authorize]
        public async Task<IActionResult> Save([FromForm] AdminAnnouncement model)
        {
            var userParam = Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await Task.Run(() => _adminAnnouncementService.SaveAdminAnnouncement(model, userParam.UserId));
            return Ok(result);
        }

        [HttpDelete("Delete/{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var userParam = Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await Task.Run(() => _adminAnnouncementService.DeleteAdminAnnouncement(id, userParam.UserId));
            return Ok(result);
        }
    }
}
