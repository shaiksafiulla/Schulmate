using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
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

            var lst = await _staffService.GetAllStaff(userParam.UserId);
            return Ok(lst);
        }

        [HttpGet("View/{id}")]
        [Authorize]
        public async Task<IActionResult> View(int id)
        {
            var branch = await _staffService.ViewStaff(id);
            return Ok(branch);
        }

        [HttpGet("AddEdit/{id}")]
        [Authorize]
        public async Task<IActionResult> AddEdit(int id)
        {
            var branch = await _staffService.GetStaff(id);
            return Ok(branch);
        }

        [HttpGet("IsExist/{fname},{lname},{branchId},{id}")]
        [Authorize]
        public async Task<IActionResult> IsExist(string fname, string lname, int branchId, int id)
        {
            var isExists = await _staffService.IsRecordExists(fname, lname, branchId, id);
            return Ok(isExists);
        }

        [HttpPost("Save")]
        [Authorize]
        public async Task<IActionResult> Save([FromForm] Personnel model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var result = await _staffService.SaveStaff(model, userParam.UserId);
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

            var result = await _staffService.DeleteStaff(id, userParam.UserId);
            return Ok(result);
        }
    }
}
