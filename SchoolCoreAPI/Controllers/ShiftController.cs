using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftController : ControllerBase
    {
        private readonly IShiftService _shiftService;

        public ShiftController(IShiftService shiftService)
        {
            _shiftService = shiftService;
        }
        private UserParam GetUserParam()
        {
            return Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var lst = await _shiftService.GetAllShift();
            return Ok(lst);
        }

        [HttpGet("View/{id}")]
        [Authorize]
        public async Task<IActionResult> View(int id)
        {
            var shift = await _shiftService.ViewShift(id);
            return Ok(shift);
        }

        [HttpGet("AddEdit/{id}")]
        [Authorize]
        public async Task<IActionResult> AddEdit(int id)
        {
            var examType = await _shiftService.GetShift(id);
            return Ok(examType);
        }

        [HttpGet("IsExist/{fromtime},{totime},{id}")]
        [Authorize]
        public async Task<IActionResult> IsExist(string fromtime, string totime, int id)
        {
            var isExists = await _shiftService.IsRecordExists(fromtime, totime, id);
            return Ok(isExists);
        }

        [HttpPost("Save")]
        [Authorize]
        public async Task<IActionResult> Save([FromBody] Shift model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await _shiftService.SaveShift(model, userParam.UserId);
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
            var result = await _shiftService.DeleteShift(id, userParam.UserId);
            return Ok(result);
        }
    }
}
