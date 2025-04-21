using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IPushSubscriptionService _pushSubscriptionService;
        private readonly APIContext _context;

        public NotificationService(APIContext context, IPushSubscriptionService pushSubscriptionService)
        {
            _context = context;
            _pushSubscriptionService = pushSubscriptionService;
        }

        public async Task<List<VNotification>> GetAllNotification(int toUserId)
        {
            var objSession = await _context.SessionYear
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.IsActive == ((int)ActiveState.Active).ToString() && x.IsDefault == true);

            if (objSession == null)
            {
                return new List<VNotification>();
            }

            return await _context.VNotification
                .AsNoTracking()
                .Where(x => x.ToUserId == toUserId && x.SessionYearId == objSession.Id)
                .OrderByDescending(x => x.Id)
                .ToListAsync();
        }

        public async Task<VNotification> ViewNotification(int id)
        {
            return await _context.VNotification
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<int> UpdateNotificationStatus(int id, int userId)
        {
            var notify = await _context.Notification.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
            if (notify == null || notify.ReadStatusId != ((int)ReadStatus.UnRead).ToString())
            {
                return 0;
            }

            notify.ReadStatusId = ((int)ReadStatus.Read).ToString();
            notify.LastModifiedByUserId = userId;
            notify.LastModifiedDate = DateTime.Now;
            _context.Entry(notify).State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateActionTakenStatus(int id, int userId)
        {
            var notify = await _context.Notification.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
            if (notify == null)
            {
                return 0;
            }

            notify.LastModifiedByUserId = userId;
            notify.IsActionTaken = ((int)ActiveState.InActive).ToString();
            notify.LastModifiedDate = DateTime.Now;
            _context.Entry(notify).State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        public async Task<ServiceResult> SendNotification(Notification model, int userId)
        {
            var noti = new Notification
            {
                BranchId = model.BranchId,
                SessionYearId = model.SessionYearId,
                RecordId = model.RecordId,
                ReadStatusId = model.ReadStatusId,
                IsActionTaken = model.IsActionTaken,
                NotificationType = model.NotificationType,
                Message = model.Message,
                FromUserType = model.FromUserType,
                ToUserType = model.ToUserType,
                FromUserId = model.FromUserId,
                ToUserId = model.ToUserId,
                FromPersonnelId = model.FromPersonnelId,
                ToPersonnelId = model.ToPersonnelId,
                Description = model.Description,
                IsActive = ((int)ActiveState.Active).ToString(),
                CreatedDate = DateTime.Now,
                CreatedByUserId = userId
            };

            _context.Notification.Add(noti);
            _context.Entry(noti).State = EntityState.Added;
            var notiIndex = await _context.SaveChangesAsync();

            if (notiIndex == 0)
            {
                return null;
            }

            var userIds = new List<int> { noti.ToUserId };
            var pushPayload = new PushPayLoad { Title = "New Notification", Message = noti.Message };
            return await _pushSubscriptionService.SendPushNotificationAsync(pushPayload, userIds);
        }

        public async Task<ServiceResult> SendDBNotification(List<Notification> model, int notificationType, int userId)
        {
            if (model == null || !model.Any())
            {
                return null;
            }
            try
            {
                var strMessage = notificationType switch
                {
                    (int)NotificationType.LeaveRequest => "Request For Leave",
                    (int)NotificationType.StudentAssignment => "Student Assignment",
                    (int)NotificationType.TeacherAnnouncement => "Teacher Announcement",
                    (int)NotificationType.AdminNotification => "Admin Notification",
                    _ => string.Empty
                };

                var targetUserIds = model.Select(x => x.ToUserId).ToList();
                var notifications = model.Select(notification => new Notification
                {
                    BranchId = notification.BranchId,
                    SessionYearId = notification.SessionYearId,
                    RecordId = notification.RecordId,
                    ReadStatusId = notification.ReadStatusId,
                    IsActionTaken = notification.IsActionTaken,
                    NotificationType = notification.NotificationType,
                    Message = notification.Message,
                    FromUserType = notification.FromUserType,
                    ToUserType = notification.ToUserType,
                    FromUserId = notification.FromUserId,
                    ToUserId = notification.ToUserId,
                    FromPersonnelId = notification.FromPersonnelId,
                    ToPersonnelId = notification.ToPersonnelId,
                    Description = notification.Description,
                    IsActive = ((int)ActiveState.Active).ToString(),
                    CreatedDate = DateTime.Now,
                    CreatedByUserId = userId
                }).ToList();

                _context.Notification.AddRange(notifications);
                var dbIndex = await _context.SaveChangesAsync();

                if (dbIndex == 0)
                {
                    return null;
                }

                var pushPayload = new PushPayLoad { Title = strMessage, Message = notifications.First().Description };
                var serresult = await _pushSubscriptionService.SendPushNotificationAsync(pushPayload, targetUserIds);
                return serresult;
            }
            catch (Exception ex)
            {
                return null;
                
            }
            
        }
    }
}
