using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using SchoolCoreWEB.IServices;
using SchoolCoreWEB.Models;

namespace SchoolCoreWEB.Controllers
{
    public class StudentTransferController : Controller
    {
        private readonly ICallService _CallService;
        private LanguageService _localization;
        string _userparam;
        private string _strtoken;
        public StudentTransferController(ICallService CallService, LanguageService localization)
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
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/studenttransfer/", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<StudentTransferVM>(httpResponseMessage);
            return View(model);
        }
        public async Task<IActionResult> GetClassByBranch(int BranchId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/studenttransfer/Class/" + BranchId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<List<SelectListItem>>(httpResponseMessage);
            return Json(model);
        }
        public async Task<IActionResult> GetSectionByClass(int BranchId, int ClassId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/studenttransfer/Section/" + BranchId + "," + ClassId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<List<SelectListItem>>(httpResponseMessage);
            return Json(model);
        }
        public async Task<IActionResult> GetStudents(int SectionId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var response = await _CallService.GetApi("api/studenttransfer/Student/" + SectionId + "", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
        }
        public async Task<IActionResult> Save(string model)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var lstTransfer = JsonConvert.DeserializeObject<List<StudentTransferSection>>(model);
            string jsonInput = JsonConvert.SerializeObject(lstTransfer);
            var response = await _CallService.PostApi(jsonInput, "api/studenttransfer/save", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
        }
       


    }
}
