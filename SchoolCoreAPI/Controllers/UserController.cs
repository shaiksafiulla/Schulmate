using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public UserController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var lst = await _permissionService.GetAllUsers();
            return Ok(lst);
        }

        [HttpGet("View/{id}")]
        [Authorize]
        public async Task<IActionResult> View(int id)
        {
            var user = await _permissionService.ViewUser(id);
            return Ok(user);
        }

        [HttpGet("AddEdit/{id}")]
        [Authorize]
        public async Task<IActionResult> AddEdit(int id)
        {
            var user = await _permissionService.GetUser(id);
            return Ok(user);
        }

        [HttpPost("Save")]
        [Authorize]
        public async Task<IActionResult> Save(userinfo model)
        {
            var userParam = Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await _permissionService.SaveUser(model, userParam.UserId);
            return Ok(result);
        }

        [HttpGet("UserRole/{id}")]
        [Authorize]
        public async Task<IActionResult> EditUserRole(int id)
        {
            var user = await _permissionService.GetUserRole(id);
            return Ok(user);
        }

        [HttpPost("UserRole/Save")]
        [Authorize]
        public async Task<IActionResult> UpdateUserRole(UserRoleVM model)
        {
            var userParam = Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var vm = await _permissionService.UpdateUserRole(model, userParam.UserId);
            return Ok(vm);
        }
    }
}
