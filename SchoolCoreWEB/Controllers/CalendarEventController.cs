using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using SchoolCoreWEB.IServices;
using SchoolCoreWEB.Models;


namespace SchoolCoreWEB.Controllers
{
    
    public class CalendarEventController : Controller
    {
        private readonly ICallService _CallService;
        private LanguageService _localization;
        string _userparam;
        private string _strtoken;
        public CalendarEventController(ICallService CallService, LanguageService localization)
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
            var httpResponseMessage = await _CallService.GetApi("api/calendarevent", _strtoken, _userparam);
            //string data = await _CallService.FetchStringFromResponse(httpResponseMessage);
            var model = await _CallService.FetchObjectFromResponse<List<CastCalender>>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> GetCalendarEvent(int Id)
        {

            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/calendarevent/view/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<CastCalender>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> View(int Id)
        {

            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/calendarevent/view/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<CastCalender>(httpResponseMessage);
            return PartialView("ViewExamType", model);
        }
        public async Task<ActionResult> AddEdit(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/calendarevent/AddEdit/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<CalendarEvent>(httpResponseMessage);
            return PartialView("AddEditCalendarEvent", model);
        }
        public async Task<ActionResult> AddEvent(string jsdate)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/calendarevent/AddEdit/" + 0 + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<CalendarEvent>(httpResponseMessage);
            model.StartDate = Convert.ToDateTime(jsdate);
            model.EndDate = Convert.ToDateTime(jsdate);
            return PartialView("AddEditCalendarEvent", model);
        }
        public async Task<ActionResult> Save(CalendarEvent model)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            string jsonInput = JsonConvert.SerializeObject(model);
            var response = await _CallService.PostApi(jsonInput, "api/calendarevent/save", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
            //var httpResponseMessage = await _CallService.GetApi("api/calendarevent/IsExist/" + model.Name + "," + model.Id + "", _strtoken, _userparam);
            //var isexist = await _CallService.FetchObjectFromResponse<bool>(httpResponseMessage);
            //if (!isexist)
            //{

            //}
            //else
            //{
            //    return Json(isexist);
            //}
        }
        public async Task<ActionResult> Delete(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var response = await _CallService.DeleteApi("api/calendarevent/Delete/" + Id + "", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
        }       
    }
}
