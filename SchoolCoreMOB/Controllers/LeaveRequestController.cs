using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolCoreMOB.IServices;
using SchoolCoreMOB.Models;

namespace SchoolCoreMOB.Controllers
{
   
    public class LeaveRequestController : Controller
    {
        string _userparam;
        string _strtoken;
        private readonly ICallService _CallService;
        private LanguageService _localization;
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
        //public async Task<ActionResult> GetAll()
        //{
        //    _userparam = HttpContext.Session.GetString("UserParam");
        //    _strtoken = HttpContext.Session.GetString("strtoken");
        //    var httpResponseMessage = await _CallService.GetApi("api/leaverequest", _strtoken, _userparam);
        //    var model = await _CallService.FetchObjectFromResponse<List<VLeaveRequest>>(httpResponseMessage);
        //    return Json(model);
        //}
        [Route("LeaveRequest/AddLeaveRequest")]
        public async Task<ActionResult> AddEdit(int Id)
        {
            if (!HttpContext.Session.Keys.Contains("UserInfo"))
            {
                return RedirectToAction("Logout", "Login");
            }
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/leaverequest/AddEdit/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<LeaveRequest>(httpResponseMessage);            
            return View("AddLeaveRequest", model);
        }
        //public async Task<ActionResult> Save(string model)
        //{
        //    try
        //    {
        //        _userparam = HttpContext.Session.GetString("UserParam");
        //        _strtoken = HttpContext.Session.GetString("strtoken");
        //        var leavemodel = JsonConvert.DeserializeObject<LeaveRequest>(model);
        //        string jsonInput = JsonConvert.SerializeObject(leavemodel);
        //        var response = await _CallService.PostApi(jsonInput, "api/leaverequest/save", _strtoken, _userparam);
        //        string data = await _CallService.FetchStringFromResponse(response);
        //        return Json(data);
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    return Json(null);            
        //}
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
