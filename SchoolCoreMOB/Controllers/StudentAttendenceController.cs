using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolCoreMOB.IServices;
using SchoolCoreMOB.Models;

namespace SchoolCoreMOB.Controllers
{    
    public class StudentAttendenceController : Controller
    {
        private readonly ICallService _CallService;
        private LanguageService _localization;
        string _userparam;  
        private string _strtoken;
        public StudentAttendenceController(ICallService CallService, LanguageService localization)
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
        //    var httpResponseMessage = await _CallService.GetApi("api/Attendence", _strtoken, _userparam);            
        //    var model = await _CallService.FetchObjectFromResponse<AttendenceVM>(httpResponseMessage);
        //    return Json(model);
        //}        
        [Route("StudentAttendence/AddStudentAttendence")]
        public async Task<ActionResult> GetMbSectionAttendence(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/Attendence/MbSection/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<SectionAttendenceVM>(httpResponseMessage);           
            return View("AddStudentAttendence", model);
        }
        //public async Task<ActionResult> GetStudentAttendence(int Id, int SectionId)
        //{
        //    _userparam = HttpContext.Session.GetString("UserParam");
        //    _strtoken = HttpContext.Session.GetString("strtoken");
        //    var httpResponseMessage = await _CallService.GetApi("api/Attendence/Student/" + Id + "," + SectionId + "", _strtoken, _userparam);
        //    var model = await _CallService.FetchObjectFromResponse<List<CastStudentAttendence>>(httpResponseMessage);
        //    return Json(model);
        //}
        //public async Task<ActionResult> Save(string model)
        //{
        //    _userparam = HttpContext.Session.GetString("UserParam");
        //    _strtoken = HttpContext.Session.GetString("strtoken");

        //    var objModel = JsonConvert.DeserializeObject<Attendence>(model);
        //    //objModel.BranchId = (int)_userInfo.BranchId;
        //    //objModel.SessionYearId = (int)_userInfo.SessionYearId;
            
        //    string jsonInput = JsonConvert.SerializeObject(objModel);
        //    var response = await _CallService.PostApi(jsonInput, "api/Attendence/save", _strtoken, _userparam);
        //    string data = await _CallService.FetchStringFromResponse(response);
        //    return Json(data);
        //}
    }
}
