using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using SchoolCoreWEB.IServices;
using SchoolCoreWEB.Models;

namespace SchoolCoreWEB.Controllers
{    
    public class AttendenceController : Controller
    {
        private readonly ICallService _CallService;
        private LanguageService _localization;
        private string _userparam;
        private string _strtoken;
        public AttendenceController(ICallService CallService, LanguageService localization)
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
            var httpResponseMessage = await _CallService.GetApi("api/Attendence", _strtoken, _userparam);
            //string data = await _CallService.FetchStringFromResponse(httpResponseMessage);
            var model = await _CallService.FetchObjectFromResponse<AttendenceVM>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> AddEdit(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/Attendence/AddEdit/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<Attendence>(httpResponseMessage);
            return PartialView("AddEditAttendence", model);
        }
        public async Task<ActionResult> View(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/Attendence/View/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<Attendence>(httpResponseMessage);
            return PartialView("ViewAttendence", model);
        }
        public async Task<ActionResult> GetSectionAttendence(int Id, int branchId, int sessionyearId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            //var httpResponseMessage = await _CallService.GetApi("api/Attendence/Section/" + Id + "," + branchId + "," + sessionyearId + "", _strtoken, _userparam);
            var httpResponseMessage = await _CallService.GetApi("api/Attendence/Section/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<List<SectionAttendence>>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> GetStudentAttendence(int Id, int SectionId, int SessionYearId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            //var httpResponseMessage = await _CallService.GetApi("api/Attendence/Student/" + Id + "," + SectionId + "," + SessionYearId + "", _strtoken, _userparam);
            var httpResponseMessage = await _CallService.GetApi("api/Attendence/Student/" + Id + "," + SectionId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<List<CastStudentAttendence>>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> GetAttendenceByPersonnelId(int PersonnelId, int PersonnelType, int SessionYearId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/Attendence/Personnel/" + PersonnelId + "," + PersonnelType + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<List<PersonnelMonthlyAttendence>>(httpResponseMessage);
            //var model = await _CallService.FetchObjectFromResponse<List<VPersonnelAttendenceBenchmark>>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> GetPersonnelAttendence(int Id, int branchId, int sessionyearId, int personnelType)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/Attendence/PersonnelAttendence/" + Id + "," + branchId + "," + sessionyearId + "," + personnelType + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<List<CastPersonnelAttendence>>(httpResponseMessage);
            return Json(model);
        }





        public async Task<ActionResult> GetBranchClassAttendenceById(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/Attendence/view/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<List<VBranchClassAttendence>>(httpResponseMessage);
            return Json(model);
        }
        
        
        public async Task<ActionResult> Save(string model)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var objModel = JsonConvert.DeserializeObject<Attendence>(model);
            string jsonInput = JsonConvert.SerializeObject(objModel);
            var response = await _CallService.PostApi(jsonInput, "api/Attendence/save", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
        }     

    }
}
