using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using System.Data;
using System.Dynamic;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly APIContext _context;
        private readonly string _connectionString;
        private readonly HostEnvironmentService _hostingEnvironment;
        private readonly S3Service _s3Service;
        public TeacherService(APIContext context, IConfiguration configuration, HostEnvironmentService hostingEnvironment, S3Service s3Service)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _hostingEnvironment = hostingEnvironment;
            _s3Service = s3Service;
        }
        public async Task<List<VTeachers>> GetAllTeacher(int UserId)
        {
            var objuser = await _context.VUserInfo.AsNoTracking().SingleOrDefaultAsync(x => x.Id == UserId);
            if (objuser == null)
            {
                return new List<VTeachers>();
            }

            return objuser.UserType == ((int)UserType.SuperAdmin).ToString()
                ? await _context.VTeachers.AsNoTracking().ToListAsync()
                : await _context.VTeachers.AsNoTracking().Where(x => x.BranchId == objuser.BranchId).ToListAsync();
        }
        public async Task<VTeachers> ViewTeacher(int Id)
        {
            return await _context.VTeachers.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
        }
        public async Task<Personnel> GetTeacher(int Id)
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
                model.PersonnelType = ((int)UserType.Teacher).ToString();
                var sessyear = _context.SessionYear.AsNoTracking().SingleOrDefault(x => x.IsActive == ((int)ActiveState.Active).ToString() && x.IsDefault == true);
                if (sessyear != null)
                {
                    model.PersonnelAdmission.SessionYearId = sessyear.Id;
                }
                return model;
            }

            var cat = await _context.Personnel.AsNoTracking().SingleOrDefaultAsync(x => x.Id == Id && x.IsActive == ((int)ActiveState.Active).ToString() && x.PersonnelType == ((int)UserType.Teacher).ToString());
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
        public async Task<PersonnelDetail> GetTeacherDetail(int Id, int BranchId)
        {
            var objsch = await _context.VTeachers.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
            if (objsch == null)
            {
                return null;
            }

            var objadmission = await _context.PersonnelAdmission.AsNoTracking().SingleOrDefaultAsync(x => x.PersonnelId == Id && x.PersonnelType == ((int)UserType.Teacher).ToString());
            var objother = await _context.PersonnelOtherDetail.AsNoTracking().SingleOrDefaultAsync(x => x.PersonnelId == Id && x.PersonnelType == ((int)UserType.Teacher).ToString());
            var objsession = await _context.SessionYear.AsNoTracking().SingleOrDefaultAsync(x => x.IsDefault == true && x.IsActive == ((int)ActiveState.Active).ToString());
            var rpt = await _context.ReportSettings.AsNoTracking().FirstOrDefaultAsync(x=>x.BranchId==BranchId);

            return new PersonnelDetail
            {
                Id = (int)objsch.Id,
                SessionYearId = objsession?.Id ?? 0,
                FilePath = objsch.FilePath,
                FullName = objsch.FullName,
                Age = objsch.Age,
                GenderName = objsch.GenderName,
                FatherName = objother?.FatherName,
                MotherName = objother?.MotherName,
                DefaultMobileNo = objsch.DefaultMobileNo,
                BranchName = objsch.BranchName,
                AdmissionDate = objadmission?.AdmissionDate.ToString(),
                AdmissionNo = objadmission?.AdmissionNo,
                EmployeeNo = objsch.EmployeeNo,
                ActiveStatus = objsch.ActiveStatus,
                ClassSubjectTeacherCount = objsch.ClassSubjectTeacherCount,
                TeacherSubjectName = objsch.TeacherSubjectName,
                AlternateMobileNo = objother?.OtherMobileNo,
                PermanentAddress = objother?.PermanentAddress,
                CurrentAddress = objother?.CurrentAddress,
                ReportSettings = rpt
            };
        }
        public async Task<bool> IsRecordExists(string fname, string lname, int Id)
        {
            return Id == 0
                ? await _context.Personnel.AsNoTracking().AnyAsync(e => e.FirstName == fname && e.LastName == lname && e.IsActive == ((int)ActiveState.Active).ToString() && e.PersonnelType == ((int)UserType.Teacher).ToString())
                : await _context.Personnel.AsNoTracking().AnyAsync(e => e.FirstName == fname && e.LastName == lname && e.Id != Id && e.IsActive == ((int)ActiveState.Active).ToString() && e.PersonnelType == ((int)UserType.Teacher).ToString());
        }
        public async Task<ServiceResult> SaveTeacher(Personnel model, int UserId)
        {
            ServiceResult result = null;
            try
            {
                if (model.Id > 0)
                {
                    var cat = _context.Personnel.SingleOrDefault(m => m.Id == model.Id && m.IsActive == ((int)ActiveState.Active).ToString() && m.PersonnelType == ((int)UserType.Teacher).ToString());
                    if (cat == null)
                    {
                        return result;
                    }

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
                        //if (!string.IsNullOrEmpty(cat.FilePath) && !cat.FilePath.Contains("nouser.png"))
                        //{
                        //    var filePath = _hostingEnvironment.GetFullUrlFromDbPath(cat.FilePath);
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
                        //model.FilePath = Shared.ProcessUploadFile((int)UploadType.Teacher, model.File, _hostingEnvironment.GetWebRootPath());
                        //cat.FilePath = model.FilePath;
                        cat.FilePath = await _s3Service.UploadFileAsync((int)UploadType.Teacher, model.File);

                    }

                    _context.Entry(cat).State = EntityState.Modified;
                    if (_context.SaveChanges() > 0 && model.PersonnelOtherDetail != null)
                    {
                        var otherdetail = _context.PersonnelOtherDetail.SingleOrDefault(x => x.PersonnelId == cat.Id && x.IsActive == ((int)ActiveState.Active).ToString() && x.PersonnelType == model.PersonnelType);
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
                            if (_context.SaveChanges() > 0)
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
                                // FilePath = model.File != null ? Shared.ProcessUploadFile((int)UploadType.Teacher, model.File, _hostingEnvironment.GetWebRootPath()) : Path.Combine("Uploads", "NoImage", "nouser.png")
                                FilePath = model.File != null
                                 ? await _s3Service.UploadFileAsync((int)UploadType.Teacher, model.File)
                                 : Path.Combine("Uploads", "NoImage", "nouser.png")

                            };

                            _context.Personnel.Add(cat);
                            _context.Entry(cat).State = EntityState.Added;
                            if (_context.SaveChanges() > 0 && model.PersonnelAdmission != null)
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
                                if (_context.SaveChanges() > 0)
                                {
                                    var lastId = _context.PersonnelAdmission.AsNoTracking().Where(x => x.PersonnelType == ((int)UserType.Teacher).ToString() && x.BranchId == (int)cat.BranchId).OrderByDescending(x => x.Id).FirstOrDefault()?.Id ?? 1;

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
                                    if (_context.SaveChanges() > 0 && model.PersonnelOtherDetail != null)
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
                                        if (_context.SaveChanges() > 0)
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
                                            if (_context.SaveChanges() > 0)
                                            {
                                                var objRole = _context.Role.AsNoTracking().SingleOrDefault(x => x.RoleType == ((int)UserType.Teacher).ToString() && x.IsActive == ((int)ActiveState.Active).ToString());
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
                                                    if (await _context.SaveChangesAsync() > 0)
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
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public async Task<ServiceResult> DeleteTeacher(int Id, int UserId)
        {
            var cat = await _context.Personnel.SingleOrDefaultAsync(m => m.Id == Id);
            if (cat == null)
            {
                return null;
            }

            cat.IsActive = ((int)ActiveState.InActive).ToString();
            cat.ModifiedByUserId = UserId;
            cat.ModifiedDate = DateTime.Now;
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
        public async Task<TeacherTimeTableVM> GetTeacherTimeTable(int TeacherId)
        {
            var result = new TeacherTimeTableVM
            {
                castTimetable = new CastTimetable { Periods = new List<Period>(), Weekdays = new List<Weekday>() }

            };
            try
            {
                var objsession = await _context.SessionYear
              .AsNoTracking()
              .SingleOrDefaultAsync(x => x.IsActive == ((int)ActiveState.Active).ToString() && x.IsDefault);

                var timetablesection = await _context.TimeTable
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.PersonnelId == TeacherId && x.IsActive == ((int)ActiveState.Active).ToString());

                if (timetablesection != null)
                {
                    var sectionDurBreak = GetClassDurationAndBreaks(timetablesection.SectionId);
                    result = new TeacherTimeTableVM
                    {
                        TeacherId = TeacherId,
                        SessionYearId = objsession?.Id ?? 0,
                        castTimetable = GenerateTimetable(sectionDurBreak.Result.Duration, sectionDurBreak.Result.LstBreak, sectionDurBreak.Result.ClassStartTime, sectionDurBreak.Result.ClassEndTime)
                    };
                }
            }
            catch (Exception ex)
            {


            }

            return result;
        }
        public async Task<TeacherTransTimeTableVM> GetTeacherTransTimeTableVM(int Id, int SessionYearId)
        {
            var result = new TeacherTransTimeTableVM
            {
                CastTransTimetable = new CastTransTimetable()
            };

            var timetablesection = await _context.TimeTable
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.PersonnelId == Id && x.SessionYearId == SessionYearId && x.IsActive == ((int)ActiveState.Active).ToString());

            if (timetablesection != null)
            {
                var sectionDurBreak = await GetClassDurationAndBreaks(timetablesection.SectionId);
                result = new TeacherTransTimeTableVM
                {
                    TeacherId = Id,
                    SessionYearId = SessionYearId,
                    CastTransTimetable = GenerateTransTimetable(sectionDurBreak.Duration, sectionDurBreak.LstBreak, sectionDurBreak.ClassStartTime, sectionDurBreak.ClassEndTime)
                };
            }

            return result;
        }
        public async Task<List<VTimeTable>> GetTimeTable(int TeacherId, int SessionYearId, int UserId)
        {
            return await _context.VTimeTable.AsNoTracking()
                .AsNoTracking()
                .Where(x => x.PersonnelId == TeacherId && x.SessionYearId == SessionYearId)
                .ToListAsync();
        }
        public async Task<ClassDurationAndBreaks> GetClassDurationAndBreaks(int sectionId)
        {
            var objsection = await _context.Section
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == sectionId && x.IsActive == ((int)ActiveState.Active).ToString());

            if (objsection == null)
            {
                return new ClassDurationAndBreaks();
            }

            var objbrcls = await _context.BranchClass
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == objsection.BranchClassId);

            var objshift = await _context.Shift
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == objbrcls.ShiftId && x.IsActive == ((int)ActiveState.Active).ToString());

            var lstperiodbreakids = await _context.BranchClassPeriodBreak
                .AsNoTracking()
                .Where(x => x.BranchClassId == objsection.BranchClassId)
                .Select(x => x.PeriodBreakId)
                .ToListAsync();

            var lstperiodbreak = await _context.PeriodBreak
                .AsNoTracking()
                .Where(x => lstperiodbreakids.Contains(x.Id) && x.IsActive == ((int)ActiveState.Active).ToString())
                .ToListAsync();

            var lstbrk = lstperiodbreak
                .Select(c => new Break
                {
                    StartTime = GetTimeSpan(c.FromTime),
                    EndTime = GetTimeSpan(c.ToTime),
                    Name = c.ShortName
                })
                .ToList();

            return new ClassDurationAndBreaks
            {
                ClassStartTime = GetTimeSpan(objshift.FromTime),
                ClassEndTime = GetTimeSpan(objshift.ToTime),
                Duration = int.Parse(objbrcls.SlotDuration),
                LstBreak = lstbrk
            };
        }
        public async Task<List<VSchedules>> GetTeacherSchedules(string teacherId)
        {
            if (string.IsNullOrEmpty(teacherId))
            {
                return new List<VSchedules>();
            }

            int tchId = int.Parse(teacherId);
            var lstsecids = await _context.ScheduleTeacherExamHall.AsNoTracking()
                .Where(x => x.TeacherId == tchId)
                .Select(x => x.ScheduleId)
                .ToListAsync();

            return await _context.VSchedules.AsNoTracking()
                .Where(x => lstsecids.Contains((int)x.Id))
                .ToListAsync();
        }
        public async Task<List<VTeacherInvigilations>> GetInvigilationsByTeacherId(int teacherId, int scheduleId)
        {
            if (teacherId <= 0 || scheduleId <= 0)
            {
                return new List<VTeacherInvigilations>();
            }

            return await _context.VTeacherInvigilations.AsNoTracking()
                .Where(x => x.PersonnelId == teacherId && x.ScheduleId == scheduleId)
                .ToListAsync();
        }
        public async Task<ScheduleTeacherProcedure> GetPivotScheduleByTeacher(string scheduleId, string teacherId)
        {
            if (string.IsNullOrEmpty(scheduleId) || string.IsNullOrEmpty(teacherId))
            {
                return null;
            }

            int schId = int.Parse(scheduleId);
            int teaId = int.Parse(teacherId);
            DataTable dt = new DataTable();

            try
            {
                //using (MySqlConnection conn = new MySqlConnection(_connectionString))
                //{
                //    MySqlCommand sqlComm = new MySqlCommand("SP_ScheduleTeacher", conn);
                //    sqlComm.Parameters.AddWithValue("@ScheduleId", schId);
                //    sqlComm.Parameters.AddWithValue("@TeacherId", teaId);

                //    sqlComm.CommandType = CommandType.StoredProcedure;
                //    MySqlDataAdapter da = new MySqlDataAdapter();
                //    da.SelectCommand = sqlComm;
                //    da.Fill(dt);
                //}
            }
            catch (Exception)
            {
                // Handle exception
            }

            return new ScheduleTeacherProcedure
            {
                StrResult = DataTableToJSON(dt)
            };
        }
        public async Task<TeacherPerformanceProcedure> GetTeacherPerformance(int ScheduleId)
        {
            if (ScheduleId <= 0)
            {
                return null;
            }

            DataTable dt = new DataTable();

            try
            {

                var rawData = _context.V_SP_TeacherPerformance
                .AsNoTracking()
                .Where(x => x.ScheduleId == ScheduleId)
                .Select(p => new
                {
                    p.TeacherId,
                    p.TeacherName,
                    p.ScheduleId,

                    p.GradeNameColor,
                    p.StudentCount
                })
                .ToList();
                var gradeNames = rawData.Select(x => x.GradeNameColor).Distinct().ToList();


                var pivotedData = rawData
            .GroupBy(x => new { x.ScheduleId, x.TeacherId, x.TeacherName })
            .Select(g =>
            {
                var pivot = new ExpandoObject() as IDictionary<string, Object>;
                pivot["TeacherName"] = g.Key.TeacherName;
                pivot["TeacherId"] = g.Key.TeacherId;
                var totalStudentCountForTeacher = g.Sum(x => x.StudentCount); // Sum of StudentCount for the entire teacher
                pivot["Total"] = totalStudentCountForTeacher;

                foreach (var grname in gradeNames)
                {
                    var gradename = g.FirstOrDefault(x => x.GradeNameColor == grname);
                    pivot[grname] = gradename?.StudentCount ?? 0;
                }

                return pivot;
            })
            .ToList();
                dt = ConvertToDataTable(pivotedData);

                //using (MySqlConnection conn = new MySqlConnection(_connectionString))
                //{
                //    MySqlCommand sqlComm = new MySqlCommand("SP_TeacherPerformance", conn);
                //    sqlComm.Parameters.AddWithValue("@ScheduleId", ScheduleId);

                //    sqlComm.CommandType = CommandType.StoredProcedure;
                //    MySqlDataAdapter da = new MySqlDataAdapter();
                //    da.SelectCommand = sqlComm;
                //    da.Fill(dt);
                //}
            }
            catch (Exception)
            {
                // Handle exception
            }

            return new TeacherPerformanceProcedure
            {
                StrResult = DataTableToJSON(dt)
            };
        }
        public async Task<List<SelectListItem>> GetBloodGroup()
        {
            return await _context.BloodGroup.AsNoTracking()
                .Where(x => x.IsActive == ((int)ActiveState.Active).ToString())
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                .ToListAsync();
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
