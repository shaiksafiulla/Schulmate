using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class BranchClassService : IBranchClassService
    {
        private readonly APIContext _context;
        public BranchClassService(APIContext context)
        {
            _context = context;
        }

        public async Task<List<VBranchClasses>> GetAllBranchClass(int UserId)
        {
            var objuser = await _context.VUserInfo.AsNoTracking().SingleOrDefaultAsync(x => x.Id == UserId);
            if (objuser == null)
            {
                return new List<VBranchClasses>();
            }

            var query = _context.VBranchClasses.AsNoTracking();
            if (objuser.UserType != ((int)UserType.SuperAdmin).ToString())
            {
                query = query.Where(x => x.BranchId == objuser.BranchId);
            }

            return await query.ToListAsync();
        }

        public async Task<VBranchClasses> ViewBranchClass(int Id)
        {
            return await _context.VBranchClasses.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
        }

        public async Task<BranchClass> GetBranchClass(int Id)
        {
            var model = new BranchClass
            {
                ShiftSheet = new List<SelectListItem>(await GetShifts())
            };

            if (Id > 0)
            {
                var cat = await _context.BranchClass.AsNoTracking().SingleOrDefaultAsync(x => x.Id == Id);
                if (cat != null)
                {
                    model.Id = cat.Id;
                    model.BranchId = cat.BranchId;
                    model.ClassId = cat.ClassId;
                    model.SlotDuration = cat.SlotDuration;
                    if (cat.ShiftId > 0)
                    {
                        model.ShiftId = cat.ShiftId;
                        var divselectedItem2 = model.ShiftSheet.Find(p => p.Value == model.ShiftId.ToString());
                        if (divselectedItem2 != null)
                        {
                            divselectedItem2.Selected = true;
                        }
                    }
                }
            }

            return model;
        }

        public async Task<ServiceResult> SaveBranchClass(BranchClass model, int UserId)
        {
            ServiceResult result = null;
            try
            {
                if (model.Id > 0)
                {
                    var cat = await _context.BranchClass.SingleOrDefaultAsync(m => m.Id == model.Id);
                    if (cat != null)
                    {
                        cat.ShiftId = model.ShiftId;
                        cat.SlotDuration = model.SlotDuration;
                        _context.Entry(cat).State = EntityState.Modified;
                        var updateindex = await _context.SaveChangesAsync();
                        if (updateindex != 0)
                        {
                            result = new ServiceResult
                            {
                                StatusId = (int)ServiceResultStatus.Updated,
                                RecordId = cat.Id
                            };
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Log exception
            }
            return result;
        }

        public async Task<BranchClassVM> GetBranchClassVM(int BranchId, int BranchClassId, int ClassId)
        {
            BranchClassVM result = null;
            try
            {
                var lst = await _context.VTeacherCurrentLocation.Where(x => x.BranchId == BranchId).ToListAsync();
                var lsttea = lst.Select(c => new CastTeacher { TeacherId = c.TeacherId, IsSelected = false, TeacherName = c.FullName }).ToList();
                var lstbrclstea = await _context.BranchClassSectionTeacher.Where(x => x.BranchClassId == BranchClassId && x.IsActive == ((int)ActiveState.Active).ToString()).ToListAsync();

                foreach (var item in lsttea)
                {
                    if (lstbrclstea.Any(brclstea => item.TeacherId == brclstea.TeacherId))
                    {
                        item.IsSelected = true;
                    }
                }

                var lstsub = await _context.VClassSubjects.Where(x => x.ClassId == ClassId).ToListAsync();
                var teachers = await GetTeachers(BranchId);
                var lstclssub = lstsub.Select(c => new CastSubjectTeacher
                {
                    ClassSubjectId = (int)c.Id,
                    SubjectName = c.SubjectName,
                    SubjectColor = c.SubjectColor,
                    TeacherId = null,
                    TeacherSheet = new List<SelectListItem>(teachers)
                }).ToList();

                var lstbrclssubtea = await _context.BranchClassSectionSubjectTeacher.Where(x => x.BranchClassId == BranchClassId).ToListAsync();

                foreach (var item in lstclssub)
                {
                    var brclssubtea = lstbrclssubtea.FirstOrDefault(x => x.ClassSubjectId == item.ClassSubjectId);
                    if (brclssubtea != null && brclssubtea.TeacherId > 0)
                    {
                        var divselectedItem = item.TeacherSheet.Find(p => p.Value == brclssubtea.TeacherId.ToString());
                        if (divselectedItem != null)
                        {
                            divselectedItem.Selected = true;
                        }
                    }
                }

                result = new BranchClassVM { BranchClassId = BranchClassId, LstCastTeacher = lsttea, LstCastSubjectTeacher = lstclssub };
            }
            catch (Exception)
            {
                // Log exception
            }
            return result;
        }

        public async Task<PeriodBreakVM> GetPeriodBreak(int Id)
        {
            var lstbreak = await _context.VPeriodBreaks.OrderByDescending(x => x.Id).ToListAsync();
            var lstperiodbreak = lstbreak.Select(c => new CastPeriodBreak
            {
                Id = (int)c.Id,
                IsDelete = ((int)Shared.ActiveState.Active).ToString(),
                IsSelected = false,
                Name = $"{c.ShortName} ({c.FromTime} - {c.ToTime})",
                SubjectColor = c.BreakColor
            }).ToList();

            if (Id > 0)
            {
                var lstbrclsperbreak = await _context.VBranchClassPeriodBreak.Where(x => x.BranchClassId == Id).ToListAsync();
                foreach (var item in lstperiodbreak)
                {
                    var clssub = lstbrclsperbreak.FirstOrDefault(x => x.PeriodBreakId == item.Id);
                    if (clssub != null)
                    {
                        item.IsSelected = true;
                        item.IsDelete = clssub.IsDelete;
                    }
                }
            }

            var objbrcls = await _context.BranchClass.SingleOrDefaultAsync(x => x.Id == Id);
            return new PeriodBreakVM { LstPeriodBreak = lstperiodbreak, BranchId = objbrcls.BranchId, BranchClassId = objbrcls.Id, ClassId = objbrcls.ClassId };
        }

        public async Task<ServiceResult> UpdatePeriodBreak(PeriodBreakVM model, int UserId)
        {
            ServiceResult result = null;
            try
            {
                var lstvclsSubject = await _context.VBranchClassPeriodBreak.Where(x => x.BranchClassId == model.BranchClassId && x.IsDelete == ((int)Shared.ActiveState.Active).ToString()).ToListAsync();
                if (lstvclsSubject.Any())
                {
                    var lstvclsSubjectids = lstvclsSubject.Select(x => x.Id).ToList();
                    var lstclsSubject = await _context.BranchClassPeriodBreak.Where(x => lstvclsSubjectids.Contains(x.Id)).ToListAsync();
                    _context.BranchClassPeriodBreak.RemoveRange(lstclsSubject);
                }

                var lstchkcount = model.LstPeriodBreak.Where(x => x.IsSelected && x.IsDelete == ((int)Shared.ActiveState.Active).ToString()).ToList();
                foreach (var item1 in lstchkcount)
                {
                    var btncls = new BranchClassPeriodBreak
                    {
                        BranchId = (int)model.BranchId,
                        BranchClassId = (int)model.BranchClassId,
                        ClassId = (int)model.ClassId,
                        PeriodBreakId = item1.Id,
                        CreatedByUserId = UserId,
                        CreatedDate = DateTime.Now
                    };
                    _context.BranchClassPeriodBreak.Add(btncls);
                }

                var index = await _context.SaveChangesAsync();
                if (index >= 0)
                {
                    result = new ServiceResult
                    {
                        StatusId = (int)ServiceResultStatus.Updated,
                        RecordId = (int)model.BranchClassId
                    };
                }
            }
            catch (Exception ex)
            {
                // Log exception
            }
            return result;
        }

        public async Task<List<SelectListItem>> GetTeachers(int BranchId)
        {
            var lst = await _context.VTeacherCurrentLocation.Where(x => x.BranchId == BranchId).ToListAsync();
            return lst.Select(x => new SelectListItem { Text = x.FullName, Value = x.TeacherId.ToString() }).ToList();
        }

        public async Task<List<SelectListItem>> GetShifts()
        {
            var lst = await _context.Shift.Where(x => x.IsActive == ((int)ActiveState.Active).ToString()).ToListAsync();
            return lst.Select(x => new SelectListItem { Text = $"{x.FromTime} - {x.ToTime}", Value = x.Id.ToString() }).ToList();
        }

        public async Task Dispose()
        {
            await _context.DisposeAsync();
        }
    }
}