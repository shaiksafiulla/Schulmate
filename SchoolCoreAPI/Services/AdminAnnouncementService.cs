using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class AdminAnnouncementService : IAdminAnnouncementService
    {
        private readonly APIContext _context;
        private readonly INotificationService _NotificationService;
        private readonly HostEnvironmentService _hostingEnvironment;
        private readonly S3Service _s3Service;

        public AdminAnnouncementService(APIContext context, INotificationService NotificationService, HostEnvironmentService hostingEnvironment, S3Service s3Service)
        {
            _context = context;
            _NotificationService = NotificationService;
            _hostingEnvironment = hostingEnvironment;
            _s3Service = s3Service;
        }

        public async Task<List<VAdminAnnouncement>> GetAllAdminAnnouncement(int UserId)
        {
            var objuser = await _context.VUserInfo.AsNoTracking().SingleOrDefaultAsync(x => x.Id == UserId);
            if (objuser == null) return new List<VAdminAnnouncement>();

            return objuser.UserType == ((int)UserType.SuperAdmin).ToString()
                ? await _context.VAdminAnnouncement.AsNoTracking().ToListAsync()
                : await _context.VAdminAnnouncement.AsNoTracking().Where(x => x.BranchId == objuser.BranchId).ToListAsync();
        }

        public async Task<VAdminAnnouncement> ViewAdminAnnouncement(int Id)
        {
            return await _context.VAdminAnnouncement.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
        }

        public async Task<AdminAnnouncement> GetAdminAnnouncement(int Id, int UserId)
        {
            var model = new AdminAnnouncement { ClassSectionSheet = new List<SelectListItem>() };

            if (Id > 0)
            {
                var cat = await _context.AdminAnnouncement.AsNoTracking().SingleOrDefaultAsync(x => x.Id == Id && x.IsActive == ((int)ActiveState.Active).ToString());
                if (cat != null)
                {
                    model = cat;
                }
            }
            else
            {
                var objsession = await _context.SessionYear.AsNoTracking().SingleOrDefaultAsync(x => x.IsActive == ((int)ActiveState.Active).ToString() && x.IsDefault == true);
                if (objsession != null) model.SessionYearId = objsession.Id;

                var objUser = await _context.VUserInfo.AsNoTracking().SingleOrDefaultAsync(x => x.Id == UserId);
                if (objUser != null) model.BranchId = (int)objUser.BranchId;

                model.ClassSectionSheet = await GetClassSectionByBranchAsync(model.BranchId);
                model.FileType = "1";
                model.FilePath = Shared.GetNoImagePath();
            }

            return model;
        }

        public async Task<ServiceResult> SaveAdminAnnouncement(AdminAnnouncement model, int UserId)
        {
            if (model.Id > 0) return null;

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var cat = new AdminAnnouncement
                    {
                        SessionYearId = model.SessionYearId,
                        BranchId = model.BranchId,
                        Title = model.Title,
                        FileType = model.FileType,
                        Description = model.Description,
                        IsActive = ((int)ActiveState.Active).ToString(),
                        CreatedDate = DateTime.Now,
                        CreatedByUserId = UserId
                    };

                    if (model.AssignmentFile != null)
                    {
                        cat.FileName = model.AssignmentFile.FileName;
                        // cat.FilePath = Shared.ProcessUploadFile((int)UploadType.Announcement, model.AssignmentFile, _hostingEnvironment.GetWebRootPath());
                        cat.FilePath = await _s3Service.UploadFileAsync((int)UploadType.Announcement, model.AssignmentFile);
                    }

                    _context.AdminAnnouncement.Add(cat);
                    await _context.SaveChangesAsync();

                    if (model.SectionIds.Count > 0)
                    {
                        var sections = model.SectionIds.Select(assignsection =>
                        {
                            var objsec = _context.VSections.AsNoTracking().SingleOrDefault(x => x.Id == int.Parse(assignsection));
                            return new AdminAnnouncementSection
                            {
                                AdminAnnouncementId = cat.Id,
                                SessionYearId = cat.SessionYearId,
                                BranchClassId = (int)objsec.BranchClassId,
                                BranchId = cat.BranchId,
                                ClassId = (int)objsec.ClassId,
                                SectionId = int.Parse(assignsection),
                                IsActive = ((int)ActiveState.Active).ToString(),
                                CreatedDate = DateTime.Now,
                                CreatedByUserId = UserId
                            };
                        }).ToList();

                        _context.AdminAnnouncementSection.AddRange(sections);
                        await _context.SaveChangesAsync();

                        var lstNotify = new List<Notification>();
                        var lstsavedsections = await _context.AdminAnnouncementSection.AsNoTracking().Where(x => x.AdminAnnouncementId == cat.Id).ToListAsync();

                        foreach (var sec in lstsavedsections)
                        {
                            var lststudentscurrentsection = await _context.VStudentCurrentLocation.AsNoTracking().Where(x => x.SectionId == sec.SectionId && x.SessionYearId == sec.SessionYearId).ToListAsync();
                            if (lststudentscurrentsection.Count > 0)
                            {
                                var lststudentids = lststudentscurrentsection.Select(x => x.StudentId).ToList();
                                lstNotify.AddRange(lststudentids.Select(stud => SetNotificationAsync(cat, UserId, stud, cat.Title).Result));
                            }
                        }

                        if (lstNotify.Count > 0)
                        {
                            var notindex = await _NotificationService.SendDBNotification(lstNotify, (int)NotificationType.AdminAnnouncement, UserId);
                            if (notindex != null)
                            {
                                await transaction.CommitAsync();
                                return new ServiceResult { StatusId = (int)ServiceResultStatus.Added, RecordId = cat.Id };
                            }
                        }
                    }

                    await transaction.CommitAsync();
                    return new ServiceResult { StatusId = (int)ServiceResultStatus.Added, RecordId = cat.Id };
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    return null;
                }
            }
        }

        public async Task<ServiceResult> DeleteAdminAnnouncement(int Id, int UserId)
        {
            var cat = await _context.AdminAnnouncement.SingleOrDefaultAsync(m => m.Id == Id);
            if (cat == null) return null;

            cat.IsActive = ((int)ActiveState.InActive).ToString();
            cat.LastModifiedByUserId = UserId;
            cat.LastModifiedDate = DateTime.Now;
            _context.Entry(cat).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return new ServiceResult { StatusId = (int)ServiceResultStatus.Deleted, RecordId = cat.Id };
        }

        public async Task<List<int>> GetSectionIdsByAdminAnnouncement(int AdminAnnounceId)
        {
            return await _context.AdminAnnouncementSection.AsNoTracking().Where(x => x.AdminAnnouncementId == AdminAnnounceId)
                .Select(x => x.SectionId)
                .ToListAsync();
        }
       

        public async Task<List<SelectListItem>> GetClassSectionByBranchAsync(int BranchId)
        {
            return await _context.VSections.AsNoTracking()
                .Where(x => x.BranchId == BranchId)
                .Select(x => new SelectListItem { Text = x.ClassName + " - " + x.Name, Value = x.Id.ToString() })
                .ToListAsync();
        }

        public async Task<Notification> SetNotificationAsync(AdminAnnouncement model, int FromUserId, int ToPersonId, string message)
        {
            var objUser = await _context.VUserInfo.AsNoTracking().SingleOrDefaultAsync(x => x.Id == FromUserId);
            var objPerson = await _context.VUserInfo.AsNoTracking().SingleOrDefaultAsync(x => x.UserType == ((int)UserType.Student).ToString() && x.PersonId == ToPersonId);

            return new Notification
            {
                BranchId = model.BranchId,
                SessionYearId = model.SessionYearId,
                RecordId = model.Id,
                ReadStatusId = ((int)ReadStatus.UnRead).ToString(),
                NotificationType = ((int)NotificationType.AdminAnnouncement).ToString(),
                Message = message,
                Description = message,
                FromUserType = objUser.UserType,
                FromUserId = FromUserId,
                FromPersonnelId = objUser.PersonId,
                ToPersonnelId = ToPersonId,
                ToUserType = ((int)UserType.Student).ToString(),
                ToUserId = objPerson.Id
            };
        }


        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
