using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MbPersonnelController : ControllerBase
    {
        private readonly IMbPersonnelService _MbPersonnelService;
        public MbPersonnelController(IMbPersonnelService MbPersonnelService)
        {
            _MbPersonnelService = MbPersonnelService;
        }
        private UserParam GetUserParam()
        {
            return Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
        }

        [HttpGet("PersonnelDetailView")]
        [Authorize]
        public async Task<IActionResult> GetPersonnelDetail()
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await _MbPersonnelService.GetPersonnelDetail(userParam.PersonId, userParam.SessionYearId, userParam.UserType);
            return Ok(result);
        }
        [HttpGet("PersonnelDetailView/{personid},{persontype}")]
        [Authorize]
        public async Task<IActionResult> GetPersonnelDetailById(int PersonId, string PersonType)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await _MbPersonnelService.GetPersonnelDetail(PersonId, userParam.SessionYearId, PersonType);
            return Ok(result);
        }
    }
}
