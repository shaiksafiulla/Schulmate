using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediumController : ControllerBase
    {
        private readonly IMediumService _mediumService;

        public MediumController(IMediumService mediumService)
        {
            _mediumService = mediumService;
        }
        private UserParam GetUserParam()
        {
            return Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var lst = await _mediumService.GetAllMedium();
            return Ok(lst);
        }

        [HttpGet("View/{id}")]
        [Authorize]
        public async Task<IActionResult> View(int id)
        {
            var examtype = await _mediumService.ViewMedium(id);
            return Ok(examtype);
        }

        [HttpGet("AddEdit/{id}")]
        [Authorize]
        public async Task<IActionResult> AddEdit(int id)
        {
            var medium = await _mediumService.GetMedium(id);
            return Ok(medium);
        }

        [HttpGet("IsExist/{name},{id}")]
        [Authorize]
        public async Task<IActionResult> IsExist(string name, int id)
        {
            var isexists = await _mediumService.IsRecordExists(name, id);
            return Ok(isexists);
        }

        [HttpPost("Save")]
        [Authorize]
        public async Task<IActionResult> Save([FromBody] Medium model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await _mediumService.SaveMedium(model, userParam.UserId);
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
            var result = await _mediumService.DeleteMedium(id, userParam.UserId);
            return Ok(result);
        }
    }
}
