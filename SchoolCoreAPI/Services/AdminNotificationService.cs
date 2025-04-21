using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using System.Globalization;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class AdminNotificationService : IAdminNotificationService
    {
        private readonly APIContext _context;
        private readonly INotificationService _notificationService;
        private readonly HostEnvironmentService _hostingEnvironment;
        private readonly S3Service _s3Service;

        public AdminNotificationService(APIContext context, INotificationService notificationService, HostEnvironmentService hostingEnvironment, S3Service s3Service)
        {
            _context = context;
            _notificationService = notificationService;
            _hostingEnvironment = hostingEnvironment;
            _s3Service = s3Service;

        }

        public async Task<List<VAdminNotification>> GetAllAdminNotification(int userId, int sessionYearId)
        {
            return await _context.VAdminNotification
                .Where(x => x.CreatedByUserId == userId && x.SessionYearId == sessionYearId)
                .OrderByDescending(x => x.Id)
                .ToListAsync();
        }

        public async Task<VAdminNotification> ViewAdminNotification(int id)
        {
            return await _context.VAdminNotification.SingleOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Tuple<byte[], string>> GetFileBytes(int id)
        {
            var objAttach = await _context.VAdminNotification.SingleOrDefaultAsync(m => m.Id == id);
            if (objAttach != null && !string.IsNullOrEmpty(objAttach.FilePath))
            {
                string filePath = _hostingEnvironment.GetFullUrlFromDbPath(objAttach.FilePath);
                var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
                return new Tuple<byte[], string>(bytes, objAttach.FileName);
            }
            return null;
        }

        public async Task<AdminNotification> GetAdminNotification(int id, int branchId, int sessionYearId, int userId)
        {
            if (id > 0)
            {
                var cat = await _context.AdminNotification.SingleOrDefaultAsync(x => x.Id == id && x.IsActive == ((int)Shared.ActiveState.Active).ToString());
                if (cat != null)
                {
                    return new AdminNotification
                    {
                        Id = cat.Id,
                        SessionYearId = cat.SessionYearId,
                        UserTypeId = cat.UserTypeId,
                        TagTypeId = cat.TagTypeId,
                        IsTag = cat.IsTag,
                        BranchId = cat.BranchId,
                        FileName = cat.FileName,
                        FilePath = cat.FilePath,
                        Title = cat.Title,
                        Message = cat.Message,
                        StrCreatedDate = cat.CreatedDate.Value.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture)
                    };
                }
            }
            return new AdminNotification
            {
                UserTypeSheet = await GetUserTypes(),
                TagTypeSheet = await GetTagTypes(branchId, sessionYearId),
                LstCastStudPersonNotification = new List<CastStudPersonNotification>(),
                SessionYearId = sessionYearId,
                BranchId = branchId,
                FileType = "1",
                IsTag = "1",
                FilePath = Shared.GetNoImagePath()
            };
        }

        public async Task<ServiceResult> SaveAdminNotification(AdminNotification model, int userId)
        {
            if (model.Id > 0) return null;

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var cat = new AdminNotification
                    {
                        SessionYearId = model.SessionYearId,
                        BranchId = model.BranchId,
                        Title = model.Title,
                        FileType = model.FileType,
                        Message = model.Message,
                        UserTypeId = model.UserTypeId,
                        TagTypeId = model.TagTypeId,
                        IsTag = model.IsTag,
                        IsActive = ((int)ActiveState.Active).ToString(),
                        CreatedDate = DateTime.Now,
                        CreatedByUserId = userId
                    };

                    if (model.NotificationFile != null)
                    {
                        cat.FileName = model.NotificationFile.FileName;
                        // cat.FilePath = ProcessUploadFile((int)UploadType.Notification, model.NotificationFile, _hostingEnvironment.GetWebRootPath());
                        cat.FilePath = await _s3Service.UploadFileAsync((int)UploadType.Notification, model.NotificationFile);
                    }

                    _context.AdminNotification.Add(cat);
                    await _context.SaveChangesAsync();

                    var notifications = new List<Notification>();
                    if (cat.IsTag == "1")
                    {
                        notifications.AddRange(await HandleTagTypeNotifications(cat, model, userId));
                    }
                    else
                    {
                        notifications.AddRange(await HandleUserTypeNotifications(cat, model, userId));
                    }

                    if (notifications.Count > 0)
                    {
                        var notificationResult = await _notificationService.SendDBNotification(notifications, (int)NotificationType.AdminNotification, userId);
                        if (notificationResult != null)
                        {
                            await transaction.CommitAsync();
                            return new ServiceResult { StatusId = (int)ServiceResultStatus.Added, RecordId = cat.Id };
                        }
                    }
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                }
            }
            return null;
        }

        private async Task<List<Notification>> HandleTagTypeNotifications(AdminNotification cat, AdminNotification model, int userId)
        {
            var notifications = new List<Notification>();
            if (cat.TagTypeId == (int)TagType.FeeReminder)
            {
                var students = await _context.VStudents
                    .Where(x => x.BranchId == model.BranchId && x.SessionYearId == model.SessionYearId && x.DueAmount > 0)
                    .ToListAsync();

                foreach (var student in students)
                {
                    var feeStructure = await _context.FeeStructure.SingleOrDefaultAsync(x => x.Id == student.FeeStructureId);
                    if (feeStructure != null)
                    {
                        string message = $"Dear Parent, your ward {student.FullName} was due for the amount {student.DueAmount}. Last date for the payment is {feeStructure.DueDate.Date.ToShortDateString()}. Please pay on or before the due date";
                        notifications.Add(SetNotification(cat, userId, (int)student.Id, cat.Title, message, ((int)NotificationType.AdminNotification).ToString(), ((int)UserType.Student).ToString()));
                    }
                }
            }
            else if (cat.TagTypeId == (int)TagType.AbsentStudents)
            {
                DateTime filterDate = DateTime.Now;
                var students = await _context.VStudentAttendence
                    .Where(x => x.BranchId == model.BranchId && x.SessionYearId == model.SessionYearId && x.WorkingDate.Date == filterDate.Date && x.AttendenceId > 0 && x.IsPresent == "0")
                    .ToListAsync();

                foreach (var student in students)
                {
                    string message = $"Dear Parent, your ward {student.FullName} is absent today";
                    notifications.Add(SetNotification(cat, userId, student.StudentId, cat.Title, message, ((int)NotificationType.AdminNotification).ToString(), ((int)UserType.Student).ToString()));
                }
            }
            return notifications;
        }

        private async Task<List<Notification>> HandleUserTypeNotifications(AdminNotification cat, AdminNotification model, int userId)
        {
            var notifications = new List<Notification>();
            if (model.UserTypeId == (int)UserType.EveryOne)
            {
                var students = await _context.VStudentCurrentLocation
                    .Where(x => x.BranchId == model.BranchId && x.SessionYearId == model.SessionYearId)
                    .Select(x => x.StudentId)
                    .ToListAsync();

                foreach (var student in students)
                {
                    notifications.Add(SetNotification(cat, userId, student, cat.Title, cat.Message, ((int)NotificationType.AdminNotification).ToString(), ((int)UserType.Student).ToString()));
                }

                var personnel = await _context.VPersonnelCurrentLocation
                    .Where(x => x.BranchId == model.BranchId && x.SessionYearId == model.SessionYearId && x.PersonnelType != ((int)UserType.Admin).ToString())
                    .ToListAsync();

                foreach (var person in personnel)
                {
                    notifications.Add(SetNotification(cat, userId, person.PersonnelId, cat.Title, cat.Message, ((int)NotificationType.AdminNotification).ToString(), person.PersonnelType));
                }
            }
            else if (!string.IsNullOrEmpty(model.StrCastStudPerson))
            {
                var castStudPersons = JsonConvert.DeserializeObject<List<CastStudPersonNotification>>(model.StrCastStudPerson);
                var selectedPersons = castStudPersons.Where(x => x.IsSelected).ToList();

                foreach (var person in selectedPersons)
                {
                    notifications.Add(SetNotification(cat, userId, person.StudPersonId, cat.Title, cat.Message, ((int)NotificationType.AdminNotification).ToString(), person.PersonnelType.ToString()));
                }
            }
            return notifications;
        }

        public Notification SetNotification(AdminNotification model, int fromUserId, int toPersonId, string title, string message, string notifyType, string userType)
        {
            var fromUser = _context.VUserInfo.SingleOrDefault(x => x.Id == fromUserId);
            var toUser = _context.VUserInfo.SingleOrDefault(x => x.UserType == userType && x.PersonId == toPersonId);

            return new Notification
            {
                BranchId = model.BranchId,
                SessionYearId = model.SessionYearId,
                RecordId = model.Id,
                ReadStatusId = ((int)ReadStatus.UnRead).ToString(),
                IsActionTaken = ((int)ActiveState.InActive).ToString(),
                NotificationType = notifyType,
                Message = title,
                Description = message,
                FromUserType = fromUser.UserType,
                FromUserId = fromUserId,
                FromPersonnelId = fromUser.PersonId,
                ToPersonnelId = toPersonId,
                ToUserType = userType,
                ToUserId = toUser.Id
            };
        }

        public async Task<List<CastStudPersonNotification>> GetStudPersonNotification(int branchId, int sessionYearId, int personnelType)
        {
            if (personnelType == (int)UserType.EveryOne) return null;

            if (personnelType == (int)UserType.Student)
            {
                var students = await _context.VStudents.Where(x => x.BranchId == branchId && x.SessionYearId == sessionYearId).ToListAsync();
                return students.Select(c => new CastStudPersonNotification
                {
                    StudPersonId = (int)c.Id,
                    PersonnelType = personnelType,
                    FullName = c.FullName,
                    RollEmployeeNo = c.RollNo,
                    ClassName = $"{c.ClassName} {c.SectionName}",
                    SectionName = c.SectionName,
                    IsSelected = false
                }).ToList();
            }
            else
            {
                var personnel = await _context.VPersonnelCurrentLocation.Where(x => x.BranchId == branchId && x.PersonnelType == personnelType.ToString() && x.SessionYearId == sessionYearId).ToListAsync();
                return personnel.Select(c => new CastStudPersonNotification
                {
                    StudPersonId = c.PersonnelId,
                    PersonnelType = int.Parse(c.PersonnelType),
                    FullName = c.FullName,
                    RollEmployeeNo = c.EmployeeNo,
                    ClassName = "",
                    SectionName = "",
                    IsSelected = false
                }).ToList();
            }
        }

        public async Task<ServiceResult> DeleteAdminNotification(int id, int userId)
        {
            var cat = await _context.AdminNotification.SingleOrDefaultAsync(m => m.Id == id);
            if (cat != null)
            {
                cat.IsActive = ((int)Shared.ActiveState.InActive).ToString();
                _context.Entry(cat).State = EntityState.Modified;
                if (await _context.SaveChangesAsync() > 0)
                {
                    return new ServiceResult { StatusId = (int)ServiceResultStatus.Deleted, RecordId = cat.Id };
                }
            }
            return null;
        }

        public async Task<List<SelectListItem>> GetUserTypes()
        {
            return await Task.Run(() => Enum.GetValues(typeof(UserType))
                .Cast<UserType>()
                .Where(x => x != UserType.SuperAdmin && x != UserType.Admin)
                .Select(c => new SelectListItem { Text = c.ToString(), Value = ((int)c).ToString() })
                .ToList());
        }

        public async Task<List<SelectListItem>> GetTagTypes(int branchId, int sessionYearId)
        {
            var tagTypes = Enum.GetValues(typeof(TagType)).Cast<TagType>().ToList();
            var filterDate = DateTime.Now;
            var studentCount = await _context.StudentAttendence.CountAsync(x => x.BranchId == branchId && x.SessionYearId == sessionYearId && x.WorkingDate.Date == filterDate.Date && x.AttendenceId > 0);

            return studentCount == 0
                ? tagTypes.Where(x => x != TagType.AbsentStudents).Select(c => new SelectListItem { Text = c.ToString(), Value = ((int)c).ToString() }).ToList()
                : tagTypes.Select(c => new SelectListItem { Text = c.ToString(), Value = ((int)c).ToString() }).ToList();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
