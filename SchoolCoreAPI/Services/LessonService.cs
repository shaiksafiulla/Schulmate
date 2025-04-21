using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class LessonService : ILessonService
    {
        private readonly APIContext _context;
        public LessonService(APIContext context)
        {
            _context = context;
        }

        public async Task<List<VLessons>> GetAllLesson()
        {
            return await _context.VLessons.AsNoTracking().OrderBy(x => x.Id).ToListAsync();
        }

        public async Task<VLessons> ViewLesson(int Id)
        {
            return await _context.VLessons.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
        }

        public async Task<Lesson> GetLesson(int Id)
        {
            var model = new Lesson
            {
                ClassSheet = new List<SelectListItem>(await GetClasses()),
                SubjectSheet = new List<SelectListItem>()
            };

            if (Id <= 0)
            {
                model.ClassSubjectId = 0;
                return model;
            }

            var cat = await _context.Lesson.AsNoTracking().SingleOrDefaultAsync(x => x.Id == Id && x.IsActive == ((int)ActiveState.Active).ToString());
            if (cat == null) return model;

            model.Id = cat.Id;
            model.Name = cat.Name;
            model.Description = cat.Description;

            if (cat.ClassSubjectId <= 0) return model;

            model.ClassSubjectId = cat.ClassSubjectId;
            var objclssubject = _context.ClassSubject.AsNoTracking().SingleOrDefault(x => x.Id == cat.ClassSubjectId);
            if (objclssubject == null) return model;

            model.ClassId = objclssubject.ClassId;
            var divselectedItem = model.ClassSheet.Find(p => p.Value == objclssubject.ClassId.ToString());
            if (divselectedItem == null) return model;

            divselectedItem.Selected = true;
            model.SubjectId = objclssubject.SubjectId;

            var subids = await _context.ClassSubject.AsNoTracking().Where(x => x.ClassId == objclssubject.ClassId).Select(x => x.SubjectId).ToListAsync();
            var lst = await _context.Subject.AsNoTracking().Where(x => x.IsActive == ((int)ActiveState.Active).ToString() && subids.Contains(x.Id)).ToListAsync();
            model.SubjectSheet = lst.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();

            var catselectedItem = model.SubjectSheet.Find(p => p.Value == model.SubjectId.ToString());
            if (catselectedItem != null)
            {
                catselectedItem.Selected = true;
            }

            return model;
        }

        public async Task<bool> IsRecordExists(int classId, int subjectId, string name, int Id)
        {
            if (classId <= 0 || subjectId <= 0) return false;

            var clssubjid = await _context.ClassSubject.AsNoTracking().Where(x => x.ClassId == classId && x.SubjectId == subjectId).Select(x => x.Id).FirstOrDefaultAsync();
            if (clssubjid == 0) return false;

            return Id == 0
                ? await _context.Lesson.AnyAsync(e => e.Name == name && e.ClassSubjectId == clssubjid && e.IsActive == ((int)ActiveState.Active).ToString())
                : await _context.Lesson.AnyAsync(e => e.Name == name && e.ClassSubjectId == clssubjid && e.Id != Id && e.IsActive == ((int)ActiveState.Active).ToString());
        }

        public async Task<ServiceResult> SaveLesson(Lesson model, int UserId)
        {
            if (model == null) return null;
            var cat = new Lesson();
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
                cat = await _context.Lesson.AsNoTracking().SingleOrDefaultAsync(m => m.Id == model.Id && m.IsActive == ((int)ActiveState.Active).ToString());
                if (cat == null) return null;

                cat.Name = model.Name;
                cat.ClassSubjectId = model.ClassSubjectId;
                cat.Description = model.Description;
                cat.LastModifiedDate = DateTime.Now;
                cat.LastModifiedByUserId = UserId;
                _context.Entry(cat).State = EntityState.Modified;
            }
            else
            {
                cat = new Lesson
                {
                    Name = model.Name,
                    ClassSubjectId = model.ClassSubjectId,
                    Description = model.Description,
                    IsActive = ((int)ActiveState.Active).ToString(),
                    CreatedDate = DateTime.Now,
                    CreatedByUserId = UserId
                };
                _context.Lesson.Add(cat);
                _context.Entry(cat).State = EntityState.Added;
            }

            var changes = await _context.SaveChangesAsync();
            if (changes == 0) return null;

            return new ServiceResult
            {
                StatusId = cat.Id > 0 ? (int)ServiceResultStatus.Updated : (int)ServiceResultStatus.Added,
                RecordId = cat.Id
            };
        }

        public async Task<ServiceResult> DeleteLesson(int Id, int UserId)
        {
            var cat = await _context.Lesson.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
            if (cat == null) return null;

            cat.IsActive = ((int)ActiveState.InActive).ToString();
            cat.LastModifiedByUserId = UserId;
            cat.LastModifiedDate = DateTime.Now;
            _context.Entry(cat).State = EntityState.Modified;

            var changes = await _context.SaveChangesAsync();
            if (changes == 0) return null;

            return new ServiceResult
            {
                StatusId = (int)ServiceResultStatus.Deleted,
                RecordId = cat.Id
            };
        }

        public async Task<List<SelectListItem>> GetClasses()
        {
            return await _context.Class.AsNoTracking()
                .Where(x => x.IsActive == ((int)Shared.ActiveState.Active).ToString())
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetSubjectsByClass(string classId)
        {
            if (string.IsNullOrEmpty(classId)) return new List<SelectListItem>();

            int clsId = int.Parse(classId);
            var subids = await _context.ClassSubject.AsNoTracking().Where(x => x.ClassId == clsId).Select(x => x.SubjectId).ToListAsync();
            return await _context.Subject.AsNoTracking()
                .Where(x => x.IsActive == ((int)Shared.ActiveState.Active).ToString() && subids.Contains(x.Id))
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                .ToListAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
