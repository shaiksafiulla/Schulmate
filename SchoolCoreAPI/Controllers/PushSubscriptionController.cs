using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;


namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PushSubscriptionController : ControllerBase
    {
        private readonly IPushSubscriptionService _subscriptionService;

        public PushSubscriptionController(IPushSubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        private UserParam GetUserParam()
        {
            return Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
        }

        [HttpPost("Subscribe")]
        [Authorize]
        public async Task<IActionResult> Subscribe([FromBody] pushsubscriptions model)
        {
            var result = await _subscriptionService.Subscribe(model).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpPost("UnSubscribe")]
        [Authorize]
        public async Task<IActionResult> UnSubscribe()
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await _subscriptionService.UnSubscribe(userParam.UserId).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpPost("SendNotification")]
        [Authorize]
        public async Task<IActionResult> SendNotification([FromBody] PushPayLoad payload)
        {
            var result = await _subscriptionService.SendPushNotificationAsync(payload, null).ConfigureAwait(false);
            return Ok(result);
        }
    }
}
