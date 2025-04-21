using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using SchoolCoreWEB.IServices;
using SchoolCoreWEB.Models;


namespace SchoolCoreWEB.Controllers
{    
    public class LeaveRequestController : Controller
    {
        private readonly ICallService _CallService;
        private LanguageService _localization;
        string _userparam;
        private string _strtoken;
        public LeaveRequestController(ICallService CallService, LanguageService localization)
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
            var httpResponseMessage = await _CallService.GetApi("api/leaverequest", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(httpResponseMessage);
            return Json(data);
        }
        public async Task<ActionResult> GetLeaveRequest(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/leaverequest/view/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<VLeaveRequest>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> GetLeaveRequestByPersonnelId(int personnelId, string personnelType)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/leaverequest/Personnel/" + personnelId + "," + personnelType + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<List<VLeaveRequest>>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> View(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/leaverequest/view/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<VLeaveRequest>(httpResponseMessage);
            return PartialView("ViewLeaveRequest", model);
        }
      
        public async Task<ActionResult> AddEdit(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/leaverequest/AddEdit/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<LeaveRequest>(httpResponseMessage);
            return PartialView("AddLeaveRequest", model);
        }
        //public async Task<ActionResult> Edit(int Id)
        //{
        //    var userInfokey = HttpContext.Session.GetString("UserInfo");
        //    if (userInfokey != null)
        //    {
        //        _userInfo = JsonConvert.DeserializeObject<VUserInfo>(userInfokey);
        //    }
        //    _strtoken = HttpContext.Session.GetString("strtoken");
        //    var httpResponseMessage = await _CallService.GetApi("api/leaverequest/view/" + Id + "", _strtoken, _userparam);
        //    var model = await _CallService.FetchObjectFromResponse<VLeaveRequest>(httpResponseMessage);
        //    return PartialView("EditLeaveRequest", model);
        //}
        public async Task<ActionResult> Save(string model)
        {
            try
            {
                _userparam = HttpContext.Session.GetString("UserParam");
                _strtoken = HttpContext.Session.GetString("strtoken");
                var leavemodel = JsonConvert.DeserializeObject<LeaveRequest>(model);
                string jsonInput = JsonConvert.SerializeObject(leavemodel);
                var response = await _CallService.PostApi(jsonInput, "api/leaverequest/save", _strtoken, _userparam);
                string data = await _CallService.FetchStringFromResponse(response);
                return Json(data);
            }
            catch (Exception ex)
            {
                
            }
            return Json(null);
            //var httpResponseMessage = await _CallService.GetApi("api/studentenquiry/IsExist/" + model.Name + "," + model.Id + "", _strtoken, _userparam);
            //var isexist = await _CallService.FetchObjectFromResponse<bool>(httpResponseMessage);
            //if (!isexist)
            //{

            //}
            //else
            //{
            //    return Json(isexist);
            //}
        }
       
        public async Task<ActionResult> Delete(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var response = await _CallService.DeleteApi("api/leaverequest/Delete/" + Id + "", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
        }       
    }
}
