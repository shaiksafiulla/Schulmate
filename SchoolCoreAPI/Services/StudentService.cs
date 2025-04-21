using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class StudentService : IStudentService
    {
        private readonly APIContext _context;
        private readonly HostEnvironmentService _hostingEnvironment;
        private readonly S3Service _s3Service;

        public StudentService(APIContext context, HostEnvironmentService hostingEnvironment, S3Service s3Service)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _s3Service = s3Service;
        }
        public async Task<List<VStudents>> GetAllStudent(int UserId)
        {
            List<VStudents> result = new List<VStudents>();
            try
            {
                var objuser = await _context.VUserInfo.AsNoTracking().SingleOrDefaultAsync(x => x.Id == UserId);
                if (objuser != null)
                {
                    result = objuser.UserType == ((int)UserType.SuperAdmin).ToString()
                        ? await _context.VStudents.AsNoTracking().ToListAsync()
                        : await _context.VStudents.AsNoTracking().Where(x => x.BranchId == objuser.BranchId).ToListAsync();

                    foreach (var item in result)
                    {
                        if (!string.IsNullOrEmpty(item.StrSpark))
                        {
                            item.LstSpark = item.StrSpark.Split(',')
                                .Select(s => decimal.TryParse(s, out decimal n) ? (decimal?)n : null)
                                .ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here if needed
            }

            return result;
        }
        public async Task<List<MbStudent>> SearchMbStudents(int PersonnelId, int BranchId, string PersonnelType, int SessionYearId, string query)
        {
            List<MbStudent> result = new List<MbStudent>();
            if (PersonnelId > 0 && SessionYearId > 0)
            {
                List<int> clsids = new List<int>();
                if (PersonnelType == ((int)UserType.Teacher).ToString())
                {
                    var lstsec = await _context.BranchClassSectionTeacher.AsNoTracking()
                        .Where(x => x.BranchId == BranchId && x.TeacherId == PersonnelId && x.SessionYearId == SessionYearId && x.IsActive == ((int)ActiveState.Active).ToString())
                        .Select(x => x.SectionId)
                        .ToListAsync();
                    clsids.AddRange(lstsec);

                    var lstbrcls = await _context.BranchClassSectionSubjectTeacher.AsNoTracking()
                        .Where(x => x.BranchId == BranchId && x.TeacherId == PersonnelId && x.SessionYearId == SessionYearId && x.IsActive == ((int)ActiveState.Active).ToString())
                        .Select(x => x.SectionId)
                        .ToListAsync();
                    clsids.AddRange(lstbrcls);
                }

                var lststudent = await _context.VStudents.AsNoTracking()
                    .Where(s => (PersonnelType != ((int)UserType.Teacher).ToString() || clsids.Contains((int)s.SectionId)) &&
                                s.BranchId == BranchId && s.SessionYearId == SessionYearId &&
                                (s.FullName.Contains(query) || s.RollNo.Contains(query) || s.DefaultMobileNo.Contains(query) || s.AdmissionNo.Contains(query)))
                    .Take(10)
                    .ToListAsync();

                result = lststudent.Select(c => new MbStudent
                {
                    Id = (int)c.Id,
                    Name = c.FullName,
                    MobileNo = c.DefaultMobileNo,
                    RollNo = c.RollNo,
                    AdmissionNo = c.AdmissionNo,
                    FilePath = string.IsNullOrEmpty(c.FilePath) ? Shared.GetNoUserPath() : c.FilePath,
                    ClassSectionName = c.ClassName + " " + c.SectionName
                }).ToList();
            }
            return result;
        }
        public async Task<List<MbStudent>> LoadDefaultMbStudents(int PersonnelId, int BranchId, string PersonnelType, int SessionYearId)
        {
            List<MbStudent> result = new List<MbStudent>();
            if (PersonnelId > 0 && SessionYearId > 0)
            {
                List<int> clsids = new List<int>();
                if (PersonnelType == ((int)UserType.Teacher).ToString())
                {
                    var lstsec = await _context.BranchClassSectionTeacher.AsNoTracking()
                        .Where(x => x.BranchId == BranchId && x.TeacherId == PersonnelId && x.SessionYearId == SessionYearId && x.IsActive == ((int)ActiveState.Active).ToString())
                        .Select(x => x.SectionId)
                        .ToListAsync();
                    clsids.AddRange(lstsec);

                    var lstbrcls =  await _context.BranchClassSectionSubjectTeacher.AsNoTracking()
                        .Where(x => x.BranchId == BranchId && x.TeacherId == PersonnelId && x.SessionYearId == SessionYearId && x.IsActive == ((int)ActiveState.Active).ToString())
                        .Select(x => x.SectionId)
                        .ToListAsync();
                    clsids.AddRange(lstbrcls);
                }

                var lststudent = await _context.VStudents.AsNoTracking()
                    .Where(s => (PersonnelType != ((int)UserType.Teacher).ToString() || clsids.Contains((int)s.SectionId)) &&
                                s.BranchId == BranchId && s.SessionYearId == SessionYearId)
                    .Take(10)
                    .ToListAsync();

                result = lststudent.Select(c => new MbStudent
                {
                    Id = (int)c.Id,
                    Name = c.FullName,
                    MobileNo = c.DefaultMobileNo,
                    RollNo = c.RollNo,
                    AdmissionNo = c.AdmissionNo,
                    FilePath = string.IsNullOrEmpty(c.FilePath) ? Shared.GetNoUserPath() : c.FilePath,
                    ClassSectionName = c.ClassName + " " + c.SectionName
                }).ToList();
            }
            return result;
        }
        public async Task<MbStudentPay> GetAllPaymentByStudentId(int StudentId, int UserId)
        {
            try
            {
                var objStudent = await _context.VStudents.AsNoTracking().SingleOrDefaultAsync(x => x.Id == StudentId);
                if (objStudent == null)
                {
                    return null;
                }

                var lst = await _context.VStudentPay.AsNoTracking().Where(x => x.StudentId == StudentId).OrderByDescending(x => x.Id).ToListAsync();
                return new MbStudentPay
                {
                    StudentId = (int)objStudent.Id,
                    FinalAmount = objStudent.FinalAmount,
                    PaidAmount = objStudent.PaidAmount,
                    DueAmount = objStudent.DueAmount,
                    LstStudentPay = lst
                };
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here if needed
                return null;
            }
        }
        public async Task<VStudentFeeReceipt> GetStudentFeeReceipt(int PaymentId)
        {
            var objPay= await _context.VStudentFeeReceipt.AsNoTracking().SingleOrDefaultAsync(x => x.Id == PaymentId);
            if (objPay == null)
            {
                return null;
            }
            return objPay;

        }
        public async Task<VStudents> ViewStudent(int Id)
        {
            var objstudent = await _context.VStudents.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
            if(objstudent != null)
            {
                if (!string.IsNullOrEmpty(objstudent.StrSpark))
                {
                    objstudent.LstSpark = objstudent.StrSpark.Split(',')
                        .Select(s => decimal.TryParse(s, out decimal n) ? (decimal?)n : null)
                        .ToList();
                }
                return objstudent;
            }
            return null;
               
        }
        public async Task<StudentDetail> GetStudentDetail(int Id, int BranchId)
        {
            var objsch = await _context.VStudents.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
            if (objsch == null)
            {
                return null;
            }

            var objother = await _context.StudentOtherDetail.AsNoTracking().SingleOrDefaultAsync(x => x.StudentId == Id);
            var objsession = await _context.SessionYear.AsNoTracking().SingleOrDefaultAsync(x => x.IsDefault && x.IsActive == ((int)ActiveState.Active).ToString());
            var rpt = await _context.ReportSettings.AsNoTracking().FirstOrDefaultAsync(x=>x.BranchId==BranchId);

            return new StudentDetail
            {
                Id = (int)objsch.Id,
                SessionYearId = (int)objsession?.Id,
                FilePath = objsch.FilePath,
                FullName = objsch.FullName,
                Age = objsch.Age,
                GenderName = objsch.GenderName,
                FatherName = objsch.FatherName,
                MotherName = objsch.MotherName,
                DefaultMobileNo = objsch.DefaultMobileNo,
                BranchName = objsch.BranchName,
                ClassName = objsch.ClassName,
                SectionName = objsch.SectionName,
                AdmissionDate = objsch.AdmissionDate,
                AdmissionNo = objsch.AdmissionNo,
                RollNo = objsch.RollNo,
                ActiveStatus = objsch.ActiveStatus,
                AlternateMobileNo = objother?.OtherMobileNo,
                PermanentAddress = objother?.PermanentAddress,
                CurrentAddress = objother?.CurrentAddress,
                ReportSettings = rpt
            };
        }
        public async Task<Student> GetStudent(int Id, int UserId)
        {
            var model = new Student
            {
                StudentAdmission = new StudentAdmission(),
                StudentOtherDetail = new StudentOtherDetail(),
                BloodGroupSheet = new List<SelectListItem>(await GetBloodGroup())
            };

            if (Id > 0)
            {
                var cat = await _context.Student.AsNoTracking().SingleOrDefaultAsync(x => x.Id == Id && x.IsActive == ((int)ActiveState.Active).ToString() && x.PersonnelType == ((int)UserType.Student).ToString());
                if (cat == null)
                {
                    return model;
                }

                model.Id = cat.Id;
                model.PersonnelType = cat.PersonnelType;
                model.FirstName = cat.FirstName;
                model.LastName = cat.LastName;
                model.Gender = cat.Gender;
                model.DateOfBirth = cat.DateOfBirth;
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

                var admission = await _context.StudentAdmission.AsNoTracking().SingleOrDefaultAsync(x => x.StudentId == model.Id && x.IsActive == ((int)ActiveState.Active).ToString());
                if (admission != null)
                {
                    model.BranchId = admission.BranchId;
                    model.ClassId = admission.ClassId;
                    model.SectionId = admission.SectionId;
                    model.StudentAdmission = new StudentAdmission
                    {
                        Id = admission.Id,
                        StudentId = admission.StudentId,
                        SessionYearId = admission.SessionYearId,
                        AdmissionDate = admission.AdmissionDate,
                        AdmissionNo = admission.AdmissionNo
                    };

                    var lstcls = await GetClassesByBranch(admission.BranchId);
                    model.ClassSheet = new List<SelectListItem>(lstcls);
                    var divbranchItem = model.ClassSheet.Find(p => p.Value == admission.ClassId.ToString());
                    if (divbranchItem != null)
                    {
                        divbranchItem.Selected = true;
                    }

                    var lstsec = await GetSectionsByBranchAndClass(admission.BranchId, admission.ClassId);
                    model.SectionSheet = new List<SelectListItem>(lstsec);
                    var divsecItem = model.SectionSheet.Find(p => p.Value == admission.SectionId.ToString());
                    if (divsecItem != null)
                    {
                        divsecItem.Selected = true;
                    }
                }

                var otherdetail = await _context.StudentOtherDetail.AsNoTracking().SingleOrDefaultAsync(x => x.StudentId == model.Id && x.IsActive == ((int)ActiveState.Active).ToString());
                if (otherdetail != null)
                {
                    model.StudentOtherDetail = new StudentOtherDetail
                    {
                        Id = otherdetail.Id,
                        StudentId = otherdetail.StudentId,
                        FatherName = otherdetail.FatherName,
                        MotherName = otherdetail.MotherName,
                        DefaultMobileNo = otherdetail.DefaultMobileNo,
                        OtherMobileNo = otherdetail.OtherMobileNo,
                        CurrentAddress = otherdetail.CurrentAddress,
                        PermanentAddress = otherdetail.PermanentAddress
                    };
                }
            }
            else
            {
                model.Gender = ((int)Gender.Male).ToString();
                model.FilePath = Shared.GetNoUserPath();
                model.PersonnelType = ((int)UserType.Student).ToString();
                var sessyear = await _context.SessionYear.AsNoTracking().SingleOrDefaultAsync(x => x.IsActive == ((int)ActiveState.Active).ToString() && x.IsDefault);
                model.StudentAdmission.SessionYearId = (int)sessyear?.Id;

                var objuser = await _context.VUserInfo.AsNoTracking().SingleOrDefaultAsync(x => x.Id == UserId);
                model.BranchId = objuser?.BranchId ?? 0;
                model.BranchClassId = 0;

                var lstcls = await GetClassesByBranch((int)model.BranchId);
                model.ClassSheet = new List<SelectListItem>(lstcls);
                model.SectionSheet = new List<SelectListItem>();
            }

            return model;
        }
        public async Task<bool> IsRecordExists(string fname, string lname, int Id)
        {
            return Id == 0
                ? await _context.Student.AsNoTracking().AnyAsync(e => e.FirstName == fname && e.LastName == lname && e.IsActive == ((int)ActiveState.Active).ToString())
                : await _context.Student.AsNoTracking().AnyAsync(e => e.FirstName == fname && e.LastName == lname && e.Id != Id && e.IsActive == ((int)ActiveState.Active).ToString());
        }
        public async Task<List<SelectListItem>> GetBloodGroup()
        {
            return await _context.BloodGroup.AsNoTracking()
                .Where(x => x.IsActive == ((int)ActiveState.Active).ToString())
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                .ToListAsync();
        }
        public async Task<List<SelectListItem>> GetPayModes()
        {
            return await _context.PayMode.AsNoTracking()
                .Where(x => x.IsActive == ((int)ActiveState.Active).ToString())
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                .ToListAsync();
        }
        public async Task<List<SelectListItem>> GetClassesByBranch(int branchId)
        {
            var subids = await _context.BranchClass.AsNoTracking()
                .Where(x => x.BranchId == branchId)
                .Select(x => x.ClassId)
                .ToListAsync();

            return await _context.Class.AsNoTracking()
                .Where(x => x.IsActive == ((int)ActiveState.Active).ToString() && subids.Contains(x.Id))
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                .ToListAsync();
        }
        public async Task<List<SelectListItem>> GetSectionsByBranchAndClass(int branchId, int classId)
        {
            var objBrCls = await _context.BranchClass.AsNoTracking()
                .Where(x => x.BranchId == branchId && x.ClassId == classId)
                .Select(x => x.Id)
                .SingleOrDefaultAsync();

            return await _context.Section
                .Where(x => x.IsActive == ((int)ActiveState.Active).ToString() && x.BranchClassId == objBrCls)
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                .ToListAsync();
        }
        public async Task<ServiceResult> SaveStudent(Student model, int UserId)
        {
            ServiceResult result = null;
            try
            {
                if (model.Id > 0)
                {
                    // Update existing student
                    var cat = await _context.Student
                        .SingleOrDefaultAsync(m => m.Id == model.Id && m.IsActive == ((int)ActiveState.Active).ToString() && m.PersonnelType == ((int)UserType.Student).ToString());

                    if (cat == null) return result;

                    cat.FirstName = model.FirstName;
                    cat.LastName = model.LastName;
                    cat.Gender = model.Gender;
                    cat.BloodGroupId = model.BloodGroupId;
                    cat.DateOfBirth = model.DateOfBirth;
                    cat.LastModifiedDate = DateTime.Now;
                    cat.LastModifiedByUserId = UserId;

                    if (model.File != null)
                    {
                        //if (!string.IsNullOrEmpty(cat.FilePath) && !cat.FilePath.Contains("nouser.png"))
                        //{
                        //    string filePath = _hostingEnvironment.GetFullUrlFromDbPath(cat.FilePath);
                        //    if (File.Exists(filePath))
                        //    {
                        //        File.Delete(filePath);
                        //    }
                        //}
                        if (!string.IsNullOrEmpty(cat.FilePath) && !cat.FilePath.Contains("nouser.png"))
                        {
                            await _s3Service.DeleteFileAsync(cat.FilePath);
                        }

                        cat.FileName = model.File.FileName;
                        // cat.FilePath = Shared.ProcessUploadFile((int)UploadType.Student, model.File, _hostingEnvironment.GetWebRootPath());
                        cat.FilePath = await _s3Service.UploadFileAsync((int)UploadType.Student, model.File);

                    }

                    _context.Entry(cat).State = EntityState.Modified;
                    if (_context.SaveChanges() > 0 && model.StudentOtherDetail != null)
                    {
                        var otherDetail = _context.StudentOtherDetail.SingleOrDefault(x => x.StudentId == cat.Id && x.IsActive == ((int)ActiveState.Active).ToString());
                        if (otherDetail != null)
                        {
                            otherDetail.FatherName = model.StudentOtherDetail.FatherName;
                            otherDetail.MotherName = model.StudentOtherDetail.MotherName;
                            otherDetail.DefaultMobileNo = model.StudentOtherDetail.DefaultMobileNo;
                            otherDetail.OtherMobileNo = model.StudentOtherDetail.OtherMobileNo;
                            otherDetail.CurrentAddress = model.StudentOtherDetail.CurrentAddress;
                            otherDetail.PermanentAddress = model.StudentOtherDetail.PermanentAddress;
                            otherDetail.LastModifiedDate = DateTime.Now;
                            otherDetail.LastModifiedByUserId = UserId;

                            _context.Entry(otherDetail).State = EntityState.Modified;
                            if (await _context.SaveChangesAsync() > 0)
                            {
                                result = new ServiceResult { StatusId = (int)ServiceResultStatus.Updated, RecordId = cat.Id };
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
                            // Add new student
                            var cat = new Student
                            {
                                PersonnelType = model.PersonnelType,
                                FirstName = model.FirstName,
                                LastName = model.LastName,
                                Gender = model.Gender,
                                DateOfBirth = model.DateOfBirth,
                                BloodGroupId = model.BloodGroupId,
                                IsActive = ((int)ActiveState.Active).ToString(),
                                CreatedDate = DateTime.Now,
                                CreatedByUserId = UserId,
                                BranchId = model.BranchId,
                                ClassId = model.ClassId,
                                SectionId = model.SectionId,
                                FileName = model.File?.FileName ?? "nouser.png",
                                // FilePath = model.File != null ? Shared.ProcessUploadFile((int)UploadType.Student, model.File, _hostingEnvironment.GetWebRootPath()) : Path.Combine("Uploads", "NoImage", "nouser.png")
                                FilePath = model.File != null
                                 ? await _s3Service.UploadFileAsync((int)UploadType.Student, model.File)
                                 : Path.Combine("Uploads", "NoImage", "nouser.png")

                            };

                            _context.Student.Add(cat);
                            _context.Entry(cat).State = EntityState.Added;
                            if (_context.SaveChanges() > 0 && model.StudentAdmission != null)
                            {
                                var admission = new StudentAdmission
                                {
                                    StudentId = cat.Id,
                                    BranchId = (int)cat.BranchId,
                                    ClassId = (int)cat.ClassId,
                                    BranchClassId = _context.BranchClass.AsNoTracking().SingleOrDefault(x => x.BranchId == cat.BranchId && x.ClassId == cat.ClassId)?.Id ?? 0,
                                    SectionId = (int)cat.SectionId,
                                    SessionYearId = model.StudentAdmission.SessionYearId,
                                    AdmissionDate = model.StudentAdmission.AdmissionDate,
                                    AdmissionNo = $"{cat.BranchId}-{DateTime.Now:HHmmss}",
                                    IsActive = ((int)ActiveState.Active).ToString(),
                                    CreatedDate = DateTime.Now,
                                    CreatedByUserId = UserId
                                };

                                _context.StudentAdmission.Add(admission);
                                _context.Entry(admission).State = EntityState.Added;
                                if (_context.SaveChanges() > 0)
                                {
                                    var lastId = _context.StudentAdmission.AsNoTracking().Where(x => x.BranchId == cat.BranchId && x.ClassId == cat.ClassId && x.SectionId == cat.SectionId && x.SessionYearId == model.StudentAdmission.SessionYearId).OrderByDescending(x => x.Id).FirstOrDefault()?.Id ?? 0;

                                    var transfer = new StudentTransferSection
                                    {
                                        SessionYearId = admission.SessionYearId,
                                        StudentId = admission.StudentId,
                                        FromBranchId = admission.BranchId,
                                        FromClassId = admission.ClassId,
                                        FromSectionId = admission.SectionId,
                                        ToBranchId = admission.BranchId,
                                        ToClassId = admission.ClassId,
                                        ToSectionId = admission.SectionId,
                                        RollNo = $"{cat.BranchId}-{cat.ClassId}-{cat.SectionId}-00{lastId + 1}",
                                        TransferType = ((int)TransferType.Admitted).ToString(),
                                        CreatedByUserId = UserId,
                                        CreatedDate = DateTime.Now
                                    };

                                    _context.StudentTransferSection.Add(transfer);
                                    _context.Entry(transfer).State = EntityState.Added;
                                    if (_context.SaveChanges() > 0 && model.StudentOtherDetail != null)
                                    {
                                        var otherModel = new StudentOtherDetail
                                        {
                                            StudentId = cat.Id,
                                            FatherName = model.StudentOtherDetail.FatherName,
                                            MotherName = model.StudentOtherDetail.MotherName,
                                            DefaultMobileNo = model.StudentOtherDetail.DefaultMobileNo,
                                            OtherMobileNo = model.StudentOtherDetail.OtherMobileNo,
                                            CurrentAddress = model.StudentOtherDetail.CurrentAddress,
                                            PermanentAddress = model.StudentOtherDetail.PermanentAddress,
                                            IsActive = ((int)ActiveState.Active).ToString(),
                                            CreatedDate = DateTime.Now,
                                            CreatedByUserId = UserId
                                        };

                                        _context.StudentOtherDetail.Add(otherModel);
                                        _context.Entry(otherModel).State = EntityState.Added;
                                        if (_context.SaveChanges() > 0)
                                        {
                                            var objFeeCls =  _context.FeeStructureClass.AsNoTracking().SingleOrDefault(x => x.ClassId == admission.ClassId && x.BranchClassId == admission.BranchClassId && x.BranchId == admission.BranchId && x.SessionYearId == admission.SessionYearId);
                                            if (objFeeCls != null)
                                            {
                                                var feeStruct = _context.FeeStructure.AsNoTracking().SingleOrDefault(x => x.Id == objFeeCls.FeeStructureId);
                                                if (feeStruct != null)
                                                {
                                                    var objStudFee = new StudentFee
                                                    {
                                                        StudentId = admission.StudentId,
                                                        FeeStructureId = feeStruct.Id,
                                                        BranchId = objFeeCls.BranchId,
                                                        SessionYearId = objFeeCls.SessionYearId,
                                                        BranchClassId = objFeeCls.BranchClassId,
                                                        ClassId = objFeeCls.ClassId,
                                                        TotalAmount = feeStruct.TotalAmount,
                                                        RequestDiscountAmount = 0,
                                                        IsActive = ((int)ActiveState.Active).ToString(),
                                                        CreatedDate = DateTime.Now,
                                                        CreatedByUserId = UserId
                                                    };

                                                    _context.StudentFee.Add(objStudFee);
                                                    _context.Entry(objStudFee).State = EntityState.Added;
                                                    if (_context.SaveChanges() > 0)
                                                    {
                                                        var userInfo = new userinfo
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

                                                        _context.userinfo.Add(userInfo);
                                                        _context.Entry(userInfo).State = EntityState.Added;
                                                        if (_context.SaveChanges() > 0)
                                                        {
                                                            var objRole =  _context.Role.AsNoTracking().SingleOrDefault(x => x.RoleType == ((int)UserType.Student).ToString() && x.IsActive == ((int)ActiveState.Active).ToString());
                                                            if (objRole != null)
                                                            {
                                                                var usr = new UserRole
                                                                {
                                                                    UserId = userInfo.Id,
                                                                    RoleId = objRole.Id,
                                                                    CreatedByUserId = UserId,
                                                                    CreatedDate = DateTime.Now
                                                                };

                                                                _context.UserRole.Add(usr);
                                                                if (await _context.SaveChangesAsync() > 0)
                                                                {
                                                                    transaction.Commit();
                                                                    result = new ServiceResult { StatusId = (int)ServiceResultStatus.Added, RecordId = cat.Id };
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception if needed
            }
            return result;
        }
        public async Task<ServiceResult> DeleteStudent(int Id, int UserId)
        {
            var cat = await _context.Student.SingleOrDefaultAsync(m => m.Id == Id);
            if (cat == null) return null;

            cat.IsActive = ((int)ActiveState.InActive).ToString();
            cat.LastModifiedByUserId = UserId;
            cat.LastModifiedDate = DateTime.Now;
            _context.Entry(cat).State = EntityState.Modified;

            if (await _context.SaveChangesAsync() > 0)
            {
                return new ServiceResult
                {
                    StatusId = (int)ServiceResultStatus.Deleted,
                    RecordId = cat.Id
                };
            }
            return null;
        }
        public async Task<StudentPay> GetStudentPay(int StudentId)
        {
            var objStudent = await _context.VStudents.AsNoTracking().SingleOrDefaultAsync(v => v.Id == StudentId);
            if (objStudent == null) return null;

            return new StudentPay
            {
                StudentId = (int)objStudent.Id,
                StudentFeeId = (int)objStudent.StudentFeeId,
                FeeStructureId = (int)objStudent.FeeStructureId,
                BranchId = (int)objStudent.BranchId,
                SessionYearId = (int)objStudent.SessionYearId,
                BranchClassId = (int)objStudent.BranchClassId,
                ClassId = (int)objStudent.ClassId,
                StudentName = objStudent.FullName,
                AdmissionNo = objStudent.AdmissionNo,
                RollNo = objStudent.RollNo,
                Age = objStudent.Age,
                Gender = objStudent.GenderName,
                MobileNo = objStudent.DefaultMobileNo,
                BranchName = objStudent.BranchName,
                ClassName = objStudent.ClassName,
                SectionName = objStudent.SectionName,
                DueAmount = objStudent.DueAmount > 0 ? (decimal)objStudent.DueAmount : 0,
                PayModeSheet = await GetPayModes()
            };
        }
        public async Task<ServiceResult> SaveStudentPay(StudentPay model, int UserId)
        {
            if (model == null || model.StudentId <= 0) return null;

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var maxId = 0;
                    var maxObj = _context.StudentPay.AsNoTracking().OrderByDescending(x => x.Id).FirstOrDefault();
                    if (maxObj != null)
                    {
                        maxId = maxObj.Id + 1;
                    }
                   // var dueAmount = _context.VStudents.AsNoTracking().SingleOrDefault(x => x.Id == model.StudentId)?.DueAmount ?? 0;
                    var pay = new StudentPay
                    {
                        StudentId = model.StudentId,
                        ReceiptNo = (maxId + "-" + model.BranchId + "-" + DateTime.Now.Day + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second),
                        StudentFeeId = model.StudentFeeId,
                        FeeStructureId = model.FeeStructureId,
                        BranchId = model.BranchId,
                        SessionYearId = model.SessionYearId,
                        BranchClassId = model.BranchClassId,
                        ClassId = model.ClassId,
                        Amount = model.Amount,
                        DueAmount = model.DueAmount,
                        PayModeId = model.PayModeId,
                        ReferenceNo = model.ReferenceNo,
                        Remarks = model.Remarks,
                        CreatedDate = DateTime.Now,
                        CreatedByUserId = UserId
                    };

                    _context.StudentPay.Add(pay);
                    _context.Entry(pay).State = EntityState.Added;

                    if (await _context.SaveChangesAsync() > 0)
                    {
                        await transaction.CommitAsync();

                        return new ServiceResult
                        {
                            StatusId = (int)ServiceResultStatus.Updated,
                            RecordId = pay.Id
                        };
                    }
                    
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    // Handle the exception
                }
            }          

            
            return null;
        }
        public async Task<StudentScheduleVM> GetStudentScheduleVM(int Id, int SessionYearId)
        {
            var revlst = await _context.VStudentScheduleResults.AsNoTracking()
                .Where(x => x.StudentId == Id && x.SessionYearId == SessionYearId)
                .OrderBy(x => x.ScheduleYearNo)
                .ThenBy(x => x.ScheduleMonthNo)
                .ToListAsync();

            var childVM = new StudentScheduleChildVM
            {
                label = "Performance",
                backgroundColor = new List<string> { "#6C63FF" },
                borderWidth = 1,
                fill = true,
                data = revlst.Select(item => new ScheduleLabel
                {
                    Strlabel = item.ScheduleMonth,
                    Percent = item.MrkPercent,
                    ScheduleId = item.ScheduleId
                }).ToList()
            };

            return new StudentScheduleVM
            {
                LstStudentScheduleResult = revlst,
                StudentScheduleGraph = new StudentScheduleGraphVM
                {
                    labels = revlst.Select(x => x.ScheduleMonth).ToList(),
                    datasets = new List<StudentScheduleChildVM> { childVM }
                }
            };
        }
        public async Task<StudentScheduleResultVM> GetStudentScheduleResultVM(int Id, int ReportCardId, int SessionYearId)
        {
            var lstrptcardsched = await _context.ReportCardSchedule.AsNoTracking()
                .Where(x => x.ReportCardId == ReportCardId && x.SessionYearId == SessionYearId)
                .Select(x => x.ScheduleId)
                .ToListAsync();

            var revlst = await _context.VStudentScheduleResults.AsNoTracking()
                .Where(x => x.StudentId == Id && lstrptcardsched.Contains(x.ScheduleId))
                .OrderBy(y => y.ScheduleYearNo)
                .ThenBy(y => y.ScheduleMonthNo)
                .ToListAsync();

            var scheduleIds = revlst.Select(r => r.ScheduleId).ToList();
            var marksDict = await _context.VStudentMarkResults.AsNoTracking()
                .Where(x => x.StudentId == Id && scheduleIds.Contains(x.ScheduleId))
                .OrderByDescending(x => x.Marks)
                .GroupBy(x => x.ScheduleId)
                .ToDictionaryAsync(g => g.Key, g => g.ToList());

            foreach (var item in revlst)
            {
                if (marksDict.TryGetValue(item.ScheduleId, out var lstmrks))
                {
                    item.LstStudentMarkResult = lstmrks;
                    item.StudentMarkGraph = new StudentMarkGraphVM
                    {
                        labels = lstmrks.Select(x => x.SubjectName).ToList(),
                        datasets = new List<StudentMarkChildVM>
                        {
                            new StudentMarkChildVM
                            {
                                label = "",
                                backgroundColor = lstmrks.Select(x => x.GradeColor).ToList(),
                                borderWidth = 1,
                                fill = true,
                                data = lstmrks.Select(x => int.Parse(x.Marks)).ToList()
                            }
                        }
                    };
                }
            }

            var childVM = new StudentScheduleChildVM
            {
                label = "Performance",
                backgroundColor = new List<string> { "#0eb5ed" },
                borderWidth = 1,
                fill = true,
                data = revlst.Select(item => new ScheduleLabel
                {
                    Strlabel = item.ScheduleMonth,
                    Percent = decimal.Parse(item.Marks.ToString()),
                    ScheduleId = item.ScheduleId
                }).ToList()
            };

            return new StudentScheduleResultVM
            {
                LstStudentScheduleResult = revlst,
                StudentScheduleGraph = new StudentScheduleGraphVM
                {
                    labels = revlst.Select(x => x.ScheduleMonth).ToList(),
                    datasets = new List<StudentScheduleChildVM> { childVM }
                }
            };
        }
        public async Task<List<MbReportCard>> GetStudentMbReportCardList(int StudentId, int SessionYearId)
        {
            var scheduleIds = await _context.VStudentScheduleResults.AsNoTracking()
                .Where(x => x.StudentId == StudentId && x.SessionYearId == SessionYearId)
                .Select(x => x.ScheduleId)
                .ToListAsync();

            var reportCardIds = await _context.ReportCardSchedule.AsNoTracking()
                .Where(x => scheduleIds.Contains(x.ScheduleId) && x.SessionYearId == SessionYearId)
                .Select(x => x.ReportCardId)
                .ToListAsync();

            return await _context.ReportCard.AsNoTracking()
                .Where(x => reportCardIds.Contains(x.Id) && x.SessionYearId == SessionYearId)
                .Select(x => new MbReportCard
                {
                    Id = x.Id,
                    StudentId = StudentId,
                    Name = x.Name
                }).ToListAsync();
        }
        public async Task<StudentMarkVM> GetStudentMarkVM(int Id, int ScheduleId)
        {
            var objsched = await _context.VStudentScheduleResults.AsNoTracking()
                .SingleOrDefaultAsync(x => x.StudentId == Id && x.ScheduleId == ScheduleId);

            if (objsched == null) return null;

            var revlst = await _context.VStudentMarkResults
                .Where(x => x.StudentId == Id && x.ScheduleId == ScheduleId)
                .OrderByDescending(x => x.Marks)
                .ToListAsync();

            objsched.LstStudentMarkResult = revlst;
            objsched.StudentMarkGraph = new StudentMarkGraphVM
            {
                labels = revlst.Select(x => x.SubjectName).ToList(),
                datasets = new List<StudentMarkChildVM>
                {
                    new StudentMarkChildVM
                    {
                        label = "",
                        backgroundColor = revlst.Select(x => x.GradeColor).ToList(),
                        borderWidth = 1,
                        fill = true,
                        data = revlst.Select(x => int.Parse(x.Marks)).ToList()
                    }
                }
            };

            return new StudentMarkVM
            {
                StudentScheduleResult = objsched
            };
        }
        public async Task<List<SelectListItem>> GetStudentSubjects(int StudentId, int SessionYearId)
        {
            return await _context.VStudentMarkResults.AsNoTracking()
                .Where(x => x.StudentId == StudentId && x.SessionYearId == SessionYearId)
                .GroupBy(p => new { p.SubjectId, p.SubjectName })
                .Select(g => new SelectListItem { Text = g.Key.SubjectName, Value = g.Key.SubjectId.ToString() })
                .ToListAsync();
        }
        public async Task<StudentMarkSubjectGraphVM> GetStudentSubjectGraphVM(int StudentId, int SubjectId, int SessionYearId)
        {
            var revlst = await _context.VStudentMarkResults.AsNoTracking()
                .Where(x => x.StudentId == StudentId && x.SessionYearId == SessionYearId && x.SubjectId == SubjectId)
                .OrderBy(x => x.YearNo).ThenBy(x => x.MonthNo)
                .ToListAsync();

            return new StudentMarkSubjectGraphVM
            {
                labels = revlst.Select(x => x.StrExamMonth).ToList(),
                datasets = new List<StudentMarkSubjectChildVM>
                {
                    new StudentMarkSubjectChildVM
                    {
                        label = string.Empty,
                        backgroundColor = new List<string> { "#6C63FF" },
                        borderWidth = 1,
                        fill = false,
                        data = revlst.Select(x => x.MrkPercent).ToList()
                    }
                }
            };
        }
        public async Task<StudentMarkSubjectVM> GetStudentMarkSubjectVM(int Id, string Subject, int SessionYearId)
        {
            var revlst = await _context.VStudentMarkResults.AsNoTracking()
                .Where(x => x.StudentId == Id && x.SessionYearId == SessionYearId && x.SubjectName == Subject)
                .OrderBy(x => x.YearNo).ThenBy(x => x.MonthNo)
                .ToListAsync();

            if (revlst.Count == 0) return new StudentMarkSubjectVM();

            foreach (var item in revlst)
            {
                item.LstExaminationLesson = await _context.VExaminationLessons.AsNoTracking()
                    .Where(x => x.ExamId == item.ExamId)
                    .ToListAsync();
            }

            return new StudentMarkSubjectVM
            {
                LstStudentMarkResult = revlst,
                StudentMarkSubjectGraph = new StudentMarkSubjectGraphVM
                {
                    labels = revlst.Select(x => x.StrExamMonth).ToList(),
                    datasets = new List<StudentMarkSubjectChildVM>
                    {
                        new StudentMarkSubjectChildVM
                        {
                            label = Subject,
                            backgroundColor = new List<string> { "#0eb5ed" },
                            borderWidth = 1,
                            fill = false,
                            data = revlst.Select(x => x.MrkPercent).ToList()
                        }
                    }
                }
            };
        }
        public async Task<List<SelectListItem>> GetBranches()
        {
            return await _context.Branch.AsNoTracking()
                .Where(x => x.IsActive == ((int)ActiveState.Active).ToString())
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                .ToListAsync();
        }
        public async Task<List<SelectListItem>> GetClasses()
        {
            return await _context.Class.AsNoTracking()
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
