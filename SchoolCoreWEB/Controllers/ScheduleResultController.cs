using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using SchoolCoreWEB.IServices;
using SchoolCoreWEB.Models;

namespace SchoolCoreWEB.Controllers
{
    
    public class ScheduleResultController : Controller
    {
        string _userparam;
        string _strtoken;
        string _downloadmsg;
        private readonly ICallService _CallService;
        private LanguageService _localization;
        public ScheduleResultController(ICallService CallService, LanguageService localization)
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
            var httpResponseMessage = await _CallService.GetApi("api/ScheduleResult", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(httpResponseMessage);
            return Json(data);
        }
        public async Task<ActionResult> GetScheduleResult(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/ScheduleResult/View/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<VSchedule>(httpResponseMessage);
            return PartialView("AddEditScheduleResult", model);
        }
        public async Task<ActionResult> GetScheduleResultDetail(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/ScheduleResult/ScheduleResultView/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<ScheduleResultDetail>(httpResponseMessage);
            if (model != null)
            {
                model.ReportSettings.AFourHeaderPhotoPath = !string.IsNullOrEmpty(model.ReportSettings.AFourHeaderPhotoPath) ? _CallService.FetchApiUrl() + model.ReportSettings.AFourHeaderPhotoPath : string.Empty;
                model.ReportSettings.AFourFooterPhotoPath = !string.IsNullOrEmpty(model.ReportSettings.AFourFooterPhotoPath) ? _CallService.FetchApiUrl() + model.ReportSettings.AFourFooterPhotoPath : string.Empty;

                model.ReportSettings.AFiveHeaderPhotoPath = !string.IsNullOrEmpty(model.ReportSettings.AFiveHeaderPhotoPath) ? _CallService.FetchApiUrl() + model.ReportSettings.AFiveHeaderPhotoPath : string.Empty;
                model.ReportSettings.AFiveFooterPhotoPath = !string.IsNullOrEmpty(model.ReportSettings.AFiveFooterPhotoPath) ? _CallService.FetchApiUrl() + model.ReportSettings.AFiveFooterPhotoPath : string.Empty;
            }
            return PartialView("ViewScheduleResult", model);
        }
        public async Task<ActionResult> GetExportData(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var response = await _CallService.GetApi("api/ScheduleResult/ExportData/" + Id + "", _strtoken, _userparam);
            if (response.IsSuccessStatusCode)
            {
                byte[] fileContent = await response.Content.ReadAsByteArrayAsync();               
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";              
                string fileName = "Input_Student_Result.xlsx";             
                return File(fileContent, contentType, fileName);
            }
            else
            {               
                return Content("Error downloading the Excel file");
            }           
        }
        public async Task<ActionResult> GetScheduleBranchClass(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/ScheduleResult/BranchClass/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<List<VScheduleBranchClass>>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> GetSPExamResult(string branchclassId, string schedId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/ScheduleResult/ExamResult/" + branchclassId + "," + schedId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<SpExamResult>(httpResponseMessage);
            return Json(model);
        }

        public async Task<ActionResult> UploadStudentResult(string model)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var objModel = JsonConvert.DeserializeObject<StudentResultUpload>(model);

            string jsonInput = JsonConvert.SerializeObject(objModel);
            var response = await _CallService.PostApi(jsonInput, "api/ScheduleResult/Upload", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
        }
        public async Task<ActionResult> SaveStudentResult(string model)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var objModel = JsonConvert.DeserializeObject<StudentResultVM>(model);

            string jsonInput = JsonConvert.SerializeObject(objModel);
            var response = await _CallService.PostApi(jsonInput, "api/ScheduleResult/Student", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
        }
        public async Task<ActionResult> GetBranchClassResultStatus(string scheduleId, string branchclassId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/ScheduleResult/ResultStatus/" + scheduleId + "," + branchclassId + "", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(httpResponseMessage);
            return Json(data);
        }       
    }
}
