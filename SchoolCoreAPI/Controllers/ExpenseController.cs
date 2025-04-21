using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public ExpenseController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        private UserParam GetUserParam()
        {
            return Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
        }
        [HttpGet("LoadExpenseParams")]
        [Authorize]
        public async Task<IActionResult> LoadExpenseParams()
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var lst = await _accountService.LoadExpenseParams();
            return Ok(lst);
        }

        [HttpGet("GetExpenseSummary")]
        [Authorize]
        public async Task<IActionResult> GetExpenseSummary()
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var lst = await _accountService.GetExpenseSummary(userParam.BranchId, userParam.SessionYearId);
            return Ok(lst);
        }

        [HttpPost("SearchExpenseSummary")]
        [Authorize]
        public async Task<IActionResult> SearchExpenseSummary(SearchExpenseParams vM)
        {
            try
            {
                var userParam = GetUserParam();
                if (userParam == null)
                {
                    return BadRequest("Missing or Invalid Header");
                }

                var lst = await _accountService.SearchExpenseSummary(vM, userParam.BranchId);
                return Ok(lst);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("GetAllExpenseByDate/{id}")]
        [Authorize]
        public async Task<IActionResult> GetAllExpenseByDate(int id)
        {
            try
            {
                var userParam = GetUserParam();
                if (userParam == null)
                {
                    return BadRequest("Missing or Invalid Header");
                }

                var lst = await _accountService.GetAllExpenseByDate(id);
                return Ok(lst);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }






        [HttpGet("LoadReceivableParams")]
        [Authorize]
        public async Task<IActionResult> LoadReceivableParams()
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var lst = await _accountService.LoadReceivableParams();
            return Ok(lst);
        }

        [HttpGet("GetReceivableSummary")]
        [Authorize]
        public async Task<IActionResult> GetReceivableSummary()
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var lst = await _accountService.GetReceivableSummary(userParam.BranchId, userParam.SessionYearId);
            return Ok(lst);
        }

        [HttpPost("SearchReceivableSummary")]
        [Authorize]
        public async Task<IActionResult> SearchReceivableSummary(SearchReceivableParams vM)
        {
            try
            {
                var userParam = GetUserParam();
                if (userParam == null)
                {
                    return BadRequest("Missing or Invalid Header");
                }

                var lst = await _accountService.SearchReceivableSummary(vM, userParam.BranchId);
                return Ok(lst);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("GetAllReceivableByDate/{id}")]
        [Authorize]
        public async Task<IActionResult> GetAllReceivableByDate(int id)
        {
            try
            {
                var userParam = GetUserParam();
                if (userParam == null)
                {
                    return BadRequest("Missing or Invalid Header");
                }

                var lst = await _accountService.GetAllReceivableByDate(id);
                return Ok(lst);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("View/{id}")]
        [Authorize]
        public async Task<IActionResult> View(int id)
        {
            var examtype = await _accountService.ViewExpense(id);
            return Ok(examtype);
        }

        [HttpGet("AddEdit/{id}")]
        [Authorize]
        public async Task<IActionResult> AddEdit(int id)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var expense = await _accountService.GetExpense(id, userParam.UserId);
            return Ok(expense);
        }

        [HttpPost("Save")]
        [Authorize]
        public async Task<IActionResult> Save([FromForm] Expense model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var result = await _accountService.SaveExpense(model, userParam.UserId);
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
            var result = await _accountService.DeleteExpense(id, userParam.UserId);
            return Ok(result);
        }
    }
}
