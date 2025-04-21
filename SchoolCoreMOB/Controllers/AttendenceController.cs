using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolCoreMOB.IServices;
using SchoolCoreMOB.Models;

namespace SchoolCoreMOB.Controllers
{    
    public class AttendenceController : Controller
    {
        private readonly ICallService _CallService;
        private LanguageService _localization;
        string _userparam;  
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
            //_userparam = HttpContext.Session.GetString("UserParam");
            //_strtoken = HttpContext.Session.GetString("strtoken");
            //var httpResponseMessage = await _CallService.GetApi("api/Attendence/Personnel", _strtoken, _userparam);
            //var model = await _CallService.FetchObjectFromResponse<List<VPersonnelAttendenceBenchmark>>(httpResponseMessage);
            return View();
        }   
        //public async Task<ActionResult> GetAttendenceByPersonnelId()
        //{
        //    _userparam = HttpContext.Session.GetString("UserParam");
        //    _strtoken = HttpContext.Session.GetString("strtoken");
        //    var httpResponseMessage = await _CallService.GetApi("api/Attendence/Personnel", _strtoken, _userparam);            
        //    var model = await _CallService.FetchObjectFromResponse<List<VPersonnelAttendenceBenchmark>>(httpResponseMessage);
        //    return Json(model);
        //}

        [Route("Attendence/StudentAttendence")]
        public async Task<ActionResult> GetSectionAttendence(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/Attendence/Section/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<List<SectionAttendence>>(httpResponseMessage);
            return View("StudentAttendence", model);
        }
        public async Task<ActionResult> GetStudentAttendence(int Id, int SectionId, int SessionYearId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/Attendence/Student/" + Id + "," + SectionId + "," + SessionYearId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<List<CastStudentAttendence>>(httpResponseMessage);
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
