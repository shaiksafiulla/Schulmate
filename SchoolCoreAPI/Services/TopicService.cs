using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class TopicService : ITopicService
    {
        private readonly APIContext _context;
        public TopicService(APIContext context)
        {
            _context = context;
        }

        public async Task<List<VTopics>> GetAllTopic()
        {
            return await _context.VTopics.AsNoTracking().OrderBy(x => x.Id).ToListAsync();
        }

        public async Task<VTopics> ViewTopic(int Id)
        {
            return await _context.VTopics.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
        }

        public async Task<Topic> GetTopic(int Id)
        {
            var model = new Topic
            {
                ClassSheet = await GetClasses(),
                SubjectSheet = new List<SelectListItem>(),
                LessonSheet = new List<SelectListItem>()
            };

            if (Id <= 0) return model;

            var cat = await _context.Topic.AsNoTracking().SingleOrDefaultAsync(x => x.Id == Id && x.IsActive == ((int)ActiveState.Active).ToString());
            if (cat == null) return model;

            model.Id = cat.Id;
            model.Name = cat.Name;
            model.Description = cat.Description;

            if (cat.ClassSubjectId <= 0) return model;

            model.ClassSubjectId = cat.ClassSubjectId;
            var objclssubject = await _context.ClassSubject.AsNoTracking().SingleOrDefaultAsync(x => x.Id == cat.ClassSubjectId);
            if (objclssubject == null) return model;

            model.ClassId = objclssubject.ClassId;
            var divselectedItem = model.ClassSheet.Find(p => p.Value == objclssubject.ClassId.ToString());
            if (divselectedItem != null)
            {
                divselectedItem.Selected = true;
                model.SubjectId = objclssubject.SubjectId;

                var subids = await _context.ClassSubject.AsNoTracking().Where(x => x.ClassId == objclssubject.ClassId).Select(x => x.SubjectId).ToListAsync();
                var lst = await _context.Subject.AsNoTracking().Where(x => x.IsActive == ((int)ActiveState.Active).ToString() && subids.Contains(x.Id)).ToListAsync();
                model.SubjectSheet = lst.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();

                var catselectedItem = model.SubjectSheet.Find(p => p.Value == model.SubjectId.ToString());
                if (catselectedItem != null)
                {
                    catselectedItem.Selected = true;
                    model.LessonId = cat.LessonId;

                    var lstlesson = await _context.Lesson.AsNoTracking().Where(x => x.ClassSubjectId == cat.ClassSubjectId).ToListAsync();
                    model.LessonSheet = lstlesson.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();

                    var lessonselectedItem = model.LessonSheet.Find(p => p.Value == model.LessonId.ToString());
                    if (lessonselectedItem != null)
                    {
                        lessonselectedItem.Selected = true;
                    }
                }
            }

            return model;
        }

        public async Task<bool> IsRecordExists(int lessonId, string name, int Id)
        {
            return Id == 0
                ? await _context.Topic.AsNoTracking().AnyAsync(e => e.Name == name && e.LessonId == lessonId && e.IsActive == ((int)ActiveState.Active).ToString())
                : await _context.Topic.AsNoTracking().AnyAsync(e => e.Name == name && e.LessonId == lessonId && e.Id != Id && e.IsActive == ((int)ActiveState.Active).ToString());
        }

        public async Task<ServiceResult> SaveTopic(Topic model, int UserId)
        {
            var cat = new Topic();
            ServiceResult result = null;
            try
            {
                if (model == null) return result;

                if (model.ClassId > 0 && model.SubjectId > 0)
                {
                    var objclasssubj = await _context.ClassSubject.AsNoTracking().SingleOrDefaultAsync(x => x.ClassId == model.ClassId && x.SubjectId == model.SubjectId);
                    if (objclasssubj != null)
                    {
                        model.ClassSubjectId = objclasssubj.Id;
                    }
                }

                if (model.Id > 0)
                {
                    cat = await _context.Topic.SingleOrDefaultAsync(m => m.Id == model.Id && m.IsActive == ((int)ActiveState.Active).ToString());
                    if (cat == null) return result;

                    cat.Name = model.Name;
                    cat.ClassSubjectId = model.ClassSubjectId;
                    cat.LessonId = model.LessonId;
                    cat.Description = model.Description;
                    cat.LastModifiedDate = DateTime.Now;
                    cat.LastModifiedByUserId = UserId;
                    _context.Entry(cat).State = EntityState.Modified;
                }
                else
                {
                    cat = new Topic
                    {
                        Name = model.Name,
                        ClassSubjectId = model.ClassSubjectId,
                        LessonId = model.LessonId,
                        Description = model.Description,
                        IsActive = ((int)ActiveState.Active).ToString(),
                        CreatedDate = DateTime.Now,
                        CreatedByUserId = UserId
                    };
                    _context.Topic.Add(cat);
                    _context.Entry(cat).State = EntityState.Added;
                }

                var changes = await _context.SaveChangesAsync();
                if (changes != 0)
                {
                    result = new ServiceResult
                    {
                        StatusId = cat.Id > 0 ? (int)ServiceResultStatus.Updated : (int)ServiceResultStatus.Added,
                        RecordId = cat.Id
                    };
                }
            }
            catch (Exception)
            {
                // Log exception
            }

            return result;
        }

        public async Task<ServiceResult> DeleteTopic(int Id, int UserId)
        {
            var result = new ServiceResult();
            var cat = await _context.Topic.SingleOrDefaultAsync(m => m.Id == Id);
            if (cat == null) return result;

            cat.IsActive = ((int)ActiveState.InActive).ToString();
            cat.LastModifiedByUserId = UserId;
            cat.LastModifiedDate = DateTime.Now;
            _context.Entry(cat).State = EntityState.Modified;

            var changes = await _context.SaveChangesAsync();
            if (changes != 0)
            {
                result.StatusId = (int)ServiceResultStatus.Deleted;
                result.RecordId = cat.Id;
            }

            return result;
        }

        public async Task<List<SelectListItem>> GetLessons()
        {
            return await _context.Lesson.AsNoTracking()
                .Where(x => x.IsActive == ((int)Shared.ActiveState.Active).ToString())
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetSubjectsByClass(int classId)
        {
            var subids = await _context.ClassSubject.AsNoTracking().Where(x => x.ClassId == classId).Select(x => x.SubjectId).ToListAsync();
            return await _context.Subject.AsNoTracking()
                .Where(x => x.IsActive == ((int)Shared.ActiveState.Active).ToString() && subids.Contains(x.Id))
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetLessonsByClassSubject(int classId, int subjectId)
        {
            var objclssub = await _context.ClassSubject.AsNoTracking().SingleOrDefaultAsync(x => x.ClassId == classId && x.SubjectId == subjectId);
            if (objclssub == null) return new List<SelectListItem>();

            return await _context.Lesson.AsNoTracking()
                .Where(x => x.ClassSubjectId == objclssub.Id && x.IsActive == ((int)Shared.ActiveState.Active).ToString())
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetClasses()
        {
            return await _context.Class.AsNoTracking()
                .Where(x => x.IsActive == ((int)Shared.ActiveState.Active).ToString())
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                .ToListAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
