using Localization.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolCoreWEB.IServices;
using SchoolCoreWEB.Models;

namespace SchoolCoreWEB.Controllers
{

    public class LoginController : Controller
    {
        private readonly ICallService _CallService;
        private LanguageService _localization;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LoginController(IHttpContextAccessor httpContextAccessor, ICallService CallService, LanguageService localization)
        {
            _httpContextAccessor = httpContextAccessor;
            _CallService = CallService;
            _localization = localization;
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

                    var userParam = SetUserParam(model,token);
                    var serializeUserParam = JsonConvert.SerializeObject(userParam);
                    HttpContext.Session.SetString("UserParam", serializeUserParam);

                    return Json(model);
                }
                else
                {
                    return RedirectToAction("Index");
                }
               
            }
            return Json(null); 

        }
        public UserParam SetUserParam(VUserInfo userinfo, string token)
        {
            var userParam = new UserParam
            {
                UserId = userinfo.Id,
                PersonId = userinfo.PersonId,
                SessionYearId = userinfo.SessionYearId.HasValue ? (int)userinfo.SessionYearId : 0,
                UserType = userinfo.UserType,
                BranchId = userinfo.BranchId.HasValue ? (int)userinfo.BranchId : 0,
                ClassId = userinfo.ClassId.HasValue ? (int)userinfo.ClassId : 0,
                SectionId = userinfo.SectionId.HasValue ? (int)userinfo.SectionId : 0,
               
            };
            return userParam;
        }
        public ActionResult LogOut()
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
