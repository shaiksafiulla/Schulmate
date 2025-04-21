using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using SchoolCoreWEB.Helpers;
using SchoolCoreWEB.IServices;
using SchoolCoreWEB.Models;
using System.Net;
using System.Net.Http;
using static SchoolCoreWEB.Helpers.Shared;


namespace SchoolCoreWEB.Controllers
{
    public class AttachmentController : Controller
    {
        private string _userparam;
        private string _strtoken;
        private readonly ICallService _CallService;
        private LanguageService _localization;
        public AttachmentController(ICallService CallService, LanguageService localization) 
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
        public async Task<IActionResult> GetAll(int type, int referid)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/attachment/Get/" + type + "," + referid + "", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(httpResponseMessage);
            return Json(data);
        }
        public async Task<IActionResult> GetAttachment(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/attachment/view/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<VAttachment>(httpResponseMessage);
            return Json(model);
        }
        public async Task<IActionResult> View(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/attachment/view/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<Attachment>(httpResponseMessage);
            return PartialView("ViewBranch", model);
        }
        public async Task<IActionResult> AddEdit(int Id, int type, int referid)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/attachment/AddEdit/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<Attachment>(httpResponseMessage);
            if(model !=null)
            {
                if(model.ReferenceId == null && string.IsNullOrEmpty(model.AttachType))
                {
                    model.ReferenceId = referid;
                    model.AttachType = type.ToString();
                }               
                model.FilePath = _CallService.FetchApiUrl() + model.FilePath;
            }
            
            return PartialView("AddEditAttachment", model);
        }
        public async Task<IActionResult> Save(Attachment model)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            string jsonInput = JsonConvert.SerializeObject(model);
            var response = await _CallService.PostApi(jsonInput, "api/attachment/save", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
            //var httpResponseMessage = await _CallService.GetApi("api/attachment/IsExist/" + model.Name + "," + model.Id + "", _strtoken, _userparam);
            //var isexist = await _CallService.FetchObjectFromResponse<bool>(httpResponseMessage);
            //if (!isexist)
            //{
            //    string jsonInput = JsonConvert.SerializeObject(model);
            //    var response = await _CallService.PostApi(jsonInput, "api/attachment/save", _strtoken, _userparam);
            //    string data = await _CallService.FetchStringFromResponse(response);
            //    return Json(data);
            //}
            //else
            //{
            //    return Json(isexist);
            //}

        }
        public async Task<IActionResult> Delete(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var response = await _CallService.DeleteApi("api/attachment/Delete/" + Id + "", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
        }     
        public async Task<FileResult> Download(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var response = await _CallService.GetApi("api/attachment/download/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<Tuple<byte[], string>>(response);            
            return File(model.Item1, "application/octet-stream", model.Item2);            
        }

    }
}
