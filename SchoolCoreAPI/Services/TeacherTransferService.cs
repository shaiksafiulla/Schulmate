using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.IsisMtt.X509;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class TeacherTransferService : ITeacherTransferService
    {
        private readonly APIContext _context;
        public TeacherTransferService(APIContext context)
        {
            _context = context;
        }

        public async Task<TeacherTransferVM> GetTeacherTransferVM()
        {
            var objsession = await _context.SessionYear
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.IsDefault && x.IsActive == ((int)ActiveState.Active).ToString());

            if (objsession == null) return null;

            var branches = await GetBranches();
            return new TeacherTransferVM
            {
                SessionYearId = objsession.Id,
                FromBranchSheet = branches,
                ToBranchSheet = branches
            };
        }

        public async Task<List<CastTeacherTransfer>> GetTeachers(int branchId)
        {
            try
            {
                var lststud = await _context.VPersonnelCurrentLocation
                    .AsNoTracking()
                    .Where(x => x.BranchId == branchId && x.PersonnelType == ((int)UserType.Teacher).ToString())
                    .Select(c => new CastTeacherTransfer
                    {
                        TeacherId = c.PersonnelId,
                        EmployeeNo = c.EmployeeNo.ToString(),
                        TeacherName = c.FullName,
                        IsSelected = false
                    })
                    .ToListAsync();

                return lststud;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ServiceResult> SaveTeacherTransfer(List<TeacherTransferBranch> model, int UserId)
        {
            if (model == null || !model.Any()) return null;

            try
            {
                var transfers = model.Select(item => new TeacherTransferBranch
                {
                    SessionYearId = item.SessionYearId,
                    TeacherId = item.TeacherId,
                    FromBranchId = item.FromBranchId,
                    ToBranchId = item.ToBranchId,
                    CreatedByUserId = UserId,
                    CreatedDate = DateTime.Now
                }).ToList();

                _context.TeacherTransferBranch.AddRange(transfers);
                //var transfer = new PersonnelTransferBranch
                //{
                //    SessionYearId = admission.SessionYearId,
                //    PersonnelId = admission.PersonnelId,
                //    PersonnelType = admission.PersonnelType,
                //    FromBranchId = admission.BranchId,
                //    ToBranchId = admission.BranchId,
                //    EmployeeNo = $"{cat.BranchId}-00{lastId + 1}",
                //    TransferType = ((int)TransferType.Admitted).ToString(),
                //    CreatedByUserId = UserId,
                //    CreatedDate = DateTime.Now
                //};

                //_context.PersonnelTransferBranch.Add(transfer);
               // _context.Entry(transfer).State = EntityState.Added;
                var addedindex = await _context.SaveChangesAsync();

                if (addedindex == 0) return null;

                return new ServiceResult
                {
                    StatusId = (int)ServiceResultStatus.Added,
                    RecordId = model.First().SessionYearId
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<SelectListItem>> GetBranches()
        {
            var lst = await _context.Branch
                .AsNoTracking()
                .Where(x => x.IsActive == ((int)ActiveState.Active).ToString())
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                .ToListAsync();

            return lst;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}