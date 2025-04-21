using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Net;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class ScheduleSyllabusService : IScheduleSyllabusService
    {
        private readonly APIContext _context;
        private readonly string _connectionString;
        private readonly HostEnvironmentService _hostingEnvironment;
        private readonly S3Service _s3Service;
        public ScheduleSyllabusService(APIContext context, IConfiguration configuration, HostEnvironmentService hostingEnvironment, S3Service s3Service)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _hostingEnvironment = hostingEnvironment;
            _s3Service = s3Service;

        }

        public async Task<bool> DeleteExamination(int Id, int UserId)
        {
            var cat = _context.Examination.SingleOrDefault(m => m.Id == Id);
            if (cat == null) return false;

            cat.IsActive = ((int)ActiveState.InActive).ToString();
            cat.LastModifiedByUserId = UserId;
            cat.LastModifiedDate = DateTime.Now;
            return _context.SaveChanges() > 0;
        }

        public async Task<List<VExaminations>> GetAllExamination()
        {
            return _context.VExaminations.AsNoTracking().OrderBy(x => x.Id).ToList();
        }

        public async Task<List<VScheduleSyllabus>> GetAllScheduleSyllabus(int BranchId)
        {
            try
            {
                return _context.VScheduleSyllabus.AsNoTracking().Where(x=>x.BranchId==BranchId).OrderBy(x => x.Id).ToList();
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> GetBranchClassQuestionStatus(string scheduleId, string branchClassId)
        {
            if (string.IsNullOrEmpty(scheduleId) || string.IsNullOrEmpty(branchClassId))
                return string.Empty;

            int schId = int.Parse(scheduleId);
            int brclsId = int.Parse(branchClassId);
            var objsyllabus = _context.VScheduleSyllabus.AsNoTracking().SingleOrDefault(x => x.Id == schId);
            if (objsyllabus != null && objsyllabus.QPaperCount == objsyllabus.SubjectCount)
            {
                var objsch = _context.Schedule.SingleOrDefault(x => x.Id == schId);
                if (objsch != null)
                {
                    objsch.StatusId = (int)ScheduleStatus.Syllabus;
                    _context.SaveChanges();
                }
            }
            var objbrcls = _context.VScheduleBranchClasses.AsNoTracking().SingleOrDefault(x => x.ScheduleId == schId && x.BranchClassId == brclsId);
            return objbrcls?.QPaperStatusColor ?? string.Empty;
        }

        public async Task<Examination> GetExamination(int Id)
        {
            if (Id <= 0) return new Examination();

            var cat = _context.Examination.AsNoTracking().SingleOrDefault(x => x.Id == Id && x.IsActive == ((int)ActiveState.Active).ToString());
            if (cat == null) return new Examination();

            return new Examination
            {
                Id = cat.Id,
                BranchClassId = cat.BranchClassId,
                ClassSubjectId = cat.ClassSubjectId
            };
        }

        public async Task<List<VExaminations>> GetExaminationByScheduleId(int scheduleId)
        {
            return _context.VExaminations.AsNoTracking().Where(x => x.ScheduleId == scheduleId).OrderBy(x => x.Id).ToList();
        }

        public async Task<ExaminationPaper> GetExaminationPaper(int Id)
        {
            if (Id <= 0) return new ExaminationPaper();

            var cat = _context.ExaminationPaper.AsNoTracking().SingleOrDefault(x => x.ExamId == Id && x.IsActive == ((int)ActiveState.Active).ToString());
            if (cat == null) return new ExaminationPaper();

            return new ExaminationPaper
            {
                Id = cat.Id,
                ExamId = cat.ExamId,
                DeltaQuestion = WebUtility.HtmlDecode(cat.DeltaQuestion),
                Question = cat.Question
            };
        }

        public async Task<ExamQuestionVM> GetExamQuestion(int Id)
        {
            var objExam = _context.VExaminations.AsNoTracking().SingleOrDefault(x => x.Id == Id);
            if (objExam == null) return null;

            var lsttopic = await _context.ExaminationLesson.AsNoTracking().Where(x => x.ExamId == Id).ToListAsync();
            if (lsttopic.Count == 0) return null;

            var lstalgo = await SetExamQuestionAlgo();
            var lsttopicids = lsttopic.Select(x => x.LessonId).ToList();
            var lstParam = await _context.VQuestionBanks.AsNoTracking().Where(x => lsttopicids.Contains((int)x.LessonId)).ToListAsync();
            var lstfilterParam = new List<VQuestionBanks>();

            if (lstParam.Count > 0)
            {
                lstParam.ForEach(x => x.QuestionColor = "#FFFFFF");
                var lstcatparam = await _context.ExaminationQuestion.AsNoTracking().Where(x => x.ExamId == Id).ToListAsync();
                if (lstcatparam.Count > 0)
                {
                    foreach (var item in lstParam)
                    {
                        foreach (var catparam in lstcatparam)
                        {
                            if (item.Id == catparam.QuestionBankId)
                            {
                                item.QuestionColor = "#D3D3D3";
                                lstfilterParam.Add(item);
                            }
                        }
                    }
                    var paramIds = lstcatparam.Select(y => y.QuestionBankId).ToList();
                    lstfilterParam = lstfilterParam.OrderBy(d => paramIds.IndexOf((int)d.Id)).ToList();
                }
            }
            return new ExamQuestionVM { LstQuestionBank = lstParam, Examination = objExam, LstFilterQuestionBank = lstfilterParam, LstExamQuestionAlgo = lstalgo };
        }
        public async Task<List<ExamQuestionAlgo>> SetExamQuestionAlgo()
        {
            List<ExamQuestionAlgo> result = new List<ExamQuestionAlgo>();
            var lstquestype = await _context.QuestionType.AsNoTracking().Where(x => x.IsActive == ((int)ActiveState.Active).ToString()).ToListAsync();

            var lstdiff = await _context.QuestionDifficulty.AsNoTracking().Where(x => x.IsActive == ((int)ActiveState.Active).ToString()).ToListAsync();
            var lstdiffItems = from x in lstdiff select new SelectListItem { Text = x.Name, Value = x.Id.ToString() };

            var lstcat = await _context.QuestionCategory.AsNoTracking().Where(x => x.IsActive == ((int)ActiveState.Active).ToString()).ToListAsync();
            var lstcatItems = from x in lstcat select new SelectListItem { Text = x.Name, Value = x.Id.ToString() };

            foreach (var item in lstquestype)
            {
                ExamQuestionAlgo algo = new ExamQuestionAlgo();
                algo.QuestionTypeId = item.Id;
                algo.QuestionTypeName = item.Name;
                algo.QuestionDifficultySheet = new List<SelectListItem>(lstdiffItems);
                algo.QuestionCategorySheet = new List<SelectListItem>(lstcatItems);
                algo.IsSelected = false;
                result.Add(algo);
            }
            return result;
        }

        public async Task<List<ExamSubject>> GetExamSubjectByBranchClass(string scheduleId, string branchclassId)
        {
            List<ExamSubject> lstresult = null;
            try
            {
                if (!string.IsNullOrEmpty(scheduleId) && !string.IsNullOrEmpty(branchclassId))
                {
                    int schId = int.Parse(scheduleId);
                    int brclsid = int.Parse(branchclassId);
                    var lstexam = await _context.VExaminations.AsNoTracking().Where(x => x.ScheduleId == schId && x.BranchClassId == brclsid).ToListAsync();
                    var lstexamItems = from x in lstexam select new ExamSubject { ExamId = (int)x.Id, SubjectName = x.SubjectName, QPaperStatusColor = x.QPaperStatusColor };
                    lstresult = new List<ExamSubject>(lstexamItems);
                }
            }
            catch (Exception ex)
            {

            }

            return lstresult;
        }

        public async Task<LessonVM> GetExamTopic(string examId)
        {
            LessonVM result = null;
            if (!string.IsNullOrEmpty(examId))
            {
                int exId = int.Parse(examId);
                var objexam = await _context.Examination.AsNoTracking().SingleOrDefaultAsync(x => x.Id == exId);
                if (objexam != null)
                {
                    var lstlesson = await _context.Lesson.AsNoTracking().Where(x => x.ClassSubjectId == objexam.ClassSubjectId && x.IsActive == ((int)ActiveState.Active).ToString()).OrderByDescending(x => x.Id).ToListAsync();
                    var lstcastlesson = (from x in lstlesson
                                         select new CastLesson
                                         {
                                             Id = x.Id,
                                             Name = x.Name,
                                             IsSelected = false
                                         }).ToList();
                    if (lstcastlesson != null && lstcastlesson.Count > 0)
                    {
                        var lstexamlesson = await _context.ExaminationLesson.AsNoTracking().Where(x => x.ExamId == objexam.Id).ToListAsync();
                        if (lstexamlesson != null && lstexamlesson.Count > 0)
                        {
                            foreach (var item in lstcastlesson)
                            {
                                foreach (var exam in lstexamlesson)
                                {
                                    if (item.Id == exam.LessonId)
                                    {
                                        item.IsSelected = true;
                                    }
                                }
                            }
                        }
                        result = new LessonVM { ExamId = objexam.Id, LstLesson = new List<CastLesson>(lstcastlesson) };
                    }
                }

            }
            return result;
        }

        public async Task<Tuple<byte[], string>> GetFileBytes(int Id, string rootpath)
        {
            Tuple<byte[], string> result = null;
            var objexam = await _context.Examination.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id && m.IsActive == ((int)ActiveState.Active).ToString());
            if (objexam != null && !string.IsNullOrEmpty(objexam.FilePath))
            {
                string filePath = Path.Combine(rootpath, objexam.FilePath);
                var bytes = System.IO.File.ReadAllBytes(filePath);
                result = new Tuple<byte[], string>(item1: bytes, item2: objexam.FileName);
            }
            return result;
        }

        public async Task<QuestionPaper> GetQuestionPaper(int examId)
        {
            try
            {
                var objexam = await _context.VExaminations.AsNoTracking().SingleOrDefaultAsync(x => x.Id == examId);
                if (objexam == null) return null;

                var objTime = await _context.ExamTime.AsNoTracking().SingleOrDefaultAsync(x => x.Id == objexam.ExamTimeId);
                if (objTime == null) return null;

                TimeSpan duration = DateTime.Parse(objTime.ToTime).Subtract(DateTime.Parse(objTime.FromTime));

                List<VExaminationQuestions> lstexamquest = null;
                ExaminationPaper paper = null;
                ViewAttach attach = null;

                switch (objexam.QPaperType)
                {
                    case var type when type == ((int)QPaperType.AutoChoose).ToString():
                        lstexamquest = await _context.VExaminationQuestions.AsNoTracking().Where(x => x.ExamId == objexam.Id).ToListAsync();
                        lstexamquest.ForEach(item => item.DeltaQuestion = WebUtility.HtmlDecode(item.DeltaQuestion));
                        break;
                    case var type when type == ((int)QPaperType.Paper).ToString():
                        paper = await _context.ExaminationPaper.AsNoTracking().SingleOrDefaultAsync(x => x.ExamId == objexam.Id);
                        if (paper != null)
                        {
                            paper.DeltaQuestion = WebUtility.HtmlDecode(paper.DeltaQuestion);
                        }
                        break;
                    case var type when type == ((int)QPaperType.Upload).ToString():
                        if (!string.IsNullOrEmpty(objexam.FilePath))
                        {
                            attach = new ViewAttach { url = objexam.FilePath };
                        }
                        break;
                }

                return new QuestionPaper
                {
                    Id = (int)objexam.Id,
                    QPaperType = objexam.QPaperType,
                    BranchName = objexam.BranchName,
                    ExamTypeName = objexam.ExamTypeName,
                    ScheduleMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(objexam.ExamDate.Value.Month),
                    ClassName = objexam.ClassName,
                    SubjectName = objexam.SubjectName,
                    ExamDate = objexam.StrExamDate,
                    ExamTime = duration.ToString(@"hh\:mm") + " hrs",
                    Marks = objexam.Marks,
                    LstExaminationQuestion = objexam.QPaperType == ((int)QPaperType.AutoChoose).ToString() ? lstexamquest : new List<VExaminationQuestions>(),
                    ExaminationPaper = paper,
                    Attach = attach
                };
            }
            catch
            {
                return null;
            }
        }
        public async Task<VSchedules> GetScheduleSyllabus(int Id)
        {
            var objSched = _context.VSchedules.AsNoTracking().SingleOrDefault(x => x.Id == Id);
            if (objSched != null)
            {
                objSched.LstScheduleBranchClass = await _context.VScheduleBranchClasses.AsNoTracking().Where(x => x.ScheduleId == Id).ToListAsync();
            }
            return objSched;
        }
        public async Task<ScheduleSyllabusDetail> GetScheduleSyllabusDetail(int Id, int BranchId)
        {
            var objsch = await _context.VSchedules.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
            if (objsch == null) return null;

            var rpt = await _context.ReportSettings.AsNoTracking().FirstOrDefaultAsync(x=>x.BranchId==BranchId);

            return new ScheduleSyllabusDetail
            {
                Id = (int)objsch.Id,
                StartDate = objsch.StrStartDate,
                EndDate = objsch.StrEndDate,
                Title = objsch.Title,
                ExamType = objsch.ExamTypeName,
                Branch = objsch.BranchName,
                ReportSettings = rpt
            };
        }
        public async Task<string> GetSPSyllabusTopic(int scheduleId)
        {
            try
            {
                DataTable dt = new DataTable();
                var rawData = _context.V_SP_ScheduleTopic
                 .AsNoTracking()
          .Where(x => x.ScheduleId == scheduleId)
         .Select(e => new
         {
             e.ScheduleId,
             e.ClassName,
             e.LessonName,
             e.SubjectName
         })
         .ToList();

                // Step 2: Get all unique StrExamDate values for the dynamic columns
                var subjects = rawData.Select(x => x.SubjectName).Distinct().ToList();

                // Step 3: Group data by ScheduleId and ClassName
                var pivotedData = rawData
                    .GroupBy(x => new { x.ScheduleId, x.ClassName })
                    .Select(g => {
                        var pivot = new ExpandoObject() as IDictionary<string, Object>;
                        pivot["ScheduleId"] = g.Key.ScheduleId;
                        pivot["ClassName"] = g.Key.ClassName;

                        // Add StrExamDate columns dynamically
                        foreach (var subject in subjects)
                        {
                            // Get the SubjectName for the current StrExamDate (if exists)
                            var lesson = g.FirstOrDefault(x => x.SubjectName == subject);
                            pivot[subject] = lesson?.LessonName; // Add Lesson or null if not available
                        }

                        return pivot;
                    })
                    .ToList();

                // Step 4: Convert pivoted data to DataTable
                dt = ConvertToDataTable(pivotedData);

                // Uncomment and implement the following code if using MySQL
                // using (MySqlConnection conn = new MySqlConnection(_connectionString))
                // {
                //     MySqlCommand sqlComm = new MySqlCommand("SP_ScheduleTopic", conn);
                //     sqlComm.Parameters.AddWithValue("@ScheduleId", scheduleId);
                //     sqlComm.CommandType = CommandType.StoredProcedure;
                //     MySqlDataAdapter da = new MySqlDataAdapter { SelectCommand = sqlComm };
                //     da.Fill(dt);
                // }
                return DataTableToJSON(dt);
            }
            catch
            {
                return null;
            }
        }
        public async Task<List<VQuestionBanks>> ProcessQuestionAlgo(ExamQuestionVM model)
        {
            if (model == null) return new List<VQuestionBanks>();

            var lsttopic = await _context.ExaminationLesson.AsNoTracking().Where(x => x.ExamId == model.Examination.Id).ToListAsync();
            if (lsttopic.Count == 0) return new List<VQuestionBanks>();

            var lsttopicids = lsttopic.Select(x => x.LessonId).ToList();
            var lstParam = await _context.VQuestionBanks.AsNoTracking().Where(x => lsttopicids.Contains((int)x.LessonId)).ToListAsync();

            var lstAlgo = model.LstExamQuestionAlgo;
            var random = new Random();
            var vm = new List<VQuestionBanks>();

            foreach (var item in lstAlgo.Where(item => item.IsSelected))
            {
                var lstres = lstParam.Where(x => x.QuestionTypeId == item.QuestionTypeId && x.QuestionDifficultyId == item.QuestionDifficultyId
                                                 && x.QuestionCategoryId == item.QuestionCategoryId && x.Marks == item.Marks).ToList();
                if (lstres.Count > 0)
                {
                    for (int i = 0; i < item.QuestionCount; i++)
                    {
                        var ranObj = lstres[random.Next(lstres.Count)];
                        vm.Add(ranObj);
                    }
                }
            }

            return vm;
        }
        public async Task<int> SaveExamination(Examination model, int UserId)
        {
            if (model == null) return 0;

            Examination cat;
            if (model.Id > 0)
            {
                // Update
                cat = await _context.Examination.SingleOrDefaultAsync(m => m.Id == model.Id && m.IsActive == ((int)ActiveState.Active).ToString());
                if (cat == null) return 0;

                cat.BranchClassId = model.BranchClassId;
                cat.ClassSubjectId = model.ClassSubjectId;
                cat.LastModifiedDate = DateTime.Now;
                cat.LastModifiedByUserId = UserId;
            }
            else
            {
                // Add Examination
                cat = new Examination
                {
                    BranchClassId = model.BranchClassId,
                    ClassSubjectId = model.ClassSubjectId,
                    ExamDate = model.ExamDate,
                    IsActive = ((int)ActiveState.Active).ToString(),
                    CreatedDate = DateTime.Now,
                    CreatedByUserId = UserId
                };
                _context.Examination.Add(cat);
            }
            return await _context.SaveChangesAsync();
        }
        public async Task<string> SaveExaminationPaper(ExaminationPaper model, int UserId)
        {
            if (model == null) return string.Empty;

            ExaminationPaper cat;
            if (model.Id > 0)
            {
                // Update
                cat = await _context.ExaminationPaper.SingleOrDefaultAsync(m => m.Id == model.Id && m.IsActive == ((int)ActiveState.Active).ToString());
                if (cat == null) return string.Empty;

                cat.ExamId = model.ExamId;
                cat.DeltaQuestion = WebUtility.HtmlEncode(model.DeltaQuestion);
                cat.Question = model.Question;
                cat.LastModifiedDate = DateTime.Now;
                cat.LastModifiedByUserId = UserId;
            }
            else
            {
                // Add
                cat = new ExaminationPaper
                {
                    ExamId = model.ExamId,
                    DeltaQuestion = WebUtility.HtmlEncode(model.DeltaQuestion),
                    Question = model.Question,
                    IsActive = ((int)ActiveState.Active).ToString(),
                    CreatedDate = DateTime.Now,
                    CreatedByUserId = UserId
                };
                _context.ExaminationPaper.Add(cat);
            }

            if (await _context.SaveChangesAsync() > 0)
            {
                var objexam = await _context.VExaminations.AsNoTracking().SingleOrDefaultAsync(x => x.Id == model.ExamId);
                return objexam?.QPaperStatusColor ?? string.Empty;
            }
            return string.Empty;
        }
        public async Task<string> UpdateExamQuestion(ExamQuestionVM model, int UserId)
        {
            if (model == null) return string.Empty;

            var lstcatparam = await _context.ExaminationQuestion.AsNoTracking().Where(x => x.ExamId == model.Examination.Id).ToListAsync();
            _context.ExaminationQuestion.RemoveRange(lstcatparam);

            var newQuestions = model.LstFilterQuestionBank.Select(item1 => new ExaminationQuestion
            {
                ExamId = (int)model.Examination.Id,
                QuestionBankId = (int)item1.Id,
                CreatedDate = DateTime.Now,
                CreatedByUserId = UserId
            }).ToList();

            _context.ExaminationQuestion.AddRange(newQuestions);

            if (await _context.SaveChangesAsync() > 0)
            {
                var objExam = await _context.Examination.SingleOrDefaultAsync(x => x.IsActive == ((int)ActiveState.Active).ToString() && x.Id == model.Examination.Id);
                if (objExam != null)
                {
                    objExam.QPaperStatusId = ((int)ActiveState.Active).ToString();
                    objExam.QPaperType = ((int)QPaperType.AutoChoose).ToString();
                    objExam.LastModifiedByUserId = UserId;
                    objExam.LastModifiedDate = DateTime.Now;

                    if (await _context.SaveChangesAsync() > 0)
                    {
                        var objvexam = await _context.VExaminations.AsNoTracking().SingleOrDefaultAsync(x => x.Id == objExam.Id);
                        return objvexam?.QPaperStatusColor ?? string.Empty;
                    }
                }
            }
            return string.Empty;
        }
        public async Task<int> UpdateExamTopic(LessonVM model)
        {
            if (model == null) return 0;

            var lstbrnclass = await _context.ExaminationLesson.Where(x => x.ExamId == model.ExamId).ToListAsync();
            _context.ExaminationLesson.RemoveRange(lstbrnclass);

            var newTopics = model.LstLesson.Where(x => x.IsSelected).Select(item1 => new ExaminationLesson
            {
                ExamId = model.ExamId,
                SessionYearId = model.SessionYearId,
                LessonId = item1.Id
            }).ToList();

            _context.ExaminationLesson.AddRange(newTopics);
            return await _context.SaveChangesAsync();
        }
        public async Task<string> UploadQPaper(UploadExamQPaper model, int UserId)
        {
            if (model == null || model.ExamId <= 0 || model.File == null) return null;

            var objExam = await _context.Examination.SingleOrDefaultAsync(m => m.Id == model.ExamId && m.IsActive == ((int)ActiveState.Active).ToString());
            if (objExam == null) return null;

            //if (!string.IsNullOrEmpty(objExam.FilePath))
            //{
            //    string filePath = _hostingEnvironment.GetFullUrlFromDbPath(objExam.FilePath);
            //    if (!objExam.FilePath.Contains("noimage.png") && File.Exists(filePath))
            //    {
            //        File.Delete(filePath);
            //    }
            //}
            if (!string.IsNullOrEmpty(objExam.FilePath) && !objExam.FilePath.Contains("noimage.png"))
            {
                await _s3Service.DeleteFileAsync(objExam.FilePath);
            }

            objExam.FileName = model.File.FileName;
            // objExam.FilePath = Shared.ProcessUploadFile((int)UploadType.QuestionPaper, model.File, _hostingEnvironment.GetWebRootPath());
            objExam.FilePath = await _s3Service.UploadFileAsync((int)UploadType.QuestionPaper, model.File);

            objExam.QPaperStatusId = ((int)ActiveState.Active).ToString();
            objExam.QPaperType = ((int)QPaperType.Upload).ToString();
            objExam.LastModifiedDate = DateTime.Now;
            objExam.LastModifiedByUserId = UserId;

            if (await _context.SaveChangesAsync() > 0)
            {
                var objvexam = await _context.VExaminations.AsNoTracking().SingleOrDefaultAsync(x => x.Id == objExam.Id);
                if (objvexam != null && !string.IsNullOrEmpty(objExam.FilePath))
                {
                    int pos = objExam.FilePath.LastIndexOf("\\") + 1;
                    return $"{objvexam.QPaperStatusColor},{objExam.FileName},{objExam.FilePath.Substring(pos)}";
                }
            }
            return null;
        }
        public async Task<VExaminations> ViewExamination(int Id)
        {
            return await _context.VExaminations.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
        }
        public async Task<ViewAttach> ViewExamPaper(int ExamId)
        {
            var objExam = await _context.Examination.AsNoTracking().SingleOrDefaultAsync(x => x.Id == ExamId);
            if (objExam != null && !string.IsNullOrEmpty(objExam.FilePath))
            {
                int pos = objExam.FilePath.LastIndexOf("\\") + 1;
                return new ViewAttach { url = objExam.FilePath.Substring(pos) };
            }
            return null;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}