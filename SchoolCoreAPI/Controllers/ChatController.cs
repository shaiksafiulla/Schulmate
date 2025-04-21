using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.Hub;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        
        //private readonly IActivityService _ActivityService;
        private IHubContext<MessageHub, IMessageHubClient> messageHub;
        string struserid;
        public ChatController(IHubContext<MessageHub, IMessageHubClient> _messageHub)
        {
            messageHub = _messageHub;
        }
        //[HttpGet]
        //[Authorize]
        //public async Task<IActionResult> GetAll()
        //{
        //    var lst = _ActivityService.GetAllActivity();
        //    return Ok(lst);
        //}       
       
      
        //[HttpPost("Save")]
        //[Authorize]
        //public async Task<IActionResult> Save(Activity model)
        //{
        //    var userParam = Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
        //    if (userParam == null)
        //    {
        //        return BadRequest("Missing or Invalid Header");
        //    }
        //    var result = _ActivityService.SaveActivity(model, userParam.UserId);
        //    return Ok(result);
        //}
        
    }
}
