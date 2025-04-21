using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using SchoolCoreWEB.IServices;
using SchoolCoreWEB.Models;

namespace SchoolCoreWEB.Controllers
{
    public class BranchController : Controller
    {
        private readonly ICallService _CallService;
        private LanguageService _localization;
        private string _userparam;
        private string _strtoken;
        public BranchController(ICallService CallService, LanguageService localization)
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
        //public async Task<IActionResult> GetAll()
        //{
        //    //var userInfokey = HttpContext.Session.GetString("UserInfo");
        //    //if(userInfokey != null)
        //    //{
        //    //    _userInfo = JsonConvert.DeserializeObject<VUserInfo>(userInfokey);
        //    //}
        //    _userparam = HttpContext.Session.GetString("UserParam");
        //    _strtoken = HttpContext.Session.GetString("strtoken");
        //    var httpResponseMessage = await _CallService.GetApi("api/branch", _strtoken, _userparam);
        //    string data = await _CallService.FetchStringFromResponse(httpResponseMessage);
        //    return Json(data);            
        //}       
        public async Task<IActionResult> GetBranch(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/branch/view/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<VBranch>(httpResponseMessage);           
            return Json(model);
        }
        public async Task<IActionResult> View(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/branch/view/" + Id + "", _strtoken, _userparam);           
            var model = await _CallService.FetchObjectFromResponse<VBranch>(httpResponseMessage);
            return PartialView("ViewBranch", model);           
        }
        public async Task<IActionResult> AddEdit(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/branch/AddEdit/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<Branch>(httpResponseMessage);
            return PartialView("AddEditBranch", model);
        }
        public async Task<IActionResult> Save(Branch model)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/branch/IsExist/" + model.Name + "," + model.Id + "", _strtoken, _userparam);
            var isexist = await _CallService.FetchObjectFromResponse<bool>(httpResponseMessage);
            if (!isexist)
            {
                string jsonInput = JsonConvert.SerializeObject(model);
                var response = await _CallService.PostApi(jsonInput, "api/branch/save", _strtoken, _userparam);
                string data = await _CallService.FetchStringFromResponse(response);
                return Json(data);
            }
            else
            {
                return Json(isexist);
            }          
           
        }
        public async Task<IActionResult> Delete(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var response = await _CallService.DeleteApi("api/branch/Delete/" + Id + "", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
        }

        public async Task<IActionResult> AddEditBranchClass(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/Branch/BranchClass/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<ClassVM>(httpResponseMessage);
            return PartialView("AddEditBranchClass", model);
        }
        public async Task<IActionResult> UpdateBranchClass(ClassVM model)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            string jsonInput = JsonConvert.SerializeObject(model);
            var response = await _CallService.PostApi(jsonInput, "api/Branch/BranchClass/Save", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
        }
        public async Task<ActionResult> ViewBranchSectionTimeTable(int BranchId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/Branch/TimeTable/" + BranchId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<BranchTimeTableRPTVM>(httpResponseMessage);
            model.ReportSettings.AFourHeaderPhotoPath = _CallService.FetchApiUrl() + model.ReportSettings.AFourHeaderPhotoPath;
            model.ReportSettings.AFourFooterPhotoPath = _CallService.FetchApiUrl() + model.ReportSettings.AFourFooterPhotoPath;
            model.ReportSettings.AFiveHeaderPhotoPath = _CallService.FetchApiUrl() + model.ReportSettings.AFiveHeaderPhotoPath;
            model.ReportSettings.AFiveFooterPhotoPath = _CallService.FetchApiUrl() + model.ReportSettings.AFiveFooterPhotoPath;
            return PartialView("ViewBranchSectionTimeTable", model);
        }
        public async Task<ActionResult> GetTimeTableFromDB(int branchId, int sessionyearId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/Branch/TimeTable/" + branchId + "," + sessionyearId + "", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(httpResponseMessage);
            return Json(data);
        }
    }
}
