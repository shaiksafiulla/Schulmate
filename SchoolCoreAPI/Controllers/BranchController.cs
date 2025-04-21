using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Features.Branch.Queries;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [EnableRateLimiting("fixed")]
    public class BranchController : ControllerBase
    {
        private readonly IBranchService _branchService;
        private readonly IReportSettingsService _reportSettingsService;
        private readonly IMediator _mediator;

        public BranchController(IBranchService branchService, IReportSettingsService reportSettingsService, IMediator mediator)
        {
            _branchService = branchService;
            _reportSettingsService = reportSettingsService;
            _mediator = mediator;
        }
        private UserParam GetUserParam()
        {
            return Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
        }


        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllBranchesQuery());
            return Ok(result);
        }

        [HttpGet("View/{id}")]
        [Authorize]
        public async Task<IActionResult> View(int id)
        {
            var result = await _mediator.Send(new ViewBranchQuery(id));
            return Ok(result);
        }

        [HttpGet("AddEdit/{id}")]
        [Authorize]
        public IActionResult AddEdit(int id)
        {
            var branch = _branchService.GetBranch(id);
            return Ok(branch);
        }

        [HttpGet("IsExist/{name},{id}")]
        [Authorize]
        public IActionResult IsExist(string name, int id)
        {
            var isExists = _branchService.IsRecordExists(name, id);
            return Ok(isExists);
        }

        [HttpPost("Save")]
        [Authorize]
        public async Task<IActionResult> Save([FromForm] BranchDTO model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            // var result = await _mediator.Send(new SaveBranchCommand(model, userParam.UserId));
            var result = await _branchService.SaveBranch(model, userParam.UserId);
            return Ok(result);
        }

        [HttpDelete("Delete/{id}")]
        [Authorize]
        public IActionResult Delete(int id)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = _branchService.DeleteBranch(id, userParam.UserId);
            return Ok(result);
        }

        [HttpGet("BranchClass/{id}")]
        [Authorize]
        public async Task<IActionResult> AddEditBranchClass(int id)
        {
            var vm = await _branchService.GetBranchClass(id);
            return Ok(vm);
        }

        [HttpPost("BranchClass/Save")]
        [Authorize]
        public async Task<IActionResult> UpdateBranchClass(ClassVM model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var vm = await _branchService.UpdateBranchClass(model, userParam.UserId);
            return Ok(vm);
        }       

        [HttpGet("TimeTable/{branchid}")]
        [Authorize]
        public async Task<IActionResult> GetBranchSectionTimeTable(int branchId)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var rptSettings = _reportSettingsService.GetReportSettingsByBranch(branchId);
            var branchTimeTableTask = _branchService.GetBranchSectionTimeTableRPT(branchId, userParam.UserId);
            branchTimeTableTask.Wait();
            var branchTimeTable = branchTimeTableTask.Result;
            var objBranchTimeTable = new BranchTimeTableRPTVM
            {
                ReportSettings = await rptSettings,
                LstBranchSectionTimeTableRPT = branchTimeTable
            };
            return Ok(objBranchTimeTable);
        }


        [HttpGet("TimeTable/{branchid},{sessionyearId}")]
        [Authorize]
        public async Task<IActionResult> GetTimeTableByBranchAndSessionYear(int branchId, int sessionYearId)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var lst = await _branchService.GetTimeTable(branchId, sessionYearId, userParam.UserId);
            return Ok(lst);
        }
    }
}
