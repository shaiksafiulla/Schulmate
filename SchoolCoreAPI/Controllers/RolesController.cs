using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public RolesController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }
        private UserParam GetUserParam()
        {
            return Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var lst = await Task.Run(() => _permissionService.GetAllRoles());
            return Ok(lst);
        }

        [HttpGet("View/{id}")]
        [Authorize]
        public async Task<IActionResult> View(int id)
        {
            var branch = await Task.Run(() => _permissionService.ViewRole(id));
            return Ok(branch);
        }

        [HttpGet("AddEdit/{id}")]
        [Authorize]
        public async Task<IActionResult> AddEdit(int id)
        {
            var branch = await Task.Run(() => _permissionService.GetRole(id));
            return Ok(branch);
        }

        [HttpGet("IsExist/{name},{id}")]
        [Authorize]
        public async Task<IActionResult> IsExist(string name, int id)
        {
            var isexists = await Task.Run(() => _permissionService.IsRecordExists(name, id));
            return Ok(isexists);
        }

        [HttpPost("Save")]
        [Authorize]
        public async Task<IActionResult> Save(Role model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await Task.Run(() => _permissionService.SaveRole(model, userParam.UserId));
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
            var result = await Task.Run(() => _permissionService.DeleteRole(id, userParam.UserId));
            return Ok(result);
        }

        [HttpGet("RolePage/{id}")]
        [Authorize]
        public async Task<IActionResult> AddEditRolePage(int id)
        {
            var vm = await Task.Run(() => _permissionService.GetRolePages(id));
            return Ok(vm);
        }

        [HttpPost("RolePage/Save")]
        [Authorize]
        public async Task<IActionResult> UpdateRolePage(PageVM model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var vm = await Task.Run(() => _permissionService.UpdateRolePage(model, userParam.UserId));
            return Ok(vm);
        }

        [HttpGet("RoleUser/{id}")]
        [Authorize]
        public async Task<IActionResult> AddEditRoleUser(int id)
        {
            var vm = await Task.Run(() => _permissionService.GetRoleUsers(id));
            return Ok(vm);
        }

        [HttpPost("RoleUser/Save")]
        [Authorize]
        public async Task<IActionResult> UpdateRoleUser(RoleUserVM model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var vm = await Task.Run(() => _permissionService.UpdateRoleUsers(model, userParam.UserId));
            return Ok(vm);
        }

        [HttpGet("PageFunction")]
        [Authorize]
        public async Task<IActionResult> GetPageFunction()
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await Task.Run(() => _permissionService.GetPageFunctions(userParam.UserId));
            return Ok(result);
        }
    }
}
