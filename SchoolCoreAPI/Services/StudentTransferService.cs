using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class StudentTransferService : IStudentTransferService
    {
        private readonly APIContext _context;
        public StudentTransferService(APIContext context)
        {
            _context = context;
        }

        public async Task<StudentTransferVM> GetStudentTransferVM()
        {
            var objsession = await _context.SessionYear.AsNoTracking()
                .SingleOrDefaultAsync(x => x.IsDefault && x.IsActive == ((int)ActiveState.Active).ToString());
            if (objsession == null) return null;

            var branches = await GetBranches();
            return new StudentTransferVM
            {
                SessionYearId = objsession.Id,
                FromBranchSheet = branches,
                FromClassSheet = new List<SelectListItem>(),
                FromSectionSheet = new List<SelectListItem>(),
                ToBranchSheet = branches,
                ToClassSheet = new List<SelectListItem>(),
                ToSectionSheet = new List<SelectListItem>()
            };
        }

        public async Task<List<SelectListItem>> GetClassByBranch(int branchId)
        {
            var lstbrclsids = await _context.BranchClass.AsNoTracking()
                .Where(x => x.BranchId == branchId)
                .Select(x => x.ClassId)
                .ToListAsync();
            if (!lstbrclsids.Any()) return new List<SelectListItem>();

            return await _context.Class.AsNoTracking()
                .Where(x => x.IsActive == ((int)ActiveState.Active).ToString() && lstbrclsids.Contains(x.Id))
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetSectionByClass(int branchId, int classId)
        {
            var brcls = await _context.BranchClass.AsNoTracking()
                .SingleOrDefaultAsync(x => x.BranchId == branchId && x.ClassId == classId);
            if (brcls == null) return new List<SelectListItem>();

            return await _context.Section.AsNoTracking()
                .Where(x => x.IsActive == ((int)ActiveState.Active).ToString() && x.BranchClassId == brcls.Id)
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                .ToListAsync();
        }

        public async Task<List<CastStudentTransfer>> GetStudents(int sectionId)
        {
            try
            {
                return await _context.VStudentCurrentLocation.AsNoTracking()
                    .Where(x => x.SectionId == sectionId)
                    .Select(c => new CastStudentTransfer
                    {
                        StudentId = c.StudentId,
                        RollNo = c.RollNo.ToString(),
                        StudentName = c.FullName,
                        IsSelected = false
                    })
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<CastStudentTransfer>();
            }
        }

        public async Task<ServiceResult> SaveStudentTransfer(List<StudentTransferSection> model, int UserId)
        {
            if (model == null || !model.Any()) return null;

            try
            {
                var lastId = await _context.StudentAdmission.AsNoTracking()
                    .Where(x => x.BranchId == model[0].ToBranchId && x.ClassId == model[0].ToClassId
                                && x.SectionId == model[0].ToSectionId && x.SessionYearId == model[0].SessionYearId)
                    .OrderByDescending(x => x.Id)
                    .Select(x => x.Id)
                    .FirstOrDefaultAsync() + 1;

                foreach (var item in model)
                {
                    var obj = new StudentTransferSection
                    {
                        SessionYearId = item.SessionYearId,
                        StudentId = item.StudentId,
                        FromBranchId = item.FromBranchId,
                        FromClassId = item.FromClassId,
                        FromSectionId = item.FromSectionId,
                        ToBranchId = item.ToBranchId,
                        ToClassId = item.ToClassId,
                        ToSectionId = item.ToSectionId,
                        RollNo = $"{item.ToBranchId}-{item.ToClassId}-{item.ToSectionId}-00{lastId}",
                        TransferType = ((int)TransferType.Transfer).ToString(),
                        CreatedByUserId = UserId,
                        CreatedDate = DateTime.Now
                    };
                    _context.StudentTransferSection.Add(obj);
                    _context.Entry(obj).State = EntityState.Added;
                    lastId++;
                }

                var addedindex =  await _context.SaveChangesAsync();
                if (addedindex == 0) return null;

                return new ServiceResult
                {
                    StatusId = (int)ServiceResultStatus.Added,
                    RecordId = model.First().SessionYearId
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<SelectListItem>> GetBranches()
        {
            return await _context.Branch.AsNoTracking()
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