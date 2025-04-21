using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private readonly IClassService _classService;
        public ClassController(IClassService classService)
        {
            _classService = classService;
        }
        private UserParam GetUserParam()
        {
            return Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var lst = await _classService.GetAllClass();
            return Ok(lst);
        }

        [HttpGet("View/{id}")]
        [Authorize]
        public async Task<IActionResult> View(int id)
        {
            var cls = await _classService.ViewClass(id);
            return Ok(cls);
        }

        [HttpGet("AddEdit/{id}")]
        [Authorize]
        public async Task<IActionResult> AddEdit(int id)
        {
            var cls = await _classService.GetClass(id);
            return Ok(cls);
        }

        [HttpGet("IsExist/{name},{mediumId},{id}")]
        [Authorize]
        public async Task<IActionResult> IsExist(string name, int mediumId, int id)
        {
            var isExists = await _classService.IsRecordExists(name, mediumId, id);
            return Ok(isExists);
        }

        [HttpPost("Save")]
        [Authorize]
        public async Task<IActionResult> Save(Class model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await _classService.SaveClass(model, userParam.UserId);
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
            var result = await _classService.DeleteClass(id, userParam.UserId);
            return Ok(result);
        }

        [HttpGet("ClassSubject/{id}")]
        [Authorize]
        public async Task<IActionResult> AddEditClassSubject(int id)
        {
            var vm = await _classService.GetClassSubject(id);
            return Ok(vm);
        }

        [HttpPost("ClassSubject/Save")]
        [Authorize]
        public async Task<IActionResult> UpdateClassSubject(SubjectVM model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var vm = await _classService.UpdateClassSubject(model, userParam.UserId);
            return Ok(vm);
        }
    }
}
