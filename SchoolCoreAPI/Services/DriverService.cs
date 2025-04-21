using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using System.Data;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class DriverService : IDriverService
    {
        private readonly APIContext _context;
        private readonly string _connectionString;
        private readonly HostEnvironmentService _hostingEnvironment;
        private readonly S3Service _s3Service;

        public DriverService(APIContext context, IConfiguration configuration, HostEnvironmentService hostingEnvironment, S3Service s3Service)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _hostingEnvironment = hostingEnvironment;
            _s3Service = s3Service;
        }

        public async Task<List<VDrivers>> GetAllDriver(int UserId)
        {
            var objuser = await _context.VUserInfo.SingleOrDefaultAsync(x => x.Id == UserId);
            return objuser.UserType == ((int)UserType.SuperAdmin).ToString()
                ? await _context.VDrivers.ToListAsync()
                : await _context.VDrivers.Where(x => x.BranchId == objuser.BranchId).ToListAsync();
        }

        public async Task<VDrivers> ViewDriver(int Id)
        {
            return await _context.VDrivers.SingleOrDefaultAsync(m => m.Id == Id);
        }

        public async Task<Personnel> GetDriver(int Id)
        {
            var model = new Personnel
            {
                PersonnelAdmission = new PersonnelAdmission(),
                PersonnelOtherDetail = new PersonnelOtherDetail(),
                BloodGroupSheet = new List<SelectListItem>(await GetBloodGroup()),
                BranchSheet = new List<SelectListItem>(await GetBranches())
            };

            if (Id > 0)
            {
                var cat = await _context.Personnel.SingleOrDefaultAsync(x => x.Id == Id && x.IsActive == ((int)ActiveState.Active).ToString() && x.PersonnelType == ((int)UserType.Driver).ToString());
                if (cat != null)
                {
                    model = MapPersonnel(cat, model);
                    model.PersonnelAdmission = await MapPersonnelAdmission(cat.Id, model.PersonnelType, model.PersonnelAdmission);
                    model.PersonnelOtherDetail = await MapPersonnelOtherDetail(cat.Id, model.PersonnelType, model.PersonnelOtherDetail);
                }
            }
            else
            {
                model.Gender = ((int)Gender.Male).ToString();
                model.FilePath = Shared.GetNoUserPath();
                model.PersonnelType = ((int)UserType.Driver).ToString();
                var sessyear = await _context.SessionYear.SingleOrDefaultAsync(x => x.IsActive == ((int)ActiveState.Active).ToString() && x.IsDefault == true);
                model.PersonnelAdmission.SessionYearId = sessyear.Id;
            }

            return model;
        }

        private Personnel MapPersonnel(Personnel source, Personnel target)
        {
            target.Id = source.Id;
            target.PersonnelType = source.PersonnelType;
            target.FirstName = source.FirstName;
            target.LastName = source.LastName;
            target.Gender = source.Gender;
            target.DateOfBirth = source.DateOfBirth;
            target.Designation = source.Designation;
            target.EmailAddress = source.EmailAddress;
            target.Qualification = source.Qualification;
            target.FileName = source.FileName;
            target.FilePath = source.FilePath;

            if (source.BloodGroupId > 0)
            {
                target.BloodGroupId = source.BloodGroupId;
                var divselectedItem = target.BloodGroupSheet.Find(p => p.Value == source.BloodGroupId.ToString());
                if (divselectedItem != null)
                {
                    divselectedItem.Selected = true;
                }
            }

            return target;
        }

        private async Task<PersonnelAdmission> MapPersonnelAdmission(int personnelId, string personnelType, PersonnelAdmission target)
        {
            var admission = await _context.PersonnelAdmission.SingleOrDefaultAsync(x => x.PersonnelId == personnelId && x.IsActive == ((int)ActiveState.Active).ToString() && x.PersonnelType == personnelType);
            if (admission != null)
            {
                target.Id = admission.Id;
                target.PersonnelType = admission.PersonnelType;
                target.PersonnelId = admission.PersonnelId;
                target.SessionYearId = admission.SessionYearId;
                target.AdmissionDate = admission.AdmissionDate;
                target.AdmissionNo = admission.AdmissionNo;

                if (admission.BranchId > 0)
                {
                    target.BranchId = admission.BranchId;
                }
            }

            return target;
        }

        private async Task<PersonnelOtherDetail> MapPersonnelOtherDetail(int personnelId, string personnelType, PersonnelOtherDetail target)
        {
            var otherdetail = await _context.PersonnelOtherDetail.SingleOrDefaultAsync(x => x.PersonnelId == personnelId && x.IsActive == ((int)ActiveState.Active).ToString() && x.PersonnelType == personnelType);
            if (otherdetail != null)
            {
                target.Id = otherdetail.Id;
                target.PersonnelType = otherdetail.PersonnelType;
                target.PersonnelId = otherdetail.PersonnelId;
                target.FatherName = otherdetail.FatherName;
                target.MotherName = otherdetail.MotherName;
                target.DefaultMobileNo = otherdetail.DefaultMobileNo;
                target.OtherMobileNo = otherdetail.OtherMobileNo;
                target.CurrentAddress = otherdetail.CurrentAddress;
                target.PermanentAddress = otherdetail.PermanentAddress;
            }

            return target;
        }

        public async Task<bool> IsRecordExists(string fname, string lname, int BranchId, int Id)
        {
            return Id == 0
                ? await _context.VDrivers.AnyAsync(e => e.FullName == fname + " " + lname && e.BranchId == BranchId && e.PersonnelType == ((int)UserType.Driver).ToString())
                : await _context.VDrivers.AnyAsync(e => e.Id != Id && e.FullName == fname + " " + lname && e.BranchId == BranchId && e.PersonnelType == ((int)UserType.Driver).ToString());
        }

        public async Task<ServiceResult> SaveDriver(Personnel model, int UserId)
        {
            ServiceResult result = null;
            try
            {
                if (model.Id > 0)
                {
                    result = await UpdateDriver(model, UserId);
                }
                else
                {
                    result = await AddDriver(model, UserId);
                }
            }
            catch (Exception)
            {
                // Log exception
            }
            return result;
        }

        private async Task<ServiceResult> UpdateDriver(Personnel model, int UserId)
        {
            var result = new ServiceResult();
            var cat = await _context.Personnel.SingleOrDefaultAsync(m => m.Id == model.Id && m.IsActive == ((int)ActiveState.Active).ToString() && m.PersonnelType == ((int)UserType.Driver).ToString());

            if (cat != null)
            {
                cat.FirstName = model.FirstName;
                cat.LastName = model.LastName;
                cat.Gender = model.Gender;
                cat.BloodGroupId = model.BloodGroupId;
                cat.DateOfBirth = model.DateOfBirth;
                cat.Designation = model.Designation;
                cat.EmailAddress = model.EmailAddress;
                cat.Qualification = model.Qualification;
                cat.ModifiedDate = DateTime.Now;
                cat.ModifiedByUserId = UserId;
                cat.BranchId = model.BranchId;

                if (model.File != null)
                {
                    //if (cat.FilePath != null && !cat.FilePath.Contains("nouser.png"))
                    //{
                    //    var filePath = _hostingEnvironment.GetFullUrlFromDbPath(cat.FilePath);
                    //    if (File.Exists(filePath))
                    //    {
                    //        File.Delete(filePath);
                    //    }
                    //}
                    if (!string.IsNullOrEmpty(cat.FilePath) && !cat.FilePath.Contains("noimage.png"))
                    {
                        await _s3Service.DeleteFileAsync(cat.FilePath);
                    }
                    cat.FileName = model.File.FileName;
                    //model.FilePath = Shared.ProcessUploadFile((int)UploadType.Driver, model.File, _hostingEnvironment.GetWebRootPath());
                    //cat.FilePath = model.FilePath;
                    cat.FilePath = await _s3Service.UploadFileAsync((int)UploadType.Driver, model.File);

                    
                }

                _context.Entry(cat).State = EntityState.Modified;
                if (await _context.SaveChangesAsync() > 0)
                {
                    await UpdatePersonnelOtherDetail(model, UserId, cat.Id);
                    result.StatusId = (int)ServiceResultStatus.Updated;
                    result.RecordId = cat.Id;
                }
            }

            return result;
        }

        private async Task UpdatePersonnelOtherDetail(Personnel model, int UserId, int personnelId)
        {
            if (model.PersonnelOtherDetail != null)
            {
                var otherdetail = await _context.PersonnelOtherDetail.SingleOrDefaultAsync(x => x.PersonnelId == personnelId && x.IsActive == ((int)ActiveState.Active).ToString() && x.PersonnelType == model.PersonnelType);
                if (otherdetail != null)
                {
                    otherdetail.FatherName = model.PersonnelOtherDetail.FatherName;
                    otherdetail.MotherName = model.PersonnelOtherDetail.MotherName;
                    otherdetail.DefaultMobileNo = model.PersonnelOtherDetail.DefaultMobileNo;
                    otherdetail.OtherMobileNo = model.PersonnelOtherDetail.OtherMobileNo;
                    otherdetail.CurrentAddress = model.PersonnelOtherDetail.CurrentAddress;
                    otherdetail.PermanentAddress = model.PersonnelOtherDetail.PermanentAddress;
                    otherdetail.LastModifiedDate = DateTime.Now;
                    otherdetail.LastModifiedByUserId = UserId;

                    _context.Entry(otherdetail).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
            }
        }

        private async Task<ServiceResult> AddDriver(Personnel model, int UserId)
        {
            var result = new ServiceResult();
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var cat = new Personnel
                    {
                        PersonnelType = model.PersonnelType,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Gender = model.Gender,
                        DateOfBirth = model.DateOfBirth,
                        BloodGroupId = model.BloodGroupId,
                        Designation = model.Designation,
                        EmailAddress = model.EmailAddress,
                        Qualification = model.Qualification,
                        IsActive = ((int)ActiveState.Active).ToString(),
                        CreatedDate = DateTime.Now,
                        CreatedByUserId = UserId,
                        BranchId = model.BranchId,
                        FileName = model.File?.FileName ?? "nouser.png",
                        //FilePath = model.File != null ? Shared.ProcessUploadFile((int)UploadType.Driver, model.File, _hostingEnvironment.GetWebRootPath()) : Path.Combine("Uploads", "NoImage", "nouser.png")
                        FilePath = model.File != null                     
                        ? await _s3Service.UploadFileAsync((int)UploadType.Driver, model.File)
                        : Path.Combine("Uploads", "NoImage", "noimage.png")

                    };

                    _context.Personnel.Add(cat);
                    _context.Entry(cat).State = EntityState.Added;
                    if (await _context.SaveChangesAsync() > 0)
                    {
                        await AddPersonnelAdmission(model, UserId, cat);
                        await AddPersonnelOtherDetail(model, UserId, cat);
                        await AddUserInfo(model, UserId, cat);
                        await transaction.CommitAsync();
                        result.StatusId = (int)ServiceResultStatus.Added;
                        result.RecordId = cat.Id;
                    }
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    // Log exception
                }
            }

            return result;
        }

        private async Task AddPersonnelAdmission(Personnel model, int UserId, Personnel cat)
        {
            if (model.PersonnelAdmission != null)
            {
                var admission = new PersonnelAdmission
                {
                    PersonnelId = cat.Id,
                    PersonnelType = cat.PersonnelType,
                    BranchId = (int)cat.BranchId,
                    SessionYearId = model.PersonnelAdmission.SessionYearId,
                    AdmissionDate = model.PersonnelAdmission.AdmissionDate,
                    AdmissionNo = $"{cat.BranchId}-{DateTime.Now:HHmmss}",
                    IsActive = ((int)ActiveState.Active).ToString(),
                    CreatedDate = DateTime.Now,
                    CreatedByUserId = UserId
                };

                _context.PersonnelAdmission.Add(admission);
                _context.Entry(admission).State = EntityState.Added;
                await _context.SaveChangesAsync();

                var lastId = (await _context.PersonnelAdmission.Where(x => x.PersonnelType == ((int)UserType.Driver).ToString() && x.BranchId == (int)cat.BranchId).OrderBy(x => x.Id).LastOrDefaultAsync())?.Id ?? 1;

                var transfer = new PersonnelTransferBranch
                {
                    SessionYearId = admission.SessionYearId,
                    PersonnelId = admission.PersonnelId,
                    PersonnelType = admission.PersonnelType,
                    FromBranchId = admission.BranchId,
                    ToBranchId = admission.BranchId,
                    EmployeeNo = $"{cat.BranchId}-00{lastId + 1}",
                    TransferType = ((int)TransferType.Admitted).ToString(),
                    CreatedByUserId = UserId,
                    CreatedDate = DateTime.Now
                };

                _context.PersonnelTransferBranch.Add(transfer);
                _context.Entry(transfer).State = EntityState.Added;
                await _context.SaveChangesAsync();
            }
        }

        private async Task AddPersonnelOtherDetail(Personnel model, int UserId, Personnel cat)
        {
            if (model.PersonnelOtherDetail != null)
            {
                var otherModel = new PersonnelOtherDetail
                {
                    PersonnelId = cat.Id,
                    PersonnelType = cat.PersonnelType,
                    FatherName = model.PersonnelOtherDetail.FatherName,
                    MotherName = model.PersonnelOtherDetail.MotherName,
                    DefaultMobileNo = model.PersonnelOtherDetail.DefaultMobileNo,
                    OtherMobileNo = model.PersonnelOtherDetail.OtherMobileNo,
                    CurrentAddress = model.PersonnelOtherDetail.CurrentAddress,
                    PermanentAddress = model.PersonnelOtherDetail.PermanentAddress,
                    IsActive = ((int)ActiveState.Active).ToString(),
                    CreatedDate = DateTime.Now,
                    CreatedByUserId = UserId
                };

                _context.PersonnelOtherDetail.Add(otherModel);
                _context.Entry(otherModel).State = EntityState.Added;
                await _context.SaveChangesAsync();
            }
        }

        private async Task AddUserInfo(Personnel model, int UserId, Personnel cat)
        {
            var userinfo = new userinfo
            {
                PersonId = cat.Id,
                UserType = cat.PersonnelType,
                UserName = cat.FirstName,
                Password = ToSHA2569(cat.FirstName),
                CreatedByUserId = UserId,
                CreatedDate = DateTime.Now,
                IsActive = ((int)ActiveState.Active).ToString(),
                HasLogin = ((int)ActiveState.Active).ToString()
            };

            _context.userinfo.Add(userinfo);
            _context.Entry(userinfo).State = EntityState.Added;
            await _context.SaveChangesAsync();

            var objRole = await _context.Role.SingleOrDefaultAsync(x => x.RoleType == ((int)UserType.Driver).ToString() && x.IsActive == ((int)ActiveState.Active).ToString());
            if (objRole != null)
            {
                var usr = new UserRole
                {
                    UserId = userinfo.Id,
                    RoleId = objRole.Id,
                    CreatedByUserId = UserId,
                    CreatedDate = DateTime.Now
                };

                _context.UserRole.Add(usr);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ServiceResult> DeleteDriver(int Id, int UserId)
        {
            var result = new ServiceResult();
            var cat = await _context.Personnel.SingleOrDefaultAsync(m => m.Id == Id);
            if (cat != null)
            {
                cat.IsActive = ((int)ActiveState.InActive).ToString();
                cat.ModifiedByUserId = UserId;
                cat.ModifiedDate = DateTime.Now;

                _context.Entry(cat).State = EntityState.Modified;
                if (await _context.SaveChangesAsync() > 0)
                {
                    result.StatusId = (int)ServiceResultStatus.Deleted;
                    result.RecordId = cat.Id;
                }
            }

            return result;
        }

        public async Task<List<SelectListItem>> GetBranches()
        {
            return await _context.Branch
                .Where(x => x.IsActive == ((int)ActiveState.Active).ToString())
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetBloodGroup()
        {
            return await _context.BloodGroup
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
