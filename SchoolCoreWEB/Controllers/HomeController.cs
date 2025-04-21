using Localization.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreWEB.Models;
using SchoolCoreWEB.IServices;
using System.Diagnostics;
using Newtonsoft.Json;
using static SchoolCoreWEB.Helpers.Shared;
using Activity = System.Diagnostics.Activity;

namespace SchoolCoreWEB.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICallService _CallService;
        private LanguageService _localization;
        string _userparam;
        private string _strtoken;
        public HomeController(ICallService CallService, LanguageService localization)
        {
            _CallService = CallService;
            _localization = localization;
        }
        public IActionResult ChangeLanguage(string culture)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions() { Expires = DateTimeOffset.UtcNow.AddYears(1) });

            return Redirect(Request.Headers["Referer"].ToString());
        }
        public IActionResult Index()
        {
            //IActionResult redirectTo = CheckSessionKey("UserInfo", RedirectToAction("Logout", "Login"));
            //if (redirectTo != null)
            //{
            //    return redirectTo; // Redirect to login page if session key does not exist
            //}           
            if (!HttpContext.Session.Keys.Contains("UserInfo"))
            {
                return RedirectToAction("Logout", "Login");
            }
            return View();            
        }
        public async Task<ActionResult> GetProfile()
        {
            var strUrl = ""; VUserInfo _userInfo = null;
           var userInfokey = HttpContext.Session.GetString("UserInfo");
            if (userInfokey != null)
            {
                _userInfo = JsonConvert.DeserializeObject<VUserInfo>(userInfokey);
            }
            if (_userInfo.UserType == ((int)UserType.SuperAdmin).ToString() || _userInfo.UserType == ((int)UserType.Admin).ToString())
            {
                strUrl = "api/admin/view/";
            }
            if (_userInfo.UserType == ((int)UserType.Teacher).ToString())
            {
                strUrl = "api/teacher/view/";
            }
            if (_userInfo.UserType == ((int)UserType.Staff).ToString())
            {
                strUrl = "api/staff/view/";
            }
            if (_userInfo.UserType == ((int)UserType.Driver).ToString())
            {
                strUrl = "api/driver/view/";
            }
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi(strUrl + _userInfo.PersonId + "", _strtoken, _userparam);
            var profileModel = await _CallService.FetchObjectFromResponse<Profile>(httpResponseMessage);
           // var profileModel = (Profile)model;
            profileModel.FilePath = _CallService.FetchApiUrl() + profileModel.FilePath;
            return PartialView("ViewProfile", profileModel);


        }
        public ActionResult EditPassword()
        {
            ChangePwd model = new ChangePwd();
            return PartialView("ChangePassword", model);
        }
        public async Task<IActionResult> UpdatePassword(ChangePwd model)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            string jsonInput = JsonConvert.SerializeObject(model);
            var response = await _CallService.PostApi(jsonInput, "api/login/ChangePassword", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);

        }
        //[HttpPost]
        //public async Task<JsonResult> ChangeUserPassword(ChangePwd model)
        //{
        //    int? index = null;
        //    var objSession = HttpContext.Session.GetObjectFromJson<VUserInfo>("UserInfo");
        //    var user = _userRepository.GetUserById(objSession.UserId);
        //    var crypto = Common.ToSHA2569(model.Password);
        //    if (user.Password == crypto)
        //    {
        //        index = _userRepository.ChangePassword(user.Id, model.NewPassword);
        //    }
        //    return Json(index);
        //}

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}