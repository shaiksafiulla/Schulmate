using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;


namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicController : ControllerBase
    {
        private readonly ITopicService _topicService;

        public TopicController(ITopicService topicService)
        {
            _topicService = topicService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var topics = await _topicService.GetAllTopic();
            return Ok(topics);
        }

        [HttpGet("View/{id}")]
        [Authorize]
        public async Task<IActionResult> View(int id)
        {
            var topic = await _topicService.ViewTopic(id);
            return Ok(topic);
        }

        [HttpGet("AddEdit/{id}")]
        [Authorize]
        public async Task<IActionResult> AddEdit(int id)
        {
            var topic = await _topicService.GetTopic(id);
            return Ok(topic);
        }

        [HttpGet("IsExist/{lessonId},{name},{id}")]
        [Authorize]
        public async Task<IActionResult> IsExist(int lessonId, string name, int id)
        {
            var exists = await _topicService.IsRecordExists(lessonId, name, id);
            return Ok(exists);
        }

        [HttpPost("Save")]
        [Authorize]
        public async Task<IActionResult> Save(Topic model)
        {
            var userParam = Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var result = await _topicService.SaveTopic(model, userParam.UserId);
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

            var result = await _topicService.DeleteTopic(id, userParam.UserId);
            return Ok(result);
        }

        [HttpGet("Subjects/{classId}")]
        [Authorize]
        public async Task<IActionResult> GetSubjectsByClass(int classId)
        {
            var subjects = await _topicService.GetSubjectsByClass(classId);
            return Ok(subjects);
        }

        [HttpGet("Lessons/{classId},{subjectId}")]
        [Authorize]
        public async Task<IActionResult> GetLessonsByClassSubject(int classId, int subjectId)
        {
            var lessons = await _topicService.GetLessonsByClassSubject(classId, subjectId);
            return Ok(lessons);
        }
    }
}
