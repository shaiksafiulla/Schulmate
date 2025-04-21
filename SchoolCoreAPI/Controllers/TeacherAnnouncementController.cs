using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherAnnouncementController : ControllerBase
    {
        private readonly ITeacherAnnouncementService _teacherAnnouncementService;

        public TeacherAnnouncementController(ITeacherAnnouncementService teacherAnnouncementService)
        {
            _teacherAnnouncementService = teacherAnnouncementService;
        }

        private UserParam GetUserParam()
        {
            return Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
        }

        private IActionResult ValidateUserParam(UserParam userParam)
        {
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            return null;
        }

        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var userParam = GetUserParam();
            var validationResult = ValidateUserParam(userParam);
            if (validationResult != null) return validationResult;

            var lst = await _teacherAnnouncementService.GetAllTeacherAnnouncement(userParam.PersonId, userParam.SessionYearId);
            return Ok(lst);
        }

        [HttpGet("TeacherAnnouncementSection")]
        [Authorize]
        public async Task<IActionResult> GetTeacherAnnouncementSections(int id)
        {
            var userParam = GetUserParam();
            var validationResult = ValidateUserParam(userParam);
            if (validationResult != null) return validationResult;

            var examtype = await _teacherAnnouncementService.GetTeacherAnnouncementSections(userParam.SectionId, userParam.SessionYearId);
            return Ok(examtype);
        }

        [HttpGet("View/{id}")]
        [Authorize]
        public async Task<IActionResult> View(int id)
        {
            var examtype = await _teacherAnnouncementService.ViewTeacherAnnouncement(id);
            return Ok(examtype);
        }

        [HttpGet("AddEdit/{id}")]
        [Authorize]
        public async Task<IActionResult> AddEdit(int id)
        {
            var userParam = GetUserParam();
            var validationResult = ValidateUserParam(userParam);
            if (validationResult != null) return validationResult;

            var teacherAnnouncement = await _teacherAnnouncementService.GetTeacherAnnouncement(id, userParam.PersonId, userParam.BranchId, userParam.SessionYearId);
            return Ok(teacherAnnouncement);
        }

        [HttpGet("Class/{branchid}")]
        [Authorize]
        public async Task<IActionResult> GetClassByBranch(int branchId)
        {
            var classes = await _teacherAnnouncementService.GetClassByBranch(branchId);
            return Ok(classes);
        }

        [HttpGet("TeacherSection/{branchclassid}")]
        [Authorize]
        public async Task<IActionResult> GetSectionByClassAndSubjectTeacher(int branchClassId)
        {
            var userParam = GetUserParam();
            var validationResult = ValidateUserParam(userParam);
            if (validationResult != null) return validationResult;

            var sections = await _teacherAnnouncementService.GetSectionByClassAndSubjectTeacher(branchClassId, userParam.PersonId, userParam.SessionYearId);
            return Ok(sections);
        }

        [HttpGet("Section/{teacherannounceid}")]
        [Authorize]
        public async Task<IActionResult> GetSectionIdsByTeacherAnnouncement(int teacherAnnounceId)
        {
            var sections = await _teacherAnnouncementService.GetSectionIdsByTeacherAnnouncement(teacherAnnounceId);
            return Ok(sections);
        }

        [HttpGet("Subject/{branchclassid}")]
        [Authorize]
        public async Task<IActionResult> GetSubjectByClass(int branchClassId)
        {
            var userParam = GetUserParam();
            var validationResult = ValidateUserParam(userParam);
            if (validationResult != null) return validationResult;

            var sections = await _teacherAnnouncementService.GetSubjectByClass(branchClassId, userParam.PersonId, userParam.SessionYearId);
            return Ok(sections);
        }

        [HttpPost("Save")]
        [Authorize]
        public async Task<IActionResult> Save([FromForm] TeacherAnnouncement model)
        {
            var userParam = GetUserParam();
            var validationResult = ValidateUserParam(userParam);
            if (validationResult != null) return validationResult;

            var result = await _teacherAnnouncementService.SaveTeacherAnnouncement(model, userParam.UserId);
            return Ok(result);
        }

        [HttpDelete("Delete/{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var userParam = GetUserParam();
            var validationResult = ValidateUserParam(userParam);
            if (validationResult != null) return validationResult;

            var result = await _teacherAnnouncementService.DeleteTeacherAnnouncement(id, userParam.UserId);
            return Ok(result);
        }

        [HttpGet("Download/{id}")]
        [Authorize]
        public async Task<IActionResult> DownloadAnnouncement(int id)
        {
            var result = await _teacherAnnouncementService.GetFileBytes(id);
            return Ok(result);
        }
    }
}
