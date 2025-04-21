using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class ClassService : IClassService, IDisposable
    {
        private readonly APIContext _context;

        public ClassService(APIContext context)
        {
            _context = context;
        }

        public async Task<List<VClasses>> GetAllClass()
        {
            return await _context.VClasses.AsNoTracking().OrderBy(x => x.Id).ToListAsync();
        }

        public async Task<VClasses> ViewClass(int Id)
        {
            return await _context.VClasses.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
        }

        public async Task<Class> GetClass(int Id)
        {
            var model = new Class
            {
                MediumSheet = new List<SelectListItem>(await GetMediums())
            };

            if (Id > 0)
            {
                var cat = await _context.Class.AsNoTracking().SingleOrDefaultAsync(x => x.Id == Id && x.IsActive == ((int)Shared.ActiveState.Active).ToString());
                if (cat != null)
                {
                    model.Id = cat.Id;
                    model.Name = cat.Name;
                    model.ShortName = cat.ShortName;
                    model.Description = cat.Description;
                    if (cat.MediumId > 0)
                    {
                        model.MediumId = cat.MediumId;
                        var divselectedItem = model.MediumSheet.Find(p => p.Value == model.MediumId.ToString());
                        if (divselectedItem != null)
                        {
                            divselectedItem.Selected = true;
                        }
                    }
                }
            }
            return model;
        }

        public async Task<bool> IsRecordExists(string name, int MediumId, int Id)
        {
            return Id == 0
                ? await _context.Class.AnyAsync(e => e.Name == name && e.MediumId == MediumId && e.IsActive == ((int)ActiveState.Active).ToString())
                : await _context.Class.AnyAsync(e => e.Name == name && e.MediumId == MediumId && e.Id != Id && e.IsActive == ((int)ActiveState.Active).ToString());
        }

        public async Task<ServiceResult> SaveClass(Class model, int UserId)
        {
            ServiceResult result = null;
            try
            {
                if (model.Id > 0)
                {
                    var cat = await _context.Class.SingleOrDefaultAsync(m => m.Id == model.Id && m.IsActive == ((int)Shared.ActiveState.Active).ToString());
                    if (cat != null)
                    {
                        cat.Name = model.Name;
                        cat.ShortName = model.ShortName;
                        cat.Description = model.Description;
                        cat.MediumId = model.MediumId;
                        cat.LastModifiedDate = DateTime.Now;
                        cat.LastModifiedByUserId = UserId;
                        _context.Entry(cat).State = EntityState.Modified;
                    }
                }
                else
                {
                    var cat = new Class
                    {
                        Name = model.Name,
                        ShortName = model.ShortName,
                        Description = model.Description,
                        MediumId = model.MediumId,
                        IsActive = ((int)Shared.ActiveState.Active).ToString(),
                        CreatedDate = DateTime.Now,
                        CreatedByUserId = UserId,
                        LastModifiedDate = DateTime.Now,
                        LastModifiedByUserId = UserId
                    };
                    _context.Class.Add(cat);
                }

                if (await _context.SaveChangesAsync() > 0)
                {
                    result = new ServiceResult
                    {
                        StatusId = model.Id > 0 ? (int)ServiceResultStatus.Updated : (int)ServiceResultStatus.Added,
                        RecordId = model.Id
                    };
                }
            }
            catch (Exception)
            {
                // Log exception
            }
            return result;
        }

        public async Task<ServiceResult> DeleteClass(int Id, int UserId)
        {
            ServiceResult result = null;
            var cat = await _context.Class.SingleOrDefaultAsync(m => m.Id == Id);
            if (cat != null)
            {
                cat.IsActive = ((int)Shared.ActiveState.InActive).ToString();
                cat.LastModifiedByUserId = UserId;
                cat.LastModifiedDate = DateTime.Now;
                if (await _context.SaveChangesAsync() > 0)
                {
                    result = new ServiceResult
                    {
                        StatusId = (int)ServiceResultStatus.Deleted,
                        RecordId = cat.Id
                    };
                }
            }
            return result;
        }

        public async Task<SubjectVM> GetClassSubject(int Id)
        {
            var lstsubj = await _context.VSubjects.AsNoTracking().OrderByDescending(x => x.Id).ToListAsync();
            var lstsubject = lstsubj.Select(c => new CastSubject
            {
                Id = (int)c.Id,
                IsDelete = ((int)Shared.ActiveState.Active).ToString(),
                IsSelected = false,
                Name = $"{c.Name} ( {c.MediumName} )",
                SubjectColor = c.SubjectColor
            }).ToList();

            if (Id > 0)
            {
                var lstclsSubj = await _context.VClassSubjects.AsNoTracking().Where(x => x.ClassId == Id).ToListAsync();
                foreach (var item in lstsubject)
                {
                    var clssub = lstclsSubj.FirstOrDefault(x => x.SubjectId == item.Id);
                    if (clssub != null)
                    {
                        item.IsSelected = true;
                        item.IsDelete = clssub.IsDelete;
                    }
                }
            }

            return new SubjectVM { LstSubject = lstsubject, ClassId = Id };
        }

        public async Task<ServiceResult> UpdateClassSubject(SubjectVM model, int UserId)
        {
            ServiceResult result = null;
            try
            {
                var lstvclsSubject = await _context.VClassSubjects.Where(x => x.ClassId == model.ClassId && x.IsDelete == ((int)Shared.ActiveState.Active).ToString()).ToListAsync();
                if (lstvclsSubject.Any())
                {
                    var lstvclsSubjectids = lstvclsSubject.Select(x => x.Id).ToList();
                    var lstclsSubject = await _context.ClassSubject.Where(x => lstvclsSubjectids.Contains(x.Id)).ToListAsync();
                    _context.ClassSubject.RemoveRange(lstclsSubject);
                }

                var lstchkcount = model.LstSubject.Where(x => x.IsSelected && x.IsDelete == ((int)Shared.ActiveState.Active).ToString()).ToList();
                if (lstchkcount.Any())
                {
                    var newClassSubjects = lstchkcount.Select(item1 => new ClassSubject
                    {
                        ClassId = model.ClassId.Value,
                        SubjectId = item1.Id,
                        CreatedByUserId = UserId,
                        CreatedDate = DateTime.Now
                    }).ToList();
                    await _context.ClassSubject.AddRangeAsync(newClassSubjects);
                }

                if (await _context.SaveChangesAsync() >= 0)
                {
                    result = new ServiceResult
                    {
                        StatusId = (int)ServiceResultStatus.Updated,
                        RecordId = model.ClassId.Value
                    };
                }
            }
            catch (Exception)
            {
                // Log exception
            }
            return result;
        }

        public async Task<List<SelectListItem>> GetMediums()
        {
            return await _context.Medium.AsNoTracking()
                .Where(x => x.IsActive == ((int)ActiveState.Active).ToString())
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                .ToListAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
