using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using System.Net;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class QuestionBankService : IQuestionBankService
    {
        private readonly APIContext _context;
        public QuestionBankService(APIContext context)
        {
            _context = context;
        }

        public List<VQuestionBanks> GetAllQuestionBank()
        {
            return _context.VQuestionBanks.AsNoTracking().OrderBy(x => x.Id).ToList();
        }

        public VQuestionBanks ViewQuestionBank(int Id)
        {
            return _context.VQuestionBanks.AsNoTracking().SingleOrDefault(m => m.Id == Id);
        }

        public QuestionBank GetQuestionBank(int Id)
        {
            var model = new QuestionBank
            {
                ClassSheet = new List<SelectListItem>(GetClasses()),
                SubjectSheet = new List<SelectListItem>(),
                LessonSheet = new List<SelectListItem>(),
                QuestionTypeSheet = new List<SelectListItem>(GetQuestionTypes()),
                QuestionDifficultySheet = new List<SelectListItem>(GetQuestionDifficulties()),
                QuestionCategorySheet = new List<SelectListItem>(GetQuestionCategories())
            };

            if (Id <= 0) return model;

            var cat = _context.QuestionBank.AsNoTracking().SingleOrDefault(x => x.Id == Id && x.IsActive == ((int)ActiveState.Active).ToString());
            if (cat == null) return model;

            model.Id = cat.Id;
            model.DeltaQuestion = WebUtility.HtmlDecode(cat.DeltaQuestion);
            model.Question = cat.Question;
            model.Marks = cat.Marks;
            model.Description = cat.Description;

            if (cat.LessonId > 0)
            {
                var objlesson = _context.Lesson.AsNoTracking().SingleOrDefault(x => x.Id == cat.LessonId);
                if (objlesson?.ClassSubjectId > 0)
                {
                    var objclssubject = _context.ClassSubject.AsNoTracking().SingleOrDefault(x => x.Id == objlesson.ClassSubjectId);
                    if (objclssubject != null)
                    {
                        model.ClassId = objclssubject.ClassId;
                        var divselectedItem = model.ClassSheet.Find(p => p.Value == objclssubject.ClassId.ToString());
                        if (divselectedItem != null)
                        {
                            divselectedItem.Selected = true;
                            model.SubjectId = objclssubject.SubjectId;

                            var lstsubj = _context.ClassSubject.AsNoTracking().Where(x => x.ClassId == objclssubject.ClassId).Select(x => x.SubjectId).ToList();
                            var lst = _context.Subject.AsNoTracking().Where(x => x.IsActive == ((int)ActiveState.Active).ToString() && lstsubj.Contains(x.Id)).ToList();
                            model.SubjectSheet = lst.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();

                            var catselectedItem = model.SubjectSheet.Find(p => p.Value == model.SubjectId.ToString());
                            if (catselectedItem != null)
                            {
                                catselectedItem.Selected = true;
                                model.LessonId = cat.LessonId;

                                var lsttopic = _context.Lesson.AsNoTracking().Where(x => x.ClassSubjectId == objclssubject.Id).ToList();
                                model.LessonSheet = lsttopic.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();

                                var divTopicItem = model.LessonSheet.Find(p => p.Value == cat.LessonId.ToString());
                                if (divTopicItem != null)
                                {
                                    divTopicItem.Selected = true;
                                }
                            }
                        }
                    }
                }
            }

            if (cat.QuestionTypeId > 0)
            {
                model.QuestionTypeId = cat.QuestionTypeId;
                var divquesttypeItem = model.QuestionTypeSheet.Find(p => p.Value == cat.QuestionTypeId.ToString());
                if (divquesttypeItem != null)
                {
                    divquesttypeItem.Selected = true;
                }
            }

            if (cat.QuestionDifficultyId > 0)
            {
                model.QuestionDifficultyId = cat.QuestionDifficultyId;
                var divdifficultItem = model.QuestionDifficultySheet.Find(p => p.Value == cat.QuestionDifficultyId.ToString());
                if (divdifficultItem != null)
                {
                    divdifficultItem.Selected = true;
                }
            }

            if (cat.QuestionCategoryId > 0)
            {
                model.QuestionCategoryId = cat.QuestionCategoryId;
                var divcategoryItem = model.QuestionCategorySheet.Find(p => p.Value == cat.QuestionCategoryId.ToString());
                if (divcategoryItem != null)
                {
                    divcategoryItem.Selected = true;
                }
            }

            return model;
        }

        public bool IsRecordExists(string name, int Id)
        {
            return Id == 0
                ? _context.QuestionBank.AsNoTracking().Any(e => e.Question == name && e.IsActive == ((int)ActiveState.Active).ToString())
                : _context.QuestionBank.AsNoTracking().Any(e => e.Question == name && e.Id != Id && e.IsActive == ((int)ActiveState.Active).ToString());
        }

        public ServiceResult SaveQuestionBank(QuestionBank model, int UserId)
        {
            var cat = new QuestionBank();
            ServiceResult result = null;
            try
            {
                if (model == null) return result;

                if (model.Id > 0)
                {
                    cat = _context.QuestionBank.SingleOrDefault(m => m.Id == model.Id && m.IsActive == ((int)ActiveState.Active).ToString());
                    if (cat == null) return result;

                    cat.LessonId = model.LessonId;
                    cat.QuestionTypeId = model.QuestionTypeId;
                    cat.QuestionDifficultyId = model.QuestionDifficultyId;
                    cat.QuestionCategoryId = model.QuestionCategoryId;
                    cat.DeltaQuestion = WebUtility.HtmlEncode(model.DeltaQuestion);
                    cat.Question = model.Question;
                    cat.Marks = model.Marks;
                    cat.Description = model.Description;
                    cat.LastModifiedDate = DateTime.Now;
                    cat.LastModifiedByUserId = UserId;
                    _context.Entry(cat).State = EntityState.Modified;
                    if (_context.SaveChanges() != 0)
                    {
                        result = new ServiceResult { StatusId = (int)ServiceResultStatus.Updated, RecordId = cat.Id };
                    }
                }
                else
                {
                    cat = new QuestionBank
                    {
                        LessonId = model.LessonId,
                        QuestionTypeId = model.QuestionTypeId,
                        QuestionDifficultyId = model.QuestionDifficultyId,
                        QuestionCategoryId = model.QuestionCategoryId,
                        DeltaQuestion = WebUtility.HtmlEncode(model.DeltaQuestion),
                        Question = model.Question,
                        Marks = model.Marks,
                        Description = model.Description,
                        IsActive = ((int)ActiveState.Active).ToString(),
                        CreatedDate = DateTime.Now,
                        CreatedByUserId = UserId
                    };
                    _context.QuestionBank.Add(cat);
                    _context.Entry(cat).State = EntityState.Added;
                    if (_context.SaveChanges() != 0)
                    {
                        result = new ServiceResult { StatusId = (int)ServiceResultStatus.Added, RecordId = cat.Id };
                    }
                }
            }
            catch (Exception)
            {
                // Log exception
            }

            return result;
        }

        public ServiceResult DeleteQuestionBank(int Id, int UserId)
        {
            var result = new ServiceResult();
            var cat = _context.QuestionBank.SingleOrDefault(m => m.Id == Id);
            if (cat == null) return result;

            cat.IsActive = ((int)ActiveState.InActive).ToString();
            cat.LastModifiedByUserId = UserId;
            cat.LastModifiedDate = DateTime.Now;
            _context.Entry(cat).State = EntityState.Modified;
            if (_context.SaveChanges() != 0)
            {
                result.StatusId = (int)ServiceResultStatus.Deleted;
                result.RecordId = cat.Id;
            }
            return result;
        }

        public string GetEditorContent(int Id)
        {
            var obj = _context.Question.AsNoTracking().SingleOrDefault();
            return obj == null ? string.Empty : WebUtility.HtmlDecode(obj.Description);
        }

        public ServiceResult SaveQuestionEditor(string model, int UserId)
        {
            var result = new ServiceResult();
            try
            {
                if (model == null) return result;

                var obj = new Question { Description = WebUtility.HtmlEncode(model) };
                _context.Question.Add(obj);
                _context.Entry(obj).State = EntityState.Added;
                if (_context.SaveChanges() != 0)
                {
                    result.StatusId = (int)ServiceResultStatus.Added;
                    result.RecordId = obj.Id;
                }
            }
            catch (Exception)
            {
                // Log exception
            }
            return result;
        }

        public List<SelectListItem> GetSubjectsByClass(string classId)
        {
            if (string.IsNullOrEmpty(classId)) return new List<SelectListItem>();

            int clsId = int.Parse(classId);
            var subids = _context.ClassSubject.AsNoTracking().Where(x => x.ClassId == clsId).Select(x => x.SubjectId).ToList();
            var lst = _context.Subject.AsNoTracking().Where(x => x.IsActive == ((int)ActiveState.Active).ToString() && subids.Contains(x.Id)).ToList();
            return lst.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
        }

        public List<SelectListItem> GetTopicsBySubject(string classId, string subjectId)
        {
            if (string.IsNullOrEmpty(subjectId)) return new List<SelectListItem>();

            int subId = int.Parse(subjectId);
            int clsId = int.Parse(classId);
            var objcls = _context.ClassSubject.AsNoTracking().SingleOrDefault(x => x.SubjectId == subId && x.ClassId == clsId);
            if (objcls == null) return new List<SelectListItem>();

            var lst = _context.Lesson.AsNoTracking().Where(x => x.IsActive == ((int)ActiveState.Active).ToString() && x.ClassSubjectId == objcls.Id).ToList();
            return lst.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
        }

        public List<SelectListItem> GetClasses()
        {
            var lst = _context.Class.AsNoTracking().Where(x => x.IsActive == ((int)ActiveState.Active).ToString()).ToList();
            return lst.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
        }

        public List<SelectListItem> GetQuestionTypes()
        {
            var lst = _context.QuestionType.AsNoTracking().Where(x => x.IsActive == ((int)ActiveState.Active).ToString()).ToList();
            return lst.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
        }

        public List<SelectListItem> GetQuestionDifficulties()
        {
            var lst = _context.QuestionDifficulty.AsNoTracking().Where(x => x.IsActive == ((int)ActiveState.Active).ToString()).ToList();
            return lst.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
        }

        public List<SelectListItem> GetQuestionCategories()
        {
            var lst = _context.QuestionCategory.AsNoTracking().Where(x => x.IsActive == ((int)ActiveState.Active).ToString()).ToList();
            return lst.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
