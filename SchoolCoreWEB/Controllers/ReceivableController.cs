using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using SchoolCoreWEB.IServices;
using SchoolCoreWEB.Models;


namespace SchoolCoreWEB.Controllers
{
    
    public class ReceivableController : Controller
    {
        private readonly ICallService _CallService;
        private LanguageService _localization;
        string _userparam;
        private string _strtoken;
        public ReceivableController(ICallService CallService, LanguageService localization)
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
            var httpResponseMessage = await _CallService.GetApi("api/Expense/LoadReceivableParams", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<SearchReceivableParams>(httpResponseMessage);
            return View(model);
        }
        //public async Task<ActionResult> GetAll()
        //{
        //    _userparam = HttpContext.Session.GetString("UserParam");
        //    _strtoken = HttpContext.Session.GetString("strtoken");
        //    var httpResponseMessage = await _CallService.GetApi("api/Expense", _strtoken, _userparam);
        //    string data = await _CallService.FetchStringFromResponse(httpResponseMessage);
        //    return Json(data);
        //}
        //public async Task<ActionResult> GetReceivable(int Id)
        //{

        //    _userparam = HttpContext.Session.GetString("UserParam");
        //    _strtoken = HttpContext.Session.GetString("strtoken");
        //    var httpResponseMessage = await _CallService.GetApi("api/Expense/view/" + Id + "", _strtoken, _userparam);
        //    var model = await _CallService.FetchObjectFromResponse<VStudentPay>(httpResponseMessage);
        //    return Json(model);
        //}
        public async Task<ActionResult> GetAllReceivableByDate(int Id)
        {
            //_userparam = HttpContext.Session.GetString("UserParam");
            //_strtoken = HttpContext.Session.GetString("strtoken");
            //var httpResponseMessage = await _CallService.GetApi("api/Expense/GetAllReceivableByDate/" + Id + "", _strtoken, _userparam);
            //var model = await _CallService.FetchObjectFromResponse<List<VStudentPay>>(httpResponseMessage);
            ReceivableSummaryId model = new ReceivableSummaryId { Id = Id};
            return PartialView("ReceivableByDateView", model);           

        }
        public async Task<ActionResult> View(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/Expense/view/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<VStudentPay>(httpResponseMessage);
            return PartialView("ViewExpense", model);
        }
          
    }
}
