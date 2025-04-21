using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using SchoolCoreWEB.IServices;
using SchoolCoreWEB.Models;

namespace SchoolCoreWEB.Controllers
{
     public class RolesController : Controller
    {
        string _userparam;
        private string _strtoken;
        private readonly ICallService _CallService;
        private LanguageService _localization;
        public RolesController(ICallService CallService, LanguageService localization)
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
            var httpResponseMessage = await _CallService.GetApi("api/roles", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(httpResponseMessage);
            return Json(data);
        }
        public async Task<ActionResult> GetRole(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/roles/view/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<VRole>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> View(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/roles/view/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<VRole>(httpResponseMessage);
            return PartialView("RoleView", model);
        }
        public async Task<ActionResult> AddEdit(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/roles/AddEdit/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<Role>(httpResponseMessage);
            return PartialView("AddEditRoles", model);
        }
        public async Task<ActionResult> Save(Role model)
        {

            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/roles/IsExist/" + model.Name + "," + model.Id + "", _strtoken, _userparam);
            var isexist = await _CallService.FetchObjectFromResponse<bool>(httpResponseMessage);
            if (!isexist)
            {
                string jsonInput = JsonConvert.SerializeObject(model);
                var response = await _CallService.PostApi(jsonInput, "api/roles/save", _strtoken, _userparam);
                string data = await _CallService.FetchStringFromResponse(response);
                return Json(data);
            }
            else
            {
                return Json(isexist);
            }

        }
        public async Task<ActionResult> DeleteRole(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var response = await _CallService.DeleteApi("api/roles/Delete/" + Id + "", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
        }
        public async Task<ActionResult> AddEditRolePage(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/roles/RolePage/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<PageVM>(httpResponseMessage);
            return PartialView("AddEditRolePage", model);
        }
        public async Task<ActionResult> UpdateRolePage(PageVM model)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            string jsonInput = JsonConvert.SerializeObject(model);
            var response = await _CallService.PostApi(jsonInput, "api/roles/RolePage/Save", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
        }
        public async Task<ActionResult> AddEditRoleUser(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/roles/RoleUser/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<RoleUserVM>(httpResponseMessage);
            return PartialView("AddEditRoleUser", model);
        }
        public async Task<ActionResult> UpdateRoleUser(RoleUserVM model)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            string jsonInput = JsonConvert.SerializeObject(model);
            var response = await _CallService.PostApi(jsonInput, "api/roles/RoleUser/Save", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
        }
        public async Task<ActionResult> GetPageFunctions()
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/roles/PageFunction", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<PageFunctionProcedure>(httpResponseMessage);
            return Json(model);
        }

    }
}
