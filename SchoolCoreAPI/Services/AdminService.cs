using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using System.Data;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class AdminService : IAdminService
    {
        private readonly APIContext _context;
        private readonly string _connectionString;
        private readonly HostEnvironmentService _hostingEnvironment;
        private readonly S3Service _s3Service;

        public AdminService(APIContext context, IConfiguration configuration, HostEnvironmentService hostingEnvironment, S3Service s3Service)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _hostingEnvironment = hostingEnvironment;
            _s3Service = s3Service;

        }

        public async Task<List<VAdmins>> GetAllAdmin(int UserId)
        {
            var objuser = await _context.VUserInfo.AsNoTracking().SingleOrDefaultAsync(x => x.Id == UserId);
            return objuser.UserType == ((int)UserType.SuperAdmin).ToString()
                ? await _context.VAdmins.AsNoTracking().ToListAsync()
                : await _context.VAdmins.AsNoTracking().Where(x => x.BranchId == objuser.BranchId).ToListAsync();
        }

        public async Task<VAdmins> ViewAdmin(int Id)
        {
            return await _context.VAdmins.SingleOrDefaultAsync(m => m.Id == Id);
        }

        public async Task<Personnel> GetAdmin(int Id)
        {
            var model = new Personnel
            {
                PersonnelAdmission = new PersonnelAdmission(),
                PersonnelOtherDetail = new PersonnelOtherDetail(),
                BloodGroupSheet = new List<SelectListItem>(await GetBloodGroupAsync()),
                BranchSheet = new List<SelectListItem>(await GetBranchesAsync())
            };

            if (Id > 0)
            {
                var cat = await _context.Personnel.SingleOrDefaultAsync(x => x.Id == Id && x.IsActive == ((int)ActiveState.Active).ToString() && x.PersonnelType == ((int)UserType.Admin).ToString());
                if (cat != null)
                {
                    model = MapPersonnel(cat, model);
                    model.PersonnelAdmission = await MapPersonnelAdmissionAsync(model.Id, model.PersonnelType, model.PersonnelAdmission);
                    model.PersonnelOtherDetail = await MapPersonnelOtherDetailAsync(model.Id, model.PersonnelType, model.PersonnelOtherDetail);
                }
            }
            else
            {
                model.Gender = ((int)Gender.Male).ToString();
                model.FilePath = Shared.GetNoUserPath();
                model.PersonnelType = ((int)UserType.Admin).ToString();
                var sessyear = await _context.SessionYear.SingleOrDefaultAsync(x => x.IsActive == ((int)ActiveState.Active).ToString() && x.IsDefault == true);
                model.PersonnelAdmission.SessionYearId = sessyear.Id;
            }

            return model;
        }

        private Personnel MapPersonnel(Personnel cat, Personnel model)
        {
            model.Id = cat.Id;
            model.PersonnelType = cat.PersonnelType;
            model.FirstName = cat.FirstName;
            model.LastName = cat.LastName;
            model.Gender = cat.Gender;
            model.DateOfBirth = cat.DateOfBirth;
            model.Designation = cat.Designation;
            model.EmailAddress = cat.EmailAddress;
            model.Qualification = cat.Qualification;
            model.FileName = cat.FileName;
            model.FilePath = cat.FilePath;

            if (cat.BloodGroupId > 0)
            {
                model.BloodGroupId = cat.BloodGroupId;
                var divselectedItem = model.BloodGroupSheet.Find(p => p.Value == cat.BloodGroupId.ToString());
                if (divselectedItem != null)
                {
                    divselectedItem.Selected = true;
                }
            }

            return model;
        }

        private async Task<PersonnelAdmission> MapPersonnelAdmissionAsync(int personnelId, string personnelType, PersonnelAdmission admissionModel)
        {
            var admission = await _context.PersonnelAdmission.SingleOrDefaultAsync(x => x.PersonnelId == personnelId && x.IsActive == ((int)ActiveState.Active).ToString() && x.PersonnelType == personnelType);
            if (admission != null)
            {
                admissionModel.Id = admission.Id;
                admissionModel.PersonnelType = admission.PersonnelType;
                admissionModel.PersonnelId = admission.PersonnelId;
                admissionModel.SessionYearId = admission.SessionYearId;
                admissionModel.AdmissionDate = admission.AdmissionDate;
                admissionModel.AdmissionNo = admission.AdmissionNo;
            }

            return admissionModel;
        }

        private async Task<PersonnelOtherDetail> MapPersonnelOtherDetailAsync(int personnelId, string personnelType, PersonnelOtherDetail otherModel)
        {
            var otherdetail = await _context.PersonnelOtherDetail.SingleOrDefaultAsync(x => x.PersonnelId == personnelId && x.IsActive == ((int)ActiveState.Active).ToString() && x.PersonnelType == personnelType);
            if (otherdetail != null)
            {
                otherModel.Id = otherdetail.Id;
                otherModel.PersonnelType = otherdetail.PersonnelType;
                otherModel.PersonnelId = otherdetail.PersonnelId;
                otherModel.FatherName = otherdetail.FatherName;
                otherModel.MotherName = otherdetail.MotherName;
                otherModel.DefaultMobileNo = otherdetail.DefaultMobileNo;
                otherModel.OtherMobileNo = otherdetail.OtherMobileNo;
                otherModel.CurrentAddress = otherdetail.CurrentAddress;
                otherModel.PermanentAddress = otherdetail.PermanentAddress;
            }

            return otherModel;
        }

        public async Task<PersonnelDetail> GetAdminDetail(int Id)
        {
            var objsch = await _context.VAdmins.SingleOrDefaultAsync(m => m.Id == Id);
            var objadmission = await _context.PersonnelAdmission.SingleOrDefaultAsync(x => x.PersonnelId == Id && x.PersonnelType == ((int)UserType.Admin).ToString());
            var objother = await _context.PersonnelOtherDetail.SingleOrDefaultAsync(x => x.PersonnelId == Id && x.PersonnelType == ((int)UserType.Admin).ToString());
            var objsession = await _context.SessionYear.SingleOrDefaultAsync(x => x.IsDefault == true && x.IsActive == ((int)ActiveState.Active).ToString());
            var rpt = await _context.ReportSettings.FirstOrDefaultAsync();

            return new PersonnelDetail
            {
                Id = (int)objsch.Id,
                SessionYearId = objsession.Id,
                FilePath = objsch.FilePath,
                FullName = objsch.FullName,
                Age = objsch.Age,
                GenderName = objsch.GenderName,
                FatherName = objother.FatherName,
                MotherName = objother.MotherName,
                DefaultMobileNo = objsch.DefaultMobileNo,
                BranchName = objsch.BranchName,
                AdmissionDate = objadmission.AdmissionDate.ToString(),
                AdmissionNo = objadmission.AdmissionNo,
                EmployeeNo = objsch.EmployeeNo,
                ActiveStatus = objsch.ActiveStatus,
                ClassSubjectTeacherCount = 0,
                TeacherSubjectName = "",
                AlternateMobileNo = objother.OtherMobileNo,
                PermanentAddress = objother.PermanentAddress,
                CurrentAddress = objother.CurrentAddress,
                ReportSettings = rpt
            };
        }

        public async Task<bool> IsRecordExists(string fname, string lname, int BranchId, int Id)
        {
            return Id == 0
                ? await _context.VAdmins.AnyAsync(e => e.FullName == fname + " " + lname && e.BranchId == BranchId && e.PersonnelType == ((int)UserType.Admin).ToString())
                : await _context.VAdmins.AnyAsync(e => e.Id != Id && e.FullName == fname + " " + lname && e.BranchId == BranchId && e.PersonnelType == ((int)UserType.Admin).ToString());
        }

        public async Task<ServiceResult> SaveAdmin(Personnel model, int UserId)
        {
            ServiceResult result = null;
            try
            {
                if (model.Id > 0)
                {
                    result = await UpdateAdminAsync(model, UserId);
                }
                else
                {
                    result = await AddAdminAsync(model, UserId);
                }
            }
            catch (Exception)
            {
                // Log exception
            }
            return result;
        }

        private async Task<ServiceResult> UpdateAdminAsync(Personnel model, int UserId)
        {
            var result = new ServiceResult();
            var cat = await _context.Personnel.SingleOrDefaultAsync(m => m.Id == model.Id && m.IsActive == ((int)ActiveState.Active).ToString() && m.PersonnelType == ((int)UserType.Admin).ToString());

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

                if (model.File != null)
                {
                    if (cat.FilePath != null && !cat.FilePath.Contains("nouser.png"))
                    {
                        var filePath = _hostingEnvironment.GetFullUrlFromDbPath(cat.FilePath);
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                    }
                    cat.FileName = model.File.FileName;
                    // model.FilePath = Shared.ProcessUploadFile((int)UploadType.Admin, model.File, _hostingEnvironment.GetWebRootPath());
                    // cat.FilePath = model.FilePath;
                    cat.FilePath = await _s3Service.UploadFileAsync((int)UploadType.Admin, model.File);
                  
                }

                _context.Entry(cat).State = EntityState.Modified;
                if (await _context.SaveChangesAsync() != 0)
                {
                    await UpdatePersonnelOtherDetailAsync(model, UserId, cat.Id);
                    result.StatusId = (int)ServiceResultStatus.Updated;
                    result.RecordId = cat.Id;
                }
            }

            return result;
        }

        private async Task UpdatePersonnelOtherDetailAsync(Personnel model, int UserId, int personnelId)
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

        private async Task<ServiceResult> AddAdminAsync(Personnel model, int UserId)
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
                        BranchId = model.BranchId
                    };

                    if (model.File != null)
                    {
                        cat.FileName = model.File.FileName;
                        // cat.FilePath = Shared.ProcessUploadFile((int)UploadType.Admin, model.File, _hostingEnvironment.GetWebRootPath());
                        cat.FilePath = await _s3Service.UploadFileAsync((int)UploadType.Admin, model.File);
                    }
                    else
                    {
                        cat.FileName = "nouser.png";
                        cat.FilePath = Path.Combine("Uploads", "NoImage", "nouser.png");
                    }

                    _context.Personnel.Add(cat);
                    _context.Entry(cat).State = EntityState.Added;
                    if (await _context.SaveChangesAsync() != 0)
                    {
                        await AddPersonnelAdmissionAsync(model, UserId, cat);
                        await AddPersonnelOtherDetailAsync(model, UserId, cat);
                        await AddUserInfoAsync(model, UserId, cat);
                        await transaction.CommitAsync();
                        result.StatusId = (int)ServiceResultStatus.Added;
                        result.RecordId = cat.Id;
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    // Log exception
                }
            }

            return result;
        }

        private async Task AddPersonnelAdmissionAsync(Personnel model, int UserId, Personnel cat)
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

                var lastId = (await _context.PersonnelAdmission.Where(x => x.PersonnelType == ((int)UserType.Admin).ToString() && x.BranchId == (int)cat.BranchId).OrderBy(x => x.Id).LastOrDefaultAsync())?.Id ?? 1;

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

        private async Task AddPersonnelOtherDetailAsync(Personnel model, int UserId, Personnel cat)
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

        private async Task AddUserInfoAsync(Personnel model, int UserId, Personnel cat)
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

            var objRole = await _context.Role.SingleOrDefaultAsync(x => x.RoleType == ((int)UserType.Admin).ToString() && x.IsActive == ((int)ActiveState.Active).ToString());
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

        public async Task<ServiceResult> DeleteAdmin(int Id, int UserId)
        {
            var result = new ServiceResult();
            var cat = await _context.Personnel.SingleOrDefaultAsync(m => m.Id == Id);
            if (cat != null)
            {
                cat.IsActive = ((int)ActiveState.InActive).ToString();
                cat.ModifiedByUserId = UserId;
                cat.ModifiedDate = DateTime.Now;

                _context.Entry(cat).State = EntityState.Modified;
                if (await _context.SaveChangesAsync() != 0)
                {
                    result.StatusId = (int)ServiceResultStatus.Deleted;
                    result.RecordId = cat.Id;
                }
            }

            return result;
        }

        public async Task<List<SelectListItem>> GetBranchesAsync()
        {
            return await _context.Branch
                .Where(x => x.IsActive == ((int)ActiveState.Active).ToString())
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetBloodGroupAsync()
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
