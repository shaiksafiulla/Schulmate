using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using SchoolCoreWEB.IServices;
using SchoolCoreWEB.Models;

namespace SchoolCoreWEB.Controllers
{
   
    public class ReportCardController : Controller
    {
        string _userparam;
        private string _strtoken;
        private readonly ICallService _CallService;
        private LanguageService _localization;
        public ReportCardController(ICallService CallService, LanguageService localization)
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
            var httpResponseMessage = await _CallService.GetApi("api/ReportCard", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(httpResponseMessage);
            return Json(data);
        }
        public async Task<ActionResult> GetReportCard(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/ReportCard/view/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<VReportCard>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> View(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/ReportCard/view/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<VReportCard>(httpResponseMessage);
            return PartialView("ViewReportCard", model);
        }
        public async Task<ActionResult> AddEdit(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/ReportCard/AddEdit/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<ReportCard>(httpResponseMessage);
            //model.BranchId = _userInfo.BranchId;
            return PartialView("AddEditReportCard", model);
        }

        public async Task<ActionResult> GetReportCardDetail(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/ReportCard/ReportCardDetailView/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<ReportCardDetail>(httpResponseMessage);            
            return PartialView("ViewReportCard", model);
        }
        //public async Task<ActionResult> Save(string model)
        //{
        //    var userInfokey = HttpContext.Session.GetString("UserInfo");
        //    if (userInfokey != null)
        //    {
        //        _userInfo = JsonConvert.DeserializeObject<VUserInfo>(userInfokey);
        //    }
        //    _strtoken = HttpContext.Session.GetString("strtoken");
        //    var objModel = JsonConvert.DeserializeObject<ReportCard>(model);
        //    var httpResponseMessage = await _CallService.GetApi("api/ReportCard/IsExist/" + objModel.Name + "," + objModel.Id + "", _strtoken, _userparam);
        //    var isexist = await _CallService.FetchObjectFromResponse<bool>(httpResponseMessage);
        //    if (!isexist)
        //    {
        //        string jsonInput = JsonConvert.SerializeObject(objModel);
        //        var response = await _CallService.PostApi(jsonInput, "api/ReportCard/save", _strtoken, _userparam);
        //        string data = await _CallService.FetchStringFromResponse(response);
        //        return Json(data);
        //    }
        //    else
        //    {
        //        return Json(isexist);
        //    }

        //}
        public async Task<ActionResult> Save(ReportCard model)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            //var objModel = JsonConvert.DeserializeObject<ReportCard>(model);
            var httpResponseMessage = await _CallService.GetApi("api/ReportCard/IsExist/" + model.Name + "," + model.Id + "", _strtoken, _userparam);
            var isexist = await _CallService.FetchObjectFromResponse<bool>(httpResponseMessage);
            if (!isexist)
            {
                string jsonInput = JsonConvert.SerializeObject(model);
                var response = await _CallService.PostApi(jsonInput, "api/ReportCard/save", _strtoken, _userparam);
                string data = await _CallService.FetchStringFromResponse(response);
                return Json(data);
            }
            else
            {
                return Json(isexist);
            }

        }
        public async Task<ActionResult> Delete(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var response = await _CallService.DeleteApi("api/ReportCard/Delete/" + Id + "", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
        }
        public async Task<ActionResult> GetBranchClassByReportCard(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/ReportCard/BranchClass/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<List<VBranchClass>>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> GetSchedulesByBranch(string branchId, string reportcardId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/ReportCard/Schedules/" + branchId + "," + reportcardId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<List<VSchedule>>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> GetSPReportCard(string reportCardId, string branchClassId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/ReportCard/SPReport/" + reportCardId + "," + branchClassId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<SpReportCardResult>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> GetRCCP_Student(string reportCardId, string StudentId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/ReportCard/RCCPStudent/" + reportCardId + "," + StudentId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<RCCP_Student>(httpResponseMessage);
            return Json(model);
        }       

    }
}
