using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using System.Data;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class StaffService : IStaffService
    {
        private readonly APIContext _context;
        private readonly string _connectionString;
        private readonly HostEnvironmentService _hostingEnvironment;
        private readonly S3Service _s3Service;

        public StaffService(APIContext context, IConfiguration configuration, HostEnvironmentService hostingEnvironment, S3Service s3Service)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _hostingEnvironment = hostingEnvironment;
            _s3Service = s3Service;

        }

        public async Task<List<VStaff>> GetAllStaff(int UserId)
        {
            var objuser = await _context.VUserInfo.AsNoTracking().SingleOrDefaultAsync(x => x.Id == UserId);
            if (objuser == null) return new List<VStaff>();

            return objuser.UserType == ((int)UserType.SuperAdmin).ToString()
                ? await _context.VStaff.AsNoTracking().ToListAsync()
                : await _context.VStaff.AsNoTracking().Where(x => x.BranchId == objuser.BranchId).ToListAsync();
        }

        public async Task<VStaff> ViewStaff(int Id)
        {
            return await _context.VStaff.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
        }

        public async Task<Personnel> GetStaff(int Id)
        {
            var model = new Personnel
            {
                PersonnelAdmission = new PersonnelAdmission(),
                PersonnelOtherDetail = new PersonnelOtherDetail(),
                BloodGroupSheet = new List<SelectListItem>(await GetBloodGroup()),
                BranchSheet = new List<SelectListItem>(await GetBranches())
            };

            if (Id <= 0)
            {
                model.Gender = ((int)Gender.Male).ToString();
                model.FilePath = Shared.GetNoUserPath();
                model.PersonnelType = ((int)UserType.Staff).ToString();
                var sessyear = await _context.SessionYear.AsNoTracking().SingleOrDefaultAsync(x => x.IsActive == ((int)ActiveState.Active).ToString() && x.IsDefault);
                if (sessyear != null)
                {
                    model.PersonnelAdmission.SessionYearId = sessyear.Id;
                }
                return model;
            }

            var cat = await _context.Personnel.AsNoTracking().SingleOrDefaultAsync(x => x.Id == Id && x.IsActive == ((int)ActiveState.Active).ToString() && x.PersonnelType == ((int)UserType.Staff).ToString());
            if (cat == null) return model;

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

            var admission = await _context.PersonnelAdmission.AsNoTracking().SingleOrDefaultAsync(x => x.PersonnelId == model.Id && x.IsActive == ((int)ActiveState.Active).ToString() && x.PersonnelType == model.PersonnelType);
            if (admission != null)
            {
                model.PersonnelAdmission = new PersonnelAdmission
                {
                    Id = admission.Id,
                    PersonnelType = admission.PersonnelType,
                    PersonnelId = admission.PersonnelId,
                    SessionYearId = admission.SessionYearId,
                    AdmissionDate = admission.AdmissionDate,
                    AdmissionNo = admission.AdmissionNo
                };

                if (admission.BranchId > 0)
                {
                    model.BranchId = admission.BranchId;
                    var divbranchItem = model.BranchSheet.Find(p => p.Value == admission.BranchId.ToString());
                    if (divbranchItem != null)
                    {
                        divbranchItem.Selected = true;
                    }
                }
            }

            var otherdetail = await _context.PersonnelOtherDetail.AsNoTracking().SingleOrDefaultAsync(x => x.PersonnelId == model.Id && x.IsActive == ((int)ActiveState.Active).ToString() && x.PersonnelType == model.PersonnelType);
            if (otherdetail != null)
            {
                model.PersonnelOtherDetail = new PersonnelOtherDetail
                {
                    Id = otherdetail.Id,
                    PersonnelType = otherdetail.PersonnelType,
                    PersonnelId = otherdetail.PersonnelId,
                    FatherName = otherdetail.FatherName,
                    MotherName = otherdetail.MotherName,
                    DefaultMobileNo = otherdetail.DefaultMobileNo,
                    OtherMobileNo = otherdetail.OtherMobileNo,
                    CurrentAddress = otherdetail.CurrentAddress,
                    PermanentAddress = otherdetail.PermanentAddress
                };
            }

            return model;
        }

        public async Task<bool> IsRecordExists(string fname, string lname, int BranchId, int Id)
        {
            return Id == 0
                ? await _context.VStaff.AsNoTracking().AnyAsync(e => e.FullName == fname + " " + lname && e.BranchId == BranchId && e.PersonnelType == ((int)UserType.Staff).ToString())
                : await _context.VStaff.AsNoTracking().AnyAsync(e => e.Id != Id && e.FullName == fname + " " + lname && e.BranchId == BranchId && e.PersonnelType == ((int)UserType.Staff).ToString());
        }

        public async Task<ServiceResult> SaveStaff(Personnel model, int UserId)
        {
            ServiceResult result = null;
            try
            {
                if (model.Id > 0)
                {
                    var cat = await _context.Personnel.AsNoTracking().SingleOrDefaultAsync(m => m.Id == model.Id && m.IsActive == ((int)ActiveState.Active).ToString() && m.PersonnelType == ((int)UserType.Staff).ToString());
                    if (cat == null) return result;

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
                        //    string filePath = _hostingEnvironment.GetFullUrlFromDbPath(cat.FilePath);
                        //    if (File.Exists(filePath))
                        //    {
                        //        System.IO.File.Delete(filePath);
                        //    }
                        //}
                        if (!string.IsNullOrEmpty(cat.FilePath) && !cat.FilePath.Contains("nouser.png"))
                        {
                            await _s3Service.DeleteFileAsync(cat.FilePath);
                        }

                        cat.FileName = model.File.FileName;
                        //model.FilePath = Shared.ProcessUploadFile((int)UploadType.Staff, model.File, _hostingEnvironment.GetWebRootPath());
                        //cat.FilePath = model.FilePath;
                        cat.FilePath = await _s3Service.UploadFileAsync((int)UploadType.Staff, model.File);

                    }

                    _context.Entry(cat).State = EntityState.Modified;
                    if (_context.SaveChanges() != 0 && model.PersonnelOtherDetail != null)
                    {
                        var otherdetail = await _context.PersonnelOtherDetail.AsNoTracking().SingleOrDefaultAsync(x => x.PersonnelId == cat.Id && x.IsActive == ((int)ActiveState.Active).ToString() && x.PersonnelType == model.PersonnelType);
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
                            if (await _context.SaveChangesAsync() != 0)
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
                else
                {
                    using (var transaction = _context.Database.BeginTransaction())
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
                                // FilePath = model.File != null ? Shared.ProcessUploadFile((int)UploadType.Staff, model.File, _hostingEnvironment.GetWebRootPath()) : Path.Combine("Uploads", "NoImage", "nouser.png")
                                FilePath = model.File != null
                                 ? await _s3Service.UploadFileAsync((int)UploadType.Staff, model.File)
                                 : Path.Combine("Uploads", "NoImage", "nouser.png")

                            };

                            _context.Personnel.Add(cat);
                            _context.Entry(cat).State = EntityState.Added;
                            if (await _context.SaveChangesAsync() != 0 && model.PersonnelAdmission != null)
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
                                if (await _context.SaveChangesAsync() != 0)
                                {
                                    var lastId = _context.PersonnelAdmission.AsNoTracking().Where(x => x.PersonnelType == ((int)UserType.Staff).ToString() && x.BranchId == (int)cat.BranchId).OrderBy(x => x.Id).LastOrDefault()?.Id ?? 1;

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
                                    if (await _context.SaveChangesAsync() != 0 && model.PersonnelOtherDetail != null)
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
                                        if (await _context.SaveChangesAsync() != 0)
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
                                            if (await _context.SaveChangesAsync() != 0)
                                            {
                                                var objRole = await _context.Role.AsNoTracking().SingleOrDefaultAsync(x => x.RoleType == ((int)UserType.Staff).ToString() && x.IsActive == ((int)ActiveState.Active).ToString());
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
                                                    if (await _context.SaveChangesAsync() != 0)
                                                    {
                                                        transaction.Commit();
                                                        result = new ServiceResult
                                                        {
                                                            StatusId = (int)ServiceResultStatus.Added,
                                                            RecordId = cat.Id
                                                        };
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        public async Task<ServiceResult> DeleteStaff(int Id, int UserId)
        {
            var cat = await _context.Personnel.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
            if (cat == null) return null;

            cat.IsActive = ((int)ActiveState.InActive).ToString();
            cat.ModifiedByUserId = UserId;
            cat.ModifiedDate = DateTime.Now;
            _context.Entry(cat).State = EntityState.Modified;
            if (await _context.SaveChangesAsync() == 0) return null;

            return new ServiceResult
            {
                StatusId = (int)ServiceResultStatus.Deleted,
                RecordId = cat.Id
            };
        }

        public async Task<List<SelectListItem>> GetBranches()
        {
            return await _context.Branch.AsNoTracking()
                .Where(x => x.IsActive == ((int)ActiveState.Active).ToString())
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetBloodGroup()
        {
            return await _context.BloodGroup.AsNoTracking()
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
