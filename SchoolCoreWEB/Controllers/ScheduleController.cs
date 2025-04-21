using Localization.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using SchoolCoreWEB.IServices;
using SchoolCoreWEB.Models;

namespace SchoolCoreWEB.Controllers
{
   
    public class ScheduleController : Controller
    {
        private string _userparam;
        private string _strtoken;
        private readonly ICallService _CallService;
        private LanguageService _localization;
        public ScheduleController(ICallService CallService, LanguageService localization)
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
            
            var httpResponseMessage = await _CallService.GetApi("api/Schedule", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(httpResponseMessage);
            return Json(data);
        }
        public async Task<ActionResult> GetSchedule(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/Schedule/view/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<VSchedule>(httpResponseMessage);
            return Json(model);
        }       
        public async Task<ActionResult> GetHallTicket(string scheduleId, string studentId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/Schedule/HallTicket/" + scheduleId + "," + studentId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<HallTicket>(httpResponseMessage);
            if (model != null)
            {
                model.ReportSettings.AFourHeaderPhotoPath = !string.IsNullOrEmpty(model.ReportSettings.AFourHeaderPhotoPath) ? _CallService.FetchApiUrl() + model.ReportSettings.AFourHeaderPhotoPath : string.Empty;
                model.ReportSettings.AFourFooterPhotoPath = !string.IsNullOrEmpty(model.ReportSettings.AFourFooterPhotoPath) ? _CallService.FetchApiUrl() + model.ReportSettings.AFourFooterPhotoPath : string.Empty;

                model.ReportSettings.AFiveHeaderPhotoPath = !string.IsNullOrEmpty(model.ReportSettings.AFiveHeaderPhotoPath) ? _CallService.FetchApiUrl() + model.ReportSettings.AFiveHeaderPhotoPath : string.Empty;
                model.ReportSettings.AFiveFooterPhotoPath = !string.IsNullOrEmpty(model.ReportSettings.AFiveFooterPhotoPath) ? _CallService.FetchApiUrl() + model.ReportSettings.AFiveFooterPhotoPath : string.Empty;
            }
            // return PartialView("HallTicketView", model);
            return View("HallTicketView", model);
        }
        public async Task<ActionResult> AddEdit(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/Schedule/AddEdit/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<Schedule>(httpResponseMessage);
            return PartialView("AddEditSchedule", model);
       
        }
        public async Task<ActionResult> Save(string model)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var objModel = JsonConvert.DeserializeObject<Schedule>(model);
            //objModel.BranchId = (int)_userInfo.BranchId;
            string jsonInput = JsonConvert.SerializeObject(objModel);
            var response = await _CallService.PostApi(jsonInput, "api/Schedule/save", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);           
        }
        public async Task<ActionResult> SaveScheduleBranchClassSubject(string model)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var objModel = JsonConvert.DeserializeObject<ScheduleBranchClassSubjectVM>(model);

            string jsonInput = JsonConvert.SerializeObject(objModel);
            var response = await _CallService.PostApi(jsonInput, "api/Schedule/ClassSubject/save", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);          
        }
        public async Task<ActionResult> SaveScheduleBranchClassSubjectTopic(string model)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var objModel = JsonConvert.DeserializeObject<ScheduleBrclsSubject>(model);

            string jsonInput = JsonConvert.SerializeObject(objModel);
            var response = await _CallService.PostApi(jsonInput, "api/Schedule/ClassSubjectTopic/save", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
        }
        public async Task<ActionResult> SaveScheduleExamAlgo(string scheduleId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            string jsonInput = JsonConvert.SerializeObject(scheduleId);
            var response = await _CallService.PostApi(jsonInput, "api/Schedule/ExamAlgo/" + scheduleId + "", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
        }
        public async Task<ActionResult> UpdateScheduleTimeTableStatus(string scheduleId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            string jsonInput = JsonConvert.SerializeObject(scheduleId);
            var response = await _CallService.PostApi(jsonInput, "api/Schedule/TimeTableStatus/" + scheduleId + "", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
        }
        public async Task<ActionResult> UpdateScheduleTeacherStatus(string scheduleId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            string jsonInput = JsonConvert.SerializeObject(scheduleId);
            var response = await _CallService.PostApi(jsonInput, "api/Schedule/TeacherStatus/" + scheduleId + "", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
        }
        public async Task<ActionResult> UpdateScheduleFinishStatus(string scheduleId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            string jsonInput = JsonConvert.SerializeObject(scheduleId);
            var response = await _CallService.PostApi(jsonInput, "api/Schedule/FinishStatus/" + scheduleId + "", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
        }
        public async Task<ActionResult> SaveScheduleTeacherExamDateAlgo(string scheduleId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            string jsonInput = JsonConvert.SerializeObject(scheduleId);
            var response = await _CallService.PostApi(jsonInput, "api/Schedule/TeacherExamDateAlgo/" + scheduleId + "", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
        }
        
        public async Task<ActionResult> GetPivotTimeTable(string scheduleId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/Schedule/TimeTable/" + scheduleId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<ScheduleExamProcedure>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> GetScheduleBranchClassGroup(string scheduleId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/Schedule/BranchClassGroup/" + scheduleId + "", _strtoken, _userparam);           
            string data = await _CallService.FetchStringFromResponse(httpResponseMessage);
            return Json(data);            
        }
        public async Task<ActionResult> GetScheduleBranchClassStudent(string model)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var objModel = JsonConvert.DeserializeObject<ClassAllocation>(model);
            string jsonInput = JsonConvert.SerializeObject(objModel);
            var response = await _CallService.PostApi(jsonInput, "api/Schedule/ClassStudents", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
        }
        public async Task<ActionResult> SaveExamHallAllocation(string model)
        {
            try
            {
                _userparam = HttpContext.Session.GetString("UserParam");
                _strtoken = HttpContext.Session.GetString("strtoken");
                var objModel = JsonConvert.DeserializeObject<List<ExamHall>>(model);
                string jsonInput = JsonConvert.SerializeObject(objModel);
                var response = await _CallService.PostApi(jsonInput, "api/Schedule/ExamHall/Save", _strtoken, _userparam);
                string data = await _CallService.FetchStringFromResponse(response);
                return Json(data);
            }
            catch (Exception ex)
            {
                return null;
            }           
        }
        public async Task<ActionResult> GetPivotScheduleTeacher(string scheduleId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/Schedule/Teacher/" + scheduleId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<ScheduleTeacherProcedure>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> GetScheduleStatus(string scheduleId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/Schedule/Status/" + scheduleId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<int>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> SaveScheduleExam(string model)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var objModel = JsonConvert.DeserializeObject<ScheduleBrclsSubject>(model);
            string jsonInput = JsonConvert.SerializeObject(objModel);
            var response = await _CallService.PostApi(jsonInput, "api/Schedule/SaveAlgoExam", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);

            //string jsonInput = JsonConvert.SerializeObject(objModel);
            //var response = await _CallService.PostApi(jsonInput, "api/Schedule/save", _strtoken, _userparam);
            //string data = await _CallService.FetchStringFromResponse(response);

            return Json(data);
        }
        public async Task<ActionResult> GetSpScheduleTeacher(string scheduleId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/Schedule/SpScheduleTeacher/" + scheduleId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<SpScheduleTeacher>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> GetSpScheduleExam(string scheduleId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/Schedule/SpScheduleExam/" + scheduleId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<SpScheduleExam>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> GetSpScheduleStudent(string scheduleId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/Schedule/SpScheduleStudent/" + scheduleId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<SpScheduleStudent>(httpResponseMessage);
            return Json(model);
        }
        //public async Task<ActionResult> GetScheduleSection(string scheduleId)
        //{
        //    var userInfokey = HttpContext.Session.GetString("UserInfo");
        //    if (userInfokey != null)
        //    {
        //        _userInfo = JsonConvert.DeserializeObject<VUserInfo>(userInfokey);
        //    }
        //    _strtoken = HttpContext.Session.GetString("strtoken");
        //    var httpResponseMessage = await _CallService.GetApi("api/Schedule/Section/" + scheduleId + "", _strtoken, _userparam);
        //    var model = await _CallService.FetchObjectFromResponse<List<VScheduleSection>>(httpResponseMessage);
        //    return Json(model);
        //}
        public async Task<ActionResult> GetScheduleExamDateTime(string scheduleId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/Schedule/ExamDateTime/" + scheduleId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<SpScheduleExamDateTime>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> SaveScheduleTeacherExamDate(string model)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var objModel = JsonConvert.DeserializeObject<ScheduleTeacherExam>(model);
            string jsonInput = JsonConvert.SerializeObject(objModel);
            var response = await _CallService.PostApi(jsonInput, "api/Schedule/TeacherExamDate/save", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
        }
        //public async Task<ActionResult> SaveTeacherSection(string model)
        //{
        //    var userInfokey = HttpContext.Session.GetString("UserInfo");
        //    if (userInfokey != null)
        //    {
        //        _userInfo = JsonConvert.DeserializeObject<VUserInfo>(userInfokey);
        //    }
        //    _strtoken = HttpContext.Session.GetString("strtoken");
        //    var objModel = JsonConvert.DeserializeObject<TeacherSection>(model);
        //    string jsonInput = JsonConvert.SerializeObject(objModel);
        //    var response = await _CallService.PostApi(jsonInput, "api/Schedule/TeacherSection/save", _strtoken, _userparam);
        //    string data = await _CallService.FetchStringFromResponse(response);
        //    return Json(data);
        //}
        public async Task<ActionResult> SaveScheduleExamination(string model)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var objModel = JsonConvert.DeserializeObject<ScheduleExam>(model);
            string jsonInput = JsonConvert.SerializeObject(objModel);
            var response = await _CallService.PostApi(jsonInput, "api/Schedule/Examination/save", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
        }
      
        public async Task<ActionResult> SaveScheduleStudent(string model)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var objModel = JsonConvert.DeserializeObject<ScheduleStudentExam>(model);
            string jsonInput = JsonConvert.SerializeObject(objModel);
            var response = await _CallService.PostApi(jsonInput, "api/Schedule/Student/save", _strtoken, _userparam);
            string data = await _CallService.FetchStringFromResponse(response);
            return Json(data);
        }
        public async Task<ActionResult> GetScheduleDate(string model)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var objModel = JsonConvert.DeserializeObject<SearchScheduleDate>(model);
            string jsonInput = JsonConvert.SerializeObject(objModel);
            var response = await _CallService.PostApi(jsonInput, "api/Schedule/Date/search", _strtoken, _userparam);            
            var data = await _CallService.FetchObjectFromResponse<List<ScheduleStrDate>>(response);
            return Json(data);
        }
        public async Task<ActionResult> GetScheduleBranchClassExam(string scheduleId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/Schedule/BranchClassExam/" + scheduleId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<List<VBranchClass>>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> GetScheduleClassByBranch(string scheduleId, string branchId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/Schedule/ClassByBranch/" + scheduleId + "," + branchId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<List<VScheduleBranchClass>>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> GetScheduleTeacherExamDate(string scheduleId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/Schedule/TeacherExamDate/" + scheduleId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<List<CastExamHall>>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> GetCastScheduleExamHall(string scheduleId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/Schedule/ScheduleStudentSeat/" + scheduleId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<List<CastScheduleExamHall>>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> GetScheduleStudent(string scheduleId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/Schedule/ScheduleStudent/" + scheduleId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<List<ScheduleStudent>>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> GetScheduleStudentInfo(string scheduleId, string branchClassId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/Schedule/ScheduleStudent/" + scheduleId + "," + branchClassId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<List<VScheduleStudentAllocation>>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> GetScheduleExamination(string scheduleId, string branchId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/Schedule/Examination/" + scheduleId + "," + branchId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<List<VExamination>>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> GetBranchClassSubjectBySchedule(string scheduleId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/Schedule/BranchClassSubject/" + scheduleId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<SpScheduleSubject>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> GetSPScheduleSection(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/Schedule/SPSection/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<SpScheduleStudentSection>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> GetBranchClassBySchedule(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/Schedule/BranchClass/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<List<VScheduleBranchClass>>(httpResponseMessage);
            return Json(model);
        }
        public async Task<ActionResult> GetScheduleClassAndSectionByBranch(string scheduleId)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/Schedule/ClassSection/" + scheduleId + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<ScheduleClassAndSectionAndTeacher>(httpResponseMessage);
            return Json(model);
        }


        //#endregion


        public async Task<IActionResult> View(int Id)
        {
            _userparam = HttpContext.Session.GetString("UserParam");
            _strtoken = HttpContext.Session.GetString("strtoken");
            var httpResponseMessage = await _CallService.GetApi("api/Schedule/view/" + Id + "", _strtoken, _userparam);
            var model = await _CallService.FetchObjectFromResponse<ScheduleDetail>(httpResponseMessage);
            if (model != null)
            {
                model.ReportSettings.AFourHeaderPhotoPath = !string.IsNullOrEmpty(model.ReportSettings.AFourHeaderPhotoPath) ? _CallService.FetchApiUrl() + model.ReportSettings.AFourHeaderPhotoPath : string.Empty;
                model.ReportSettings.AFourFooterPhotoPath = !string.IsNullOrEmpty(model.ReportSettings.AFourFooterPhotoPath) ? _CallService.FetchApiUrl() + model.ReportSettings.AFourFooterPhotoPath : string.Empty;

                model.ReportSettings.AFiveHeaderPhotoPath = !string.IsNullOrEmpty(model.ReportSettings.AFiveHeaderPhotoPath) ? _CallService.FetchApiUrl() + model.ReportSettings.AFiveHeaderPhotoPath : string.Empty;
                model.ReportSettings.AFiveFooterPhotoPath = !string.IsNullOrEmpty(model.ReportSettings.AFiveFooterPhotoPath) ? _CallService.FetchApiUrl() + model.ReportSettings.AFiveFooterPhotoPath : string.Empty;
            }
            return PartialView("ViewSchedule", model);

        }


        //[HttpPost]
        //public ActionResult Delete(int Id)
        //{
        //    //var objSession = HttpContext.Session.GetObjectFromJson<VUserInfo>("UserInfo");
        //    //var result = _lookupRepository.DeleteSchedule(Id, objSession.UserId);
        //    var result = _lookupRepository.DeleteSchedule(Id, 1);
        //    return Json(result);
        //}



        //public async Task<IActionResult> AddEditTimeTable(int Id)
        //{
        //    var vm = _lookupRepository.GetTimeTable(Id);
        //    return PartialView("AddEditTimeTable", vm);
        //}
        //public async Task<IActionResult> GetBranchBySchedule(int Id)
        //{
        //    var vm = _lookupRepository.GetBranchBySchedule(Id);
        //    return Json(vm);
        //}



        //}
        //[HttpPost]
        //public async Task<IActionResult> GetBranchClassSubjectTopicBySchedule(string scheduleId, string branchId)
        //{
        //    var vm = _lookupRepository.GetBranchClassSubjectTopicBySchedule(scheduleId, branchId);
        //    return Json(vm);
        //}

        //[HttpPost]
        //public async Task<IActionResult> GetSubjectsByBranchClassId(string Id, string schedId)
        //{           
        //    var testvm = _lookupRepository.GetSubjectsByBranchClassId(Id, schedId);
        //    return Json(testvm);
        //}
        //[HttpPost]
        //public async Task<IActionResult> GetExaminationByBranchClassId(string Id, string schedId)
        //{           
        //    var testvm = _lookupRepository.GetExaminationByBranchClassId(Id, schedId);
        //    return Json(testvm);
        //}
        ////[HttpPost]
        ////public async Task<IActionResult> GetExamSubjectStudentByBranchClassId(string Id, string schedId)
        ////{
        ////    //var testvm = _lookupRepository.GetExamSubjectStudentByBranchClassId(Id, schedId);
        ////    //return Json(testvm);
        ////    SpExamResult examresult = new SpExamResult();
        ////    var result = _lookupRepository.GetSPExamResult(Id, schedId);
        ////    examresult.PreResult = result.Item1;
        ////    examresult.PostResult = result.Item2;
        ////    return Json(examresult);
        ////   // return Json(result);
        ////}

        //[HttpPost]
        //public async Task<JsonResult> SaveTimeTable(string model)
        //{
        //    //var objSession = HttpContext.Session.GetObjectFromJson<VUserInfo>("UserInfo");
        //    //var result = _lookupRepository.SaveSubject(model, objSession.UserId);
        //    var objModel = JsonConvert.DeserializeObject<TimeTableSubjectVM>(model);
        //    var result = _lookupRepository.SaveScheduleTimeTable(objModel, 1);
        //    return Json(result);
        //}

        ////[HttpPost]
        ////public async Task<JsonResult> SaveStudentResult(string model)
        ////{
        ////    //var objSession = HttpContext.Session.GetObjectFromJson<VUserInfo>("UserInfo");
        ////    //var result = _lookupRepository.SaveSubject(model, objSession.UserId);
        ////    var objModel = JsonConvert.DeserializeObject<StudentResultVM>(model);
        ////    var result = _lookupRepository.SaveStudentResult(objModel, 1);
        ////    return Json(result);
        ////}
        //[HttpPost]
        //public async Task<JsonResult> SaveQuestionEditor(string model)
        //{
        //    //var objSession = HttpContext.Session.GetObjectFromJson<VUserInfo>("UserInfo");
        //    //var result = _lookupRepository.SaveSubject(model, objSession.UserId);            
        //    var result = _lookupRepository.SaveQuestionEditor(model, 1);
        //    return Json(result);
        //}
        //public async Task<JsonResult> GetEditorContent(int Id)
        //{
        //    //var objSession = HttpContext.Session.GetObjectFromJson<VUserInfo>("UserInfo");
        //    //var result = _lookupRepository.SaveSubject(model, objSession.UserId);            
        //    var result = _lookupRepository.GetEditorContent(Id);          
        //    return Json(result);
        //}

        //public async Task<IActionResult> ViewTimeTable(int Id)
        //{
        //    var vm = _lookupRepository.ViewTimeTable(Id);
        //    return PartialView("ViewTimeTable", vm);
        //}
        //public async Task<IActionResult> GetExamResultTable(int Id)
        //{
        //    var vm = _lookupRepository.GetExamResultTable(Id);
        //    return PartialView("AddEditResultTable", vm);
        //}    




        //public async Task<IActionResult> GetAutoSchedule(int Id)
        //{
        //    var vm = _lookupRepository.GetAutoScheduleVM(Id);
        //    return PartialView("AddEditAutoSchedule", vm);
        //}

        //public async Task<IActionResult> getQuestionEditor(int Id)
        //{            
        //    return PartialView("QuestionEditor");
        //}
        //[HttpPost]
        //public async Task<IActionResult> GetAutoScheduleClass(string Id)
        //{
        //    AutoScheduleClass obj = new AutoScheduleClass();
        //    var result = _lookupRepository.GetAutoScheduleClass(Id);
        //    obj.StrClass = result;            
        //    return Json(obj);
        //}
        //public async Task<IActionResult> GetScheduleStudentSection(int Id)
        //{
        //    var vm = _lookupRepository.GetScheduleStudentSection(Id);
        //    return PartialView("AddEditScheduleStudentSection", vm);
        //}

        ////[HttpPost]
        ////public async Task<IActionResult> ProcessScheduleInput(string model)
        ////{
        ////    var objModel = JsonConvert.DeserializeObject<ScheduleInput>(model);
        ////    var result = _lookupRepository.ProcessScheduleInput(objModel);            
        ////    return Json(result);
        ////}
        ////[HttpPost]
        ////public async Task<IActionResult> SaveScheduleStudentSection(string model)
        ////{
        ////    var objModel = JsonConvert.DeserializeObject<ScheduleStudentSectionDataVM>(model);
        ////    var result = _lookupRepository.SaveScheduleStudentSection(objModel);
        ////    return Json(result);
        ////}


        //public async Task<IActionResult> GetScheduleTeacherSection(int Id)
        //{
        //    var vm = _lookupRepository.GetScheduleTeacherSection(Id);
        //    return PartialView("AddEditScheduleTeacherSection", vm);
        //}


        ////[HttpPost]
        ////public async Task<IActionResult> ProcessScheduleTeacher(int Id)
        ////{
        ////    //var objModel = JsonConvert.DeserializeObject<ScheduleInput>(model);
        ////    var result = _lookupRepository.ProcessScheduleTeacher(Id);
        ////    return Json(result);
        ////}

    }
}
