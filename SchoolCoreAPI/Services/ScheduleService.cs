using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Net;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly APIContext _context;
        private readonly string _connectionString;
        public ScheduleService(APIContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }





        public async Task<List<VSchedules>> GetAllSchedule(int BranchId, int SessionYearId)
        {
            return await _context.VSchedules
                .Where(x => x.BranchId == BranchId && x.SessionYearId == SessionYearId)
                .OrderBy(x => x.Id)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<ScheduleDetail> ViewSchedule(int Id, int BranchId)
        {
            var rpt = await _context.ReportSettings.AsNoTracking().FirstOrDefaultAsync(x=>x.BranchId==BranchId);
            var objsch = await _context.VSchedules.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);

            if (objsch == null)
            {
                return null;
            }

            return new ScheduleDetail
            {
                Id = (int)objsch.Id,
                StartDate = objsch.StrStartDate,
                EndDate = objsch.StrEndDate,
                Title = objsch.Title,
                ExamType = objsch.ExamTypeName,
                Branch = objsch.BranchName,
                ReportSettings = rpt
            };
        }
        public async Task<Schedule> GetSchedule(int Id, int BranchId, int SessionYearId)
        {
            var model = new Schedule
            {
                BranchId = BranchId,
                SessionYearId = SessionYearId,
                ExamTypeSheet = await GetExamTypes(),
                BranchSheet = await GetBranches(),
                LstExamTime = _context.ExamTime
                    .Where(x => x.IsActive == ((int)ActiveState.Active).ToString())
                    .AsNoTracking()
                    .ToList(),
                LstScheduleClass = new List<ScheduleClass>(),
                LstScheduleSections = new List<ScheduleSections>(),
                LstScheduleTeachers = new List<ScheduleTeachers>()
            };

            if (Id <= 0) return model;

            var cat = await _context.Schedule
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == Id && x.IsActive == ((int)ActiveState.Active).ToString());

            if (cat == null) return model;

            model.Id = cat.Id;
            model.BranchId = cat.BranchId;
            model.SessionYearId = cat.SessionYearId;
            model.StatusId = cat.StatusId;
            model.Title = cat.Title;
            model.ExamCount = cat.ExamCount;
            model.Description = cat.Description;
            model.StartDate = cat.StartDate;
            model.EndDate = cat.EndDate;

            if (cat.ExamTypeId <= 0) return model;

            model.ExamTypeId = cat.ExamTypeId;
            var divselectedItem = model.ExamTypeSheet.Find(p => p.Value == model.ExamTypeId.ToString());
            if (divselectedItem != null)
            {
                divselectedItem.Selected = true;
            }

            var divbranchItem = model.BranchSheet.Find(p => p.Value == model.BranchId.ToString());
            if (divbranchItem != null)
            {
                divbranchItem.Selected = true;
            }

            var lstschtime = await _context.ScheduleExamTime
                .Where(x => x.ScheduleId == Id)
                .AsNoTracking()
                .ToListAsync();

            foreach (var item in model.LstExamTime)
            {
                item.IsSelected = lstschtime.Any(item1 => item.Id == item1.ExamTimeId);
            }

            return model;
        }
        public async Task<int> SaveSchedule(Schedule model, int UserId)
        {
            int result = 0;
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (model.Id > 0)
                    {
                        var objsched = await _context.Schedule.SingleOrDefaultAsync(x => x.Id == model.Id);
                        if (objsched != null)
                        {
                            objsched.SessionYearId = model.SessionYearId;
                            objsched.BranchId = model.BranchId;
                            objsched.Title = model.Title;
                            objsched.StatusId = (int)ScheduleStatus.Schedule;
                            objsched.ExamTypeId = model.ExamTypeId;
                            objsched.ExamCount = model.ExamCount;
                            objsched.StartDate = model.StartDate;
                            objsched.EndDate = model.EndDate;
                            objsched.Description = model.Description;
                            objsched.LastModifiedDate = DateTime.Now;
                            objsched.LastModifiedByUserId = UserId;

                            await RemoveScheduleDetails(objsched.Id);

                            if (await SaveScheduleDetails(model, UserId))
                            {
                                transaction.Commit();
                                result = model.Id;
                            }
                        }
                    }
                    else
                    {
                        var cat = new Schedule
                        {
                            SessionYearId = model.SessionYearId,
                            BranchId = model.BranchId,
                            Title = model.Title,
                            StatusId = (int)ScheduleStatus.Schedule,
                            ExamTypeId = model.ExamTypeId,
                            ExamCount = model.ExamCount,
                            StartDate = model.StartDate,
                            EndDate = model.EndDate,
                            Description = model.Description,
                            IsActive = ((int)ActiveState.Active).ToString(),
                            CreatedDate = DateTime.Now,
                            CreatedByUserId = UserId
                        };

                        _context.Schedule.Add(cat);
                        if (await _context.SaveChangesAsync() > 0)
                        {
                            model.Id = cat.Id;
                            if (await SaveScheduleDetails(model, UserId))
                            {
                                transaction.Commit();
                                result = model.Id;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }
            return result;
        }
        private async Task RemoveScheduleDetails(int scheduleId)
        {
            var lstexamtime = await _context.ScheduleExamTime.Where(x => x.ScheduleId == scheduleId).ToListAsync();
            _context.ScheduleExamTime.RemoveRange(lstexamtime);

            var lstexamdate = await _context.ScheduleExamDate.Where(x => x.ScheduleId == scheduleId).ToListAsync();
            _context.ScheduleExamDate.RemoveRange(lstexamdate);

            var lstschbrcls = await _context.ScheduleBranchClass.Where(x => x.ScheduleId == scheduleId).ToListAsync();
            _context.ScheduleBranchClass.RemoveRange(lstschbrcls);

            var lstschbrclssub = await _context.ScheduleBranchClassSubject.Where(x => x.ScheduleId == scheduleId).ToListAsync();
            _context.ScheduleBranchClassSubject.RemoveRange(lstschbrclssub);

            var lstexam = await _context.Examination.Where(x => x.ScheduleId == scheduleId).ToListAsync();
            _context.Examination.RemoveRange(lstexam);
        }
        private async Task<bool> SaveScheduleDetails(Schedule model, int UserId)
        {
            if (await SaveScheduleExamTime(model, UserId) != 0 &&
                await SaveScheduleExamDate(model, UserId) != 0 &&
                await SaveScheduleBranchClass(model, UserId) != 0 &&
                await SaveScheduleTeacher(model, UserId) != 0)
            {
                return true;
            }
            return false;
        }
        public async Task<int> SaveScheduleExamTime(Schedule model, int UserId)
        {
            if (model.Id <= 0) return 0;

            var lstschbrcls = await _context.ScheduleExamTime.Where(x => x.ScheduleId == model.Id).ToListAsync();
            _context.ScheduleExamTime.RemoveRange(lstschbrcls);

            var lstchkcount = model.LstExamTime.Where(x => x.IsSelected).ToList();
            if (lstchkcount.Count > 0)
            {
                foreach (var item1 in lstchkcount)
                {
                    var subTea = new ScheduleExamTime
                    {
                        ScheduleId = model.Id,
                        SessionYearId = model.SessionYearId,
                        ExamTimeId = item1.Id,
                        CreatedByUserId = UserId,
                        CreatedDate = DateTime.Now
                    };
                    _context.ScheduleExamTime.Add(subTea);
                }
            }
            return await _context.SaveChangesAsync();
        }

        public async Task<int> SaveScheduleExamDate(Schedule model, int UserId)
        {
            if (model.Id <= 0) return 0;

            var lstschbrcls = await _context.ScheduleExamDate.Where(x => x.ScheduleId == model.Id).ToListAsync();
            _context.ScheduleExamDate.RemoveRange(lstschbrcls);

            var lstchkcount = model.LstStrDate.Where(x => x.IsSelected).ToList();
            if (lstchkcount.Count > 0)
            {
                var newExamDates = lstchkcount.Select(item1 => new ScheduleExamDate
                {
                    ScheduleId = model.Id,
                    SessionYearId = model.SessionYearId,
                    ExamDate = DateTime.ParseExact(item1.ExamDate, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                    CreatedByUserId = UserId,
                    CreatedDate = DateTime.Now
                }).ToList();

                _context.ScheduleExamDate.AddRange(newExamDates);
            }

            return await _context.SaveChangesAsync();
        }
        public async Task<int> SaveScheduleBranchClass(Schedule model, int UserId)
        {
            if (model.Id <= 0) return 0;

            var lstschbrcls = await _context.ScheduleBranchClass.Where(x => x.ScheduleId == model.Id).ToListAsync();
            _context.ScheduleBranchClass.RemoveRange(lstschbrcls);

            var lstchkcount = model.LstScheduleClass.Where(x => x.IsSelected).ToList();
            if (lstchkcount.Count > 0)
            {
                var newBranchClasses = lstchkcount.Select(item1 => new ScheduleBranchClass
                {
                    ScheduleId = model.Id,
                    SessionYearId = model.SessionYearId,
                    BranchClassId = item1.Id,
                    BranchId = item1.BranchId,
                    ClassId = item1.ClassId,
                    CreatedByUserId = UserId,
                    CreatedDate = DateTime.Now
                }).ToList();

                _context.ScheduleBranchClass.AddRange(newBranchClasses);
            }

            return await _context.SaveChangesAsync();
        }
        public async Task<int> SaveScheduleTeacher(Schedule model, int UserId)
        {
            if (model.Id <= 0) return 0;

            var lstschbrcls = await _context.ScheduleTeacher.Where(x => x.ScheduleId == model.Id).ToListAsync();
            _context.ScheduleTeacher.RemoveRange(lstschbrcls);

            var lstchkcount = model.LstScheduleTeachers.Where(x => x.IsSelected).ToList();
            if (lstchkcount.Count > 0)
            {
                var newTeachers = lstchkcount.Select(item1 => new ScheduleTeacher
                {
                    ScheduleId = model.Id,
                    SessionYearId = model.SessionYearId,
                    TeacherId = item1.Id,
                    BranchId = model.BranchId,
                    CreatedByUserId = UserId,
                    CreatedDate = DateTime.Now
                }).ToList();

                _context.ScheduleTeacher.AddRange(newTeachers);
            }

            return await _context.SaveChangesAsync();
        }
        public async Task<bool> DeleteSchedule(int Id, int UserId)
        {
            var cat = await _context.Schedule.SingleOrDefaultAsync(m => m.Id == Id);
            if (cat == null) return false;

            cat.IsActive = ((int)ActiveState.InActive).ToString();
            cat.LastModifiedByUserId = UserId;
            cat.LastModifiedDate = DateTime.Now;

            return _context.SaveChanges() > 0;
        }
        public async Task<HallTicket> GetHallTicket(string scheduleId, string studentId, int BranchId)
        {
            if (string.IsNullOrEmpty(scheduleId) || string.IsNullOrEmpty(studentId)) return null;

            int schId = int.Parse(scheduleId);
            int studid = int.Parse(studentId);

            var rptsettings = await _context.ReportSettings.AsNoTracking().SingleOrDefaultAsync(x=>x.BranchId==BranchId);
            var studentallocation = await _context.VScheduleStudentAllocation.AsNoTracking().SingleOrDefaultAsync(x => x.ScheduleId == schId && x.StudentId == studid);
            if (studentallocation == null) return null;

            var lstclasssub = await _context.VScheduleBranchClassSubjects.AsNoTracking().Where(x => x.ScheduleId == schId && x.ClassId == studentallocation.ClassId).ToListAsync();
            var lstsubject = lstclasssub.Select(x => new HallTicketSubject
            {
                SubjectName = x.SubjectName,
                InvSignature = ""
            }).ToList();

            return new HallTicket
            {
                ReportSettings = rptsettings,
                ScheduleStudentAllocation = studentallocation,
                LstHallTicketSubject = lstsubject
            };
        }
        public async Task<int> SaveScheduleBranchClassSubject(ScheduleBranchClassSubjectVM model, int UserId)
        {
            int result = 0;
            try
            {
                if (model.ScheduleId > 0)
                {
                    var lstschbrcls = await _context.ScheduleBranchClassSubject.Where(x => x.ScheduleId == model.ScheduleId).ToListAsync();
                    _context.ScheduleBranchClassSubject.RemoveRange(lstschbrcls);

                    if (model.LstScheduleBranchClassSubject != null && model.LstScheduleBranchClassSubject.Count > 0)
                    {
                        var lstchkcount = model.LstScheduleBranchClassSubject.Where(x => x.IsSelected).ToList();
                        if (lstchkcount.Count > 0)
                        {
                            var lstclssubject = _context.ClassSubject.AsNoTracking().ToDictionary(x => x.Id);
                            var lstschedbrcls = _context.ScheduleBranchClass.AsNoTracking().Where(x => x.ScheduleId == model.ScheduleId).ToDictionary(x => x.ClassId);

                            var newSubjects = lstchkcount.Select(item1 =>
                            {
                                var objclssub = lstclssubject[item1.ClassSubjectId];
                                var objbrcls = lstschedbrcls[objclssub.ClassId];

                                return new ScheduleBranchClassSubject
                                {
                                    ScheduleId = model.ScheduleId,
                                    SessionYearId = model.SessionYearId,
                                    ClassSubjectId = item1.ClassSubjectId,
                                    Marks = item1.Marks,
                                    BranchClassId = objbrcls.BranchClassId,
                                    BranchId = objbrcls.BranchId,
                                    ClassId = objbrcls.ClassId,
                                    SubjectId = objclssub.SubjectId,
                                    CreatedByUserId = UserId,
                                    CreatedDate = DateTime.Now
                                };
                            }).ToList();

                            _context.ScheduleBranchClassSubject.AddRange(newSubjects);
                        }
                    }

                    if (await _context.SaveChangesAsync() > 0)
                    {
                        var objsched = await _context.Schedule.SingleOrDefaultAsync(x => x.Id == model.ScheduleId);
                        if (objsched != null)
                        {
                            objsched.StatusId = (int)ScheduleStatus.Subject;
                            objsched.LastModifiedByUserId = UserId;
                            objsched.LastModifiedDate = DateTime.Now;
                            result = _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Handle exception
            }
            return result;
        }
        public async Task<int> SaveScheduleExamAlgo1(string ScheduleId, int UserId)
        {
            int index = 0;
            if (!string.IsNullOrEmpty(ScheduleId))
            {
                int schId = int.Parse(ScheduleId);

                var lstbrclssubj = _context.ScheduleBranchClassSubject.Where(x => x.ScheduleId == schId).ToList();
                var lstschedates = _context.ScheduleExamDate.Where(x => x.ScheduleId == schId).ToList();
                var lstschetimes = _context.ScheduleExamTime.Where(x => x.ScheduleId == schId).ToList();
                var lsttimes = _context.ExamTime.Where(x => lstschetimes.Select(t => t.ExamTimeId).Contains(x.Id)).ToList();

                var objSched = _context.Schedule.SingleOrDefault(x => x.Id == schId);
                if (objSched != null)
                {
                    var lastbrclsid = 0;
                    var lastsubid = 0;
                    DateTime? lastexdt = null;
                    var lastextimeid = 0;

                    if (lsttimes.Count == 1 && lstschedates.Count == lstbrclssubj.Count)
                    {
                        for (int i = 0; i < lstschedates.Count; i++)
                        {
                            var objExam = new Examination
                            {
                                ScheduleId = lstschedates[i].ScheduleId,
                                ExamDate = lstschedates[i].ExamDate,
                                ExamTimeId = lsttimes.First().Id,
                                BranchClassId = lstbrclssubj[i].BranchClassId,
                                ClassSubjectId = lstbrclssubj[i].ClassSubjectId,
                                BranchId = (int)lstbrclssubj[i].BranchId,
                                ClassId = (int)lstbrclssubj[i].ClassId,
                                SubjectId = (int)lstbrclssubj[i].SubjectId,
                                CreatedDate = DateTime.Now,
                                CreatedByUserId = UserId,
                                IsActive = ((int)ActiveState.Active).ToString()
                            };
                            _context.Examination.Add(objExam);
                        }
                    }
                    else if (lsttimes.Count > 1 && lstschedates.Count < lstbrclssubj.Count)
                    {
                        for (int i = 0; i < lstbrclssubj.Count; i++)
                        {
                            var objExam = new Examination
                            {
                                ScheduleId = lstbrclssubj[i].ScheduleId,
                                ExamDate = lastexdt != null ? lstschedates.First(x => x.ExamDate == lastexdt).ExamDate : lstschedates[i].ExamDate,
                                ExamTimeId = lastextimeid > 0 ? lsttimes.First(x => x.Id == lastextimeid).Id : lsttimes[i].Id,
                                BranchClassId = lstbrclssubj[i].BranchClassId,
                                ClassSubjectId = lstbrclssubj[i].ClassSubjectId,
                                BranchId = (int)lstbrclssubj[i].BranchId,
                                ClassId = (int)lstbrclssubj[i].ClassId,
                                SubjectId = (int)lstbrclssubj[i].SubjectId,
                                CreatedDate = DateTime.Now,
                                CreatedByUserId = UserId,
                                IsActive = ((int)ActiveState.Active).ToString()
                            };
                            lastexdt = objExam.ExamDate;
                            lastextimeid = (int)objExam.ExamTimeId;
                            _context.Examination.Add(objExam);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < lstschedates.Count; i++)
                        {
                            var objExam = new Examination
                            {
                                ScheduleId = lstschedates[i].ScheduleId,
                                ExamDate = lstschedates[i].ExamDate,
                                ExamTimeId = lastextimeid > 0 ? lsttimes.First(x => x.Id == lastextimeid).Id : lsttimes[i].Id,
                                BranchClassId = lastbrclsid > 0 ? lstbrclssubj.First(x => x.Id == lastbrclsid).BranchClassId : lstbrclssubj[i].BranchClassId,
                                ClassSubjectId = lastsubid > 0 ? lstbrclssubj.First(x => x.Id == lastsubid).ClassSubjectId : lstbrclssubj[i].ClassSubjectId,
                                BranchId = (int)lstbrclssubj[i].BranchId,
                                ClassId = (int)lstbrclssubj[i].ClassId,
                                SubjectId = (int)lstbrclssubj[i].SubjectId,
                                CreatedDate = DateTime.Now,
                                CreatedByUserId = UserId,
                                IsActive = ((int)ActiveState.Active).ToString()
                            };
                            lastextimeid = (int)objExam.ExamTimeId;
                            lastbrclsid = (int)objExam.BranchClassId;
                            lastsubid = (int)objExam.ClassSubjectId;
                            _context.Examination.Add(objExam);
                        }
                    }
                    index = _context.SaveChanges();
                }
            }
            return index;
        }
        public async Task<int> SaveScheduleExamAlgo(string ScheduleId, int UserId)
        {
            int index = 0;
            if (!string.IsNullOrEmpty(ScheduleId))
            {
                int schId = int.Parse(ScheduleId);

                var lstbrclssubj = await _context.ScheduleBranchClassSubject.AsNoTracking().Where(x => x.ScheduleId == schId).ToListAsync();
                var lstschedates = await _context.ScheduleExamDate.AsNoTracking().Where(x => x.ScheduleId == schId).ToListAsync();
                var lstschetimes = await _context.ScheduleExamTime.AsNoTracking().Where(x => x.ScheduleId == schId).ToListAsync();
                var lsttimes = await _context.ExamTime.AsNoTracking().Where(x => lstschetimes.Select(t => t.ExamTimeId).Contains(x.Id)).ToListAsync();

                var objSched = await _context.Schedule.SingleOrDefaultAsync(x => x.Id == schId);
                if (objSched != null)
                {
                    var nextscheduledateid = lstschedates.First().Id;
                    var nextscheduletimeid = lstschetimes.First().Id;

                    for (int i = 0; i < lstbrclssubj.Count; i++)
                    {
                        var objinddt = lstschedates.First(x => x.Id == nextscheduledateid);
                        nextscheduledateid = lstschedates.NextOf(objinddt).Id;

                        var objindtt = lstschetimes.First(x => x.Id == nextscheduletimeid);
                        nextscheduletimeid = lstschetimes.NextOf(objindtt).Id;

                        var objExam = new Examination
                        {
                            ScheduleId = schId,
                            SessionYearId = objSched.SessionYearId,
                            BranchClassId = lstbrclssubj[i].BranchClassId,
                            ClassSubjectId = lstbrclssubj[i].ClassSubjectId,
                            BranchId = (int)lstbrclssubj[i].BranchId,
                            ClassId = (int)lstbrclssubj[i].ClassId,
                            SubjectId = (int)lstbrclssubj[i].SubjectId,
                            ExamDate = objinddt.ExamDate,
                            ExamTimeId = objindtt.ExamTimeId,
                            ExamTypeId = objSched.ExamTypeId,
                            CreatedDate = DateTime.Now,
                            CreatedByUserId = UserId,
                            IsActive = ((int)ActiveState.Active).ToString()
                        };
                        _context.Examination.Add(objExam);
                    }
                    index = _context.SaveChanges();
                }
            }
            return index;
        }
        public async Task<int> UpdateScheduleTimeTableStatus(string ScheduleId, int UserId)
        {
            if (string.IsNullOrEmpty(ScheduleId))
            {
                return 0;
            }

            int schId = int.Parse(ScheduleId);
            var objSch = await _context.Schedule.SingleOrDefaultAsync(x => x.Id == schId);
            if (objSch == null)
            {
                return 0;
            }

            objSch.StatusId = (int)ScheduleStatus.TimeTable;
            return await _context.SaveChangesAsync();
        }
        public async Task<int> UpdateScheduleTeacherStatus(string ScheduleId, int UserId)
        {
            if (string.IsNullOrEmpty(ScheduleId))
            {
                return 0;
            }

            int schId = int.Parse(ScheduleId);
            var objSch = await _context.Schedule.SingleOrDefaultAsync(x => x.Id == schId);
            if (objSch == null)
            {
                return 0;
            }

            objSch.StatusId = (int)ScheduleStatus.Finish; //teacher
            return await _context.SaveChangesAsync();
        }
        public async Task<int> UpdateScheduleFinishStatus(string ScheduleId, int UserId)
        {
            if (string.IsNullOrEmpty(ScheduleId))
            {
                return 0;
            }

            int schId = int.Parse(ScheduleId);
            var objSch = await _context.Schedule.SingleOrDefaultAsync(x => x.Id == schId);
            if (objSch == null)
            {
                return 0;
            }

            objSch.StatusId = (int)ScheduleStatus.Finish;
            return await _context.SaveChangesAsync();
        }
        public async Task<int> SaveScheduleTeacherExamDateAlgo(string ScheduleId, int UserId)
        {
            if (string.IsNullOrEmpty(ScheduleId))
            {
                return 0;
            }

            int schId = int.Parse(ScheduleId);
            var objSched = await _context.Schedule.SingleOrDefaultAsync(x => x.Id == schId);
            if (objSched == null)
            {
                return 0;
            }

            var lstschedates = await _context.ScheduleExamDate.Where(x => x.ScheduleId == schId).AsNoTracking().ToListAsync();
            var lstexamhall = await _context.ScheduleExamHall.Where(x => x.ScheduleId == schId).AsNoTracking().ToListAsync();
            var lstteacher = await _context.ScheduleTeacher.Where(x => x.ScheduleId == schId).AsNoTracking().ToListAsync();
            var firstschteacher = lstteacher.FirstOrDefault();

            if (firstschteacher == null)
            {
                return 0;
            }

            int nextschteacherid = firstschteacher.Id;
            foreach (var hall in lstexamhall)
            {
                foreach (var dt in lstschedates)
                {
                    var objindtea = lstteacher.FirstOrDefault(x => x.Id == nextschteacherid);
                    nextschteacherid = lstteacher.NextOf(objindtea).Id;

                    var objTea = new ScheduleTeacherExamHall
                    {
                        ScheduleId = dt.ScheduleId,
                        SessionYearId = objSched.SessionYearId,
                        TeacherId = lstteacher.NextOf(objindtea).TeacherId,
                        ScheduleExamHallId = hall.Id,
                        ScheduleExamDateId = dt.Id,
                        CreatedDate = DateTime.Now,
                        CreatedByUserId = UserId
                    };
                    _context.ScheduleTeacherExamHall.Add(objTea);
                }
            }
            return await _context.SaveChangesAsync();
        }
        public async Task<ScheduleExamProcedure> GetPivotTimeTable(string ScheduleId, int UserId)
        {
            if (string.IsNullOrEmpty(ScheduleId))
            {
                return null;
            }

            int schId = int.Parse(ScheduleId);
            DataTable dt = new DataTable();
            try
            {

                var rawData = _context.V_SP_ScheduleExamination
                    .AsNoTracking()
             .Where(x => x.ScheduleId == schId)
            .Select(e => new
            {
                e.ScheduleId,
                e.ClassName,
                e.StrExamDate,
                e.SubjectName
            })
            .ToList();

                // Step 2: Get all unique StrExamDate values for the dynamic columns
                var examDates = rawData.Select(x => x.StrExamDate).Distinct().ToList();

                // Step 3: Group data by ScheduleId and ClassName
                var pivotedData = rawData
                    .GroupBy(x => new { x.ScheduleId, x.ClassName })
                    .Select(g => {
                        var pivot = new ExpandoObject() as IDictionary<string, Object>;
                        pivot["ScheduleId"] = g.Key.ScheduleId;
                        pivot["ClassName"] = g.Key.ClassName;

                        // Add StrExamDate columns dynamically
                        foreach (var examDate in examDates)
                        {
                            // Get the SubjectName for the current StrExamDate (if exists)
                            var subject = g.FirstOrDefault(x => x.StrExamDate == examDate);
                            pivot[examDate] = subject?.SubjectName; // Add SubjectName or null if not available
                        }

                        return pivot;
                    })
                    .ToList();

                // Step 4: Convert pivoted data to DataTable
                dt = ConvertToDataTable(pivotedData);


                //using (MySqlConnection conn = new MySqlConnection(_connectionString))
                //{
                //    MySqlCommand sqlComm = new MySqlCommand("SP_ScheduleExamination", conn);
                //    sqlComm.Parameters.AddWithValue("@ScheduleId", schId);
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

            return new ScheduleExamProcedure { StrResult = DataTableToJSON(dt) };
        }
        public async Task<List<ScheduleBranchClassGroup>> GetScheduleBranchClassGroup(string ScheduleId, int UserId)
        {
            if (string.IsNullOrEmpty(ScheduleId))
            {
                return new List<ScheduleBranchClassGroup>();
            }

            int schId = int.Parse(ScheduleId);
            return await _context.VScheduleBranchClasses.AsNoTracking()
                .Where(x => x.ScheduleId == schId)
                .Select(x => new ScheduleBranchClassGroup
                {
                    Id = (int)x.Id,
                    ScheduleId = (int)x.ScheduleId,
                    BranchClassId = (int)x.BranchClassId,
                    BranchId = (int)x.BranchId,
                    ClassId = (int)x.ClassId,
                    ClassName = x.ClassName,
                    StudentCount = (int)x.StudentCount
                })
                .ToListAsync();
        }
        public async Task<List<ScheduleBranchClassStudent>> GetScheduleBranchClassStudent(ClassAllocation model, int UserId)
        {
            if (model == null || model.ScheduleBranchClassIds == null || !model.ScheduleBranchClassIds.Any())
            {
                return new List<ScheduleBranchClassStudent>();
            }

            var lstschbrcls = await _context.VScheduleBranchClasses.AsNoTracking()
                .Where(x => model.ScheduleBranchClassIds.Contains((int)x.Id))
                .ToListAsync();

            if (!lstschbrcls.Any())
            {
                return new List<ScheduleBranchClassStudent>();
            }

            var lstclassids = lstschbrcls.Select(x => x.BranchClassId).Distinct().ToList();
            var lststudent = await _context.VStudentCurrentLocation.AsNoTracking()
                .Where(x => lstclassids.Contains(x.BranchClassId))
                .ToListAsync();

            return lststudent.Select(x => new ScheduleBranchClassStudent
            {
                StudentId = x.StudentId,
                ScheduleId = model.ScheduleId,
                BranchClassId = (int)x.BranchClassId,
                BranchId = x.BranchId,
                ClassId = x.ClassId,
                SectionId = x.SectionId,
                FullName = x.FullName,
                RollNo = x.RollNo
            }).ToList();
        }
        public async Task<int> SaveScheduleExamHall(List<ExamHall> lstModel, int UserId)
        {
            if (lstModel == null || !lstModel.Any())
            {
                return 0;
            }

            int index = 0;
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var model in lstModel)
                    {
                        var hall = new ScheduleExamHall
                        {
                            ScheduleId = model.ScheduleId,
                            SessionYearId = model.SessionYearId,
                            Name = model.Name,
                            RowCount = model.RowCount,
                            ColumnCount = model.ColumnCount,
                            CreatedDate = DateTime.Now,
                            CreatedByUserId = UserId
                        };

                        _context.ScheduleExamHall.Add(hall);
                        await _context.SaveChangesAsync();

                        foreach (var seat in model.Seats)
                        {
                            var objseat = new ScheduleExamHallSeat
                            {
                                ScheduleId = model.ScheduleId,
                                SessionYearId = model.SessionYearId,
                                ExamHallId = hall.Id,
                                SeatNumber = seat.SeatNumber,
                                IsAllocated = seat.IsAllocated ? "1" : "0",
                                CreatedDate = DateTime.Now,
                                CreatedByUserId = UserId
                            };

                            _context.ScheduleExamHallSeat.Add(objseat);
                            await _context.SaveChangesAsync();

                            if (seat.ScheduleBranchClassStudent != null)
                            {
                                var allocation = new ScheduleStudentAllocation
                                {
                                    ScheduleId = model.ScheduleId,
                                    SessionYearId = model.SessionYearId,
                                    ExamHallId = hall.Id,
                                    SeatId = objseat.Id,
                                    StudentId = seat.ScheduleBranchClassStudent.StudentId,
                                    BranchClassId = seat.ScheduleBranchClassStudent.BranchClassId,
                                    BranchId = seat.ScheduleBranchClassStudent.BranchId,
                                    ClassId = seat.ScheduleBranchClassStudent.ClassId,
                                    SectionId = seat.ScheduleBranchClassStudent.SectionId,
                                    CreatedDate = DateTime.Now,
                                    CreatedByUserId = UserId
                                };

                                _context.ScheduleStudentAllocation.Add(allocation);
                                await _context.SaveChangesAsync();
                                index++;
                            }
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }

            return index;
        }
        public async Task<ScheduleTeacherProcedure> GetPivotScheduleTeacher(string ScheduleId, int UserId)
        {
            if (string.IsNullOrEmpty(ScheduleId))
            {
                return null;
            }

            int schId = int.Parse(ScheduleId);
            DataTable dt = new DataTable();

            try
            {            

                var rawData = _context.V_SP_ScheduleTeacher
                    .AsNoTracking()
           .Where(x => x.ScheduleId == schId)
           .Select(e => new
           {
               e.ScheduleId,
               e.TeacherName,
               e.StrExamDate,
               e.ExamHallName
           })
           .ToList();

                // Step 2: Get all unique StrExamDate values for the dynamic columns
                var examDates = rawData.Select(x => x.StrExamDate).Distinct().ToList();

                // Step 3: Group data by ScheduleId and TeacherName
                var pivotedData = rawData
                    .GroupBy(x => new { x.ScheduleId, x.TeacherName })
                    .Select(g => {
                        var pivot = new ExpandoObject() as IDictionary<string, Object>;
                        pivot["ScheduleId"] = g.Key.ScheduleId;
                        pivot["TeacherName"] = g.Key.TeacherName;

                        // Add StrExamDate columns dynamically
                        foreach (var examDate in examDates)
                        {
                            // Get the ExamHallName for the current StrExamDate (if exists)
                            var hall = g.FirstOrDefault(x => x.StrExamDate == examDate);
                            pivot[examDate.ToString()] = hall?.ExamHallName; // Add ExamHallName or null if not available
                        }

                        return pivot;
                    })
                    .ToList();

                // Step 4: Convert pivoted data to DataTable
                dt = ConvertToDataTable(pivotedData);


                //using (MySqlConnection conn = new MySqlConnection(_connectionString))
                //{
                //    MySqlCommand sqlComm = new MySqlCommand("SP_ScheduleTeachers", conn);
                //    sqlComm.Parameters.AddWithValue("@ScheduleId", schId);
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

            return new ScheduleTeacherProcedure { StrResult = DataTableToJSON(dt) };
        }
        public async Task<int> GetScheduleStatus(string ScheduleId)
        {
            if (string.IsNullOrEmpty(ScheduleId))
            {
                return 0;
            }

            int schId = int.Parse(ScheduleId);
            var objSch = await _context.Schedule.AsNoTracking().SingleOrDefaultAsync(x => x.Id == schId && x.IsActive == ((int)ActiveState.Active).ToString());
            return objSch?.StatusId ?? 0;
        }
        public async Task<int> SaveScheduleExam(ScheduleBrclsSubject model, int UserId)
        {
            if (model == null)
            {
                return 0;
            }
            var lstvexam = await _context.VExaminations.Where(m => m.ScheduleId == model.ScheduleId).ToListAsync();
            var lstexamids = lstvexam.Select(x => x.Id).ToList();
            var lstExam = await _context.Examination.Where(x => lstexamids.Contains(x.Id)).ToListAsync();

            foreach (var brcls in model.LstBranchClass)
            {
                foreach (var param in brcls.LstClassSubject)
                {
                    var exam = lstExam.SingleOrDefault(x => x.ClassSubjectId == param.Id && x.BranchClassId == brcls.Id && x.ScheduleId == model.ScheduleId);
                    if (exam != null)
                    {
                        exam.ExamDate = DateTime.ParseExact(param.StrExamDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                        exam.ExamTimeId = param.ExamTimeId;
                        exam.LastModifiedDate = DateTime.Now;
                        exam.LastModifiedByUserId = UserId;
                    }
                }
            }

            return await _context.SaveChangesAsync();
        }
        public async Task<SpScheduleTeacher> GetSpScheduleTeacher(string scheduleId)
        {
            if (string.IsNullOrEmpty(scheduleId))
            {
                return null;
            }

            int schid = int.Parse(scheduleId);
            var lstexamdate = await _context.VScheduleExamDates.AsNoTracking()
                .Where(x => x.ScheduleId == schid)
                .Select(x => x.StrExamDate + "," + x.Id)
                .ToListAsync();

            string joineddates = "[" + string.Join("],[", lstexamdate) + "]";
            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();

            try
            {
                //var connstring = _context.Database.GetDbConnection().ConnectionString;
                //using (MySqlConnection conn = new MySqlConnection(_connectionString))
                //{
                //    MySqlCommand sqlComm = new MySqlCommand("SP_PreScheduleTeacher", conn);
                //    //sqlComm.Parameters.AddWithValue("@ColumnToPivot", "StrExamDate");
                //    //sqlComm.Parameters.AddWithValue("@ListToPivot", joineddates);
                //    sqlComm.Parameters.AddWithValue("@ScheduleId", scheduleId);

                //    sqlComm.CommandType = CommandType.StoredProcedure;
                //    MySqlDataAdapter da = new MySqlDataAdapter();
                //    da.SelectCommand = sqlComm;
                //    da.Fill(dt);
                //}
                //using (MySqlConnection conn = new MySqlConnection(_connectionString))
                //{
                //    MySqlCommand sqlComm = new MySqlCommand("SP_PostScheduleTeacher", conn);
                //    sqlComm.Parameters.AddWithValue("@ColumnToPivot", "StrExamDate");
                //    sqlComm.Parameters.AddWithValue("@ListToPivot", joineddates);
                //    sqlComm.Parameters.AddWithValue("@ScheduleId", scheduleId);

                //    sqlComm.CommandType = CommandType.StoredProcedure;
                //    MySqlDataAdapter da = new MySqlDataAdapter();
                //    da.SelectCommand = sqlComm;
                //    da.Fill(dt2);
                //}
            }
            catch (Exception)
            {
                // Handle exception
            }

            return new SpScheduleTeacher { PreResult = DataTableToJSON(dt), PostResult = DataTableToJSON(dt2) };
        }
        public async Task<SpScheduleExam> GetSpScheduleExam(string scheduleId)
        {
            if (string.IsNullOrEmpty(scheduleId))
            {
                return null;
            }

            int schid = int.Parse(scheduleId);
            var lstbrclssubids = await _context.VScheduleBranchClassSubjects.AsNoTracking()
                .Where(x => x.ScheduleId == schid)
                .Select(x => x.SubjectName)
                .Distinct()
                .ToListAsync();

            string joinedsub = "[" + string.Join("],[", lstbrclssubids) + "]";
            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();

            try
            {
                //var connstring = _context.Database.GetDbConnection().ConnectionString;
                //using (MySqlConnection conn = new MySqlConnection(_connectionString))
                //{
                //    MySqlCommand sqlComm = new MySqlCommand("SP_PreScheduleExam", conn);
                //    sqlComm.Parameters.AddWithValue("@ColumnToPivot", "SubjectName");
                //    sqlComm.Parameters.AddWithValue("@ListToPivot", joinedsub);
                //    sqlComm.Parameters.AddWithValue("@ScheduleId", scheduleId);

                //    sqlComm.CommandType = CommandType.StoredProcedure;
                //    MySqlDataAdapter da = new MySqlDataAdapter();
                //    da.SelectCommand = sqlComm;
                //    da.Fill(dt);
                //}
                //using (MySqlConnection conn = new MySqlConnection(_connectionString))
                //{
                //    MySqlCommand sqlComm = new MySqlCommand("SP_PostScheduleExam", conn);
                //    sqlComm.Parameters.AddWithValue("@ColumnToPivot", "SubjectName");
                //    sqlComm.Parameters.AddWithValue("@ListToPivot", joinedsub);
                //    sqlComm.Parameters.AddWithValue("@ScheduleId", scheduleId);

                //    sqlComm.CommandType = CommandType.StoredProcedure;
                //    MySqlDataAdapter da = new MySqlDataAdapter();
                //    da.SelectCommand = sqlComm;
                //    da.Fill(dt2);
                //}
            }
            catch (Exception)
            {
                // Handle exception
            }

            return new SpScheduleExam { PreResult = DataTableToJSON(dt), PostResult = DataTableToJSON(dt2) };
        }
        public async Task<SpScheduleExamDateTime> GetScheduleExamDateTime(string scheduleId)
        {
            if (string.IsNullOrEmpty(scheduleId))
            {
                return null;
            }

            int schid = int.Parse(scheduleId);
            var lstdt = await _context.VScheduleExamDates.AsNoTracking()
                .Where(x => x.ScheduleId == schid)
                .ToListAsync();

            var lsttime = await _context.VScheduleExamTimes.AsNoTracking()
                .Where(x => x.ScheduleId == schid)
                .ToListAsync();

            return new SpScheduleExamDateTime
            {
                StrDate = JsonConvert.SerializeObject(lstdt),
                StrTime = JsonConvert.SerializeObject(lsttime)
            };
        }
        public async Task<int> SaveScheduleTeacherExamDate(ScheduleTeacherExam model, int UserId)
        {
            if (model == null)
            {
                return 0;
            }

            try
            {
                var AllscheTeacherExamHall = await _context.ScheduleTeacherExamHall
                    .Where(x => x.ScheduleId == model.ScheduleId)
                    .ToListAsync();

                foreach (var hall in model.LstCastExamHall)
                {
                    foreach (var examdate in hall.LstCastExamDate)
                    {
                        var tbl = AllscheTeacherExamHall
                            .FirstOrDefault(x => x.ScheduleExamDateId == examdate.Id && x.ScheduleExamHallId == hall.Id);

                        if (tbl != null)
                        {
                            tbl.TeacherId = (int)examdate.TeacherId;
                        }
                    }
                }

                return await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                // Handle exception
                return 0;
            }
        }
        public async Task<int> SaveScheduleExamination(ScheduleExam model, int UserId)
        {
            if (model == null || model.LstScheduleExamination.Count == 0)
            {
                return 0;
            }

            try
            {
                foreach (var item in model.LstScheduleExamination)
                {
                    var objResult = await _context.Examination
                        .SingleOrDefaultAsync(x => x.ScheduleId == item.ScheduleId && x.BranchClassId == item.BranchClassId && x.ClassSubjectId == item.ClassSubjectId);

                    if (objResult != null)
                    {
                        objResult.StrExamDate = item.StrExamDate;
                        if (!string.IsNullOrEmpty(item.StrExamDate))
                        {
                            objResult.ExamDate = Convert.ToDateTime(item.StrExamDate);
                        }
                        objResult.ExamTimeId = item.ExamTimeId;
                        objResult.LastModifiedByUserId = UserId;
                        objResult.LastModifiedDate = DateTime.Now;
                    }
                    else
                    {
                        var objExam = new Examination
                        {
                            SessionYearId = item.SessionYearId,
                            ScheduleId = item.ScheduleId,
                            StrExamDate = item.StrExamDate,
                            ExamDate = !string.IsNullOrEmpty(item.StrExamDate) ? Convert.ToDateTime(item.StrExamDate) : (DateTime?)null,
                            ExamTimeId = item.ExamTimeId,
                            BranchClassId = item.BranchClassId,
                            ClassSubjectId = item.ClassSubjectId,
                            CreatedDate = DateTime.Now,
                            CreatedByUserId = UserId,
                            IsActive = ((int)ActiveState.Active).ToString()
                        };
                        _context.Examination.Add(objExam);
                    }
                }

                var schresult = await _context.Schedule
                    .SingleOrDefaultAsync(x => x.Id == model.ScheduleId && x.IsActive == ((int)ActiveState.Active).ToString());

                if (schresult != null)
                {
                    schresult.StatusId = (int)ScheduleStatus.TimeTable;
                    return await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                // Handle exception
            }

            return 0;
        }
        public async Task<List<ScheduleStrDate>> GetScheduleDate(string startDate, string endDate, string scheduleId)
        {
            var lstResult = new List<ScheduleStrDate>();

            if (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate))
            {
                return lstResult;
            }

            DateTime startDt = DateTime.Parse(startDate);
            DateTime endDt = DateTime.Parse(endDate);

            for (var dt = startDt; dt <= endDt; dt = dt.AddDays(1))
            {
                var objHoliday = await _context.Holiday.AsNoTracking()
                    .SingleOrDefaultAsync(x => x.StartDate >= dt && x.EndDate <= dt);

                var obj = new ScheduleStrDate
                {
                    ExamDate = dt.ToString("dd/MM/yyyy"),
                    Comment = objHoliday?.Title ?? dt.DayOfWeek.ToString(),
                    DateColor = objHoliday != null || dt.DayOfWeek == DayOfWeek.Sunday ? "#f5424b" : "#171717"
                };

                lstResult.Add(obj);
            }

            if (!string.IsNullOrEmpty(scheduleId))
            {
                int schId = int.Parse(scheduleId);
                var lstschdate = await _context.ScheduleExamDate.AsNoTracking()
                    .Where(x => x.ScheduleId == schId)
                    .ToListAsync();

                foreach (var item in lstResult)
                {
                    if (lstschdate.Any(x => x.ExamDate.ToShortDateString() == item.ExamDate))
                    {
                        item.IsSelected = true;
                    }
                }
            }

            return lstResult;
        }
        public async Task<List<VBranchClasses>> GetScheduleBranchClassExam(string scheduleId)
        {
            if (string.IsNullOrEmpty(scheduleId))
            {
                return new List<VBranchClasses>();
            }

            int schId = int.Parse(scheduleId);
            var lst = await _context.ScheduleBranchClass.AsNoTracking()
                .Where(x => x.ScheduleId == schId)
                .ToListAsync();

            var brclsids = lst
                .Select(x => x.BranchClassId)
                .Distinct()
                .ToList();

            var lstbrcls = await _context.VBranchClasses.AsNoTracking()
                .Where(x => brclsids.Contains((int)x.Id))
                .ToListAsync();

            foreach (var item in lstbrcls)
            {
                var lstclasssubj = await _context.ScheduleBranchClassSubject.AsNoTracking()
                    .Where(x => x.ScheduleId == schId && x.BranchClassId == item.Id)
                    .OrderByDescending(x => x.Id)
                    .ToListAsync();

                var lstclssubjids = lstclasssubj
                    .Select(x => x.ClassSubjectId)
                    .Distinct()
                    .ToList();

                var lstclssubject = await _context.VClassSubjects.AsNoTracking()
                    .Where(x => lstclssubjids.Contains((int)x.Id))
                    .ToListAsync();

                if (lstclssubject.Count > 0)
                {
                    var lstschedates = await _context.ScheduleExamDate.AsNoTracking()
                        .Where(x => x.ScheduleId == schId)
                        .ToListAsync();

                    var lstdate = lstschedates
                        .Select(x => x.ExamDate.ToShortDateString())
                        .ToList();

                    var lstSelectdate = lstdate
                        .Select(x => new SelectListItem { Text = x, Value = x })
                        .ToList();

                    var lstschetimes = _context.ScheduleExamTime
                        .Where(x => x.ScheduleId == schId)
                        .ToList();

                    var lstschtimeids = lstschetimes
                        .Select(x => x.ExamTimeId)
                        .ToList();

                    var lsttimes = _context.ExamTime
                        .Where(x => lstschtimeids.Contains(x.Id))
                        .ToList();

                    var lstSelectTime = lsttimes
                        .Select(x => new SelectListItem { Text = x.FromTime + " - " + x.ToTime, Value = x.Id.ToString() })
                        .ToList();

                    foreach (var sub in lstclssubject)
                    {
                        sub.ScheduleId = schId;
                        sub.BranchClassId = item.Id;
                        sub.ExamDateSheet = new List<SelectListItem>(lstSelectdate);
                        sub.ExamTimeSheet = new List<SelectListItem>(lstSelectTime);

                        var objExam = await _context.Examination.AsNoTracking()
                            .SingleOrDefaultAsync(x => x.ScheduleId == sub.ScheduleId && x.BranchClassId == sub.BranchClassId && x.ClassSubjectId == sub.Id && x.IsActive == ((int)ActiveState.Active).ToString());

                        if (objExam != null)
                        {
                            if (objExam.ExamDate != null)
                            {
                                var strdate = (DateTime)objExam.ExamDate;
                                sub.StrExamDate = strdate.ToString("dd-MM-yyyy");
                                var divselectedItem = sub.ExamDateSheet.Find(p => p.Value == sub.StrExamDate);
                                if (divselectedItem != null)
                                {
                                    divselectedItem.Selected = true;
                                }
                            }

                            if (objExam.ExamTimeId > 0)
                            {
                                sub.ExamTimeId = objExam.ExamTimeId;
                                var divselectedTime = sub.ExamTimeSheet.Find(p => p.Value == objExam.ExamTimeId.ToString());
                                if (divselectedTime != null)
                                {
                                    divselectedTime.Selected = true;
                                }
                            }
                        }
                    }
                }

                item.LstClassSubject = new List<VClassSubjects>(lstclssubject);
            }

            return new List<VBranchClasses>(lstbrcls);
        }
        public async Task<List<VScheduleBranchClasses>> GetScheduleClassByBranch(string scheduleId, string branchId)
        {
            if (string.IsNullOrEmpty(scheduleId) || string.IsNullOrEmpty(branchId))
            {
                return new List<VScheduleBranchClasses>();
            }

            int brId = int.Parse(branchId);
            int schId = int.Parse(scheduleId);
            return await _context.VScheduleBranchClasses.AsNoTracking()
                .Where(x => x.ScheduleId == schId && x.BranchId == brId).AsNoTracking()
                .ToListAsync();
        }
        public async Task<List<ScheduleExamHall>> GetScheduleTeacherExamDate1(string scheduleId)
        {
            if (string.IsNullOrEmpty(scheduleId))
            {
                return new List<ScheduleExamHall>();
            }

            int schId = int.Parse(scheduleId);

            var allScheduleExamHalls = _context.ScheduleExamHall.Where(x => x.ScheduleId == schId).ToList();
            var allScheduleExamDates = _context.VScheduleExamDates.Where(x => x.ScheduleId == schId).ToList();
            var allScheduleTeachers = _context.VScheduleTeachers.Where(x => x.ScheduleId == schId).ToList();
            var allScheduleTeacherExamHalls = _context.ScheduleTeacherExamHall.Where(x => x.ScheduleId == schId).ToList();

            var teacherSelectList = allScheduleTeachers
                .Select(x => new SelectListItem { Text = x.TeacherName, Value = x.TeacherId.ToString() })
                .ToList();

            var examHallIds = allScheduleTeacherExamHalls.Select(x => x.ScheduleExamHallId).Distinct().ToList();
            var examHalls = allScheduleExamHalls.Where(x => examHallIds.Contains(x.Id)).ToList();

            foreach (var hall in examHalls)
            {
                var examDates = allScheduleTeacherExamHalls
                    .Where(x => x.ScheduleExamHallId == hall.Id)
                    .Select(x => x.ScheduleExamDateId)
                    .Distinct()
                    .ToList();

                hall.LstScheduleExamDates = allScheduleExamDates
                    .Where(x => examDates.Contains(x.Id))
                    .ToList();

                foreach (var examDate in hall.LstScheduleExamDates)
                {
                    examDate.TeacherSheet = new List<SelectListItem>(teacherSelectList);
                    var examHall = allScheduleTeacherExamHalls
                        .FirstOrDefault(x => x.ScheduleExamHallId == hall.Id && x.ScheduleExamDateId == examDate.Id);

                    if (examHall?.TeacherId > 0)
                    {
                        examDate.TeacherId = examHall.TeacherId;
                        var selectedItem = examDate.TeacherSheet.FirstOrDefault(p => p.Value == examHall.TeacherId.ToString());
                        if (selectedItem != null)
                        {
                            selectedItem.Selected = true;
                        }
                    }
                }
            }

            return examHalls;
        }
        public async Task<List<CastExamHall>> GetScheduleTeacherExamDate(string scheduleId)
        {
            if (string.IsNullOrEmpty(scheduleId))
            {
                return new List<CastExamHall>();
            }

            int schId = int.Parse(scheduleId);
            var allScheduleTeachers = await _context.VScheduleTeachers.AsNoTracking().Where(x => x.ScheduleId == schId).AsNoTracking().ToListAsync();
            var teacherSelectList = allScheduleTeachers
                .Select(x => new SelectListItem { Text = x.TeacherName, Value = x.TeacherId.ToString() })
                .ToList();

            var examHalls = await _context.ScheduleExamHall.AsNoTracking().Where(x => x.ScheduleId == schId).ToListAsync();
            var result = examHalls.Select(hall => new CastExamHall
            {
                Id = hall.Id,
                Name = hall.Name,
                LstCastExamDate =  _context.VScheduleTeacherExamHall.AsNoTracking()
                    .Where(x => x.ScheduleId == schId && x.ScheduleExamHallId == hall.Id)
                    .Select(x => new CastExamDate
                    {
                        Id = x.ScheduleExamDateId,
                        TeacherId = x.TeacherId,
                        StrExamDate = x.StrExamDate,
                        TeacherSheet = new List<SelectListItem>(teacherSelectList)
                    })
                    .ToList()
            }).ToList();

            foreach (var hall in result)
            {
                foreach (var date in hall.LstCastExamDate)
                {
                    if (date.TeacherId > 0)
                    {
                        var selectedItem = date.TeacherSheet.FirstOrDefault(p => p.Value == date.TeacherId.ToString());
                        if (selectedItem != null)
                        {
                            selectedItem.Selected = true;
                        }
                    }
                }
            }

            return result.ToList();
        }
        public async Task<List<CastScheduleExamHall>> GetCastScheduleExamHall(string scheduleId)
        {
            if (string.IsNullOrEmpty(scheduleId))
            {
                return new List<CastScheduleExamHall>();
            }

            int schId = int.Parse(scheduleId);
            var examHalls = await _context.ScheduleExamHall.Where(x => x.ScheduleId == schId).AsNoTracking().ToListAsync();
            var examHallSeats = await _context.ScheduleExamHallSeat.Where(x => x.ScheduleId == schId).AsNoTracking().ToListAsync();
            var studentAllocations = await _context.VScheduleStudentAllocation.Where(x => x.ScheduleId == schId).AsNoTracking().ToListAsync();

            var seats = examHallSeats.Select(x => new CastExamHallSeat
            {
                Id = x.Id,
                ExamHallId = x.ExamHallId,
                IsAllocated = x.IsAllocated == "1",
                SeatNumber = x.SeatNumber
            }).ToList();

            var allocations = studentAllocations.Select(x => new CastStudentAllocation
            {
                StudentId = x.StudentId,
                ExamHallId = x.ExamHallId,
                SeatId = x.SeatId,
                SeatNumber = x.SeatNumber,
                Name = x.FullName,
                RollNo = x.RollNo
            }).ToList();

            return examHalls.Select(hall => new CastScheduleExamHall
            {
                Id = hall.Id,
                RowCount = hall.RowCount,
                ColumnCount = hall.ColumnCount,
                Name = hall.Name,
                LstCastExamHallSeat = seats.Where(y => y.ExamHallId == hall.Id).ToList(),
                LstCastStudentAllocation = allocations.Where(y => y.ExamHallId == hall.Id).ToList()
            }).ToList();
        }
        public async Task<List<VScheduleStudentAllocation>> GetScheduleStudentInfo(string scheduleId, string branchClassId)
        {
            if (string.IsNullOrEmpty(scheduleId) || string.IsNullOrEmpty(branchClassId))
            {
                return new List<VScheduleStudentAllocation>();
            }

            int schId = int.Parse(scheduleId);
            int brclsId = int.Parse(branchClassId);

            var studentAllocations = await _context.VScheduleStudentAllocation.AsNoTracking()
                .Where(x => x.ScheduleId == schId && x.BranchClassId == brclsId)
                .OrderBy(x => x.RollNo)
                .ToListAsync();

            int serial = 0;
            foreach (var allocation in studentAllocations)
            {
                allocation.SerialNo = ++serial;
            }

            return studentAllocations;
        }
        public async Task<List<VExaminations>> GetScheduleExamination(string scheduleId, string branchId)
        {
            if (string.IsNullOrEmpty(scheduleId) || string.IsNullOrEmpty(branchId))
            {
                return new List<VExaminations>();
            }

            int brId = int.Parse(branchId);
            int schId = int.Parse(scheduleId);

            return await _context.VExaminations.AsNoTracking()
                .Where(x => x.ScheduleId == schId && x.BranchId == brId)
                .ToListAsync();
        }
        public async Task<TimeTableVM> GetTimeTable(int ScheduleId)
        {
            if (ScheduleId <= 0)
            {
                return new TimeTableVM();
            }

            var schedule =  await _context.VSchedules.AsNoTracking().SingleOrDefaultAsync(x => x.Id == ScheduleId);
            if (schedule == null)
            {
                return new TimeTableVM();
            }

            var branchClassIds = await _context.ScheduleBranchClass.AsNoTracking()
                .Where(x => x.ScheduleId == schedule.Id)
                .Select(x => x.BranchClassId)
                .ToListAsync();

            var branchClasses = await _context.VBranchClasses.AsNoTracking()
                .Where(x => branchClassIds.Contains((int)x.Id))
                .ToListAsync();

            var branchIds = branchClasses
                .Select(x => x.BranchId)
                .Distinct()
                .ToList();

            var branches = await _context.vbranches.AsNoTracking()
                .Where(x => branchIds.Contains(x.Id))
                .OrderBy(x => x.Id)
                .ToListAsync();

            foreach (var branch in branches)
            {
                branch.LstBranchClass = branchClasses
                    .Where(x => x.BranchId == branch.Id)
                    .ToList();
            }

            return new TimeTableVM
            {
                Schedule = schedule,
                LstBranch = branches
            };
        }

        public async Task<List<VScheduleBranchClasses>> GetBranchClassBySchedule(int scheduleId)
        {
            return await _context.VScheduleBranchClasses
                .AsNoTracking()
                .Where(x => x.ScheduleId == scheduleId)
                .ToListAsync();
        }
        public async Task<Tuple<string, string>> GetSPBranchClassSubject(string scheduleId)
        {
            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            try
            {

              
                var rawData = _context.V_SP_PreSchedSubject
             .AsNoTracking()
             .Where(x => x.ScheduleId == int.Parse(scheduleId))
            .Select(p => new
            {
                p.BranchClassId,
                p.ClassName,
                p.SubjectName,
                p.ClassSubjectId
            })
            .ToList();

                // Step 2: Get all unique SubjectNames for the column headers
                var subjectNames = rawData.Select(x => x.SubjectName).Distinct().ToList();

                // Step 3: Group data by ClassName and build the pivoted structure
                var pivotedData = rawData
            .GroupBy(x => new { x.ClassName, x.BranchClassId }) // Grouping by both ClassName and BranchClassId
            .Select(g => {
                // Create an anonymous object with ClassName, BranchClassId, and dynamically created Subject columns
                var pivot = new ExpandoObject() as IDictionary<string, Object>;
                pivot["ClassName"] = g.Key.ClassName;
                pivot["Id"] = g.Key.BranchClassId;

                // Add subject columns dynamically
                foreach (var subject in subjectNames)
                {
                    var classSubject = g.FirstOrDefault(x => x.SubjectName == subject);
                    pivot[subject] = classSubject?.ClassSubjectId; // Add ClassSubjectId or null if not available
                }

                return pivot;
            })
            .ToList();
                dt = ConvertToDataTable(pivotedData);

                //var connstring = _context.Database.GetDbConnection().ConnectionString;
                //using (MySqlConnection conn = new MySqlConnection(_connectionString))
                //{
                //    MySqlCommand sqlComm = new MySqlCommand("SP_PreSchedSubject", conn);
                //    sqlComm.Parameters.AddWithValue("@ScheduleId", scheduleId);
                //    sqlComm.CommandType = CommandType.StoredProcedure;
                //    MySqlDataAdapter da = new MySqlDataAdapter();
                //    da.SelectCommand = sqlComm;
                //    da.Fill(dt);
                //}

                //using (MySqlConnection conn = new MySqlConnection(_connectionString))
                //{
                //    MySqlCommand sqlComm = new MySqlCommand("SP_PostSchedSubject", conn);
                //    sqlComm.Parameters.AddWithValue("@ScheduleId", scheduleId);
                //    sqlComm.CommandType = CommandType.StoredProcedure;
                //    MySqlDataAdapter da = new MySqlDataAdapter();
                //    da.SelectCommand = sqlComm;
                //    da.Fill(dt2);
                //}
            }
            catch (Exception ex)
            {
                // Handle exception
            }
            return new Tuple<string, string>(DataTableToJSON(dt), DataTableToJSON(dt2));
        }
        public async Task<List<VBranchClasses>> GetBranchClassSubjectTopicBySchedule(string scheduleId, string branchId)
        {
            if (string.IsNullOrEmpty(scheduleId) || string.IsNullOrEmpty(branchId))
            {
                return new List<VBranchClasses>();
            }

            int brId = int.Parse(branchId);
            int schId = int.Parse(scheduleId);

            var lstbrcls = await _context.VBranchClasses
                .AsNoTracking()
                .Where(x => _context.VScheduleBranchClasses
                    .AsNoTracking()
                    .Where(y => y.ScheduleId == schId && y.BranchId == brId)
                    .Select(y => y.BranchClassId)
                    .Distinct()
                    .Contains((int)x.Id))
                .ToListAsync();

            foreach (var item in lstbrcls)
            {
                var lstclasssubj = _context.VClassSubjects
                    .AsNoTracking()
                    .Where(x => _context.VScheduleBranchClassSubjects
                        .AsNoTracking()
                        .Where(y => y.ScheduleId == schId && y.BranchId == brId && y.BranchClassId == item.Id)
                        .Select(y => y.ClassSubjectId)
                        .Distinct()
                        .Contains((int)x.Id))
                    .ToList();

                foreach (var subject in lstclasssubj)
                {
                    var lessons = _context.VLessons
                        .AsNoTracking()
                        .Where(x => x.ClassSubjectId == subject.Id)
                        .ToList();

                    foreach (var lesson in lessons)
                    {
                        lesson.IsSelected = true;
                    }

                    subject.LstLesson = lessons;
                }
                item.LstClassSubject = lstclasssubj;
            }

            return lstbrcls;
        }
        public async Task<TimeTableSubjectVM> GetSubjectsByBranchClassId(string branchClassId, string scheduleId)
        {
            var model = new TimeTableSubjectVM();
            if (string.IsNullOrEmpty(branchClassId) || string.IsNullOrEmpty(scheduleId))
            {
                return model;
            }

            int brclsId = int.Parse(branchClassId);
            int schId = int.Parse(scheduleId);

            var objCls = await _context.BranchClass
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == brclsId);

            if (objCls == null)
            {
                return model;
            }

            var lstsub = await _context.VClassSubjects
                .AsNoTracking()
                .Where(x => x.ClassId == objCls.ClassId)
                .ToListAsync();

            if (!lstsub.Any())
            {
                return model;
            }

            var lstdate = await GetStringDateList(schId);
            foreach (var sub in lstsub)
            {
                sub.ScheduleId = schId;
                sub.BranchClassId = brclsId;
                sub.ExamDateSheet = new List<SelectListItem>(lstdate);

                var objExam = await _context.Examination
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.ScheduleId == sub.ScheduleId && x.BranchClassId == sub.BranchClassId && x.ClassSubjectId == sub.Id && x.IsActive == ((int)ActiveState.Active).ToString());

                if (objExam != null && objExam.ExamDate != null)
                {
                    sub.StrExamDate = ((DateTime)objExam.ExamDate).ToString("dd-MM-yyyy");
                    var divselectedItem = sub.ExamDateSheet.Find(p => p.Value == sub.StrExamDate);
                    if (divselectedItem != null)
                    {
                        divselectedItem.Selected = true;
                    }
                }
            }
            model.LstClassSubject = lstsub;
            return model;
        }
        public async Task<List<VExaminations>> GetExaminationByBranchClassId(string branchClassId, string scheduleId)
        {
            if (string.IsNullOrEmpty(branchClassId) || string.IsNullOrEmpty(scheduleId))
            {
                return new List<VExaminations>();
            }

            int brclsId = int.Parse(branchClassId);
            int schId = int.Parse(scheduleId);

            return await _context.VExaminations
                .AsNoTracking()
                .Where(x => x.ScheduleId == schId && x.BranchClassId == brclsId)
                .ToListAsync();
        }
        public async Task<List<VExamTimeTable>> GetExamTimeTableByScheduleAndClass(int ScheduleId, int BranchClassId, int SessionYearId)
        {
            var result = await _context.VExamTimeTable
                .AsNoTracking()
                .Where(x => x.ScheduleId == ScheduleId && x.BranchClassId == BranchClassId && x.SessionYearId == SessionYearId)
                .ToListAsync();

            foreach (var exam in result)
            {
                exam.LstExaminationLesson = _context.VExaminationLessons
                    .AsNoTracking()
                    .Where(x => x.ExamId == exam.Id)
                    .ToList();
            }

            return result;
        }
        public async Task<List<MbSyllabus>> GetExamLessonByScheduleAndClass(int ScheduleId, int BranchClassId, int SessionYearId)
        {
            var lstexam = await _context.Examination
                .AsNoTracking()
                .Where(x => x.ScheduleId == ScheduleId && x.BranchClassId == BranchClassId && x.SessionYearId == SessionYearId)
                .ToListAsync();

            var lstexamids = lstexam.Select(x => x.Id).ToList();

            var lstlesson = _context.VLessons
                .AsNoTracking()
                .Where(x => _context.ExaminationLesson
                    .AsNoTracking()
                    .Where(y => lstexamids.Contains(y.ExamId))
                    .Select(y => y.LessonId)
                    .Contains((int)x.Id))
                .ToList();

            return lstlesson
                .GroupBy(x => new { x.SubjectId, x.SubjectName })
                .Select(grp => new MbSyllabus
                {
                    Id = (int)grp.Key.SubjectId,
                    SubjectName = grp.Key.SubjectName,
                    LstLesson = grp.ToList()
                })
                .ToList();
        }
        public async Task<int> SaveScheduleTimeTable(TimeTableSubjectVM model, int UserId)
        {
            if (model == null)
            {
                return 0;
            }

            var lstExam = await _context.Examination
                .Where(m => m.ScheduleId == model.ScheduleId && m.BranchClassId == model.BranchClassId && m.IsActive == ((int)ActiveState.Active).ToString())
                .ToListAsync();

            foreach (var param in model.LstClassSubject)
            {
                if (lstExam.Any())
                {
                    foreach (var exam in lstExam)
                    {
                        if (param.Id == exam.ClassSubjectId && param.BranchClassId == exam.BranchClassId && param.ScheduleId == exam.ScheduleId)
                        {
                            exam.ExamDate = DateTime.Parse(param.StrExamDate);
                            exam.ExamTimeId = param.ExamTimeId;
                            exam.LastModifiedDate = DateTime.Now;
                            exam.LastModifiedByUserId = UserId;
                        }
                    }
                }
                else
                {
                    var objExam = new Examination
                    {
                        ScheduleId = (int)param.ScheduleId,
                        SessionYearId = model.SessionYearId,
                        BranchClassId = (int)param.BranchClassId,
                        ClassSubjectId = (int)param.Id,
                        ExamDate = !string.IsNullOrEmpty(param.StrExamDate) ? DateTime.Parse(param.StrExamDate) : (DateTime?)null,
                        ExamTimeId = param.ExamTimeId,
                        CreatedDate = DateTime.Now,
                        CreatedByUserId = UserId,
                        IsActive = ((int)ActiveState.Active).ToString()
                    };
                    _context.Examination.Add(objExam);
                }
            }

            return await _context.SaveChangesAsync();
        }
        public async Task<int> SaveQuestionEditor(string model, int UserId)
        {
            if (string.IsNullOrEmpty(model))
            {
                return 0;
            }

            try
            {
                var obj = new Question
                {
                    Description = WebUtility.HtmlEncode(model)
                };
                _context.Question.Add(obj);
                return await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public async Task<string> GetEditorContent(int Id)
        {
            var obj = await _context.Question.AsNoTracking().SingleOrDefaultAsync(x => x.Id == Id);
            return obj != null ? WebUtility.HtmlDecode(obj.Description) : string.Empty;
        }
        public async Task<TimeTableVM> ViewTimeTable(int ScheduleId)
        {
            if (ScheduleId <= 0)
            {
                return null;
            }

            var objSched = await _context.VSchedules.AsNoTracking().SingleOrDefaultAsync(x => x.Id == ScheduleId);
            if (objSched == null)
            {
                return null;
            }

            var lstexam = await _context.VExaminations.AsNoTracking().Where(x => x.ScheduleId == objSched.Id).ToListAsync();
            if (!lstexam.Any())
            {
                return null;
            }

            var lstbrids = lstexam.Where(x => x.BranchId != null).Select(x => x.BranchId).Distinct().ToList();
            var lst = await _context.vbranches.AsNoTracking().Where(x => lstbrids.Contains(x.Id)).OrderBy(x => x.Id).ToListAsync();
            foreach (var branch in lst)
            {
                branch.LstBranchClass = await _context.VBranchClasses.AsNoTracking().Where(x => x.BranchId == branch.Id).ToListAsync();
            }

            return new TimeTableVM { Schedule = objSched, LstBranch = lst };
        }
        public async Task<TimeTableVM> GetExamResultTable(int ScheduleId)
        {
            if (ScheduleId <= 0)
            {
                return null;
            }

            var objSched = await _context.VSchedules.AsNoTracking().SingleOrDefaultAsync(x => x.Id == ScheduleId);
            if (objSched == null)
            {
                return null;
            }

            var lstexam = await _context.VExaminations.AsNoTracking().Where(x => x.ScheduleId == objSched.Id).ToListAsync();
            if (!lstexam.Any())
            {
                return null;
            }

            var lstbrids = lstexam.Where(x => x.BranchId != null).Select(x => x.BranchId).Distinct().ToList();
            var lst = await _context.vbranches.AsNoTracking().Where(x => lstbrids.Contains(x.Id)).OrderBy(x => x.Id).ToListAsync();
            foreach (var branch in lst)
            {
                branch.LstBranchClass = await _context.VBranchClasses.AsNoTracking().Where(x => x.BranchId == branch.Id).ToListAsync();
            }

            return new TimeTableVM { Schedule = objSched, LstBranch = lst };
        }
        public async Task<string> GetAutoScheduleClass(string branchId)
        {
            if (string.IsNullOrEmpty(branchId) || !int.TryParse(branchId, out int brid))
            {
                return string.Empty;
            }

            var lstbrcls = await _context.VBranchClasses.AsNoTracking().Where(x => x.BranchId == brid).ToListAsync();
            return JsonConvert.SerializeObject(lstbrcls);
        }
        public async Task<Tuple<string, string>> GetScheduleClassAndSectionByBranch(string branchId, string scheduleId)
        {
            if (string.IsNullOrEmpty(branchId) || !int.TryParse(branchId, out int brid))
            {
                return new Tuple<string, string>(string.Empty, string.Empty);
            }

            var lstbrcls = await _context.VBranchClasses.AsNoTracking().Where(x => x.BranchId == brid && x.StudentCount > 0).ToListAsync();
            var lsttea = await _context.VTeachers.AsNoTracking().Where(x => x.BranchId == brid).ToListAsync();

            if (!string.IsNullOrEmpty(scheduleId) && int.TryParse(scheduleId, out int schId) && schId > 0)
            {
                var lstschbrcls = await _context.ScheduleBranchClass.AsNoTracking().Where(x => x.ScheduleId == schId).ToListAsync();
                foreach (var item in lstbrcls)
                {
                    item.IsSelected = lstschbrcls.Any(x => x.BranchClassId == item.Id);
                }

                var lstschtea = await _context.ScheduleTeacher.AsNoTracking().Where(x => x.ScheduleId == schId).ToListAsync();
                foreach (var item in lsttea)
                {
                    item.IsSelected = lstschtea.Any(x => x.TeacherId == item.Id);
                }
            }

            return new Tuple<string, string>(JsonConvert.SerializeObject(lstbrcls), JsonConvert.SerializeObject(lsttea));
        }
        public async Task<string> GetSPScheduleSection(int scheduleId)
        {
            DataTable dt = new DataTable();
            try
            {
                //using (MySqlConnection conn = new MySqlConnection(_connectionString))
                //{
                //    MySqlCommand sqlComm = new MySqlCommand("SP_ScheduleStudent", conn);
                //    sqlComm.Parameters.AddWithValue("@ScheduleId", scheduleId);
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
            return DataTableToJSON(dt);
        }
        public async Task<List<ScheduleTitle>> GetSchedulesByPersonnelId(int PersonnelId, string PersonnelType, int SessionYearId, int BranchId)
        {
            if (PersonnelId <= 0)
            {
                return new List<ScheduleTitle>();
            }

            List<VSchedules> lstSchedule = new List<VSchedules>();

            if (PersonnelType == ((int)UserType.Teacher).ToString())
            {
                var lstsecids = await _context.ScheduleTeacherExamHall.AsNoTracking()
                    .Where(x => x.TeacherId == PersonnelId && x.SessionYearId == SessionYearId)
                    .Select(x => x.ScheduleId)
                    .ToListAsync();
                lstSchedule = await _context.VSchedules.AsNoTracking()
                    .Where(x => lstsecids.Contains((int)x.Id))
                    .OrderByDescending(x => x.Id)
                    .ToListAsync();
            }
            else if (PersonnelType == ((int)UserType.Student).ToString())
            {
                var objStudent = await _context.VStudents.AsNoTracking().SingleOrDefaultAsync(x => x.Id == PersonnelId);
                if (objStudent != null)
                {
                    var lstsecids = await _context.ScheduleBranchClass.AsNoTracking()
                        .Where(x => x.BranchClassId == objStudent.BranchClassId && x.SessionYearId == SessionYearId)
                        .Select(x => x.ScheduleId)
                        .ToListAsync();
                    lstSchedule = await _context.VSchedules.AsNoTracking()
                        .Where(x => lstsecids.Contains((int)x.Id))
                        .OrderByDescending(x => x.Id)
                        .ToListAsync();
                }
            }
            else
            {
                lstSchedule = BranchId > 0
                    ? await _context.VSchedules.AsNoTracking()
                        .Where(x => x.SessionYearId == SessionYearId && x.BranchId == BranchId)
                        .OrderByDescending(x => x.Id)
                        .ToListAsync()
                    : await _context.VSchedules.AsNoTracking()
                        .Where(x => x.SessionYearId == SessionYearId)
                        .OrderByDescending(x => x.Id)
                        .ToListAsync();
            }

            return lstSchedule.Select(c => new ScheduleTitle
            {
                Id = (int)c.Id,
                Title = c.Title + " " + c.ExamTypeName,
                ResultPercent = (decimal)c.ResultPercent,
                StrStartDate = c.StrStartDate,
                StrEndDate = c.StrEndDate,
                StatusId = (int)c.StatusId
            }).ToList();
        }
        public async Task<AutoScheduleVM> GetAutoScheduleVM(int Id)
        {
            var result = new AutoScheduleVM
            {
                Schedule = await _context.VSchedules.AsNoTracking().SingleOrDefaultAsync(x => x.Id == Id),
                LstBranch = await _context.vbranches.AsNoTracking()
                    .Where(x => _context.ScheduleBranchClass
                        .Where(bc => bc.ScheduleId == Id)
                        .Select(bc => bc.BranchId)
                        .Contains((int)x.Id))
                    .ToListAsync()
            };
            return result;
        }
        public async Task<ScheduleStudentSectionVM> GetScheduleStudentSection(int Id)
        {
            var result = new ScheduleStudentSectionVM
            {
                ScheduleId = Id,
                LstBranch = await _context.vbranches.AsNoTracking()
                    .Where(x => _context.ScheduleBranchClass
                        .Where(bc => bc.ScheduleId == Id)
                        .Select(bc => bc.BranchId)
                        .Contains((int)x.Id))
                    .ToListAsync()
            };
            return result;
        }
        public async Task<ScheduleTeacherSectionVM> GetScheduleTeacherSection(int Id)
        {
            var result = new ScheduleTeacherSectionVM
            {
                ScheduleId = Id,
                LstBranch = await _context.vbranches.AsNoTracking()
                    .Where(x => _context.ScheduleBranchClass
                        .Where(bc => bc.ScheduleId == Id)
                        .Select(bc => bc.BranchId)
                        .Contains((int)x.Id))
                    .ToListAsync()
            };
            return result;
        }
        public async Task<MBScheduleDetail> GetMBScheduleById(int ScheduleId)
        {
            var objSchedule = await _context.VSchedules.AsNoTracking().SingleOrDefaultAsync(x => x.Id == ScheduleId);
            if (objSchedule == null)
            {
                return null;
            }

            return new MBScheduleDetail
            {
                Id = (int)objSchedule.Id,
                Title = objSchedule.Title + " " + objSchedule.ExamTypeName,
                ResultPercent = (decimal)objSchedule.ResultPercent,
                StrStartDate = objSchedule.StrStartDate,
                StrEndDate = objSchedule.StrEndDate,
                Status = objSchedule.StatusName,
                ExamHallCount = objSchedule.ExamHallCount,
                ClassCount = objSchedule.ClassCount,
                StudentCount = objSchedule.StudentCount,
            };
        }
        public async Task<List<MbBranchClass>> GetBranchClassesByScheduleId(int ScheduleId, int PersonnelId, string PersonnelType, int SessionYearId)
        {
            if (PersonnelId <= 0 || SessionYearId <= 0)
            {
                return new List<MbBranchClass>();
            }

            var lstschedbrclsids = await _context.ScheduleBranchClass.AsNoTracking()
                .Where(x => x.ScheduleId == ScheduleId)
                .Select(x => x.BranchClassId)
                .ToListAsync();

            List<int> clsids = new List<int>();

            if (PersonnelType == ((int)UserType.Teacher).ToString())
            {
                clsids.AddRange(await _context.BranchClassSectionTeacher.AsNoTracking()
                    .Where(x => lstschedbrclsids.Contains(x.BranchClassId) && x.TeacherId == PersonnelId && x.SessionYearId == SessionYearId && x.IsActive == ((int)ActiveState.Active).ToString())
                    .Select(x => x.BranchClassId)
                    .ToListAsync());

                clsids.AddRange(await _context.BranchClassSectionSubjectTeacher.AsNoTracking()
                    .Where(x => lstschedbrclsids.Contains(x.BranchClassId) && x.TeacherId == PersonnelId && x.SessionYearId == SessionYearId && x.IsActive == ((int)ActiveState.Active).ToString())
                    .Select(x => x.BranchClassId)
                    .ToListAsync());
            }
            else
            {
                clsids.AddRange(await _context.BranchClassSectionTeacher.AsNoTracking()
                    .Where(x => lstschedbrclsids.Contains(x.BranchClassId) && x.SessionYearId == SessionYearId && x.IsActive == ((int)ActiveState.Active).ToString())
                    .Select(x => x.BranchClassId)
                    .ToListAsync());
            }

            var lstbranchclass = await _context.BranchClass.AsNoTracking()
                .Where(x => clsids.Contains((int)x.Id))
                .ToListAsync();

            var lstclass = await _context.Class.AsNoTracking()
                .Where(x => lstbranchclass.Select(bc => bc.ClassId).Contains((int)x.Id))
                .ToDictionaryAsync(x => x.Id, x => x.Name);

            return lstbranchclass
                .Select(c => new MbBranchClass
                {
                    Id = (int)c.Id,
                    ClassName = lstclass[c.ClassId]
                })
                .ToList();
        }
        public async Task<List<MbSubject>> GetSubjectByScheduleAndClass(int ScheduleId, int BranchClassId, int SessionYearId)
        {
            var lstsubjectids = await _context.StudentResult.AsNoTracking()
                .Where(x => x.ScheduleId == ScheduleId && x.BranchClassId == BranchClassId && x.SessionYearId == SessionYearId && x.IsActive == ((int)ActiveState.Active).ToString())
                .Select(x => x.SubjectId)
                .Distinct()
                .ToListAsync();

            return await _context.Subject.AsNoTracking()
                .Where(x => lstsubjectids.Contains(x.Id) && x.IsActive == ((int)ActiveState.Active).ToString())
                .GroupBy(x => new { x.Id, x.ShortName })
                .Select(gcs => new MbSubject
                {
                    Id = gcs.Key.Id,
                    SubjectName = gcs.Key.ShortName
                })
                .ToListAsync();
        }
        public async Task<List<MbStudentMark>> GetStudentsByScheduleAndClassAndSubject(int ScheduleId, int BranchClassId, int SessionYearId, int SubjectId)
        {
            return await _context.VStudentMarkResults.AsNoTracking()
                .Where(x => x.ScheduleId == ScheduleId && x.BranchClassId == BranchClassId && x.SessionYearId == SessionYearId && x.SubjectId == SubjectId)
                .Select(c => new MbStudentMark
                {
                    Id = (int)c.StudentId,
                    StudentName = c.FullName,
                    MaxMarks = c.MaxMarks,
                    Marks = c.Marks,
                    GradeColor = c.GradeColor
                })
                .ToListAsync();
        }
        public async Task<List<SelectListItem>> GetStringDateList(int scheduleId)
        {
            var objSched = await _context.Schedule.AsNoTracking().SingleOrDefaultAsync(x => x.Id == scheduleId && x.IsActive == ((int)ActiveState.Active).ToString());
            if (objSched == null)
            {
                return new List<SelectListItem>();
            }

            var dates = Enumerable.Range(0, (objSched.EndDate - objSched.StartDate).Value.Days + 1)
                .Select(offset => objSched.StartDate.Value.AddDays(offset).ToString("dd/MM/yyyy"))
                .ToList();

            return dates.Select(x => new SelectListItem { Text = x, Value = x }).ToList();
        }
        public async Task<List<SelectListItem>> GetBranches()
        {
            return await _context.Branch.AsNoTracking()
                .Where(x => x.IsActive == ((int)ActiveState.Active).ToString())
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                .ToListAsync();
        }
        public async Task<List<SelectListItem>> GetExamTypes()
        {
            return await _context.ExamType.AsNoTracking()
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
