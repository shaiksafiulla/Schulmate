using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentAssignmentController : ControllerBase
    {
        private readonly IStudentAssignmentService _studentAssignmentService;

        public StudentAssignmentController(IStudentAssignmentService studentAssignmentService)
        {
            _studentAssignmentService = studentAssignmentService;
        }
        private UserParam GetUserParam()
        {
            return Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
        }


        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var lst = await _studentAssignmentService.GetAllStudentAssignment();
            return Ok(lst);
        }

        [HttpGet("View/{id}")]
        [Authorize]
        public async Task<IActionResult> View(int id)
        {
            var examtype = await _studentAssignmentService.ViewStudentAssignment(id);
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
            var assignment = await _studentAssignmentService.GetStudentAssignment(id, userParam.PersonId, userParam.BranchId, userParam.SessionYearId);
            return Ok(assignment);
        }

        [HttpGet("Class/{branchid}")]
        [Authorize]
        public async Task<IActionResult> GetClassByBranch(int branchId)
        {
            var classes = await _studentAssignmentService.GetClassByBranch(branchId);
            return Ok(classes);
        }

        [HttpGet("Section/{branchid},{classid}")]
        [Authorize]
        public async Task<IActionResult> GetSectionByClass(int branchId, int classId)
        {
            var sections = await _studentAssignmentService.GetSectionByClass(branchId, classId);
            return Ok(sections);
        }

        [HttpGet("Section/{studentassignid}")]
        [Authorize]
        public async Task<IActionResult> GetSectionIdsByStudentAssignment(int studentAssignId)
        {
            var sections = await _studentAssignmentService.GetSectionIdsByStudentAssignment(studentAssignId);
            return Ok(sections);
        }

        [HttpGet("Subject/{classid},{teacherid}")]
        [Authorize]
        public async Task<IActionResult> GetSubjectByClass(int classId, int teacherId)
        {
            var sections = await _studentAssignmentService.GetSubjectByClass(classId, teacherId);
            return Ok(sections);
        }

        [HttpPost("Save")]
        [Authorize]
        public async Task<IActionResult> Save([FromForm] StudentAssignment model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await _studentAssignmentService.SaveStudentAssignment(model, userParam.UserId);
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
            var result = await _studentAssignmentService.DeleteStudentAssignment(id, userParam.UserId);
            return Ok(result);
        }

        [HttpGet("Student")]
        [Authorize]
        public async Task<IActionResult> GetAllStudentAssign()
        {
            var lst = await _studentAssignmentService.GetAllStudentAssignStatus();
            return Ok(lst);
        }

        [HttpGet("Student/{assignid}")]
        [Authorize]
        public async Task<IActionResult> GetStudentAssignStatus(int assignId)
        {
            var examtype = await _studentAssignmentService.GetStudentAssignStatus(assignId);
            return Ok(examtype);
        }

        [HttpPost("Student/Save")]
        [Authorize]
        public async Task<IActionResult> SaveStudentAssign(StudentAssignStatus model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await _studentAssignmentService.SaveStudentAssignStatus(model, userParam.UserId);
            return Ok(result);
        }

        [HttpGet("Student/View/{assignid}")]
        [Authorize]
        public async Task<IActionResult> ViewStudentAssignStatus(int assignId)
        {
            var examtype = await _studentAssignmentService.ViewStudentAssignStatus(assignId);
            return Ok(examtype);
        }
    }
}
