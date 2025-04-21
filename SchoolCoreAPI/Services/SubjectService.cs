using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly APIContext _context;
        public SubjectService(APIContext context)
        {
            _context = context;
        }

        public async Task<List<VSubjects>> GetAllSubject()
        {
            return await _context.VSubjects.AsNoTracking().OrderByDescending(x => x.Id).ToListAsync();
        }

        public async Task<VSubjects> ViewSubject(int Id)
        {
            return await  _context.VSubjects.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
        }

        public async Task<bool> IsRecordExists(string name, int MediumId, int Id)
        {
            return Id == 0
                ? await _context.Subject.AnyAsync(e => e.Name == name && e.MediumId == MediumId && e.IsActive == ((int)ActiveState.Active).ToString())
                : await _context.Subject.AnyAsync(e => e.Name == name && e.MediumId == MediumId && e.Id != Id && e.IsActive == ((int)ActiveState.Active).ToString());
        }

        public async Task<Subject> GetSubject(int Id)
        {
            //var model = new Subject
            //{
            //    MediumSheet = new List<SelectListItem>(GetMediums())
            //};
            var model = new Subject();
            var sheet = new List<SelectListItem>(GetMediums());
            model.MediumSheet = sheet;

            if (Id > 0)
            {
                var cat = await _context.Subject.AsNoTracking().SingleOrDefaultAsync(x => x.Id == Id && x.IsActive == ((int)ActiveState.Active).ToString());
                if (cat != null)
                {
                    model = cat;
                    model.MediumSheet = sheet;
                    var divselectedItem = model.MediumSheet.Find(p => p.Value == model.MediumId.ToString());
                    if (divselectedItem != null)
                    {
                        divselectedItem.Selected = true;
                    }
                }
            }
            else
            {
                model.SubjectType = ((int)SubjectType.Theory).ToString();
            }

            return model;
        }

        public async Task<ServiceResult> SaveSubject(Subject model, int UserId)
        {
            var cat = new Subject();
            ServiceResult result = null;
            if (model.Id > 0)
            {
                cat = await _context.Subject.SingleOrDefaultAsync(m => m.Id == model.Id && m.IsActive == ((int)ActiveState.Active).ToString());
                if (cat != null)
                {
                    cat.Name = model.Name;
                    cat.ShortName = model.ShortName;
                    cat.MediumId = model.MediumId;
                    cat.SubjectType = model.SubjectType;
                    cat.SubjectColor = model.SubjectColor;
                    cat.SubjectCode = model.SubjectCode;
                    cat.Description = model.Description;
                    cat.LastModifiedDate = DateTime.Now;
                    cat.LastModifiedByUserId = UserId;
                    _context.Entry(cat).State = EntityState.Modified;
                    if (await _context.SaveChangesAsync() > 0)
                    {
                        result = new ServiceResult
                        {
                            StatusId = (int)ServiceResultStatus.Updated,
                            RecordId = cat.Id
                        };
                    }
                }
            }
            else
            {
                cat = new Subject
                {
                    Name = model.Name,
                    ShortName = model.ShortName,
                    MediumId = model.MediumId,
                    SubjectType = model.SubjectType,
                    SubjectColor = model.SubjectColor,
                    SubjectCode = model.SubjectCode,
                    Description = model.Description,
                    IsActive = ((int)ActiveState.Active).ToString(),
                    CreatedDate = DateTime.Now,
                    CreatedByUserId = UserId,
                    LastModifiedDate = DateTime.Now,
                    LastModifiedByUserId = UserId
                };
                _context.Subject.Add(cat);
                if (await _context.SaveChangesAsync() > 0)
                {
                    result = new ServiceResult
                    {
                        StatusId = (int)ServiceResultStatus.Added,
                        RecordId = cat.Id
                    };
                }
            }
            return result;
        }

        public async Task<ServiceResult> DeleteSubject(int Id, int UserId)
        {
            var result = new ServiceResult();
            var cat = _context.Subject.SingleOrDefault(m => m.Id == Id);
            if (cat != null)
            {
                cat.IsActive = ((int)ActiveState.InActive).ToString();
                cat.LastModifiedByUserId = UserId;
                cat.LastModifiedDate = DateTime.Now;
                _context.Entry(cat).State = EntityState.Modified;
                if (await _context.SaveChangesAsync() > 0)
                {
                    result.StatusId = (int)ServiceResultStatus.Deleted;
                    result.RecordId = cat.Id;
                }
            }
            return result;
        }

        public List<SelectListItem> GetMediums()
        {
            return  _context.Medium
                .AsNoTracking()
                .Where(x => x.IsActive == ((int)ActiveState.Active).ToString())
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                .ToList();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
