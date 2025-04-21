using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
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

            var lst = await _scheduleService.GetAllSchedule(userParam.BranchId, userParam.SessionYearId);
            return Ok(lst);
        }

        [HttpGet("HallTicket/{scheduleId},{studentId}")]
        [Authorize]
        public async Task<IActionResult> GetHallTicket(string scheduleId, string studentId)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var branch = await _scheduleService.GetHallTicket(scheduleId, studentId, userParam.BranchId);
            return Ok(branch);
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

            var result = await _scheduleService.GetSchedule(id, userParam.BranchId, userParam.SessionYearId);
            return Ok(result);
        }

        [HttpGet("TimeTable/{scheduleId}")]
        [Authorize]
        public async Task<IActionResult> GetPivotTimeTable(string scheduleId)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var result = await _scheduleService.GetPivotTimeTable(scheduleId, userParam.UserId);
            return Ok(result);
        }

        [HttpGet("PersonnelSchedule")]
        [Authorize]
        public async Task<IActionResult> GetSchedulesByPersonnelId()
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var vm = await _scheduleService.GetSchedulesByPersonnelId(userParam.PersonId, userParam.UserType, userParam.SessionYearId, userParam.BranchId);
            return Ok(vm);
        }

        [HttpGet("MBSchedule/{scheduleid}")]
        [Authorize]
        public async Task<IActionResult> GetMBScheduleById(int scheduleId)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var vm = await _scheduleService.GetMBScheduleById(scheduleId);
            return Ok(vm);
        }

        [HttpGet("ExamTimeTable/{scheduleId},{classId}")]
        [Authorize]
        public async Task<IActionResult> GetExamTimeTableByScheduleAndClass(int scheduleId, int classId)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var result = await _scheduleService.GetExamTimeTableByScheduleAndClass(scheduleId, classId, userParam.SessionYearId);
            return Ok(result);
        }

        [HttpGet("Syllabus/{scheduleId},{classId}")]
        [Authorize]
        public async Task<IActionResult> GetExamLessonByScheduleAndClass(int scheduleId, int classId)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var result = await _scheduleService.GetExamLessonByScheduleAndClass(scheduleId, classId, userParam.SessionYearId);
            return Ok(result);
        }

        [HttpGet("BranchClassGroup/{scheduleId}")]
        [Authorize]
        public async Task<IActionResult> GetScheduleBranchClassGroup(string scheduleId)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var result = await _scheduleService.GetScheduleBranchClassGroup(scheduleId, userParam.UserId);
            return Ok(result);
        }

        [HttpGet("MbBranchClass/{scheduleId}")]
        [Authorize]
        public async Task<IActionResult> GetBranchClassesByScheduleId(int scheduleId)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var vm = await _scheduleService.GetBranchClassesByScheduleId(scheduleId, userParam.PersonId, userParam.UserType, userParam.SessionYearId);
            return Ok(vm);
        }

        [HttpGet("MbSubject/{scheduleId},{branchClassId}")]
        [Authorize]
        public async Task<IActionResult> GetSubjectByScheduleAndClass(int scheduleId, int branchClassId)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var vm = await _scheduleService.GetSubjectByScheduleAndClass(scheduleId, branchClassId, userParam.SessionYearId);
            return Ok(vm);
        }

        [HttpGet("MbStudentMark/{scheduleId},{branchClassId},{subjectId}")]
        [Authorize]
        public async Task<IActionResult> GetStudentsByScheduleAndClassAndSubject(int scheduleId, int branchClassId, int subjectId)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var vm = await _scheduleService.GetStudentsByScheduleAndClassAndSubject(scheduleId, branchClassId, userParam.SessionYearId, subjectId);
            return Ok(vm);
        }

        [HttpPost("ClassStudents")]
        [Authorize]
        public async Task<IActionResult> GetScheduleBranchClassStudent(ClassAllocation model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var result = await _scheduleService.GetScheduleBranchClassStudent(model, userParam.UserId);
            return Ok(result);
        }

        [HttpPost("ExamHall/Save")]
        [Authorize]
        public async Task<IActionResult> SaveExamHallAllocation(List<ExamHall> lstModel)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var result = await _scheduleService.SaveScheduleExamHall(lstModel, userParam.UserId);
            return Ok(result);
        }

        [HttpGet("Teacher/{scheduleId}")]
        [Authorize]
        public async Task<IActionResult> GetPivotScheduleTeacher(string scheduleId)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var result = await _scheduleService.GetPivotScheduleTeacher(scheduleId, userParam.UserId);
            return Ok(result);
        }

        [HttpGet("Status/{scheduleId}")]
        [Authorize]
        public async Task<IActionResult> GetScheduleStatus(string scheduleId)
        {
            var result = await _scheduleService.GetScheduleStatus(scheduleId);
            return Ok(result);
        }

        [HttpGet("SpScheduleTeacher/{scheduleId}")]
        [Authorize]
        public async Task<IActionResult> GetSpScheduleTeacher(string scheduleId)
        {
            var result = await _scheduleService.GetSpScheduleTeacher(scheduleId);
            return Ok(result);
        }

        [HttpGet("SpScheduleExam/{scheduleId}")]
        [Authorize]
        public async Task<IActionResult> GetSpScheduleExam(string scheduleId)
        {
            var result = await _scheduleService.GetSpScheduleExam(scheduleId);
            return Ok(result);
        }

        [HttpGet("ExamDateTime/{scheduleId}")]
        [Authorize]
        public async Task<IActionResult> GetScheduleExamDateTime(string scheduleId)
        {
            var result = await _scheduleService.GetScheduleExamDateTime(scheduleId);
            return Ok(result);
        }

        [HttpPost("Save")]
        [Authorize]
        public async Task<IActionResult> Save(Schedule model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var result = await _scheduleService.SaveSchedule(model, userParam.UserId);
            return Ok(result);
        }

        [HttpPost("ClassSubject/Save")]
        [Authorize]
        public async Task<IActionResult> SaveScheduleBranchClassSubject(ScheduleBranchClassSubjectVM model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var result = await _scheduleService.SaveScheduleBranchClassSubject(model, userParam.UserId);
            return Ok(result);
        }

        [HttpPost("ExamAlgo/{scheduleId}")]
        [Authorize]
        public async Task<IActionResult> SaveScheduleExamAlgo(string scheduleId)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var result = await _scheduleService.SaveScheduleExamAlgo(scheduleId, userParam.UserId);
            return Ok(result);
        }

        [HttpPost("TimeTableStatus/{scheduleId}")]
        [Authorize]
        public async Task<IActionResult> UpdateScheduleTimeTableStatus(string scheduleId)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var result = await _scheduleService.UpdateScheduleTimeTableStatus(scheduleId, userParam.UserId);
            return Ok(result);
        }

        [HttpPost("TeacherStatus/{scheduleId}")]
        [Authorize]
        public async Task<IActionResult> UpdateScheduleTeacherStatus(string scheduleId)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var result = await _scheduleService.UpdateScheduleTeacherStatus(scheduleId, userParam.UserId);
            return Ok(result);
        }

        [HttpPost("FinishStatus/{scheduleId}")]
        [Authorize]
        public async Task<IActionResult> UpdateScheduleFinishStatus(string scheduleId)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var result = await _scheduleService.UpdateScheduleFinishStatus(scheduleId, userParam.UserId);
            return Ok(result);
        }

        [HttpPost("TeacherExamDateAlgo/{scheduleId}")]
        [Authorize]
        public async Task<IActionResult> SaveScheduleTeacherExamDateAlgo(string scheduleId)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var result = await _scheduleService.SaveScheduleTeacherExamDateAlgo(scheduleId, userParam.UserId);
            return Ok(result);
        }

        [HttpPost("SaveAlgoExam")]
        [Authorize]
        public async Task<IActionResult> SaveScheduleExam(ScheduleBrclsSubject model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var result = await _scheduleService.SaveScheduleExam(model, userParam.UserId);
            return Ok(result);
        }

        [HttpPost("TeacherExamDate/save")]
        [Authorize]
        public async Task<IActionResult> SaveScheduleTeacherExamDate(ScheduleTeacherExam model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var result = await _scheduleService.SaveScheduleTeacherExamDate(model, userParam.UserId);
            return Ok(result);
        }

        [HttpPost("Examination/save")]
        [Authorize]
        public async Task<IActionResult> SaveScheduleExamination(ScheduleExam model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var result = await _scheduleService.SaveScheduleExamination(model, userParam.UserId);
            return Ok(result);
        }

        [HttpPost("Date/search")]
        [Authorize]
        public async Task<IActionResult> GetScheduleDate(SearchScheduleDate search)
        {
            var result = await _scheduleService.GetScheduleDate(search.StartDate, search.EndDate, search.ScheduleId);
            return Ok(result);
        }

        [HttpGet("BranchClassExam/{scheduleId}")]
        [Authorize]
        public async Task<IActionResult> GetScheduleBranchClassExam(string scheduleId)
        {
            var result = await _scheduleService.GetScheduleBranchClassExam(scheduleId);
            return Ok(result);
        }

        [HttpGet("ClassByBranch/{scheduleId},{branchId}")]
        [Authorize]
        public async Task<IActionResult> GetScheduleClassByBranch(string scheduleId, string branchId)
        {
            var result = await _scheduleService.GetScheduleClassByBranch(scheduleId, branchId);
            return Ok(result);
        }

        [HttpGet("TeacherExamDate/{scheduleId}")]
        [Authorize]
        public async Task<IActionResult> GetScheduleTeacherExamDate(string scheduleId)
        {
            var result = await _scheduleService.GetScheduleTeacherExamDate(scheduleId);
            return Ok(result);
        }

        [HttpGet("ScheduleStudentSeat/{scheduleId}")]
        [Authorize]
        public async Task<IActionResult> GetCastScheduleExamHall(string scheduleId)
        {
            var result = await _scheduleService.GetCastScheduleExamHall(scheduleId);
            return Ok(result);
        }

        [HttpGet("ScheduleStudent/{scheduleId},{branchClassId}")]
        [Authorize]
        public async Task<IActionResult> GetScheduleStudentInfo(string scheduleId, string branchClassId)
        {
            var result = await _scheduleService.GetScheduleStudentInfo(scheduleId, branchClassId);
            return Ok(result);
        }

        [HttpGet("Examination/{scheduleId},{branchId}")]
        [Authorize]
        public async Task<IActionResult> GetScheduleExamination(string scheduleId, string branchId)
        {
            var result = await _scheduleService.GetScheduleExamination(scheduleId, branchId);
            return Ok(result);
        }

        [HttpGet("BranchClass/{id}")]
        [Authorize]
        public async Task<IActionResult> GetBranchClassBySchedule(int id)
        {
            var vm = await _scheduleService.GetBranchClassBySchedule(id);
            return Ok(vm);
        }

        [HttpGet("BranchClassSubject/{scheduleId}")]
        [Authorize]
        public async Task<IActionResult> GetBranchClassSubjectBySchedule(string scheduleId)
        {
            var result = await _scheduleService.GetSPBranchClassSubject(scheduleId);
            var examResult = new SpScheduleSubject
            {
                PreResult = result.Item1,
                PostResult = result.Item2
            };
            return Ok(examResult);
        }

        [HttpGet("ClassSection/{scheduleId}")]
        [Authorize]
        public async Task<IActionResult> GetScheduleClassAndSectionByBranch(string scheduleId)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var result = await _scheduleService.GetScheduleClassAndSectionByBranch(userParam.BranchId.ToString(), scheduleId);
            var obj = new ScheduleClassAndSectionAndTeacher
            {
                StrClass = result.Item1,
                StrTeacher = result.Item2
            };
            return Ok(obj);
        }

        [HttpGet("SPSection/{id}")]
        [Authorize]
        public async Task<IActionResult> GetSPScheduleSection(int id)
        {
            var result = await _scheduleService.GetSPScheduleSection(id);
            var obj = new SpScheduleStudentSection
            {
                StrResult = result
            };
            return Ok(obj);
        }

        [HttpGet("View/{id}")]
        [Authorize]
        public async Task<IActionResult> View(int id)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var branch = await _scheduleService.ViewSchedule(id, userParam.BranchId);
            return Ok(branch);
        }
    }
}
