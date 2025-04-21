using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using System.Collections.Concurrent;

namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private static readonly ConcurrentDictionary<Guid, StreamWriter> Clients = new ConcurrentDictionary<Guid, StreamWriter>();

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
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

            var lst = await _notificationService.GetAllNotification(userParam.UserId);
            return Ok(lst);
        }

        [HttpGet("View/{id}")]
        [Authorize]
        public async Task<IActionResult> View(int id)
        {
            var examtype = await _notificationService.ViewNotification(id);
            return Ok(examtype);
        }

        [HttpGet("Update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var notistatus = await _notificationService.UpdateNotificationStatus(id, userParam.UserId);
            return Ok(notistatus);
        }

        [HttpGet("ActionTaken/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateActionTakenStatus(int id)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var actionstatus = await _notificationService.UpdateActionTakenStatus(id, userParam.UserId);
            return Ok(actionstatus);
        }

        private void SendToClientAsync(Guid clientId, string message)
        {
            if (Clients.TryGetValue(clientId, out var clientStream))
            {
                try
                {
                    clientStream.WriteLine($"data: {message}\n\n");
                    clientStream.Flush();
                }
                catch (Exception ex)
                {
                    // Handle exceptions or log them
                }
            }
        }
    }
}
