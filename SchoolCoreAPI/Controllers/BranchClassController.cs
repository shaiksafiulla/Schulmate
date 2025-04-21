using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchClassController : ControllerBase
    {
        private readonly IBranchClassService _BranchClassService;
        public BranchClassController(IBranchClassService BranchClassService)
        {
            _BranchClassService = BranchClassService;
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
            var lst = await Task.Run(() => _BranchClassService.GetAllBranchClass(userParam.UserId));
            return Ok(lst);
        }

        [HttpGet("AddEdit/{id}")]
        [Authorize]
        public async Task<IActionResult> AddEdit(int Id)
        {
            var cls = await Task.Run(() => _BranchClassService.GetBranchClass(Id));
            return Ok(cls);
        }

        [HttpGet("View/{id}")]
        [Authorize]
        public async Task<IActionResult> View(int Id)
        {
            var branch = await Task.Run(() => _BranchClassService.ViewBranchClass(Id));
            return Ok(branch);
        }

        [HttpPost("Save")]
        [Authorize]
        public async Task<IActionResult> Save(BranchClass model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await Task.Run(() => _BranchClassService.SaveBranchClass(model, userParam.UserId));
            return Ok(result);
        }

        [HttpGet("ClassTeacher/{branchid},{branchclassid},{classid}")]
        [Authorize]
        public async Task<IActionResult> AddEditClassTeacher(int BranchId, int BranchClassId, int ClassId)
        {
            var vm = await Task.Run(() => _BranchClassService.GetBranchClassVM(BranchId, BranchClassId, ClassId));
            return Ok(vm);
        }

        [HttpGet("BranchClassPeriodBreak/{id}")]
        [Authorize]
        public async Task<IActionResult> AddEditBranchClassPeriodBreak(int Id)
        {
            var vm = await Task.Run(() => _BranchClassService.GetPeriodBreak(Id));
            return Ok(vm);
        }

        [HttpPost("BranchClassPeriodBreak/Save")]
        [Authorize]
        public async Task<IActionResult> UpdateBranchClassPeriodBreak(PeriodBreakVM model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var vm = await Task.Run(() => _BranchClassService.UpdatePeriodBreak(model, userParam.UserId));
            return Ok(vm);
        }
    }
}
