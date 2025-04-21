using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private readonly ITeacherService _teacherService;

        public TeacherController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var userParam = Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var lst = await _teacherService.GetAllTeacher(userParam.UserId);
            return Ok(lst);
        }

        [HttpGet("View/{id}")]
        [Authorize]
        public async Task<IActionResult> View(int id)
        {
            var branch = await _teacherService.ViewTeacher(id);
            return Ok(branch);
        }

        [HttpGet("TeacherDetailView/{id}")]
        [Authorize]
        public async Task<IActionResult> GetTeacherDetail(int id)
        {
            var userParam = Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await  _teacherService.GetTeacherDetail(id, userParam.BranchId);
            return Ok(result);
        }

        [HttpGet("AddEdit/{id}")]
        [Authorize]
        public async Task<IActionResult> AddEdit(int id)
        {
            var branch = await _teacherService.GetTeacher(id);
            return Ok(branch);
        }

        [HttpGet("IsExist/{fname},{lname},{id}")]
        [Authorize]
        public async Task<IActionResult> IsExist(string fname, string lname, int id)
        {
            var isExists = await _teacherService.IsRecordExists(fname, lname, id);
            return Ok(isExists);
        }

        [HttpPost("Save")]
        [Authorize]
        public async Task<IActionResult> Save([FromForm] Personnel model)
        {
            var userParam = Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await _teacherService.SaveTeacher(model, userParam.UserId);
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
            var result = await _teacherService.DeleteTeacher(id, userParam.UserId);
            return Ok(result);
        }

        #region MBTEACHER
        [HttpGet("TransTimeTable")]
        [Authorize]
        public async Task<IActionResult> GetTeacherTransTimeTableVM()
        {
            var userParam = Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var cls = await  _teacherService.GetTeacherTransTimeTableVM(userParam.PersonId, userParam.SessionYearId);
            return Ok(cls);
        }

        [HttpGet("TimeTable")]
        [Authorize]
        public async Task<IActionResult> GetTimeTableByTeacherAndSessionYear()
        {
            var userParam = Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var lst = await _teacherService.GetTimeTable(userParam.PersonId, userParam.SessionYearId, userParam.UserId);
            return Ok(lst);
        }
        #endregion

        #region WEBTEACHER
        [HttpGet("WebTimeTable/{TeacherId}")]
        [Authorize]
        public async Task<IActionResult> GetTeacherTimeTable(int teacherId)
        {
            var cls = await _teacherService.GetTeacherTimeTable(teacherId);
            return Ok(cls);
        }

        [HttpGet("WebTimeTableDB/{TeacherId}")]
        [Authorize]
        public async Task<IActionResult> GetTimeTableByTeacherAndSessionYear(int teacherId)
        {
            var userParam = Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var lst = await _teacherService.GetTimeTable(teacherId, userParam.SessionYearId, userParam.UserId);
            return Ok(lst);
        }
        #endregion

        [HttpGet("Schedule/{id}")]
        [Authorize]
        public async Task<IActionResult> GetTeacherSchedules(string id)
        {
            var vm = await _teacherService.GetTeacherSchedules(id);
            return Ok(vm);
        }

        [HttpGet("TeacherInvigilation/{scheduleId}")]
        [Authorize]
        public async Task<IActionResult> GetInvigilationsByTeacherId(int scheduleId)
        {
            var userParam = Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var vm = await _teacherService.GetInvigilationsByTeacherId(userParam.PersonId, scheduleId);
            return Ok(vm);
        }

        [HttpGet("PivotSchedule/{scheduleId},{teacherId}")]
        [Authorize]
        public async Task<IActionResult> GetPivotScheduleByTeacher(string scheduleId, string teacherId)
        {
            var vm = await _teacherService.GetPivotScheduleByTeacher(scheduleId, teacherId);
            return Ok(vm);
        }

        [HttpGet("Performance/{scheduleId}")]
        [Authorize]
        public async Task<IActionResult> GetTeacherPerformance(int scheduleId)
        {
            var result = await  _teacherService.GetTeacherPerformance(scheduleId);
            return Ok(result);
        }
    }
}
