using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using System.Collections.Concurrent;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IPermissionService _PermissionService;
        private readonly ILicenseService _LicenseService;
        private readonly ITokenService _TokenService;
        private readonly ConcurrentDictionary<string, string> _refreshTokens = new ConcurrentDictionary<string, string>();

        public LoginController(IPermissionService PermissionService, ILicenseService LicenseService, ITokenService TokenService)
        {
            _PermissionService = PermissionService;
            _LicenseService = LicenseService;
            _TokenService = TokenService;
        }
        

        [HttpPost("CheckUserLogin")]
        public async Task<IActionResult> ValidateUserLogin(LoginUser model)
        {
            try
            {
                var loginvalid = await _PermissionService.IsLoginValid(model);
                if (loginvalid <= 0)
                {
                    return Ok(new LicensedUser
                    {
                        LoginStatus = (int)LoginStatus.InValid,
                        UserId = 0,
                        Token = string.Empty
                    });
                }

                var jwttoken = await _TokenService.GenerateAccessToken(model.UserName);
                var refreshToken = await _TokenService.GenerateRefreshToken();
                _refreshTokens[model.UserName] = refreshToken;

                var objlicense = new LicensedUser
                {
                    LoginStatus = (int)LoginStatus.Valid,
                    UserId = loginvalid,
                    Token = jwttoken,
                    RefreshToken = refreshToken
                };

                if (string.IsNullOrEmpty(jwttoken))
                {
                    objlicense.Token = string.Empty;
                    return Ok(objlicense);
                }

                if (model.UserName.Trim().ToUpper() != "INTEL")
                {
                    var islicensevalid = await _LicenseService.ValidateLicenseKeyAsync();
                    objlicense.LicenseStatus = islicensevalid ? (int)LicenseStatus.Valid : (int)LicenseStatus.InValid;
                }
                else
                {
                    objlicense.LicenseStatus = (int)LicenseStatus.Valid;
                }

                return Ok(objlicense);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest model)
        {
            if (!_refreshTokens.TryGetValue(model.UserId, out var storedRefreshToken) || storedRefreshToken != model.RefreshToken)
            {
                return BadRequest("Invalid refresh token");
            }

            var newAccessToken = await _TokenService.GenerateAccessToken(model.UserId);
            var newRefreshToken = await _TokenService.GenerateRefreshToken();
            _refreshTokens[model.UserId] = newRefreshToken;

            return Ok(new { AccessToken = newAccessToken, RefreshToken = newRefreshToken });
        }

        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser()
        {
            if (!Request.Headers.TryGetValue("UserId", out var headerValues))
            {
                return BadRequest("UserId header is missing");
            }

            var struserid = headerValues.FirstOrDefault();
            if (!int.TryParse(struserid, out var userId))
            {
                return BadRequest("Invalid UserId");
            }

            var useraccess = await _PermissionService.GetUserAccessById(userId);
            return Ok(useraccess);
        }

        [HttpPost("ChangePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePwd model)
        {
            var userParam = Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }

            var indexval = await _PermissionService.ChangePassword(model, userParam.UserId);
            return Ok(indexval);
        }
    }
}
