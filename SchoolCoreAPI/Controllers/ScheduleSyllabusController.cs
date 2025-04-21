using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleSyllabusController : ControllerBase
    {
        private readonly IScheduleSyllabusService _scheduleSyllabusService;

        public ScheduleSyllabusController(IScheduleSyllabusService scheduleSyllabusService)
        {
            _scheduleSyllabusService = scheduleSyllabusService;
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
            var lst = await _scheduleSyllabusService.GetAllScheduleSyllabus(userParam.BranchId);
            return Ok(lst);
        }

        [HttpGet("ExamPaper/{id}")]
        [Authorize]
        public async Task<IActionResult> GetExaminationPaper(int id)
        {
            var result = await _scheduleSyllabusService.GetExaminationPaper(id);
            return Ok(result);
        }

        [HttpPost("ExamPaper/save")]
        [Authorize]
        public async Task<IActionResult> SaveExaminationPaper(ExaminationPaper model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await _scheduleSyllabusService.SaveExaminationPaper(model, userParam.UserId);
            return Ok(result);
        }

        [HttpGet("View/{id}")]
        [Authorize]
        public async Task<IActionResult> GetScheduleSyllabus(int id)
        {
            var result = await _scheduleSyllabusService.GetScheduleSyllabus(id);
            return Ok(result);
        }

        [HttpGet("ScheduleSyllabusView/{id}")]
        [Authorize]
        public async Task<IActionResult> GetScheduleSyllabusDetail(int id)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await _scheduleSyllabusService.GetScheduleSyllabusDetail(id, userParam.BranchId);
            return Ok(result);
        }

        [HttpGet("Topic/{id}")]
        [Authorize]
        public async Task<IActionResult> GetSyllabusTopic(int id)
        {
            var result = await _scheduleSyllabusService.GetSPSyllabusTopic(id);
            var topic = new SpSyllabusTopic { StrResult = result };
            return Ok(topic);
        }

        [HttpGet("Examination/{id}")]
        [Authorize]
        public async Task<IActionResult> GetExaminationByScheduleId(int id)
        {
            var result = await _scheduleSyllabusService.GetExaminationByScheduleId(id);
            return Ok(result);
        }

        [HttpGet("QuestionPaper/{examId}")]
        [Authorize]
        public async Task<IActionResult> GetQuestionPaper(int examId)
        {
            var result = await _scheduleSyllabusService.GetQuestionPaper(examId);
            return Ok(result);
        }

        [HttpGet("ExamSubject/{scheduleId},{branchclassId}")]
        [Authorize]
        public async Task<IActionResult> GetExamSubjectByBranchClass(string scheduleId, string branchclassId)
        {
            var result = await _scheduleSyllabusService.GetExamSubjectByBranchClass(scheduleId, branchclassId);
            return Ok(result);
        }

        [HttpGet("QuestionStatus/{scheduleId},{branchclassId}")]
        [Authorize]
        public async Task<IActionResult> GetBranchClassQuestionStatus(string scheduleId, string branchclassId)
        {
            var result = await _scheduleSyllabusService.GetBranchClassQuestionStatus(scheduleId, branchclassId);
            return Ok(result);
        }

        [HttpGet("ExamTopic/{examId}")]
        [Authorize]
        public async Task<IActionResult> GetExamTopic(string examId)
        {
            var result = await _scheduleSyllabusService.GetExamTopic(examId);
            return Ok(result);
        }

        [HttpPost("ExamTopic/save")]
        [Authorize]
        public async Task<IActionResult> UpdateExamTopic(LessonVM model)
        {
            var result = await _scheduleSyllabusService.UpdateExamTopic(model);
            return Ok(result);
        }

        [HttpPost("UploadQPaper")]
        [Authorize]
        public async Task<IActionResult> UploadQPaper([FromForm] UploadExamQPaper model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await _scheduleSyllabusService.UploadQPaper(model, userParam.UserId);
            return Ok(result);
        }

        [HttpGet("ExamQuestion/{id}")]
        [Authorize]
        public async Task<IActionResult> GetExamQuestion(int id)
        {
            var result = await _scheduleSyllabusService.GetExamQuestion(id);
            return Ok(result);
        }

        [HttpPost("ExamQuestion/save")]
        [Authorize]
        public async Task<IActionResult> UpdateExamQuestion(ExamQuestionVM model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await _scheduleSyllabusService.UpdateExamQuestion(model, userParam.UserId);
            return Ok(result);
        }

        [HttpPost("ProcessQuestion")]
        [Authorize]
        public async Task<IActionResult> ProcessQuestionAlgo(ExamQuestionVM model)
        {
            var result = await _scheduleSyllabusService.ProcessQuestionAlgo(model);
            return Ok(result);
        }
    }
}
