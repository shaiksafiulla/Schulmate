using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }
        private UserParam GetUserParam()
        {
            return Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
        }      

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var userParam =  GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var lst = await _studentService.GetAllStudent(userParam.UserId);
            return Ok(lst);
        }

        [HttpGet("DefaultMbStudent")]
        [Authorize]
        public async Task<IActionResult> LoadDefaultMbStudents()
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var vm = await _studentService.LoadDefaultMbStudents(userParam.PersonId, userParam.BranchId, userParam.UserType, userParam.SessionYearId);
            return Ok(vm);
        }

        [HttpGet("MbStudent/{query}")]
        [Authorize]
        public async Task<IActionResult> SearchMbStudents(string query)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var vm = await _studentService.SearchMbStudents(userParam.PersonId, userParam.BranchId, userParam.UserType, userParam.SessionYearId, query);
            return Ok(vm);
        }

        [HttpGet("Payments/{studentId}")]
        [Authorize]
        public async Task<IActionResult> GetAllPaymentByStudentId(int studentId)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var lst = await _studentService.GetAllPaymentByStudentId(studentId, userParam.UserId);
            return Ok(lst);
        }

        [HttpGet("View/{id}")]
        [Authorize]
        public async Task<IActionResult> View(int id)
        {
            var branch = await _studentService.ViewStudent(id);
            return Ok(branch);
        }

        [HttpGet("StudentDetailView/{id}")]
        [Authorize]
        public async Task<IActionResult> GetStudentDetail(int id)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await _studentService.GetStudentDetail(id, userParam.BranchId);
            return Ok(result);
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
            var branch = await _studentService.GetStudent(id, userParam.UserId);
            return Ok(branch);
        }

        [HttpGet("IsExist/{fname},{lname},{id}")]
        [Authorize]
        public async Task<IActionResult> IsExist(string fname, string lname, int id)
        {
            var isexists = await _studentService.IsRecordExists(fname, lname, id);
            return Ok(isexists);
        }

        [HttpGet("Class/{branchId}")]
        [Authorize]
        public async Task<IActionResult> GetClassesByBranch(int branchId)
        {
            var branch = await _studentService.GetClassesByBranch(branchId);
            return Ok(branch);
        }

        [HttpGet("Section/{branchId},{classId}")]
        [Authorize]
        public async Task<IActionResult> GetSectionsByBranchAndClass(int branchId, int classId)
        {
            var branch = await _studentService.GetSectionsByBranchAndClass(branchId, classId);
            return Ok(branch);
        }

        [HttpPost("Save")]
        [Authorize]
        public async Task<IActionResult> Save([FromForm] Student model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await _studentService.SaveStudent(model, userParam.UserId);
            return Ok(result);
        }

        [HttpGet("StudentPay/{studentid}")]
        [Authorize]
        public async Task<IActionResult> GetStudentPay(int studentId)
        {
            var studentPay = await _studentService.GetStudentPay(studentId);
            return Ok(studentPay);
        }

        [HttpPost("StudentPay/Save")]
        [Authorize]
        public async Task<IActionResult> SaveStudentPay([FromForm] StudentPay model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await _studentService.SaveStudentPay(model, userParam.UserId);
            return Ok(result);
        }       
        [HttpGet("StudentFeeReceipt/{paymentid}")]
        [Authorize]
        public async Task<IActionResult> GetStudentFeeReceipt(int paymentid)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await _studentService.GetStudentFeeReceipt(paymentid);
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
            var result = await _studentService.DeleteStudent(id, userParam.UserId);
            return Ok(result);
        }

        [HttpGet("Schedule/{id}")]
        [Authorize]
        public async Task<IActionResult> GetStudentSchedule(int id)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var schedule = await _studentService.GetStudentScheduleVM(id, userParam.SessionYearId);
            return Ok(schedule);
        }

        [HttpGet("ScheduleResult/{id},{rptcardId}")]
        [Authorize]
        public async Task<IActionResult> GetStudentScheduleResult(int id, int rptcardId)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var schresult = await _studentService.GetStudentScheduleResultVM(id, rptcardId, userParam.SessionYearId);
            return Ok(schresult);
        }

        [HttpGet("ReportCard/{StudentId}")]
        [Authorize]
        public async Task<IActionResult> GetStudentMbReportCardList(int studentId)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var schresult = await _studentService.GetStudentMbReportCardList(studentId, userParam.SessionYearId);
            return Ok(schresult);
        }

        [HttpGet("ScheduleMark/{id},{scheduleId}")]
        [Authorize]
        public async Task<IActionResult> GetStudentMark(int id, int scheduleId)
        {
            var schresult = await _studentService.GetStudentMarkVM(id, scheduleId);
            return Ok(schresult);
        }

        [HttpGet("SubjectMark/{id},{subject}")]
        [Authorize]
        public async Task<IActionResult> GetStudentSubjectMark(int id, string subject)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var schresult = await _studentService.GetStudentMarkSubjectVM(id, subject, userParam.SessionYearId);
            return Ok(schresult);
        }

        [HttpGet("Subjects/{studentId}")]
        [Authorize]
        public async Task<IActionResult> GetStudentSubjects(int studentId)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var schresult = await _studentService.GetStudentSubjects(studentId, userParam.SessionYearId);
            return Ok(schresult);
        }

        [HttpGet("SubjectGraph/{studentId},{subjectId}")]
        [Authorize]
        public async Task<IActionResult> GetStudentSubjectGraph(int studentId, int subjectId)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var schresult = await _studentService.GetStudentSubjectGraphVM(studentId, subjectId, userParam.SessionYearId);
            return Ok(schresult);
        }
    }
}
