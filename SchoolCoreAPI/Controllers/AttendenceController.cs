using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using SchoolCoreAPI.Services;

namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendenceController : ControllerBase
    {
        private readonly IAttendenceService _attendenceService;

        public AttendenceController(IAttendenceService attendenceService)
        {
            _attendenceService = attendenceService;
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

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var userParam = GetUserParam();
            var validationResult = ValidateUserParam(userParam);
            if (validationResult != null) return validationResult;

            var vm = await _attendenceService.GetAllAttendence(userParam.UserId);
            return Ok(vm);
        }

        [HttpGet("MBDailyAttendance")]
        [Authorize]
        public async Task<IActionResult> GetDailyAttendence()
        {
            var userParam = GetUserParam();
            var validationResult = ValidateUserParam(userParam);
            if (validationResult != null) return validationResult;

            var branch = await _attendenceService.GetDailyAttendence(userParam.BranchId, userParam.SessionYearId);
            return Ok(branch);
        }

        [HttpGet("AddEdit/{id}")]
        [Authorize]
        public async Task<IActionResult> AddEdit(int id)
        {
            var userParam = GetUserParam();
            var validationResult = ValidateUserParam(userParam);
            if (validationResult != null) return validationResult;

            var branch = await _attendenceService.GetAttendence(id, userParam.UserId);
            return Ok(branch);
        }
        [HttpGet("View/{id}")]
        [Authorize]
        public async Task<IActionResult> View(int id)
        {
            var userParam = GetUserParam();
            var validationResult = ValidateUserParam(userParam);
            if (validationResult != null) return validationResult;

            var branch = await _attendenceService.ViewAttendence(id);
            return Ok(branch);
        }

        [HttpGet("Section/{id}")]
        [Authorize]
        public async Task<IActionResult> GetSectionAttendence(int id)
        {
            var userParam = GetUserParam();
            var validationResult = ValidateUserParam(userParam);
            if (validationResult != null) return validationResult;

            var lst = await _attendenceService.GetSectionAttendence(id, userParam.BranchId, userParam.SessionYearId);
            return Ok(lst);
        }

        [HttpGet("MbSection/{id}")]
        [Authorize]
        public async Task<IActionResult> GetMbSectionAttendence(int id)
        {
            var userParam = GetUserParam();
            var validationResult = ValidateUserParam(userParam);
            if (validationResult != null) return validationResult;

            var lst = await _attendenceService.GetMbSectionAttendence(id, userParam.PersonId, userParam.BranchId, userParam.SessionYearId);
            return Ok(lst);
        }

        [HttpGet("Student/{id},{sectionId}")]
        [Authorize]
        public async Task<IActionResult> GetStudentAttendence(int id, int sectionId, int sessionYearId)
        {
            var userParam = GetUserParam();
            var validationResult = ValidateUserParam(userParam);
            if (validationResult != null) return validationResult;

            var lst = await _attendenceService.GetStudentAttendence(id, sectionId, userParam.SessionYearId);
            return Ok(lst);
        }

        [HttpGet("Personnel")]
        [Authorize]
        public async Task<IActionResult> GetMyAttendence()
        {
            var userParam = GetUserParam();
            var validationResult = ValidateUserParam(userParam);
            if (validationResult != null) return validationResult;

            var lst = await _attendenceService.GetTotalAttendenceByPersonnelId(userParam.PersonId, int.Parse(userParam.UserType), userParam.SessionYearId);
            return Ok(lst);
        }

        [HttpGet("Personnel/{personid},{persontype}")]
        [Authorize]
        public async Task<IActionResult> GetAttendenceByPersonnelId(int personId, string personType)
        {
            var userParam = GetUserParam();
            var validationResult = ValidateUserParam(userParam);
            if (validationResult != null) return validationResult;

            var lst = await _attendenceService.GetAttendenceByPersonnelId(personId, int.Parse(personType), userParam.SessionYearId);
            return Ok(lst);
        }


        [HttpGet("AttendenceMetric/{personnelId},{personnelType}")] 
        [Authorize]
        public async Task<IActionResult> GetFn_AttendenceMetric(int personnelId, int personnelType)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await _attendenceService.GetFn_AttendenceMetric(personnelId, personnelType, userParam.SessionYearId);            
            var result_attendence = result["result_attendence"].ToObject<List<AttendenceMetrics>>();            
            return Ok(result_attendence);
        }


        [HttpGet("MbPersonnel/{personid},{persontype}")]
        [Authorize]
        public async Task<IActionResult> GetAttendenceDetailByPersonnelId(int personId, int personType)
        {
            var userParam = GetUserParam();
            var validationResult = ValidateUserParam(userParam);
            if (validationResult != null) return validationResult;

            var lst = await _attendenceService.GetAttendenceDetailByPersonnelId(personId, personType, userParam.SessionYearId);
            return Ok(lst);
        }

        [HttpGet("PersonnelAttendence/{id},{branchId},{sessionYearId},{personnelType}")]
        [Authorize]
        public async Task<IActionResult> GetPersonnelAttendence(int id, int branchId, int sessionYearId, int personnelType)
        {
            var lst = await _attendenceService.GetPersonnelAttendence(id, branchId, sessionYearId, personnelType);
            return Ok(lst);
        }

        [HttpPost("Save")]
        [Authorize]
        public async Task<IActionResult> Save(Attendence model)
        {
            var userParam = GetUserParam();
            var validationResult = ValidateUserParam(userParam);
            if (validationResult != null) return validationResult;

            var result = await _attendenceService.SaveAttendence(model, userParam.UserId);
            return Ok(result);
        }
    }
}
