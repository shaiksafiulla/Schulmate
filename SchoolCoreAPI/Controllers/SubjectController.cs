using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var subjects = await _subjectService.GetAllSubject();
            return Ok(subjects);
        }

        [HttpGet("View/{id}")]
        [Authorize]
        public async Task<IActionResult> View(int id)
        {
            var subject = await _subjectService.ViewSubject(id);
            return Ok(subject);
        }

        [HttpGet("AddEdit/{id}")]
        [Authorize]
        public async Task<IActionResult> AddEdit(int id)
        {
            var subject = await _subjectService.GetSubject(id);
            return Ok(subject);
        }

        [HttpGet("IsExist/{name},{mediumId},{id}")]
        [Authorize]
        public async Task<IActionResult> IsExist(string name, int mediumId, int id)
        {
            var exists = await _subjectService.IsRecordExists(name, mediumId, id);
            return Ok(exists);
        }

        [HttpPost("Save")]
        [Authorize]
        public async Task<IActionResult> Save(Subject model)
        {
            var userParam = Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var result = await _subjectService.SaveSubject(model, userParam.UserId);
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

            var result = await _subjectService.DeleteSubject(id, userParam.UserId);
            return Ok(result);
        }
    }
}
