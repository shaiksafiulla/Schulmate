using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveRequestController : ControllerBase
    {
        private readonly ILeaveRequestService _leaveRequestService;

        public LeaveRequestController(ILeaveRequestService leaveRequestService)
        {
            _leaveRequestService = leaveRequestService;
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
            var lst = await _leaveRequestService.GetAllLeaveRequest(userParam.UserId);
            return Ok(lst);
        }

        [HttpGet("Personnel/{personnelId},{personnelType}")]
        [Authorize]
        public async Task<IActionResult> GetLeaveRequestByPersonnelId(int personnelId, string personnelType)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var lst = await _leaveRequestService.GetLeaveRequestByPersonnelId(personnelId, personnelType);
            return Ok(lst);
        }

        [HttpGet("View/{id}")]
        [Authorize]
        public async Task<IActionResult> View(int id)
        {
            var examtype = await _leaveRequestService.ViewLeaveRequest(id);
            return Ok(examtype);
        }

        [HttpGet("AddEdit/{id}")]
        [Authorize]
        public async Task<IActionResult> AddEdit(int id)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var examtype = await _leaveRequestService.GetLeaveRequest(id, userParam.PersonId, userParam.UserType, userParam.BranchId, userParam.SessionYearId);
            return Ok(examtype);
        }

        [HttpPost("Save")]
        [Authorize]
        public async Task<IActionResult> Save(LeaveRequest model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await _leaveRequestService.SaveLeaveRequest(model, userParam.UserId);
            return Ok(result);
        }

        [HttpPost("Update")]
        [Authorize]
        public async Task<IActionResult> Update(CastLeaveRequest model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await _leaveRequestService.UpdateLeaveRequest(model, userParam.UserId);
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
            var result = await _leaveRequestService.DeleteLeaveRequest(id, userParam.UserId);
            return Ok(result);
        }

        [HttpGet("LeaveReport")]
        [Authorize]
        public async Task<IActionResult> GetLeaveReport()
        {
            var lst = await _leaveRequestService.GetLeaveReport();
            return Ok(lst);
        }

        [HttpGet("LeaveReport/{personid}")]
        [Authorize]
        public async Task<IActionResult> GetSPLeaveReport(int personId)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var lst = await _leaveRequestService.GetSPLeaveReport(personId);
            return Ok(lst);
        }
    }
}
