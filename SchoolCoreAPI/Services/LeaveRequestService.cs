using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using System.Data;
using System.Dynamic;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly APIContext _context;
        private readonly INotificationService _NotificationService;
        private readonly string _connectionString;

        public LeaveRequestService(APIContext context, INotificationService NotificationService, IConfiguration config)
        {
            _context = context;
            _NotificationService = NotificationService;
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public async Task<List<VLeaveRequest>>GetAllLeaveRequest(int userId)
        {
            return await _context.VLeaveRequest.AsNoTracking()
                .Where(x => x.CreatedByUserId == userId)
                .OrderByDescending(x => x.Id)
                .ToListAsync();
        }

        public async Task<List<VLeaveRequest>> GetLeaveRequestByPersonnelId(int personnelId, string personnelType)
        {
            return await _context.VLeaveRequest.AsNoTracking()
                .Where(x => x.PersonnelId == personnelId && personnelType.Contains(x.PersonnelType))
                .OrderByDescending(x => x.Id)
                .ToListAsync();
        }

        public async Task<VLeaveRequest> ViewLeaveRequest(int Id)
        {
            var objleavereq = await _context.VLeaveRequest.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
            if (objleavereq != null)
            {
                objleavereq.RequestId = Id;
            }
            return objleavereq;
        }

        public async Task<LeaveRequest> GetLeaveRequest(int Id, int PersonId, string PersonType, int BranchId, int SessionYearId)
        {
            if (Id > 0)
            {
                var cat = await _context.LeaveRequest.AsNoTracking().SingleOrDefaultAsync(x => x.Id == Id && x.IsActive == ((int)ActiveState.Active).ToString());
                if (cat != null)
                {
                    return new LeaveRequest
                    {
                        Id = cat.Id,
                        FromDate = cat.FromDate,
                        ToDate = cat.ToDate,
                        StatusId = cat.StatusId,
                        Reason = cat.Reason,
                        SessionYearId = cat.SessionYearId,
                        BranchId = cat.BranchId,
                        PersonnelType = cat.PersonnelType,
                        PersonnelId = cat.PersonnelId,
                        Comment = cat.Comment,
                        LeaveRequestType = cat.LeaveRequestType
                    };
                }
            }
            return new LeaveRequest
            {
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                PersonnelType = PersonType,
                PersonnelId = PersonId,
                BranchId = BranchId,
                SessionYearId = SessionYearId,
                LeaveRequestType = ((int)LeaveRequestType.Casual).ToString()
            };
        }

        public async Task<ServiceResult> SaveLeaveRequest(LeaveRequest model, int UserId)
        {
            if (model.Id != 0) return null;

            var cat = new LeaveRequest
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                StatusId = ((int)LeaveRequestStatus.UnderProcess).ToString(),
                SessionYearId = model.SessionYearId,
                BranchId = model.BranchId,
                LeaveRequestType = model.LeaveRequestType,
                PersonnelType = model.PersonnelType,
                PersonnelId = model.PersonnelId,
                Reason = model.Reason,
                IsActive = ((int)ActiveState.Active).ToString(),
                CreatedDate = DateTime.Now,
                CreatedByUserId = UserId
            };

            _context.LeaveRequest.Add(cat);
            _context.Entry(cat).State = EntityState.Added;
            if (_context.SaveChanges() == 0) return null;

            foreach (var item in model.LstLeaveRequestDate)
            {
                var reqdate = new LeaveRequestDate
                {
                    LeaveRequestId = cat.Id,
                    LeaveDate = item.LeaveDate,
                    LeaveType = item.LeaveType,
                    IsActive = ((int)ActiveState.Active).ToString(),
                    CreatedDate = DateTime.Now,
                    CreatedByUserId = UserId
                };
                _context.LeaveRequestDate.Add(reqdate);
                _context.Entry(reqdate).State = EntityState.Added;
            }

            if (_context.SaveChanges() == 0) return null;

            var objNotify = await SetNotification(cat, UserId);
            if (await _NotificationService.SendNotification(objNotify, UserId) == null) return null;

            return new ServiceResult
            {
                StatusId = (int)ServiceResultStatus.Added,
                RecordId = cat.Id
            };
        }

        public async Task<Notification> SetNotification(LeaveRequest model, int UserId)
        {
            var objNotification = new Notification
            {
                BranchId = model.BranchId,
                SessionYearId = model.SessionYearId,
                RecordId = model.Id,
                ReadStatusId = ((int)ReadStatus.UnRead).ToString(),
                NotificationType = ((int)NotificationType.LeaveRequest).ToString(),
                Message = "Request For Leave",
                Description = "Request For Leave",
                FromUserType = model.PersonnelType,
                FromUserId = UserId,
                FromPersonnelId = model.PersonnelId
            };

            if (model.PersonnelType == ((int)UserType.Admin).ToString())
            {
                var objsuperadmin = await _context.Personnel.AsNoTracking().SingleOrDefaultAsync(x => x.PersonnelType == ((int)UserType.SuperAdmin).ToString() && x.IsActive == ((int)ActiveState.Active).ToString());
                if (objsuperadmin != null)
                {
                    objNotification.ToPersonnelId = objsuperadmin.Id;
                    objNotification.ToUserType = ((int)UserType.SuperAdmin).ToString();
                    objNotification.IsActionTaken = ((int)ActiveState.Active).ToString();
                }
            }
            else if (model.PersonnelType == ((int)UserType.Teacher).ToString())
            {
                var objAdmin = await _context.vbranches.AsNoTracking().SingleOrDefaultAsync(x => x.Id == model.BranchId);
                if (objAdmin != null)
                {
                    objNotification.ToPersonnelId = objAdmin.AdminId > 0 ? (int)objAdmin.AdminId : 0;
                    objNotification.ToUserType = ((int)UserType.Admin).ToString();
                    objNotification.IsActionTaken = ((int)ActiveState.Active).ToString();
                }
            }
            else
            {
                var objStudent = await _context.VStudents.AsNoTracking().SingleOrDefaultAsync(x => x.Id == model.PersonnelId 
                && x.SessionYearId == model.SessionYearId);
                if (objStudent != null)
                {
                    var objbranchclassteacher = await _context.BranchClassSectionTeacher.AsNoTracking().SingleOrDefaultAsync(x => x.BranchId == objStudent.BranchClassId && x.SectionId == objStudent.SectionId);
                    objNotification.ToPersonnelId = objbranchclassteacher != null && objbranchclassteacher.TeacherId > 0 ? (int)objbranchclassteacher.TeacherId : 0;
                    objNotification.ToUserType = ((int)UserType.Teacher).ToString();
                    objNotification.IsActionTaken = ((int)ActiveState.InActive).ToString();
                }
            }
            if (model.PersonnelType == ((int)UserType.Admin).ToString())
            {
                var objUser = await _context.VUserInfo.AsNoTracking().SingleOrDefaultAsync(x => x.PersonId == objNotification.ToPersonnelId
             && x.UserType == objNotification.ToUserType);
                if (objUser != null)
                {
                    objNotification.ToUserId = objUser.Id;
                }
            }
            else
            {
                var objUser = await _context.VUserInfo.AsNoTracking().SingleOrDefaultAsync(x => x.PersonId == objNotification.ToPersonnelId
            && x.SessionYearId == objNotification.SessionYearId && x.UserType == objNotification.ToUserType);
                if (objUser != null)
                {
                    objNotification.ToUserId = objUser.Id;
                }
            }
                

            return objNotification;
        }

        public async Task<ServiceResult> UpdateLeaveRequest(CastLeaveRequest model, int UserId)
        {
            if (model.RequestId <= 0) return null;
            try
            {
                var request = await _context.LeaveRequest.SingleOrDefaultAsync(x => x.Id == model.RequestId);
                if (request == null) return null;

                request.StatusId = model.StatusId;
                request.Comment = model.Comment;
                request.LastModifiedByUserId = UserId;
                request.LastModifiedDate = DateTime.Now;
                _context.Entry(request).State = EntityState.Modified;

                if (_context.SaveChanges() == 0) return null;

                if (await UpdateNotification((int)model.NotificationId, UserId) <= 0) return null;

                var objNotify = await SetUpdateNotification(request, UserId);
                if (_NotificationService.SendDBNotification(new List<Notification> { objNotify }, (int)NotificationType.LeaveRequest, UserId) == null) return null;

                return new ServiceResult
                {
                    StatusId = (int)ServiceResultStatus.Updated,
                    RecordId = request.Id
                };
            }
            catch (Exception ex)
            {
                
            }
            return null;
            
        }

        public async Task<Notification> SetUpdateNotification(LeaveRequest model, int UserId)
        {
            var objUser = await _context.VUserInfo.AsNoTracking().SingleOrDefaultAsync(x => x.Id == UserId);

            return new Notification
            {
                BranchId = model.BranchId,
                SessionYearId = model.SessionYearId,
                RecordId = model.Id,
                ReadStatusId = ((int)ReadStatus.UnRead).ToString(),
                IsActionTaken = ((int)ActiveState.InActive).ToString(),
                NotificationType = ((int)NotificationType.LeaveRequest).ToString(),
                Message = model.StatusId == ((int)LeaveRequestStatus.Approved).ToString() ? "Your Request Has Been Approved" : "Your Request Has Been Rejected",
                Description = "Request For Leave",
                FromUserType = objUser.UserType,
                FromUserId = UserId,
                FromPersonnelId = objUser.PersonId,
                ToPersonnelId = model.PersonnelId,
                ToUserType = model.PersonnelType,
                ToUserId = (int)model.CreatedByUserId
            };
        }

        public async Task<int> UpdateNotification(int Id, int UserId)
        {
            var objNotification = await _context.Notification.SingleOrDefaultAsync(x => x.Id == Id && x.IsActive == ((int)ActiveState.Active).ToString());
            if (objNotification == null) return 0;

            objNotification.IsActionTaken = ((int)ActiveState.InActive).ToString();
            objNotification.LastModifiedByUserId = UserId;
            objNotification.LastModifiedDate = DateTime.Now;
            _context.Entry(objNotification).State = EntityState.Modified;

            return _context.SaveChanges();
        }

        public async Task<ServiceResult> DeleteLeaveRequest(int Id, int UserId)
        {
            var cat = await _context.LeaveRequest.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
            if (cat == null) return null;

            cat.IsActive = ((int)ActiveState.InActive).ToString();
            cat.LastModifiedByUserId = UserId;
            cat.LastModifiedDate = DateTime.Now;
            _context.Entry(cat).State = EntityState.Modified;

            if (await _context.SaveChangesAsync() == 0) return null;

            return new ServiceResult
            {
                StatusId = (int)ServiceResultStatus.Deleted,
                RecordId = cat.Id
            };
        }

        public async Task<LeaveReport> GetLeaveReport()
        {
            var lst = await _context.SessionYear.AsNoTracking().Where(x => x.IsActive == ((int)ActiveState.Active).ToString()).ToListAsync();
            var lstSelectItems = lst.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();

            var defaultSession = await _context.SessionYear.AsNoTracking().SingleOrDefaultAsync(x => x.IsActive == ((int)ActiveState.Active).ToString() && x.IsDefault == true);

            var report = new LeaveReport
            {
                SessionYearList = lstSelectItems,
                SessionYearId = defaultSession?.Id ?? 0
            };

            var divselectedItem = report.SessionYearList.Find(p => p.Value == report.SessionYearId.ToString());
            if (divselectedItem != null)
            {
                divselectedItem.Selected = true;
            }

            return report;
        }

        public async Task<SpLeaveReportResult> GetSPLeaveReport(int PersonId)
        {
            var result = new SpLeaveReportResult();
            var dt = new DataTable();

            try
            {
                var rawData = _context.V_SP_LeaveSummary
    .AsNoTracking()
    .Where(x => x.PersonId == PersonId)
    .Select(p => new
    {
        p.CreatedByUserId,
        p.SessionMonthYear,
        p.Allocated,
        p.TotalDays,
        p.CL,
        p.LWP,
        p.RemainCL
    })
    .ToList();

                // Extract distinct SessionMonthYear values where Allocated is greater than 0
                var monthNames = rawData
                    .Where(x => x.Allocated > 0) // Only include months with a non-zero allocation
                    .Select(x => x.SessionMonthYear)
                    .Distinct()
                    .ToList();

                // Group the data by CreatedByUserId and pivot the rows for each month
                var pivotedData = new List<ExpandoObject>();

                // List of leave types
                var leaveTypes = new[] { "TotalDays", "CL", "LWP", "RemainCL" };

                foreach (var leaveType in leaveTypes)
                {
                    var pivot = new ExpandoObject() as IDictionary<string, object>;
                    pivot["Title"] = leaveType;  // Add the leave type to the Title column

                    // For each month, get the value for the current leave type
                    foreach (var monname in monthNames)
                    {
                        var monthData = rawData.FirstOrDefault(x => x.SessionMonthYear == monname);

                        // Set the value for the leave type (e.g., TotalDays, CL, etc.)
                        if (monthData != null)
                        {
                            if (leaveType == "TotalDays" && monthData.TotalDays == 0)
                            {
                                pivot[$"{monname}({monthData.Allocated})"] = "";  // Set to empty string if TotalDays is 0
                            }
                            else
                            {
                                pivot[$"{monname}({monthData.Allocated})"] = leaveType switch
                                {
                                    "TotalDays" => monthData.TotalDays,
                                    "CL" => monthData.CL,
                                    "LWP" => monthData.LWP,
                                    "RemainCL" => monthData.RemainCL,
                                    _ => 0
                                };
                            }
                        }
                        else
                        {
                            pivot[$"{monname}({monthData.Allocated})"] = 0;  // Set to 0 if no data for that month
                        }
                    }

                    pivotedData.Add((ExpandoObject)pivot);
                }

                // Now pivotedData contains rows like:
                // Title = TotalDays, with columns for each month (Nov-24(1.5), Dec-24(1.5), Jan-25(1.5))
                // Title = CL, with columns for each month (Nov-24(1.5), Dec-24(1.5), Jan-25(1.5))
                // Title = LWP, with columns for each month (Nov-24(1.5), Dec-24(1.5), Jan-25(1.5))
                // Title = RemainCL, with columns for each month (Nov-24(1.5), Dec-24(1.5), Jan-25(1.5))


                dt = ConvertToDataTable(pivotedData);
                //using (MySqlConnection conn = new MySqlConnection(_connectionString))
                //{
                //    MySqlCommand sqlComm = new MySqlCommand("SP_UserLeaves", conn);
                //    sqlComm.Parameters.AddWithValue("@sessionid", SessionYearId.ToString());
                //    sqlComm.Parameters.AddWithValue("@userid", UserId.ToString());
                //    sqlComm.CommandType = CommandType.StoredProcedure;
                //    MySqlDataAdapter da = new MySqlDataAdapter();
                //    da.SelectCommand = sqlComm;
                //    da.Fill(dt);
                //}
                result.Result = DataTableToJSON(dt);
            }
            catch (Exception)
            {
                // Handle exception
            }

            return result;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
