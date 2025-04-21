using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Npgsql;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using System.Globalization;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class AttendenceService : IAttendenceService
    {
        private readonly APIContext _context;
        private readonly string _connectionString;
        public AttendenceService(APIContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<AttendenceVM> GetAllAttendence(int UserId)
        {
            AttendenceVM result = new AttendenceVM();
            try
            {
                var strToday = DateTime.Now.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
                var objuser = await _context.VUserInfo.AsNoTracking().SingleOrDefaultAsync(x => x.Id == UserId);
                if (objuser != null)
                {
                    bool isSuperAdmin = objuser.UserType == ((int)UserType.SuperAdmin).ToString();
                    var query = _context.VAttendence.AsNoTracking().AsQueryable();

                    if (!isSuperAdmin)
                    {
                        query = query.Where(x => x.BranchId == objuser.BranchId);
                    }

                    var isAttendenceExist = await query.AnyAsync(x => x.StrWorkingDate == strToday);
                    var lst = await query.OrderByDescending(x => x.Id).ToListAsync();
                    result = new AttendenceVM { IsAttendenceExist = isAttendenceExist, LstAttendence = lst };
                }
            }
            catch (Exception ex)
            {
                // Log the exception
            }
            return result;
        }

        public async Task<MBDailyAttendenceVM> GetDailyAttendence(int BranchId, int SessionYearId)
        {
            MBDailyAttendenceVM result = new MBDailyAttendenceVM();
            try
            {
                var strToday = DateTime.Now.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
                var isAttendenceExist = await _context.VAttendence.AsNoTracking().AnyAsync(x => x.BranchId == BranchId && x.SessionYearId == SessionYearId && x.StrWorkingDate == strToday);
                var lst = await _context.Attendence.AsNoTracking()
                                                   .Where(x => x.BranchId == BranchId && x.SessionYearId == SessionYearId)
                                                   .OrderByDescending(x => x.Id)
                                                   .Select(c => new MBDailyAttendence { Id = c.Id, WorkingDate = c.WorkingDate, Month = c.WorkingDate.Month, Day = c.WorkingDate.Day })
                                                   .ToListAsync();
                result = new MBDailyAttendenceVM { IsAttendenceExist = isAttendenceExist, LstDailyAttendence = lst };
            }
            catch (Exception ex)
            {
                // Log the exception
            }
            return result;
        }

        public async Task<Attendence> GetAttendence(int Id, int UserId)
        {
            Attendence model = new Attendence();
            if (Id > 0)
            {
                var cat = await _context.Attendence.AsNoTracking().SingleOrDefaultAsync(x => x.Id == Id && x.IsActive == ((int)ActiveState.Active).ToString());
                if (cat != null)
                {
                    model = new Attendence
                    {
                        Id = cat.Id,
                        WorkingDate = cat.WorkingDate,
                        BranchId = cat.BranchId,
                        SessionYearId = cat.SessionYearId
                    };
                }
            }
            else
            {
                var objuser = await _context.VUserInfo.AsNoTracking().SingleOrDefaultAsync(x => x.Id == UserId);
                if (objuser != null)
                {
                    model.BranchId = objuser.BranchId ?? 0;
                    var objsession = await _context.SessionYear.AsNoTracking().SingleOrDefaultAsync(x => x.IsActive == ((int)ActiveState.Active).ToString() && x.IsDefault == true);
                    if (objsession != null)
                    {
                        model.SessionYearId = objsession.Id > 0 ? objsession.Id : 0;
                        model.WorkingDate = DateTime.Now;
                    }
                }
            }
            return model;
        }
        public async Task<Attendence> ViewAttendence(int Id)
        {
            Attendence model = new Attendence();
            var cat = await _context.Attendence.AsNoTracking().SingleOrDefaultAsync(x => x.Id == Id && x.IsActive == ((int)ActiveState.Active).ToString());
            if (cat != null)
            {
                model = new Attendence
                {
                    Id = cat.Id,
                    WorkingDate = cat.WorkingDate,
                    BranchId = cat.BranchId,
                    SessionYearId = cat.SessionYearId
                };
            }
            return model;
        }

        public async Task<List<SectionAttendence>> GetSectionAttendence(int Id, int BranchId, int SessionYearId)
        {
            List<SectionAttendence> result = new List<SectionAttendence>();
            if (Id == 0)
            {
                var lststudsecids = await _context.VStudentCurrentLocation
                    .AsNoTracking()
                    .Where(x => x.BranchId == BranchId && x.SessionYearId == SessionYearId)
                    .Select(x => x.SectionId)
                    .ToListAsync();

                result = await _context.VSections
                    .AsNoTracking()
                    .Where(x => lststudsecids.Contains(x.Id ?? 0))
                    .Select(c => new SectionAttendence { Id = c.Id ?? 0, Name = c.ClassShortName + " " + c.ShortName, AttStatusColor = "", AttStatusName = "" })
                    .ToListAsync();
            }
            else
            {
                var lststudsecids = await _context.StudentAttendence
                    .AsNoTracking()
                    .Where(x => x.AttendenceId == Id)
                    .Select(x => x.SectionId)
                    .ToListAsync();

                if (lststudsecids.Count > 0)
                {
                    result = await _context.VSections
                        .AsNoTracking()
                        .Where(x => lststudsecids.Contains(x.Id ?? 0))
                        .Select(c => new SectionAttendence { Id = c.Id ?? 0, Name = c.ClassShortName + " " + c.ShortName, AttStatusColor = "", AttStatusName = "" })
                        .ToListAsync();
                }
                else
                {
                    var lststudsecids2 = await _context.VStudentCurrentLocation
                        .AsNoTracking()
                        .Where(x => x.BranchId == BranchId && x.SessionYearId == SessionYearId)
                        .Select(x => x.SectionId)
                        .ToListAsync();

                    result = await _context.VSections
                        .AsNoTracking()
                        .Where(x => lststudsecids2.Contains(x.Id ?? 0))
                        .Select(c => new SectionAttendence { Id = c.Id ?? 0, Name = c.ClassShortName + " " + c.ShortName, AttStatusColor = "", AttStatusName = "" })
                        .ToListAsync();
                }
            }
            return result;
        }

        public async Task<SectionAttendenceVM> GetMbSectionAttendence(int Id, int TeacherId, int branchId, int SessionYearId)
        {
            SectionAttendenceVM result = new SectionAttendenceVM();
            List<SectionAttendence> lstAttend = new List<SectionAttendence>();
            bool isedit = false;
            string strworkingdate = string.Empty;

            List<int> clsids = await _context.BranchClassSectionTeacher
                .AsNoTracking()
                .Where(x => x.BranchId == branchId && x.TeacherId == TeacherId && x.SessionYearId == SessionYearId && x.IsActive == ((int)ActiveState.Active).ToString())
                .Select(x => x.SectionId)
                .ToListAsync();

            clsids.AddRange(await _context.BranchClassSectionSubjectTeacher
                .AsNoTracking()
                .Where(x => x.BranchId == branchId && x.TeacherId == TeacherId && x.SessionYearId == SessionYearId && x.IsActive == ((int)ActiveState.Active).ToString())
                .Select(x => x.SectionId)
                .ToListAsync());

            if (Id == 0)
            {
                var lstsection = await _context.VSections
                    .AsNoTracking()
                    .Where(x => x.BranchId == branchId && clsids.Contains(x.Id ?? 0))
                    .Select(c => new SectionAttendence { Id = c.Id ?? 0, Name = c.ClassShortName + " " + c.ShortName, AttStatusColor = "", AttStatusName = "" })
                    .ToListAsync();

                lstAttend = new List<SectionAttendence>(lstsection);
                isedit = true;
                strworkingdate = DateTime.Now.Date.ToString("dd-MM-yyyy");
            }
            else
            {
                var objAttend = await _context.VAttendence
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.BranchId == branchId && x.Id == Id);

                if (objAttend != null)
                {
                    strworkingdate = objAttend.StrWorkingDate;
                    isedit = objAttend.IsEdit == "0" ? false : true;

                    var lstsection2 = await _context.VSections
                        .AsNoTracking()
                        .Where(x => x.BranchId == branchId && clsids.Contains(x.Id ?? 0))
                        .Select(c => new SectionAttendence { Id = c.Id ?? 0, Name = c.ClassShortName + " " + c.ShortName, AttStatusColor = "", AttStatusName = "" })
                        .ToListAsync();

                    lstAttend = new List<SectionAttendence>(lstsection2);
                }
            }

            result = new SectionAttendenceVM
            {
                Id = Id,
                StrWorkingDate = strworkingdate,
                IsEdit = isedit,
                BranchId = branchId,
                SessionYearId = SessionYearId,
                LstSectionAttendence = lstAttend
            };
            return result;
        }

        public async Task<List<CastStudentAttendence>> GetStudentAttendence(int Id, int SectionId, int SessionYearId)
        {
            List<CastStudentAttendence> result = new List<CastStudentAttendence>();
            var strToday = DateTime.Today.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);

            if (Id == 0)
            {
                var lststud = await _context.VStudentCurrentLocation
                    .AsNoTracking()
                    .Where(x => x.SectionId == SectionId && x.SessionYearId == SessionYearId)
                    .Select(c => new CastStudentAttendence
                    {
                        StudentId = c.StudentId,
                        StudentName = c.FullName,
                        SessionYearId = c.SessionYearId,
                        BranchId = c.BranchId > 0 ? c.BranchId : 0,
                        BranchClassId = c.BranchClassId ?? 0,
                        ClassId = c.ClassId > 0 ? c.ClassId : 0,
                        RollNo = c.RollNo,
                        SectionId = c.SectionId > 0 ? c.SectionId : 0,
                        IsSelected = false,
                        LeaveType = "",
                        Remarks = ""
                    })
                    .ToListAsync();

                foreach (var c in lststud)
                {
                    var leave = await _context.VLeaveRequestDate
                        .AsNoTracking()
                        .SingleOrDefaultAsync(x => x.PersonnelId == c.StudentId && x.PersonnelType == ((int)UserType.Student).ToString() && x.StrLeaveDate == strToday);

                    if (leave != null)
                    {
                        c.LeaveType = leave.LeaveType;
                    }
                }
                result = lststud;
            }
            else
            {
                var objAttendence = await _context.Attendence
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.Id == Id);

                var lststud = await _context.StudentAttendence
                    .AsNoTracking()
                    .Where(x => x.AttendenceId == Id && x.SectionId == SectionId && x.SessionYearId == SessionYearId)
                    .ToListAsync();

                if (lststud.Count > 0)
                {
                    var lststudids = lststud.Select(x => x.StudentId).ToList();

                    var lstcurrstud = await _context.VStudentCurrentLocation
                        .AsNoTracking()
                        .Where(x => x.SectionId == SectionId && x.SessionYearId == SessionYearId && lststudids.Contains(x.StudentId))
                        .Select(c => new CastStudentAttendence
                        {
                            StudentId = c.StudentId,
                            StudentName = c.FullName,
                            SessionYearId = c.SessionYearId,
                            BranchId = c.BranchId > 0 ? c.BranchId : 0,
                            BranchClassId = c.BranchClassId ?? 0,
                            ClassId = c.ClassId > 0 ? c.ClassId : 0,
                            RollNo = c.RollNo,
                            SectionId = c.SectionId > 0 ? c.SectionId : 0,
                            IsSelected = false,
                            LeaveType = "",
                            Remarks = ""
                        })
                        .ToListAsync();

                    foreach (var c in lstcurrstud)
                    {
                        var stud = lststud.FirstOrDefault(s => s.StudentId == c.StudentId);
                        if (stud != null)
                        {
                            c.IsSelected = stud.IsPresent == ((int)ActiveState.Active).ToString();
                            c.Remarks = stud.Remarks;
                        }
                    }
                    result = lstcurrstud;
                }
                else
                {
                    var lststud2 = await _context.VStudentCurrentLocation
                        .AsNoTracking()
                        .Where(x => x.SectionId == SectionId && x.SessionYearId == SessionYearId)
                        .Select(c => new CastStudentAttendence
                        {
                            StudentId = c.StudentId,
                            StudentName = c.FullName,
                            SessionYearId = c.SessionYearId,
                            BranchId = c.BranchId > 0 ? c.BranchId : 0,
                            BranchClassId = c.BranchClassId ?? 0,
                            ClassId = c.ClassId > 0 ? c.ClassId : 0,
                            RollNo = c.RollNo,
                            SectionId = c.SectionId > 0 ? c.SectionId : 0,
                            IsSelected = false,
                            LeaveType = "",
                            Remarks = ""
                        })
                        .ToListAsync();

                    foreach (var c in lststud2)
                    {
                        var leave = await _context.VLeaveRequestDate
                            .AsNoTracking()
                            .SingleOrDefaultAsync(x => x.PersonnelId == c.StudentId && x.PersonnelType == ((int)UserType.Student).ToString() && x.StrLeaveDate == strToday);

                        if (leave != null)
                        {
                            c.LeaveType = leave.LeaveType;
                        }
                    }
                    result = lststud2;
                }
            }
            return result;
        }

        public async Task<List<MBPersonnelAttendence>> GetAttendenceDetailByPersonnelId(int PersonnelId, int PersonnelType, int SessionYearId)
        {
            if (PersonnelId <= 0 || SessionYearId <= 0 || PersonnelType <= 0)
                return new List<MBPersonnelAttendence>();

            if (PersonnelType == (int)UserType.Student)
            {
                return await _context.StudentAttendence
                    .AsNoTracking()
                    .Where(x => x.StudentId == PersonnelId && x.SessionYearId == SessionYearId)
                    .Select(c => new MBPersonnelAttendence
                    {
                        Id = c.Id,
                        WorkingDate = c.WorkingDate,
                        Month = c.WorkingDate.Month,
                        Day = c.WorkingDate.Day,
                        StudentPersonnelId = c.StudentId,
                        IsPresent = c.IsPresent
                    })
                    .ToListAsync();
            }
            else
            {
                return await _context.PersonnelAttendence
                    .AsNoTracking()
                    .Where(x => x.PersonnelId == PersonnelId && x.PersonnelType == PersonnelType.ToString() && x.SessionYearId == SessionYearId)
                    .Select(c => new MBPersonnelAttendence
                    {
                        Id = c.Id,
                        WorkingDate = c.WorkingDate,
                        Month = c.WorkingDate.Month,
                        Day = c.WorkingDate.Day,
                        StudentPersonnelId = c.PersonnelId,
                        IsPresent = c.IsPresent
                    })
                    .ToListAsync();
            }
        }

        public async Task<List<PersonnelMonthlyAttendence>> GetAttendenceByPersonnelId(int PersonnelId, int PersonnelType, int SessionYearId)
        {
            if (PersonnelId <= 0 || SessionYearId <= 0 || PersonnelType <= 0)
                return new List<PersonnelMonthlyAttendence>();

            var lstchild = (PersonnelType == (int)UserType.Student
                ? await _context.StudentAttendence
                    .AsNoTracking()
                    .Where(x => x.StudentId == PersonnelId && x.SessionYearId == SessionYearId)
                    .Select(c => new HeatMapPersonnelAttendence
                    {
                        PersonnelId = c.StudentId,
                        PersonnelType = PersonnelType,
                        AttendenceDate = c.WorkingDate,
                        YearId = c.WorkingDate.Year,
                        MonthId = c.WorkingDate.Month,
                        Status = c.IsPresent
                    })
                    .ToListAsync()
                : await _context.PersonnelAttendence
                    .AsNoTracking()
                    .Where(x => x.PersonnelId == PersonnelId && x.PersonnelType == PersonnelType.ToString() && x.SessionYearId == SessionYearId)
                    .Select(c => new HeatMapPersonnelAttendence
                    {
                        PersonnelId = c.PersonnelId,
                        PersonnelType = PersonnelType,
                        AttendenceDate = c.WorkingDate,
                        YearId = c.WorkingDate.Year,
                        MonthId = c.WorkingDate.Month,
                        Status = c.IsPresent
                    })
                    .ToListAsync());

            if (lstchild.Count == 0)
                return new List<PersonnelMonthlyAttendence>();

            var groupedMonths = lstchild
                .GroupBy(x => new { x.AttendenceDate.Year, x.AttendenceDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    MonthNumber = g.Key.Month,
                    MonthName = g.First().AttendenceDate.ToString("MMMM", CultureInfo.InvariantCulture),
                    Count = g.Count()
                })
                .OrderBy(m => m.Year).ThenBy(m => m.MonthNumber)
                .ToList();

            return groupedMonths.Select(c => new PersonnelMonthlyAttendence
            {
                YearId = c.Year,
                MonthId = c.MonthNumber,
                MonthName = c.MonthName,
                WorkingDaysCount = c.Count,
                LstHeatMapPersonnelAttendence = lstchild.Where(x => x.YearId == c.Year && x.MonthId == c.MonthNumber).ToList()
            }).ToList();
        }

        //public async Task<JObject> GetFn_AttendenceMetric(int PersonnelId, int PersonnelType, int SessionYearId)
        //{
        //    using var connection = new NpgsqlConnection(_connectionString);
        //    await connection.OpenAsync();

        //    if (PersonnelType == (int)UserType.Student)
        //    {

        //        using var command = new NpgsqlCommand("SELECT fn_student_attend_metric(@p_sessionyear_id,@p_student_id)", connection);
        //        // Add parameter to the command         

        //        command.Parameters.AddWithValue("p_sessionyear_id", SessionYearId.ToString());
        //        command.Parameters.AddWithValue("p_student_id", PersonnelId.ToString());

        //        var result = await command.ExecuteScalarAsync(); // Executes the function
        //                                                         // The result will be in JSONB format
        //        if (result != null)
        //        {
        //            var jsonString = result.ToString();
        //            var json = JObject.Parse(jsonString); // Parse JSON into JObject
        //            return json;
        //        }
        //    }
        //    else if (PersonnelType != (int)UserType.Student)
        //    {

        //        using var command = new NpgsqlCommand("SELECT fn_personnel_attend_metric(@p_sessionyear_id,@p_personnel_id)", connection);
        //        // Add parameter to the command         

        //        command.Parameters.AddWithValue("p_sessionyear_id", SessionYearId.ToString());
        //        command.Parameters.AddWithValue("p_personnel_id", PersonnelId.ToString());

        //        var result = await command.ExecuteScalarAsync(); // Executes the function
        //                                                         // The result will be in JSONB format
        //        if (result != null)
        //        {
        //            var jsonString = result.ToString();
        //            var json = JObject.Parse(jsonString); // Parse JSON into JObject
        //            return json;
        //        }
        //    }
        //    return null;
        //}
        public async Task<JObject> GetFn_AttendenceMetric(int PersonnelId, int PersonnelType, int SessionYearId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            // Determine which function to call and which parameter to use based on PersonnelType
            string functionName = GetFunctionNameForPersonnelType(PersonnelType);
            string personnelParamName = GetPersonnelParamName(PersonnelType);

            // Execute the function and return the result as JObject
            return await ExecuteFunctionAsync(connection, functionName, SessionYearId, PersonnelId, personnelParamName);
        }

        private string GetFunctionNameForPersonnelType(int PersonnelType)
        {
            return PersonnelType == (int)UserType.Student
                ? "fn_student_attend_metric"
                : "fn_personnel_attend_metric";
        }

        private string GetPersonnelParamName(int PersonnelType)
        {
            // Return the correct parameter name based on PersonnelType
            return PersonnelType == (int)UserType.Student
                ? "p_student_id"
                : "p_personnel_id";
        }

        private async Task<JObject> ExecuteFunctionAsync(NpgsqlConnection connection, string functionName, int sessionYearId, int personnelId, string personnelParamName)
        {
            using var command = new NpgsqlCommand($"SELECT {functionName}(@p_sessionyear_id, @{personnelParamName})", connection);

            // Add parameters to the command
            command.Parameters.AddWithValue("p_sessionyear_id", sessionYearId.ToString());
            command.Parameters.AddWithValue(personnelParamName, personnelId.ToString());

            var result = await command.ExecuteScalarAsync(); // Executes the function
            if (result != null)
            {
                var jsonString = result.ToString();
                return JObject.Parse(jsonString); // Parse JSON into JObject
            }
            return null;
        }


        public async Task<List<VPersonnelAttendenceBenchmark>> GetTotalAttendenceByPersonnelId(int PersonnelId, int PersonnelType, int SessionYearId)
        {
            if (PersonnelId <= 0 || SessionYearId <= 0 || PersonnelType <= 0)
                return new List<VPersonnelAttendenceBenchmark>();

            if (PersonnelType == (int)UserType.Student)
            {
                return await _context.VStudentAttendenceBenchmark
                    .AsNoTracking()
                    .Where(x => x.StudentId == PersonnelId && x.SessionYearId == SessionYearId)
                    .OrderBy(y => y.YearNo).ThenBy(z => z.MonthNo)
                    .Select(c => new VPersonnelAttendenceBenchmark
                    {
                        PersonnelId = c.StudentId,
                        SessionYearId = c.SessionYearId,
                        WorkingDays = c.WorkingDays,
                        YearNo = c.YearNo,
                        MonthNo = c.MonthNo,
                        MonthName = c.MonthName,
                        PresentDays = c.PresentDays,
                        AbsentDays = c.AbsentDays,
                        LeaveDays = c.LeaveDays
                    })
                    .ToListAsync();
            }
            else
            {
                return await _context.VPersonnelAttendenceBenchmark
                    .AsNoTracking()
                    .Where(x => x.PersonnelId == PersonnelId && x.SessionYearId == SessionYearId)
                    .OrderBy(y => y.YearNo).ThenBy(z => z.MonthNo)
                    .Select(c => new VPersonnelAttendenceBenchmark
                    {
                        PersonnelId = c.PersonnelId,
                        SessionYearId = c.SessionYearId,
                        WorkingDays = c.WorkingDays,
                        YearNo = c.YearNo,
                        MonthNo = c.MonthNo,
                        MonthName = c.MonthName,
                        PresentDays = c.PresentDays,
                        AbsentDays = c.AbsentDays,
                        LeaveDays = c.LeaveDays
                    })
                    .ToListAsync();
            }
        }

        public async Task<List<CastPersonnelAttendence>> GetPersonnelAttendence(int Id, int BranchId, int SessionYearId, int PersonnelType)
        {
            if (Id == 0)
            {
                var lstcast = await _context.VPersonnelCurrentLocation
                    .AsNoTracking()
                    .Where(x => x.BranchId == BranchId && x.SessionYearId == SessionYearId && x.PersonnelType == PersonnelType.ToString())
                    .Select(c => new CastPersonnelAttendence
                    {
                        PersonnelId = c.PersonnelId,
                        PersonnelType = c.PersonnelType,
                        EmployeeName = c.FullName,
                        SessionYearId = c.SessionYearId,
                        BranchId = (int)c.BranchId,
                        EmployeeNo = c.EmployeeNo,
                        IsSelected = false,
                        LeaveType = "",
                        Remarks = ""
                    }).ToListAsync();

                var strToday = DateTime.Today.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
                foreach (var c in lstcast)
                {
                    var leave = await _context.VLeaveRequestDate
                        .AsNoTracking()
                        .SingleOrDefaultAsync(x => x.PersonnelId == c.PersonnelId && x.PersonnelType == c.PersonnelType && x.StrLeaveDate == strToday);
                    if (leave != null)
                    {
                        c.LeaveType = leave.LeaveType;
                    }
                }
                return lstcast;
            }
            else
            {
                var objAttendence = await _context.Attendence
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.Id == Id);
                var lstpers = await _context.PersonnelAttendence
                    .AsNoTracking()
                    .Where(x => x.AttendenceId == Id && x.BranchId == BranchId && x.SessionYearId == SessionYearId && x.PersonnelType == PersonnelType.ToString())
                    .ToListAsync();

                if (lstpers.Count > 0)
                {
                    var lstpersonnelids = lstpers.Select(x => x.PersonnelId).ToList();
                    var lstcurrper = await _context.VPersonnelCurrentLocation
                        .AsNoTracking()
                        .Where(x => x.BranchId == BranchId && x.SessionYearId == SessionYearId && x.PersonnelType == PersonnelType.ToString() && lstpersonnelids.Contains(x.PersonnelId))
                        .Select(c => new CastPersonnelAttendence
                        {
                            PersonnelId = c.PersonnelId,
                            PersonnelType = c.PersonnelType,
                            EmployeeName = c.FullName,
                            SessionYearId = c.SessionYearId,
                            BranchId = (int)c.BranchId,
                            EmployeeNo = c.EmployeeNo,
                            IsSelected = false,
                            LeaveType = "",
                            Remarks = ""
                        }).ToListAsync();

                    foreach (var c in lstcurrper)
                    {
                        var stud = lstpers.FirstOrDefault(s => s.PersonnelId == c.PersonnelId);
                        if (stud != null)
                        {
                            c.IsSelected = stud.IsPresent == ((int)ActiveState.Active).ToString();
                            c.Remarks = stud.Remarks;
                        }
                    }
                    return lstcurrper;
                }
                else
                {
                    return await _context.VPersonnelCurrentLocation
                        .AsNoTracking()
                        .Where(x => x.BranchId == BranchId && x.SessionYearId == SessionYearId && x.PersonnelType == PersonnelType.ToString())
                        .Select(c => new CastPersonnelAttendence
                        {
                            PersonnelId = c.PersonnelId,
                            PersonnelType = c.PersonnelType,
                            EmployeeName = c.FullName,
                            SessionYearId = c.SessionYearId,
                            BranchId = (int)c.BranchId,
                            EmployeeNo = c.EmployeeNo,
                            IsSelected = false,
                            LeaveType = "",
                            Remarks = ""
                        }).ToListAsync();
                }
            }
        }

        public async Task<ServiceResult> SaveAttendence(Attendence model, int UserId)
        {
            if (model == null) return null;

            ServiceResult result = null;
            if (model.Id > 0)
            {
                var objAttendence = await _context.Attendence
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.Id == model.Id && x.IsActive == ((int)ActiveState.Active).ToString());
                if (objAttendence != null)
                {
                    result = new ServiceResult { StatusId = (int)ServiceResultStatus.Updated, RecordId = objAttendence.Id };
                    if (model.PersonnelType == (int)UserType.Student)
                    {
                        await SaveStudentAttendence(model.LstStudentAttendence, model.Id, objAttendence.WorkingDate, model.PersonnelType, UserId);
                    }
                    else
                    {
                        await SavePersonnelAttendence(model.LstPersonnelAttendence, objAttendence.Id, objAttendence.WorkingDate, model.PersonnelType, UserId);
                    }
                }
            }
            else
            {
                var obj = new Attendence
                {
                    BranchId = model.BranchId,
                    SessionYearId = model.SessionYearId,
                    WorkingDate = DateTime.Now,
                    CreatedDate = DateTime.Now,
                    CreatedByUserId = UserId,
                    IsActive = ((int)ActiveState.Active).ToString()
                };
                _context.Attendence.Add(obj);
                _context.Entry(obj).State = EntityState.Added;
                if (await _context.SaveChangesAsync() > 0)
                {
                    result = new ServiceResult { StatusId = (int)ServiceResultStatus.Added, RecordId = obj.Id };
                    if (model.PersonnelType == (int)UserType.Student)
                    {
                        await SaveStudentAttendence(model.LstStudentAttendence, obj.Id, obj.WorkingDate, model.PersonnelType, UserId);
                    }
                    else
                    {
                        await SavePersonnelAttendence(model.LstPersonnelAttendence, obj.Id, obj.WorkingDate, model.PersonnelType, UserId);
                    }
                }
            }
            return result;
        }

        public async Task<int> SaveStudentAttendence(List<CastStudentAttendence> lstStudent, int attendenceId, DateTime workingDate, int personnelType, int UserId)
        {
            if (lstStudent == null || lstStudent.Count == 0) return 0;

            var objstud = lstStudent.FirstOrDefault();
            var chklst = await _context.StudentAttendence
                .Where(x => x.AttendenceId == attendenceId && x.BranchId == objstud.BranchId && x.ClassId == objstud.ClassId && x.SectionId == objstud.SectionId)
                .ToListAsync();

            foreach (var chk in chklst)
            {
                var item = lstStudent.FirstOrDefault(s => s.StudentId == chk.StudentId);
                if (item != null)
                {
                    chk.IsPresent = item.IsSelected ? ((int)ActiveState.Active).ToString() : ((int)ActiveState.InActive).ToString();
                    chk.Remarks = item.Remarks;
                    chk.LastModifiedDate = DateTime.Now;
                    chk.LastModifiedByUserId = UserId;
                }
            }

            if (chklst.Count == 0)
            {
                var lststud = lstStudent.Select(item => new StudentAttendence
                {
                    AttendenceId = attendenceId,
                    StudentId = item.StudentId,
                    SessionYearId = item.SessionYearId,
                    BranchId = item.BranchId,
                    BranchClassId = item.BranchClassId,
                    ClassId = item.ClassId,
                    SectionId = item.SectionId,
                    WorkingDate = workingDate,
                    IsPresent = item.IsSelected ? ((int)ActiveState.Active).ToString() : ((int)ActiveState.InActive).ToString(),
                    Remarks = item.Remarks,
                    CreatedDate = DateTime.Now,
                    CreatedByUserId = UserId,
                    IsActive = ((int)ActiveState.Active).ToString()
                }).ToList();

                await _context.StudentAttendence.AddRangeAsync(lststud);
            }

            return await _context.SaveChangesAsync();
        }

        public async Task<int> SavePersonnelAttendence(List<CastPersonnelAttendence> lstPersonnel, int attendenceId, DateTime workingDate, int personnelType, int UserId)
        {
            if (lstPersonnel == null || lstPersonnel.Count == 0) return 0;

            var chklst = await _context.PersonnelAttendence
                .Where(x => x.AttendenceId == attendenceId && x.PersonnelType == personnelType.ToString())
                .ToListAsync();

            foreach (var chk in chklst)
            {
                var item = lstPersonnel.FirstOrDefault(s => s.PersonnelId == chk.PersonnelId);
                if (item != null)
                {
                    chk.IsPresent = item.IsSelected ? ((int)ActiveState.Active).ToString() : ((int)ActiveState.InActive).ToString();
                    chk.Remarks = item.Remarks;
                    chk.LastModifiedDate = DateTime.Now;
                    chk.LastModifiedByUserId = UserId;
                }
            }

            if (chklst.Count == 0)
            {
                var lststud = lstPersonnel.Select(item => new PersonnelAttendence
                {
                    AttendenceId = attendenceId,
                    PersonnelId = item.PersonnelId,
                    PersonnelType = item.PersonnelType,
                    SessionYearId = item.SessionYearId,
                    BranchId = item.BranchId,
                    WorkingDate = workingDate,
                    IsPresent = item.IsSelected ? ((int)ActiveState.Active).ToString() : ((int)ActiveState.InActive).ToString(),
                    Remarks = item.Remarks,
                    CreatedDate = DateTime.Now,
                    CreatedByUserId = UserId,
                    IsActive = ((int)ActiveState.Active).ToString()
                }).ToList();

                await _context.PersonnelAttendence.AddRangeAsync(lststud);
            }

            return await _context.SaveChangesAsync();
        }

        public async Task<List<SelectListItem>> GetBranches()
        {
            return await _context.Branch
                .AsNoTracking()
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
