using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionBankController : ControllerBase
    {
        private readonly IQuestionBankService _questionBankService;

        public QuestionBankController(IQuestionBankService questionBankService)
        {
            _questionBankService = questionBankService;
        }
        private UserParam GetUserParam()
        {
            return Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var lst = await Task.Run(() => _questionBankService.GetAllQuestionBank());
            return Ok(lst);
        }

        [HttpGet("View/{id}")]
        [Authorize]
        public async Task<IActionResult> View(int id)
        {
            var branch = await Task.Run(() => _questionBankService.ViewQuestionBank(id));
            return Ok(branch);
        }

        [HttpGet("AddEdit/{id}")]
        [Authorize]
        public async Task<IActionResult> AddEdit(int id)
        {
            var branch = await Task.Run(() => _questionBankService.GetQuestionBank(id));
            return Ok(branch);
        }

        [HttpGet("IsExist/{name},{id}")]
        [Authorize]
        public async Task<IActionResult> IsExist(string name, int id)
        {
            var isExists = await Task.Run(() => _questionBankService.IsRecordExists(name, id));
            return Ok(isExists);
        }

        [HttpPost("Save")]
        [Authorize]
        public async Task<IActionResult> Save(QuestionBank model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await Task.Run(() => _questionBankService.SaveQuestionBank(model, userParam.UserId));
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
            var result = await Task.Run(() => _questionBankService.DeleteQuestionBank(id, userParam.UserId));
            return Ok(result);
        }

        [HttpGet("Subject/{classId}")]
        [Authorize]
        public async Task<IActionResult> GetSubjectsByClass(string classId)
        {
            var lst = await Task.Run(() => _questionBankService.GetSubjectsByClass(classId));
            return Ok(lst);
        }

        [HttpGet("Topic/{classId},{subjectId}")]
        [Authorize]
        public async Task<IActionResult> GetTopicsBySubject(string classId, string subjectId)
        {
            var lst = await Task.Run(() => _questionBankService.GetTopicsBySubject(classId, subjectId));
            return Ok(lst);
        }
    }
}
