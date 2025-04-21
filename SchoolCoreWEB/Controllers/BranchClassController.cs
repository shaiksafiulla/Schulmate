using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using SchoolCoreWEB.IServices;
using SchoolCoreWEB.Models;

namespace SchoolCoreWEB.Controllers
{
    public class BranchClassController : Controller
    {
        private readonly ICallService _CallService;
        private LanguageService _localization;
        string _userparam;
        private string _strtoken;
        public BranchClassController(ICallService CallService, LanguageService localization)
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
        public async Task<IActionResult> GetAll()
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");

            var httpResponseMessage = await _CallService.GetApi("api/branchclass", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(httpResponseMessage);
            return Json(data);            
        }
        public async Task<IActionResult> GetBranchClass(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/branchclass/view/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<VBranchClass>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> AddEdit(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/branchclass/AddEdit/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<BranchClass>(httpResponseMessage);
            return PartialView("AddEditBranchClass", model);
        }
        public async Task<ActionResult> Save(BranchClass model)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            string jsonInput = JsonConvert.SerializeObject(model);
            var response = await _CallService.PostApi(jsonInput, "api/branchclass/Save", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
        }

        public async Task<ActionResult> AddEditPeriodBreak(int branchclassId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/branchclass/BranchClassPeriodBreak/" + branchclassId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<PeriodBreakVM>(httpResponseMessage);
            return PartialView("AddEditPeriodBreak", model);
        }
        public async Task<ActionResult> UpdatePeriodBreak(PeriodBreakVM model)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            string jsonInput = JsonConvert.SerializeObject(model);
            var response = await _CallService.PostApi(jsonInput, "api/branchclass/BranchClassPeriodBreak/Save", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
        }

        public async Task<IActionResult> AddEditClassTeacher(int BranchId, int BranchClassId, int ClassId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/branchclass/ClassTeacher/" + BranchId + "," + BranchClassId + "," + ClassId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<BranchClassVM>(httpResponseMessage);
            return PartialView("AddEditClassTeacher", model);
        }
        public async Task<ActionResult> UpdateClassTeacher(BranchClassVM model)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            string jsonInput = JsonConvert.SerializeObject(model);
            var response = await _CallService.PostApi(jsonInput, "api/branchclass/ClassTeacher/Save", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
        }

    }
}
