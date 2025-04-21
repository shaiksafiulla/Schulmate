using Localization.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using SchoolCoreWEB.IServices;
using SchoolCoreWEB.Models;
using System.Reflection;
using static SchoolCoreWEB.Helpers.Shared;


namespace SchoolCoreWEB.Controllers
{    
    public class NotificationsController : Controller
    {
        private readonly ICallService _CallService;
        private LanguageService _localization;
        string _userparam;
        private string _strtoken;
        
        public NotificationsController(ICallService CallService, LanguageService localization)
        {
            _CallService = CallService;
            _localization = localization;
        }

        public async Task<ActionResult> Index()
        {
            if (!HttpContext.Session.Keys.Contains("UserInfo"))
            {
                return RedirectToAction("Logout", "Login");
            }
            return View();
        }
        public async Task<ActionResult> GetAll()
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/notification", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(httpResponseMessage);
            return Json(data);
        }
        public async Task<ActionResult> GetNotification(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/notification/view/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<VNotification>(httpResponseMessage);            
            return Json(model);
        }
        public async Task<ActionResult> UpdateNotificationStatus(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/notification/Update/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<int>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> Edit(int Id, int RecordId, string ReadStatusId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");

            if (ReadStatusId == ((int)ReadStatus.UnRead).ToString())
            {
                var httpResponseMessage = await _CallService.GetApi("api/notification/Update/" + Id + "", _strtoken, _userparam);
                var readStatus = await _CallService.FetchObjectFromResponse<int>(httpResponseMessage);
                if (readStatus > 0)
                {
                    var notifycount = int.Parse(HttpContext.Session.GetString("noticount"));
                    HttpContext.Session.SetString("noticount", (notifycount-1).ToString());
                    var responsedata = await _CallService.GetApi("api/leaverequest/view/" + RecordId + "", _strtoken, _userparam);
                    var modeldata = await _CallService.FetchObjectFromResponse<VLeaveRequest>(responsedata);
                    modeldata.NotificationId = Id;
                    return PartialView("~/views/LeaveRequest/EditLeaveRequest.cshtml", modeldata);
                }                
            }
            else
            {
                var response = await _CallService.GetApi("api/leaverequest/view/" + RecordId + "", _strtoken, _userparam);
                var reqdata = await _CallService.FetchObjectFromResponse<VLeaveRequest>(response);
                return PartialView("~/views/LeaveRequest/EditLeaveRequest.cshtml", reqdata);
            }
            return View();
        }
        public async Task<ActionResult> UpdateLeaveRequest(string model)
        {
            try
            {
                _userparam = HttpContext.Session.GetString("UserParam");
                _strtoken = HttpContext.Session.GetString("strtoken");
                var leavemodel = JsonConvert.DeserializeObject<CastLeaveRequest>(model);
                string jsonInput = JsonConvert.SerializeObject(leavemodel);
                var response = await _CallService.PostApi(jsonInput, "api/leaverequest/update", _strtoken, _userparam);
                string data = await _CallService.FetchStringFromResponse(response);
                return Json(data);
            }
            catch (Exception ex)
            {
            }
            return Json(null);
        }
    }
}
