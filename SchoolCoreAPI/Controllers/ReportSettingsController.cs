using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportSettingsController : ControllerBase
    {
        private readonly IReportSettingsService _reportSettingsService;

        public ReportSettingsController(IReportSettingsService reportSettingsService)
        {
            _reportSettingsService = reportSettingsService;
        }
        private UserParam GetUserParam()
        {
            return Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
        }

        [HttpGet("AddEdit")]
        [Authorize]
        public async Task<IActionResult> AddEdit()
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var settings = await Task.Run(() => _reportSettingsService.GetReportSettings(userParam.UserId));
            return Ok(settings);
        }

        [HttpPost("Save")]
        [Authorize]
        public async Task<IActionResult> Save([FromForm] ReportSettings model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var result = await Task.Run(() => _reportSettingsService.SaveReportSettings(model, userParam.UserId));
            return Ok(result);
        }
    }
}