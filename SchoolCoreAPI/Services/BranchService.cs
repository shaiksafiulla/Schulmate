using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class BranchService : IBranchService
    {
        private readonly APIContext _context;
        private readonly IMapper _mapper;

        public BranchService(APIContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<vbranches>> GetAllBranch()
        {
            try
            {
                return await _context.vbranches.AsNoTracking().OrderByDescending(x => x.Id).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<vbranches> ViewBranch(int Id)
        {
            return await _context.vbranches.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
        }

        public BranchDTO GetBranch(int Id)
        {
            var model = new BranchDTO { AdminSheet = new List<SelectListItem>() };

            if (Id > 0)
            {
                var cat =  _context.Branch.AsNoTracking().FirstOrDefault(x => x.Id == Id && x.IsActive == ((int)Shared.ActiveState.Active).ToString());
                if (cat != null)
                {
                    model = _mapper.Map<BranchDTO>(cat);
                    model.AdminSheet = GetAdmins(Id);
                    if (model.AdminId > 0)
                    {
                        var divbranchItem = model.AdminSheet.Find(p => p.Value == cat.AdminId.ToString());
                        if (divbranchItem != null)
                        {
                            divbranchItem.Selected = true;
                        }
                    }
                }
            }
            else
            {
                model.IsActive = ((int)ActiveState.Active).ToString();
            }

            return model;
        }

        public async Task<bool> IsRecordExists(string name, int Id)
        {
            return Id == 0
                ? await _context.Branch.AnyAsync(e => e.Name == name && e.IsActive == ((int)ActiveState.Active).ToString())
                : await _context.Branch.AnyAsync(e => e.Name == name && e.Id != Id && e.IsActive == ((int)ActiveState.Active).ToString());
        }

        public async Task<ServiceResult> SaveBranch(BranchDTO model, int UserId)
        {
            try
            {
                Branch cat;
                if (model.Id > 0)
                {
                    cat = await _context.Branch.SingleOrDefaultAsync(m => m.Id == model.Id && m.IsActive == ((int)ActiveState.Active).ToString());
                    if (cat == null) return null;

                    cat.Name = model.Name;
                    cat.Theme = model.Theme;
                    cat.Address = model.Address;
                    cat.PhoneNo = model.PhoneNo;
                    cat.EmailAddress = model.EmailAddress;
                    cat.AdminId = model.AdminId > 0 ? model.AdminId : 0;
                    cat.LastModifiedDate = DateTime.Now;
                    cat.LastModifiedByUserId = UserId;
                    _context.Entry(cat).State = EntityState.Modified;
                }
                else
                {
                    cat = new Branch
                    {
                        Name = model.Name,
                        Theme = model.Theme,
                        Address = model.Address,
                        PhoneNo = model.PhoneNo,
                        EmailAddress = model.EmailAddress,
                        IsActive = ((int)Shared.ActiveState.Active).ToString(),
                        CreatedDate = DateTime.Now,
                        CreatedByUserId = UserId,
                        LastModifiedDate = DateTime.Now,
                        LastModifiedByUserId = UserId
                    };
                    await _context.Branch.AddAsync(cat);
                }

                var changes = await _context.SaveChangesAsync();
                if (changes == 0) return null;

                return new ServiceResult
                {
                    StatusId = model.Id > 0 ? (int)ServiceResultStatus.Updated : (int)ServiceResultStatus.Added,
                    RecordId = cat.Id
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<ServiceResult> DeleteBranch(int Id, int UserId)
        {
            var cat = await _context.Branch.SingleOrDefaultAsync(m => m.Id == Id);
            if (cat == null) return null;

            cat.IsActive = ((int)Shared.ActiveState.InActive).ToString();
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

        public async Task<ClassVM> GetBranchClass(int Id)
        {
            try
            {
                var lstclass = _context.VClasses.AsNoTracking().OrderByDescending(x => x.Id).ToList();
                var lstcastclass = lstclass.Select(c => new CastClass
                {
                    Id = (int)c.Id,
                    IsDelete = ((int)Shared.ActiveState.Active).ToString(),
                    IsSelected = false,
                    Name = $"{c.Name} ({c.MediumName})"
                }).ToList();

                if (Id > 0)
                {
                    var lstbrnclass = _context.VBranchClasses.AsNoTracking().Where(x => x.BranchId == Id).ToList();
                    foreach (var item in lstcastclass)
                    {
                        var brnclass = lstbrnclass.FirstOrDefault(b => b.ClassId == item.Id);
                        if (brnclass != null)
                        {
                            item.IsSelected = true;
                            item.IsDelete = brnclass.IsDelete;
                        }
                    }
                }

                return new ClassVM { LstClass = lstcastclass, BranchId = Id };
            }
            catch (Exception ex)
            {

                return null;
            }
            
        }

        public async Task<ServiceResult> UpdateBranchClass1(ClassVM model, int UserId)
        {
            try
            {
                
                var lstvbrclsids = await _context.VBranchClasses
    .Where(x => x.BranchId == model.BranchId && x.IsDelete == ((int)Shared.ActiveState.Active).ToString())
    .Select(x => x.Id)
    .ToListAsync();

                if (lstvbrclsids.Any())
                {
                    // Directly use the list of ids in the query for BranchClass removal
                    var lstbrnclass = await _context.BranchClass
                        .Where(x => lstvbrclsids.Contains(x.Id))
                        .ToListAsync();

                    // Remove the entries
                    _context.BranchClass.RemoveRange(lstbrnclass);
                }

                var lstchkcount = model.LstClass.Where(x => x.IsSelected && x.IsDelete == ((int)Shared.ActiveState.Active).ToString()).ToList();
                if (lstchkcount.Any())
                {
                    var newBranchClasses = lstchkcount.Select(item1 => new BranchClass
                    {
                        BranchId = (int)model.BranchId,
                        ClassId = item1.Id,
                        CreatedDate = DateTime.Now,
                        CreatedByUserId = UserId
                    }).ToList();

                    await _context.BranchClass.AddRangeAsync(newBranchClasses);
                }

                var changes = await _context.SaveChangesAsync();
                if (changes == 0) return null;

                return new ServiceResult
                {
                    StatusId = (int)ServiceResultStatus.Updated,
                    RecordId = (int)model.BranchId
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ServiceResult> UpdateBranchClass(ClassVM model, int UserId)
        {
            try
            {
                // Fetch the ids of the branch classes that are active and belong to the given BranchId
                var lstvbrclsids = await _context.VBranchClasses
                    .Where(x => x.BranchId == model.BranchId && x.IsDelete == ((int)Shared.ActiveState.Active).ToString())
                    .Select(x => x.Id)
                    .ToListAsync();

                // If there are existing BranchClass entries for these ids, remove them
                if (lstvbrclsids.Any())
                {
                    var lstbrnclass = await _context.BranchClass
                        .Where(x => lstvbrclsids.Contains(x.Id))
                        .ToListAsync();

                    _context.BranchClass.RemoveRange(lstbrnclass);
                }

                // Select and add new BranchClass entries from the checked classes
                var lstchkcount = model.LstClass
                    .Where(x => x.IsSelected && x.IsDelete == ((int)Shared.ActiveState.Active).ToString())
                    .ToList();

                if (lstchkcount.Any())
                {
                    var newBranchClasses = lstchkcount.Select(item => new BranchClass
                    {
                        BranchId = (int)model.BranchId,
                        ClassId = item.Id,
                        CreatedDate = DateTime.Now,
                        CreatedByUserId = UserId
                    }).ToList();

                    await _context.BranchClass.AddRangeAsync(newBranchClasses);
                }

                // Save changes to the database
                var changes = await _context.SaveChangesAsync();
               // if (changes == 0) return null;  // No changes were made, return null

                // Return success result with the updated branch id
                return new ServiceResult
                {
                    StatusId = (int)ServiceResultStatus.Updated,
                    RecordId = (int)model.BranchId
                };
            }
            catch (Exception ex)
            {
                // Ideally log the exception here for better debugging
                // _logger.LogError(ex, "An error occurred while updating the branch class.");
                return new ServiceResult
                {
                    StatusId = 0,
                    RecordId = 0
                };
            }
        }

        public List<SelectListItem> GetAdmins(int branchId)
        {
            var lst =  _context.VAdmins.AsNoTracking().Where(x => x.BranchId == branchId).ToList();
            return lst.Select(x => new SelectListItem { Text = x.FullName, Value = x.Id.ToString() }).ToList();
        }

        public async Task<List<BranchSectionTimeTableRPT>> GetBranchSectionTimeTableRPT(int Id, int UserId)
        {
            var lstBranchSectionTimeTableRPT = new List<BranchSectionTimeTableRPT>();
            var objBranch = await _context.Branch.AsNoTracking().SingleOrDefaultAsync(x => x.Id == Id && x.IsActive == ((int)ActiveState.Active).ToString());
            var objsession = await _context.SessionYear.AsNoTracking().SingleOrDefaultAsync(x => x.IsActive == ((int)ActiveState.Active).ToString() && x.IsDefault);

            if (objBranch != null)
            {
                var lstsections = await _context.VSections.AsNoTracking().Where(x => x.BranchId == objBranch.Id).ToListAsync();
                foreach (var section in lstsections)
                {
                    var sectionDurBreak = await GetClassDurationAndBreaks((int)section.Id);
                    var SectionTimeTableRPT = new BranchSectionTimeTableRPT
                    {
                        SessionYearId = objsession.Id,
                        BranchId = (int)section.BranchId,
                        BranchClassId = (int)section.BranchClassId,
                        ClassId = (int)section.ClassId,
                        SectionId = (int)section.Id,
                        ClassSectionName = $"{section.ClassName}-{section.Name}",
                        timetable = GenerateTimetable(sectionDurBreak.Duration, sectionDurBreak.LstBreak, sectionDurBreak.ClassStartTime, sectionDurBreak.ClassEndTime)
                    };
                    lstBranchSectionTimeTableRPT.Add(SectionTimeTableRPT);
                }
            }

            return lstBranchSectionTimeTableRPT;
        }

        public async Task<List<VTimeTable>> GetTimeTable(int BranchId, int SessionYearId, int UserId)
        {
            return await _context.VTimeTable.AsNoTracking().Where(x => x.BranchId == BranchId && x.SessionYearId == SessionYearId).ToListAsync();
        }

        public async Task<ClassDurationAndBreaks> GetClassDurationAndBreaks(int sectionId)
        {
            var objsection = await _context.Section.AsNoTracking().SingleOrDefaultAsync(x => x.Id == sectionId && x.IsActive == ((int)ActiveState.Active).ToString());
            if (objsection == null) return null;

            var objbrcls = await _context.BranchClass.AsNoTracking().SingleOrDefaultAsync(x => x.Id == objsection.BranchClassId);
            var objshift = await _context.Shift.AsNoTracking().SingleOrDefaultAsync(x => x.Id == objbrcls.ShiftId && x.IsActive == ((int)ActiveState.Active).ToString());

            var lstperiodbreakids = await _context.BranchClassPeriodBreak.AsNoTracking().Where(x => x.BranchClassId == objsection.BranchClassId).Select(x => x.PeriodBreakId).ToListAsync();
            var lstperiodbreak = await _context.PeriodBreak.AsNoTracking().Where(x => lstperiodbreakids.Contains(x.Id) && x.IsActive == ((int)ActiveState.Active).ToString()).ToListAsync();
            var lstbrk = lstperiodbreak.Select(c => new Break
            {
                StartTime = GetTimeSpan(c.FromTime),
                EndTime = GetTimeSpan(c.ToTime),
                Name = c.ShortName
            }).ToList();

            return new ClassDurationAndBreaks
            {
                ClassStartTime = GetTimeSpan(objshift.FromTime),
                ClassEndTime = GetTimeSpan(objshift.ToTime),
                Duration = int.Parse(objbrcls.SlotDuration),
                LstBreak = lstbrk
            };
        }

        public async Task Dispose()
        {
            await _context.DisposeAsync();
        }
    }
}