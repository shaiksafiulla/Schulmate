using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnquiryStatusController : ControllerBase
    {
        private readonly IEnquiryStatusService _enquiryStatusService;

        public EnquiryStatusController(IEnquiryStatusService enquiryStatusService)
        {
            _enquiryStatusService = enquiryStatusService;
        }
        private UserParam GetUserParam()
        {
            return Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var lst = await _enquiryStatusService.GetAllEnquiryStatus();
            return Ok(lst);
        }

        [HttpGet("View/{id}")]
        [Authorize]
        public async Task<IActionResult> View(int id)
        {
            var examtype = await _enquiryStatusService.ViewEnquiryStatus(id);
            return Ok(examtype);
        }

        [HttpGet("AddEdit/{id}")]
        [Authorize]
        public async Task<IActionResult> AddEdit(int id)
        {
            var examtype = await _enquiryStatusService.GetEnquiryStatus(id);
            return Ok(examtype);
        }

        [HttpGet("IsExist/{name},{id}")]
        [Authorize]
        public async Task<IActionResult> IsExist(string name, int id)
        {
            var isexists = await _enquiryStatusService.IsRecordExists(name, id);
            return Ok(isexists);
        }

        [HttpPost("Save")]
        [Authorize]
        public async Task<IActionResult> Save(EnquiryStatus model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await _enquiryStatusService.SaveEnquiryStatus(model, userParam.UserId);
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
            var result = await _enquiryStatusService.DeleteEnquiryStatus(id, userParam.UserId);
            return Ok(result);
        }
    }
}
