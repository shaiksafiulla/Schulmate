using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly IDriverService _DriverService;

        public DriverController(IDriverService DriverService)
        {
            _DriverService = DriverService;
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

            var lst = await _DriverService.GetAllDriver(userParam.UserId);
            return Ok(lst);
        }

        [HttpGet("View/{id}")]
        [Authorize]
        public async Task<IActionResult> View(int Id)
        {
            var branch = await _DriverService.ViewDriver(Id);
            return Ok(branch);
        }

        [HttpGet("AddEdit/{id}")]
        [Authorize]
        public async Task<IActionResult> AddEdit(int Id)
        {
            var branch = await _DriverService.GetDriver(Id);
            return Ok(branch);
        }

        [HttpGet("IsExist/{fname},{lname},{id}")]
        [Authorize]
        public async Task<IActionResult> IsExist(string fname, string lname, int BranchId, int Id)
        {
            var isexists = await _DriverService.IsRecordExists(fname, lname, BranchId, Id);
            return Ok(isexists);
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

            var result = await _DriverService.SaveDriver(model, userParam.UserId);
            return Ok(result);
        }

        [HttpDelete("Delete/{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int Id)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var result = await _DriverService.DeleteDriver(Id, userParam.UserId);
            return Ok(result);
        }
    }
}
