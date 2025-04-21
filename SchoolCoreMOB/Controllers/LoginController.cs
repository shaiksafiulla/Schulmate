﻿using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolCoreMOB.IServices;
using SchoolCoreMOB.Models;

namespace SchoolCoreMOB.Controllers
{

    public class LoginController : Controller
    {
        private readonly ICallService _CallService;
        private LanguageService _localization;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        public LoginController(IHttpContextAccessor httpContextAccessor, ICallService CallService, LanguageService localization, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _CallService = CallService;
            _localization = localization;
            _configuration = configuration;
        }
        //public LoginController(ICallService CallService, LanguageService localization) : base(localization)
        //{
        //    _CallService = CallService;
        //    _localization = localization;
        //}
        public async Task<ActionResult> Index()
        {
           
            return View();
        }        
        public async Task<IActionResult> CheckUserLogin(LoginUser model)
        {
            try
            {               
                string jsonInput = JsonConvert.SerializeObject(model);
                var response = await _CallService.LoginApi(jsonInput, "api/Login/CheckUserLogin");
                string data = await _CallService.FetchStringFromResponse(response);
                return Json(data);
            }
            catch (Exception ex)
            {
                return Json(ex);
            }           

        }       
        //public async Task<IActionResult> RegisterFingerPrint(string userName)
        //{
        //    try
        //    {
        //        string jsonInput = JsonConvert.SerializeObject(userName);
        //        var response = await _CallService.WebAuthnApi(jsonInput, "api/WebAuthn/credential-options");
        //        string data = await _CallService.FetchStringFromResponse(response);
        //        return Json(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(ex);
        //    }

        //}
        //public async Task<IActionResult> CompleteRegisterFingerPrint(string credentialRequest)
        //{
        //    try
        //    {
        //        var objModel = JsonConvert.DeserializeObject<Root>(credentialRequest);
        //        //Encoding.UTF8.GetBytes(userKey)
        //        string jsonInput = JsonConvert.SerializeObject(credentialRequest);
        //        var response = await _CallService.WebAuthnApi(jsonInput, "api/WebAuthn/credential");
        //        string data = await _CallService.FetchStringFromResponse(response);
        //        return Json(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(ex);
        //    }

        //}
        public async Task<IActionResult> SetUser(int userId, string token)
        {

            if (token != null)
            {
                HttpContext.Session.SetString("strtoken", token);

                var httpResponseMessage = await _CallService.GetApiUser("api/Login/GetUser", token, userId.ToString());
                var model = await _CallService.FetchObjectFromResponse<VUserInfo>(httpResponseMessage);
                if (model != null)
                {
                    var fullPath = _CallService.FetchApiUrl() + model.FilePath;
                    model.FilePath = !string.IsNullOrEmpty(model.FilePath) ? fullPath : string.Empty;
                    var serializedUser = JsonConvert.SerializeObject(model);
                    HttpContext.Session.SetString("UserInfo", serializedUser);
                    HttpContext.Session.SetString("noticount", model.NotiCount.ToString());

                    var userParam = SetUserParam(model);
                    var serializeUserParam = JsonConvert.SerializeObject(userParam);
                    HttpContext.Session.SetString("UserParam", serializeUserParam);

                    //ViewData["apiUrl"] = _configuration["ApiUrl"];
                    //ViewData["strtoken"] = token;
                    //ViewData["userParam"] = serializeUserParam;

                    ViewData["apiUrl"] = _configuration["ApiUrl"];
                    ViewData["strtoken"] = HttpContext.Session.GetString("strtoken");
                    ViewData["userParam"] = HttpContext.Session.GetString("UserParam");

                    return Json(model);
                }
                else
                {
                    return RedirectToAction("Index");
                }

            }
            return Json(null);

        }
        public UserParam SetUserParam(VUserInfo userinfo)
        {
            var userParam = new UserParam
            {
                UserId = userinfo.Id,
                PersonId = userinfo.PersonId,
                SessionYearId = userinfo.SessionYearId.HasValue ? (int)userinfo.SessionYearId : 0,
                UserType = userinfo.UserType,
                BranchId = userinfo.BranchId.HasValue ? (int)userinfo.BranchId : 0,
                BranchClassId = userinfo.BranchClassId.HasValue ? (int)userinfo.BranchClassId : 0,
                ClassId = userinfo.ClassId.HasValue ? (int)userinfo.ClassId : 0,
                SectionId = userinfo.SectionId.HasValue ? (int)userinfo.SectionId : 0,

            };
            return userParam;
        }
        public IActionResult LogOut()
        {
            ISession session = _httpContextAccessor.HttpContext.Session;
            session.Remove("strtoken");
            session.Remove("UserInfo");
            session.Remove("noticount");
            session.Remove("UserParam");
            // Clear the session data
            session.Clear();
            return RedirectToAction("Index");
        }

        //public ActionResult EditPassword()
        //{
        //    ChangePwd model = new ChangePwd();            
        //    return PartialView("ChangePassword", model);
        //}

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

    }
}
