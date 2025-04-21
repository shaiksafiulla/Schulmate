using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using SchoolCoreWEB.IServices;
using SchoolCoreWEB.Models;

namespace SchoolCoreWEB.Controllers
{
   
    public class ScheduleSyllabusController : Controller
    {
        string _userparam;
        string _strtoken;       
        private readonly ICallService _CallService;
        private LanguageService _localization;
        public ScheduleSyllabusController(ICallService CallService, LanguageService localization)
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
            var httpResponseMessage = await _CallService.GetApi("api/ScheduleSyllabus", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(httpResponseMessage);
            return Json(data);
        }        
        public async Task<ActionResult> GetExaminationPaper(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/ScheduleSyllabus/ExamPaper/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<ExaminationPaper>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> GetQuestionPaper(int examId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/ScheduleSyllabus/QuestionPaper/" + examId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<QuestionPaper>(httpResponseMessage);
            if(model != null)
            {
                model.Attach.url = _CallService.FetchApiUrl() + model.Attach.url;
            }
            return PartialView("QuestionPaperView", model);
        }
        public async Task<ActionResult> SaveExaminationPaper(string model)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var objModel = JsonConvert.DeserializeObject<ExaminationPaper>(model);
            string jsonInput = JsonConvert.SerializeObject(objModel);
            var response = await _CallService.PostApi(jsonInput, "api/ScheduleSyllabus/ExamPaper/save", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
        }
        public async Task<ActionResult> GetScheduleSyllabus(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/ScheduleSyllabus/view/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<VSchedule>(httpResponseMessage);
            return PartialView("AddEditScheduleSyllabus", model);
        }
        public async Task<ActionResult> GetScheduleSyllabusView(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/ScheduleSyllabus/ScheduleSyllabusView/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<ScheduleSyllabusDetail>(httpResponseMessage);
            if (model != null)
            {
                model.ReportSettings.AFourHeaderPhotoPath = !string.IsNullOrEmpty(model.ReportSettings.AFourHeaderPhotoPath) ? _CallService.FetchApiUrl() + model.ReportSettings.AFourHeaderPhotoPath : string.Empty;
                model.ReportSettings.AFourFooterPhotoPath = !string.IsNullOrEmpty(model.ReportSettings.AFourFooterPhotoPath) ? _CallService.FetchApiUrl() + model.ReportSettings.AFourFooterPhotoPath : string.Empty;

                model.ReportSettings.AFiveHeaderPhotoPath = !string.IsNullOrEmpty(model.ReportSettings.AFiveHeaderPhotoPath) ? _CallService.FetchApiUrl() + model.ReportSettings.AFiveHeaderPhotoPath : string.Empty;
                model.ReportSettings.AFiveFooterPhotoPath = !string.IsNullOrEmpty(model.ReportSettings.AFiveFooterPhotoPath) ? _CallService.FetchApiUrl() + model.ReportSettings.AFiveFooterPhotoPath : string.Empty;
            }
            return PartialView("ViewScheduleSyllabus", model);
        }
        public async Task<ActionResult> GetSyllabusInfo(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/ScheduleSyllabus/Topic/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<SpSyllabusTopic>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> GetExaminationByScheduleId(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/ScheduleSyllabus/Examination/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<List<VExamination>>(httpResponseMessage);
            return Json(model);
        }
      
        public async Task<ActionResult> GetExamSubjectByBranchClass(string scheduleId, string branchclassId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/ScheduleSyllabus/ExamSubject/" + scheduleId + "," + branchclassId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<List<ExamSubject>>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> GetBranchClassQuestionStatus(string scheduleId, string branchclassId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/ScheduleSyllabus/QuestionStatus/" + scheduleId + "," + branchclassId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<string>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> GetExamTopic(string examId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/ScheduleSyllabus/ExamTopic/" + examId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<LessonVM>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> UpdateExamTopic(string model)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var objModel = JsonConvert.DeserializeObject<LessonVM>(model);
            string jsonInput = JsonConvert.SerializeObject(objModel);
            var response = await _CallService.PostApi(jsonInput, "api/ScheduleSyllabus/ExamTopic/save", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
        }
        public async Task<ActionResult> GetExamQuestion(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/ScheduleSyllabus/ExamQuestion/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<ExamQuestionVM>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> UpdateExamQuestion(string model)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var objModel = JsonConvert.DeserializeObject<ExamQuestionVM>(model);
            string jsonInput = JsonConvert.SerializeObject(objModel);
            var response = await _CallService.PostApi(jsonInput, "api/ScheduleSyllabus/ExamQuestion/save", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
        }
        public async Task<ActionResult> ProcessQuestionAlgo(string model)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var objModel = JsonConvert.DeserializeObject<ExamQuestionVM>(model);
            string jsonInput = JsonConvert.SerializeObject(objModel);
            var response = await _CallService.PostApi(jsonInput, "api/ScheduleSyllabus/ProcessQuestion", _strtoken, _userparam);
            var data = await _CallService.FetchObjectFromResponse<List<VQuestionBank>>(response);
            return Json(data);
        }
        public async Task<ActionResult> UploadQPaper()
        {
            //var files = Request.Form.Files;
            //var strModel = Request.Form["model"];

            //var userInfokey = HttpContext.Session.GetString("UserInfo");
            //if (userInfokey != null)
            //{
            //    _userInfo = JsonConvert.DeserializeObject<VUserInfo>(userInfokey);
            //}
            //_strtoken = HttpContext.Session.GetString("strtoken");
            //var objModel = JsonConvert.DeserializeObject<UploadExamQPaper>(strModel);
            //var strjson = JsonConvert.SerializeObject(objModel);
            //var response = await _CallService.PostFileApi(files, strjson, "api/ScheduleSyllabus/UploadQPaper", _strtoken, _userparam);
            //string data = await _CallService.FetchStringFromResponse(response);
            //return Json(data);
            return Json(null);
        }
    }
}
