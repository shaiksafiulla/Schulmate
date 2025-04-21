using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionCategoryController : ControllerBase
    {
        private readonly IQuestionCategoryService _questionCategoryService;

        public QuestionCategoryController(IQuestionCategoryService questionCategoryService)
        {
            _questionCategoryService = questionCategoryService;
        }
        private UserParam GetUserParam()
        {
            return Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var lst = await Task.Run(() => _questionCategoryService.GetAllQuestionCategory());
            return Ok(lst);
        }

        [HttpGet("View/{id}")]
        [Authorize]
        public async Task<IActionResult> View(int id)
        {
            var branch = await Task.Run(() => _questionCategoryService.ViewQuestionCategory(id));
            return Ok(branch);
        }

        [HttpGet("AddEdit/{id}")]
        [Authorize]
        public async Task<IActionResult> AddEdit(int id)
        {
            var branch = await Task.Run(() => _questionCategoryService.GetQuestionCategory(id));
            return Ok(branch);
        }

        [HttpGet("IsExist/{name},{id}")]
        [Authorize]
        public async Task<IActionResult> IsExist(string name, int id)
        {
            var isExists = await Task.Run(() => _questionCategoryService.IsRecordExists(name, id));
            return Ok(isExists);
        }

        [HttpPost("Save")]
        [Authorize]
        public async Task<IActionResult> Save(QuestionCategory model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await Task.Run(() => _questionCategoryService.SaveQuestionCategory(model, userParam.UserId));
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
            var result = await Task.Run(() => _questionCategoryService.DeleteQuestionCategory(id, userParam.UserId));
            return Ok(result);
        }
    }
}
