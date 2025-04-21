using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentTransferController : ControllerBase
    {        
        private readonly IStudentTransferService _StudentTransferService;        
        string struserid;
        public StudentTransferController(IStudentTransferService StudentTransferService)
        {
           _StudentTransferService = StudentTransferService;           
        }       
        [HttpGet]       
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var branch = await _StudentTransferService.GetStudentTransferVM();
            return Ok(branch);
        }
        [HttpGet("Class/{branchid}")]
        [Authorize]
        public async Task<IActionResult> GetClassByBranch(int BranchId)
        {
            var classes = await _StudentTransferService.GetClassByBranch(BranchId);
            return Ok(classes);
        }
        [HttpGet("Section/{branchid},{classid}")]
        [Authorize]
        public async Task<IActionResult> GetSectionByClass(int BranchId, int ClassId)
        {
            var sections = await _StudentTransferService.GetSectionByClass(BranchId, ClassId);
            return Ok(sections);
        }
        [HttpGet("Student/{sectionid}")]
        [Authorize]
        public async Task<IActionResult> GetStudents(int SectionId)
        {
            var students = await _StudentTransferService.GetStudents(SectionId);
            return Ok(students);
        }
        [HttpPost("Save")]       
        [Authorize]
        public async Task<IActionResult> Save([FromBody] List<StudentTransferSection> model)
        {
            var userParam = Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await _StudentTransferService.SaveStudentTransfer(model, userParam.UserId);          
           return Ok(result);
        }
        
    }
}
