using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentEnquiryController : ControllerBase
    {
        private readonly IStudentEnquiryService _studentEnquiryService;

        public StudentEnquiryController(IStudentEnquiryService studentEnquiryService)
        {
            _studentEnquiryService = studentEnquiryService;
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
            var lst =  await _studentEnquiryService.GetAllStudentEnquiry(userParam.UserId);
            return Ok(lst);
        }

        [HttpGet("Class/{branchId}")]
        [Authorize]
        public async Task<IActionResult> GetClassByBranch(int branchId)
        {
            var classes = await  _studentEnquiryService.GetClassByBranch(branchId);
            return Ok(classes);
        }

        [HttpGet("View/{id}")]
        [Authorize]
        public async Task<IActionResult> View(int id)
        {
            var examType =  await _studentEnquiryService.ViewStudentEnquiry(id);
            return Ok(examType);
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
            var enquiry = await  _studentEnquiryService.GetStudentEnquiry(id, userParam.UserId);
            return Ok(enquiry);
        }

        [HttpGet("IsExist/{name},{id}")]
        [Authorize]
        public async Task<IActionResult> IsExist(string name, int id)
        {
            var isExists = await _studentEnquiryService.IsRecordExists(name, id);
            return Ok(isExists);
        }

        [HttpPost("Save")]
        [Authorize]
        public async Task<IActionResult> Save(StudentEnquiry model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await _studentEnquiryService.SaveStudentEnquiry(model, userParam.UserId);
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
            var result = await _studentEnquiryService.DeleteStudentEnquiry(id, userParam.UserId);
            return Ok(result);
        }
    }
}
