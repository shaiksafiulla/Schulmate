using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using SchoolCoreWEB.IServices;
using SchoolCoreWEB.Models;
using static SchoolCoreWEB.Helpers.Shared;


namespace SchoolCoreWEB.Controllers
{
    public class DriverController : Controller
    {
        private readonly ICallService _CallService;
        private LanguageService _localization;
        string _userparam;
        private string _strtoken;
        public DriverController(ICallService CallService, LanguageService localization)
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
            var httpResponseMessage = await _CallService.GetApi("api/driver", _strtoken, _userparam);
            var model = await _CallService.FetchObjectListFromResponse<VAdmin>(httpResponseMessage);
            if (model != null)
            {
                foreach (var admin in model)
                {
                    if (!string.IsNullOrEmpty(admin.FilePath))
                    {
                        admin.FilePath = _CallService.FetchApiUrl() + admin.FilePath;
                    }
                }
            }
            string strserialize = JsonConvert.SerializeObject(model);
            return Json(strserialize);
        }
        public async Task<ActionResult> GetDriver(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/driver/view/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<VDriver>(httpResponseMessage);
            model.FilePath = !string.IsNullOrEmpty(model.FilePath) ? _CallService.FetchApiUrl() + model.FilePath : string.Empty;
            return Json(model);
        }
        public async Task<ActionResult> View(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/driver/view/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<VDriver>(httpResponseMessage);
            model.FilePath = !string.IsNullOrEmpty(model.FilePath) ? _CallService.FetchApiUrl() + model.FilePath : string.Empty;
            return PartialView("ViewDriver", model);
        }
        public async Task<ActionResult> LoadAttachments(int referid)
        {
            int type = (int)AttachmentType.driver;
            AttachModel model = new AttachModel { Type = type, ReferId = referid };
            return PartialView("~/views/Attachment/index.cshtml", model);
        }
        public async Task<ActionResult> AddEdit(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/driver/AddEdit/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<Personnel>(httpResponseMessage);
            model.FilePath = !string.IsNullOrEmpty(model.FilePath) ? _CallService.FetchApiUrl() + model.FilePath : string.Empty;
            return PartialView("AddEditDriver", model);
        }
        //public async Task<ActionResult> Save(Personnel model)
        //{
        //_userparam = HttpContext.Session.GetString("UserParam");
        //    _strtoken = HttpContext.Session.GetString("strtoken");     
        //    var httpResponseMessage = await _CallService.GetApi("api/admin/IsExist/" + model.FirstName + "," + model.LastName + "," + model.PersonnelAdmission.BranchId + "," + model.Id + "", _strtoken, _userparam);
        //    var isexist = await _CallService.FetchObjectFromResponse<bool>(httpResponseMessage);
        //    if (!isexist)
        //    {
        //        string jsonInput = JsonConvert.SerializeObject(model);
        //        var response = await _CallService.PostApi(jsonInput, "api/admin/save", _strtoken, _userparam);
        //        string data = await _CallService.FetchStringFromResponse(response);
        //        return Json(data);
        //    }
        //    else
        //    {
        //        return Json(isexist);
        //    }
        //}
        public async Task<ActionResult> Delete(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var response = await _CallService.DeleteApi("api/driver/Delete/" + Id + "", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
        }
    }
}
