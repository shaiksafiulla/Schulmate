using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class TeacherAnnouncementService : ITeacherAnnouncementService
    {
        private readonly APIContext _context;
        private readonly INotificationService _notificationService;
        private readonly HostEnvironmentService _hostingEnvironment;
        private readonly S3Service _s3Service;
        public TeacherAnnouncementService(APIContext context, INotificationService notificationService, HostEnvironmentService hostingEnvironment, S3Service s3Service)
        {
            _context = context;
            _notificationService = notificationService;
            _hostingEnvironment = hostingEnvironment;
            _s3Service = s3Service;

        }

        public async Task<List<VTeacherAnnouncement>> GetAllTeacherAnnouncement(int teacherId, int sessionYearId)
        {
            var classIds = await _context.BranchClassSectionTeacher.AsNoTracking()
                .Where(x => x.TeacherId == teacherId && x.SessionYearId == sessionYearId && x.IsActive == ((int)ActiveState.Active).ToString())
                .Select(x => x.ClassId)
                .Union(_context.BranchClassSectionSubjectTeacher
                    .Where(x => x.TeacherId == teacherId && x.SessionYearId == sessionYearId && x.IsActive == ((int)ActiveState.Active).ToString())
                    .Select(x => x.ClassId))
                .ToListAsync();

            return _context.VTeacherAnnouncement
                .Where(x => classIds.Contains(x.ClassId))
                .OrderByDescending(x => x.Id)
                .ToList();
        }

        public async Task<List<VTeacherAnnouncementSection>> GetTeacherAnnouncementSections(int sectionId, int sessionYearId)
        {
            return await _context.VTeacherAnnouncementSection.AsNoTracking()
                .Where(x => x.SectionId == sectionId && x.SessionYearId == sessionYearId)
                .OrderByDescending(x => x.Id)
                .ToListAsync();
        }

        public async Task<VTeacherAnnouncement> ViewTeacherAnnouncement(int id)
        {
            return await _context.VTeacherAnnouncement.SingleOrDefaultAsync(m => m.Id == id);
        }

        public async Task<TeacherAnnouncement> GetTeacherAnnouncement(int id, int personId, int branchId, int sessionYearId)
        {
            var model = new TeacherAnnouncement
            {
                SubjectSheet = new List<SelectListItem>()
            };

            if (id <= 0)
            {
                model.SessionYearId = sessionYearId;
                model.BranchId = branchId;
                model.BranchClassSheet = new List<SelectListItem>(await GetBranchClassBySubjectTeacherId(personId, sessionYearId));
                model.BranchClassId = 0;
                model.ClassSubjectId = 0;
                model.FileType = "1";
                model.FilePath = Shared.GetNoImagePath();
            }

            return model;
        }

        public async Task<ServiceResult> SaveTeacherAnnouncement(TeacherAnnouncement model, int userId)
        {
            ServiceResult result = null;

            if (model.Id > 0)
            {
                var cat = await _context.TeacherAnnouncement.SingleOrDefaultAsync(m => m.Id == model.Id && m.IsActive == ((int)ActiveState.Active).ToString());

                if (cat != null)
                {
                    UpdateTeacherAnnouncement(cat, model, userId);
                    result = new ServiceResult { StatusId = (int)ServiceResultStatus.Updated, RecordId = cat.Id };
                }
            }
            else
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var cat = new TeacherAnnouncement
                        {
                            SessionYearId = model.SessionYearId,
                            BranchId = model.BranchId,
                            BranchClassId = model.BranchClassId,
                            SubjectId = model.SubjectId,
                            Title = model.Title,
                            FileType = model.FileType,
                            Description = model.Description,
                            IsActive = ((int)ActiveState.Active).ToString(),
                            CreatedDate = DateTime.Now,
                            CreatedByUserId = userId
                        };

                        var objBrcls = _context.BranchClass.SingleOrDefault(x => x.Id == model.BranchClassId);
                        if (objBrcls != null)
                        {
                            cat.ClassId = objBrcls.ClassId;
                            cat.ClassSubjectId = await GetClassSubjectId(objBrcls.ClassId, model.SubjectId);
                        }

                        if (model.AssignmentFile != null)
                        {
                            cat.FileName = model.AssignmentFile.FileName;
                            //cat.FilePath = Shared.ProcessUploadFile((int)UploadType.Announcement, model.AssignmentFile, _hostingEnvironment.GetWebRootPath());
                            cat.FilePath = await _s3Service.UploadFileAsync((int)UploadType.Announcement, model.AssignmentFile);

                        }

                        _context.TeacherAnnouncement.Add(cat);
                        _context.SaveChanges();

                        if (model.SectionIds != null)
                        {
                            var secList = JsonConvert.DeserializeObject<List<int>>(model.SectionIds);
                            foreach (var assignsection in secList)
                            {
                                var section = new TeacherAnnouncementSection
                                {
                                    TeacherAnnouncementId = cat.Id,
                                    SessionYearId = cat.SessionYearId,
                                    BranchClassId = cat.BranchClassId,
                                    BranchId = cat.BranchId,
                                    ClassId = cat.ClassId,
                                    ClassSubjectId = cat.ClassSubjectId,
                                    SubjectId = cat.SubjectId,
                                    SectionId = assignsection,
                                    IsActive = ((int)ActiveState.Active).ToString(),
                                    CreatedDate = DateTime.Now,
                                    CreatedByUserId = userId
                                };

                                _context.TeacherAnnouncementSection.Add(section);
                            }

                            await _context.SaveChangesAsync();

                            var lstNotify = new List<Notification>();
                            var lstsavedsections = _context.TeacherAnnouncementSection.Where(x => x.TeacherAnnouncementId == cat.Id).ToList();
                            foreach (var sec in lstsavedsections)
                            {
                                var lststudentscurrentsection = _context.VStudentCurrentLocation.Where(x => x.SectionId == sec.SectionId && x.SessionYearId == sec.SessionYearId).ToList();
                                foreach (var stud in lststudentscurrentsection.Select(x => x.StudentId))
                                {
                                    var objNotify = await SetNotification(cat, userId, stud, cat.Title);
                                    lstNotify.Add(objNotify);
                                }
                            }

                            if (lstNotify.Any())
                            {
                                var notindex = await _notificationService.SendDBNotification(lstNotify, (int)NotificationType.TeacherAnnouncement, userId);
                                if (notindex != null)
                                {
                                    transaction.Commit();
                                    result = new ServiceResult { StatusId = (int)ServiceResultStatus.Added, RecordId = cat.Id };
                                }
                            }
                            else
                            {
                                transaction.Commit();
                                result = new ServiceResult { StatusId = (int)ServiceResultStatus.Added, RecordId = cat.Id };
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                    }
                }
            }

            return result;
        }

        private async void UpdateTeacherAnnouncement(TeacherAnnouncement cat, TeacherAnnouncement model, int userId)
        {
            cat.SessionYearId = model.SessionYearId;
            cat.BranchId = model.BranchId;
            cat.BranchClassId = model.BranchClassId;
            cat.SubjectId = model.SubjectId;

            var objBrcls = _context.BranchClass.SingleOrDefault(x => x.Id == model.BranchClassId);
            if (objBrcls != null)
            {
                cat.ClassId = objBrcls.ClassId;
                cat.ClassSubjectId = await GetClassSubjectId(objBrcls.ClassId, model.SubjectId);
            }

            cat.Title = model.Title;
            cat.Description = model.Description;
            cat.LastModifiedDate = DateTime.Now;
            cat.LastModifiedByUserId = userId;

            if (model.AssignmentFile != null)
            {
                //if (!string.IsNullOrEmpty(cat.FilePath) && !cat.FilePath.Contains("noimage.png"))
                //{
                //    var filePath = _hostingEnvironment.GetFullUrlFromDbPath(cat.FilePath);
                //    if (File.Exists(filePath))
                //    {
                //        File.Delete(filePath);
                //    }
                //}
               // cat.FilePath = Shared.ProcessUploadFile((int)UploadType.Announcement, model.AssignmentFile, _hostingEnvironment.GetWebRootPath());
                if (!string.IsNullOrEmpty(cat.FilePath) && !cat.FilePath.Contains("noimage.png"))
                {
                    await _s3Service.DeleteFileAsync(cat.FilePath);
                }
                cat.FilePath = await _s3Service.UploadFileAsync((int)UploadType.Announcement, model.AssignmentFile);

            }

            _context.Entry(cat).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public async Task<Notification> SetNotification(TeacherAnnouncement model, int fromUserId, int toPersonId, string message)
        {
            var objUser = await _context.VUserInfo.AsNoTracking().SingleOrDefaultAsync(x => x.Id == fromUserId);
            var objPerson = await _context.VUserInfo.AsNoTracking().SingleOrDefaultAsync(x => x.UserType == ((int)UserType.Student).ToString() && x.PersonId == toPersonId);

            return new Notification
            {
                BranchId = model.BranchId,
                SessionYearId = model.SessionYearId,
                RecordId = model.Id,
                ReadStatusId = ((int)ReadStatus.UnRead).ToString(),
                IsActionTaken = ((int)ActiveState.InActive).ToString(),
                NotificationType = ((int)NotificationType.TeacherAnnouncement).ToString(),
                Message = message,
                Description = message,
                FromUserType = objUser.UserType,
                FromUserId = fromUserId,
                FromPersonnelId = objUser.PersonId,
                ToPersonnelId = toPersonId,
                ToUserType = ((int)UserType.Student).ToString(),
                ToUserId = objPerson.Id
            };
        }

        public async Task<Tuple<byte[], string>> GetFileBytes(int id)
        {
            var objattach = _context.TeacherAnnouncement.SingleOrDefault(m => m.Id == id && m.IsActive == ((int)ActiveState.Active).ToString());
            if (objattach != null && !string.IsNullOrEmpty(objattach.FilePath))
            {
                var filePath = _hostingEnvironment.GetFullUrlFromDbPath(objattach.FilePath);
                var bytes = File.ReadAllBytes(filePath);
                return new Tuple<byte[], string>(bytes, objattach.FileName);
            }

            return null;
        }

        public async Task<ServiceResult> DeleteTeacherAnnouncement(int id, int userId)
        {
            var cat = await _context.TeacherAnnouncement.AsNoTracking().SingleOrDefaultAsync(m => m.Id == id);
            if (cat != null)
            {
                cat.IsActive = ((int)ActiveState.InActive).ToString();
                cat.LastModifiedByUserId = userId;
                cat.LastModifiedDate = DateTime.Now;

                _context.Entry(cat).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return new ServiceResult { StatusId = (int)ServiceResultStatus.Deleted, RecordId = cat.Id };
            }

            return null;
        }

        public async Task<List<SelectListItem>> GetBranch()
        {
            return await _context.Branch.AsNoTracking()
                .Where(x => x.IsActive == ((int)ActiveState.Active).ToString())
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetSessionYear()
        {
            return await _context.SessionYear.AsNoTracking()
                .Where(x => x.IsActive == ((int)ActiveState.Active).ToString())
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetClassByBranch(int branchId)
        {
            var classIds = await _context.BranchClass.AsNoTracking()
                .Where(x => x.BranchId == branchId)
                .Select(x => x.ClassId)
                .ToListAsync();

            return await _context.Class.AsNoTracking()
                .Where(x => x.IsActive == ((int)ActiveState.Active).ToString() && classIds.Contains(x.Id))
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetBranchClassBySubjectTeacherId(int teacherId, int sessionYearId)
        {
            var branchClassIds = await _context.BranchClassSectionTeacher.AsNoTracking()
                .Where(x => x.TeacherId == teacherId && x.SessionYearId == sessionYearId && x.IsActive == ((int)ActiveState.Active).ToString())
                .Select(x => x.BranchClassId)
                .Union(_context.BranchClassSectionSubjectTeacher
                    .Where(x => x.TeacherId == teacherId && x.SessionYearId == sessionYearId && x.IsActive == ((int)ActiveState.Active).ToString())
                    .Select(x => x.BranchClassId))
                .ToListAsync();

            var branchClasses = await _context.BranchClass.AsNoTracking()
                .Where(x => branchClassIds.Contains(x.Id))
                .ToListAsync();

            var classIds = branchClasses.Select(x => x.ClassId).ToList();

            var classes = await _context.Class.AsNoTracking()
                .Where(x => x.IsActive == ((int)ActiveState.Active).ToString() && classIds.Contains(x.Id))
                .ToListAsync();

            return branchClasses
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = classes.SingleOrDefault(y => y.Id == x.ClassId)?.Name
                })
                .ToList();
        }

        public async Task<List<SelectListItem>> GetSectionByClassAndSubjectTeacher(int branchClassId, int teacherId, int sessionYearId)
        {
            var sectionIds = await _context.BranchClassSectionTeacher.AsNoTracking()
                .Where(x => x.TeacherId == teacherId && x.SessionYearId == sessionYearId && x.BranchClassId == branchClassId && x.IsActive == ((int)ActiveState.Active).ToString())
                .Select(x => x.SectionId)
                .Union(_context.BranchClassSectionSubjectTeacher
                    .Where(x => x.TeacherId == teacherId && x.SessionYearId == sessionYearId && x.BranchClassId == branchClassId && x.IsActive == ((int)ActiveState.Active).ToString())
                    .Select(x => x.SectionId))
                .ToListAsync();

            return await _context.Section.AsNoTracking()
                .Where(x => x.IsActive == ((int)ActiveState.Active).ToString() && sectionIds.Contains(x.Id))
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                .ToListAsync();
        }

        public async Task<List<int>> GetSectionIdsByTeacherAnnouncement(int teacherAnnouncementId)
        {
            return await _context.TeacherAnnouncementSection.AsNoTracking()
                .Where(x => x.TeacherAnnouncementId == teacherAnnouncementId)
                .Select(x => x.SectionId)
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetSubjectByClass(int branchClassId, int teacherId, int sessionYearId)
        {
            var subjectIds = await _context.BranchClassSectionTeacher.AsNoTracking()
                .Where(x => x.TeacherId == teacherId && x.SessionYearId == sessionYearId && x.BranchClassId == branchClassId && x.IsActive == ((int)ActiveState.Active).ToString())
                .SelectMany(x => _context.BranchClassSectionSubjectTeacher
                    .Where(y => y.BranchClassId == branchClassId && y.SessionYearId == sessionYearId)
                    .Select(y => y.SubjectId))
                .Union(_context.BranchClassSectionSubjectTeacher
                    .Where(x => x.BranchClassId == branchClassId && x.TeacherId == teacherId && x.SessionYearId == sessionYearId)
                    .Select(x => x.SubjectId))
                .ToListAsync();

            return await _context.Subject.AsNoTracking()
                .Where(x => x.IsActive == ((int)ActiveState.Active).ToString() && subjectIds.Contains(x.Id))
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                .ToListAsync();
        }

        public async Task<int> GetBranchClassId(int branchId, int classId)
        {
            return await _context.BranchClass.AsNoTracking()
                .Where(x => x.BranchId == branchId && x.ClassId == classId)
                .Select(x => x.Id)
                .SingleOrDefaultAsync();
        }

        public async Task<int> GetClassSubjectId(int classId, int subjectId)
        {
            return await _context.ClassSubject.AsNoTracking()
                .Where(x => x.ClassId == classId && x.SubjectId == subjectId)
                .Select(x => x.Id)
                .SingleOrDefaultAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
