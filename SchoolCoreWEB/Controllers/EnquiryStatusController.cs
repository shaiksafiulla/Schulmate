﻿using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using SchoolCoreWEB.IServices;
using SchoolCoreWEB.Models;


namespace SchoolCoreWEB.Controllers
{
    
    public class EnquiryStatusController : Controller
    {
        private readonly ICallService _CallService;
        private LanguageService _localization;
        string _userparam;
        private string _strtoken;
        public EnquiryStatusController(ICallService CallService, LanguageService localization)
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
            var httpResponseMessage = await _CallService.GetApi("api/enquirystatus", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(httpResponseMessage);
            return Json(data);
        }
        public async Task<ActionResult> GetEnquiryStatus(int Id)
        {

            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/enquirystatus/view/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<VEnquiryStatus>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> View(int Id)
        {

            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/enquirystatus/view/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<VEnquiryStatus>(httpResponseMessage);
            return PartialView("ViewEnquiryStatus", model);
        }
        public async Task<ActionResult> AddEdit(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/enquirystatus/AddEdit/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<EnquiryStatus>(httpResponseMessage);
            return PartialView("AddEditEnquiryStatus", model);
        }
        public async Task<ActionResult> Save(EnquiryStatus model)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/enquirystatus/IsExist/" + model.Name + "," + model.Id + "", _strtoken, _userparam);
            var isexist = await _CallService.FetchObjectFromResponse<bool>(httpResponseMessage);
            if (!isexist)
            {
                string jsonInput = JsonConvert.SerializeObject(model);
                var response = await _CallService.PostApi(jsonInput, "api/enquirystatus/save", _strtoken, _userparam);
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
            var response = await _CallService.DeleteApi("api/enquirystatus/Delete/" + Id + "", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
        }       
    }
}
