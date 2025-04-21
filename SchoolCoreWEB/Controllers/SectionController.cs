using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using SchoolCoreWEB.IServices;
using SchoolCoreWEB.Models;

namespace SchoolCoreWEB.Controllers
{    
    public class SectionController : Controller
    {
        string _userparam;
        string _strtoken;
        private readonly ICallService _CallService;
        private LanguageService _localization;
        public SectionController(ICallService CallService, LanguageService localization)
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
            //else
            //{
            //    var userInfokey = HttpContext.Session.GetString("UserInfo");
            //    if (userInfokey != null)
            //    {
            //        _userInfo = JsonConvert.DeserializeObject<VUserInfo>(userInfokey);
            //        _strtoken = HttpContext.Session.GetString("strtoken");
            //    }
            //}
            return View();
        }
        public async Task<ActionResult> GetAll()
        {

            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/section", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(httpResponseMessage);
            return Json(data);
        }
        public async Task<ActionResult> GetSection(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/section/view/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<VSection>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> View(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/section/view/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<VSection>(httpResponseMessage);
            return PartialView("ViewSection", model);
        }
        public async Task<ActionResult> AddEdit(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/section/AddEdit/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<Section>(httpResponseMessage);
            return PartialView("AddEditSection", model);
        }
        public async Task<ActionResult> Save(Section model)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/section/IsExist/" + model.Name + ","+model.BranchClassId+"," + model.Id + "", _strtoken, _userparam);
            var isexist = await _CallService.FetchObjectFromResponse<bool>(httpResponseMessage);
            if (!isexist)
            {
                string jsonInput = JsonConvert.SerializeObject(model);
                var response = await _CallService.PostApi(jsonInput, "api/section/save", _strtoken, _userparam);
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
            var response = await _CallService.DeleteApi("api/section/Delete/" + Id + "", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
        }

        public async Task<ActionResult> GetClassesByBranch(string branchId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/section/GetClassesByBranch/" + branchId+ "", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(httpResponseMessage);
            return Json(data);
        }
        public async Task<ActionResult> GetSectionsByBranchAndClass(string branchId, string classId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/section/GetSectionsByBranchAndClass/" + branchId + "," + classId + ",", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(httpResponseMessage);
            return Json(data);
        }


        //public async Task<ActionResult> AddEditSectionStudent(int SectionId,int ClassId)
        //{
        //    var userInfokey = HttpContext.Session.GetString("UserInfo");
        //    if (userInfokey != null)
        //    {
        //        _userInfo = JsonConvert.DeserializeObject<VUserInfo>(userInfokey);
        //    }
        //    _strtoken = HttpContext.Session.GetString("strtoken");
        //    var httpResponseMessage = await _CallService.GetApi("api/section/AddEditSectionStudent/" + SectionId + ","+ ClassId + "", _strtoken, _userparam);
        //    var model = await _CallService.FetchObjectFromResponse<StudentVM>(httpResponseMessage);
        //    return PartialView("AddEditSectionStudent", model);
        //}
        //public async Task<ActionResult> UpdateSectionStudent(string model)
        //{
        //    var userInfokey = HttpContext.Session.GetString("UserInfo");
        //    if (userInfokey != null)
        //    {
        //        _userInfo = JsonConvert.DeserializeObject<VUserInfo>(userInfokey);
        //    }
        //    _strtoken = HttpContext.Session.GetString("strtoken");
        //    var objModel = JsonConvert.DeserializeObject<StudentVM>(model);
        //    string jsonInput = JsonConvert.SerializeObject(objModel);
        //    var response = await _CallService.PostApi(jsonInput, "api/section/SectionStudent/Save", _strtoken, _userparam);
        //    string data = await _CallService.FetchStringFromResponse(response);
        //    return Json(data);
        //}
        public async Task<IActionResult> AddEditSectionTeacher(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/section/SectionTeacher/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<SectionVM>(httpResponseMessage);
            return PartialView("AddEditSectionTeacher", model);
        }
        public async Task<ActionResult> UpdateSectionTeacher(string model)
        {
            try
            {
                _userparam = HttpContext.Session.GetString("UserParam");
                _strtoken = HttpContext.Session.GetString("strtoken");
                var objModel = JsonConvert.DeserializeObject<SectionVM>(model);
                string jsonInput = JsonConvert.SerializeObject(objModel);
                var response = await _CallService.PostApi(jsonInput, "api/section/SectionTeacher/Save", _strtoken, _userparam);
                string data = await _CallService.FetchStringFromResponse(response);
                return Json(data);
            }
            catch (Exception ex)
            {                
            }            
            return Json(null);
        }

        public async Task<IActionResult> AddEditSectionActivityPersonnel(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/section/SectionActivityPersonnel/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<SectionActivityVM>(httpResponseMessage);
            return PartialView("AddEditSectionActivityPersonnel", model);
        }
        public async Task<ActionResult> UpdateSectionActivityPersonnel(string model)
        {
            try
            {
                _userparam = HttpContext.Session.GetString("UserParam");
                _strtoken = HttpContext.Session.GetString("strtoken");
                var objModel = JsonConvert.DeserializeObject<SectionActivityVM>(model);
                string jsonInput = JsonConvert.SerializeObject(objModel);
                var response = await _CallService.PostApi(jsonInput, "api/section/SectionActivity/Save", _strtoken, _userparam);
                string data = await _CallService.FetchStringFromResponse(response);
                return Json(data);
            }
            catch (Exception ex)
            {
            }
            return Json(null);
        }
        public async Task<ActionResult> AddEditSectionTimeTable(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/section/TimeTable/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<SectionTimeTableVM>(httpResponseMessage);
            return PartialView("AddEditSectionTimeTable", model);
        }
        public async Task<ActionResult> GetTimeTableFromDB(int sectionId, int sessionyearId)
        {

            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            //var httpResponseMessage = await _CallService.GetApi("api/section/TimeTable/"+sectionId+","+ sessionyearId + "", _strtoken, _userparam);
            var httpResponseMessage = await _CallService.GetApi("api/section/MbTimeTable/" + sectionId + "", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(httpResponseMessage);
            return Json(data);
        }
        public async Task<ActionResult> UpdateSectionTimeTable(string model)
        {
            try
            {
                _userparam = HttpContext.Session.GetString("UserParam");
                _strtoken = HttpContext.Session.GetString("strtoken");
                var objModel = JsonConvert.DeserializeObject<TimeTableDTO>(model);
                string jsonInput = JsonConvert.SerializeObject(objModel);
                var response = await _CallService.PostApi(jsonInput, "api/section/TimeTable/Save", _strtoken, _userparam);
                string data = await _CallService.FetchStringFromResponse(response);
                return Json(data);
            }
            catch (Exception ex)
            {
            }
            return Json(null);
        }

    }
}
