using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamTimeController : ControllerBase
    {
        private readonly IExamTimeService _examTimeService;

        public ExamTimeController(IExamTimeService examTimeService)
        {
            _examTimeService = examTimeService;
        }
        private UserParam GetUserParam()
        {
            return Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var lst = await _examTimeService.GetAllExamTime();
            return Ok(lst);
        }

        [HttpGet("View/{id}")]
        [Authorize]
        public async Task<IActionResult> View(int id)
        {
            var examTime = await _examTimeService.ViewExamTime(id);
            return Ok(examTime);
        }

        [HttpGet("AddEdit/{id}")]
        [Authorize]
        public async Task<IActionResult> AddEdit(int id)
        {
            var examType = await _examTimeService.GetExamTime(id);
            return Ok(examType);
        }

        [HttpGet("IsExist/{fromtime},{totime},{id}")]
        [Authorize]
        public async Task<IActionResult> IsExist(string fromtime, string totime, int id)
        {
            var isExists = await _examTimeService.IsRecordExists(fromtime, totime, id);
            return Ok(isExists);
        }

        [HttpPost("Save")]
        [Authorize]
        public async Task<IActionResult> Save(ExamTime model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await _examTimeService.SaveExamTime(model, userParam.UserId);
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
            var result = await _examTimeService.DeleteExamTime(id, userParam.UserId);
            return Ok(result);
        }
    }
}
