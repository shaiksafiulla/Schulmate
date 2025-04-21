using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class StudentAssignmentService : IStudentAssignmentService
    {
        private readonly APIContext _context;
        private readonly INotificationService _notificationService;
        private readonly HostEnvironmentService _hostingEnvironment;
        private readonly S3Service _s3Service;

        public StudentAssignmentService(APIContext context, INotificationService notificationService, HostEnvironmentService hostingEnvironment, S3Service s3Service)
        {
            _context = context;
            _notificationService = notificationService;
            _hostingEnvironment = hostingEnvironment;
            _s3Service = s3Service;

        }

        public async Task<List<VStudentAssignment>> GetAllStudentAssignment()
        {
            return await _context.VStudentAssignment.AsNoTracking().OrderByDescending(x => x.Id).ToListAsync();
        }

        public async Task<VStudentAssignment> ViewStudentAssignment(int id)
        {
            return await _context.VStudentAssignment.AsNoTracking().SingleOrDefaultAsync(m => m.Id == id);
        }

        public async Task<StudentAssignment> GetStudentAssignment(int id, int personId, int branchId, int sessionYearId)
        {
            var model = new StudentAssignment
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
                model.SubmissionDate = DateTime.Now;
                model.FileType = "1";
                model.FilePath = Shared.GetNoImagePath();
            }

            return model;
        }

        public async Task<List<SelectListItem>> GetBranchClassBySubjectTeacherId(int teacherId, int sessionYearId)
        {
            var clsids = await _context.BranchClassSectionTeacher.AsNoTracking()
                .Where(x => x.TeacherId == teacherId && x.SessionYearId == sessionYearId && x.IsActive == ((int)ActiveState.Active).ToString())
                .Select(x => x.BranchClassId)
                .Union(_context.BranchClassSectionSubjectTeacher
                    .Where(x => x.TeacherId == teacherId && x.SessionYearId == sessionYearId && x.IsActive == ((int)ActiveState.Active).ToString())
                    .Select(x => x.BranchClassId))
                .ToListAsync();

            var lstbranchclass = await _context.BranchClass.AsNoTracking().Where(x => clsids.Contains(x.Id)).ToListAsync();
            var lstbranchclassids = lstbranchclass.Select(x => x.ClassId).ToList();
            var lstcls = await _context.Class.AsNoTracking().Where(x => x.IsActive == ((int)ActiveState.Active).ToString() && lstbranchclassids.Contains(x.Id)).ToListAsync();

            return lstbranchclass.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = lstcls.SingleOrDefault(y => y.Id == x.ClassId)?.Name
            }).ToList();
        }

        public async Task<List<SelectListItem>> GetClassBySubjectTeacherId(int teacherId, int sessionYearId)
        {
            var clsids = await _context.BranchClassSectionTeacher.AsNoTracking()
                .Where(x => x.TeacherId == teacherId && x.SessionYearId == sessionYearId && x.IsActive == ((int)ActiveState.Active).ToString())
                .Select(x => x.ClassId)
                .Union(_context.BranchClassSectionSubjectTeacher
                    .Where(x => x.TeacherId == teacherId && x.SessionYearId == sessionYearId && x.IsActive == ((int)ActiveState.Active).ToString())
                    .Select(x => x.ClassId))
                .ToListAsync();

            var lstcls = await _context.Class.AsNoTracking().Where(x => x.IsActive == ((int)ActiveState.Active).ToString() && clsids.Contains(x.Id)).ToListAsync();

            return lstcls.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
        }

        public async Task<ServiceResult> SaveStudentAssignment(StudentAssignment model, int userId)
        {
            ServiceResult result = null;

            if (model.Id > 0)
            {
                // Update logic here
            }
            else
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var cat = new StudentAssignment
                        {
                            SessionYearId = model.SessionYearId,
                            BranchId = model.BranchId,
                            BranchClassId = model.BranchClassId,
                            SubjectId = model.SubjectId,
                            ClassId = _context.BranchClass.SingleOrDefault(x => x.Id == model.BranchClassId)?.ClassId ?? 0,
                            ClassSubjectId = await GetClassSubjectId(model.ClassId, model.SubjectId),
                            Title = model.Title,
                            SubmissionDate = model.SubmissionDate,
                            FileType = model.FileType,
                            Description = model.Description,
                            IsActive = ((int)ActiveState.Active).ToString(),
                            CreatedDate = DateTime.Now,
                            CreatedByUserId = userId
                        };

                        if (model.AssignmentFile != null)
                        {
                            cat.FileName = model.AssignmentFile.FileName;
                            // cat.FilePath = Shared.ProcessUploadFile((int)UploadType.Assignment, model.AssignmentFile, _hostingEnvironment.GetWebRootPath());
                            cat.FilePath = await _s3Service.UploadFileAsync((int)UploadType.Assignment, model.AssignmentFile);

                        }

                        _context.StudentAssignment.Add(cat);
                        await _context.SaveChangesAsync();

                        if (model.SectionIds != null)
                        {
                            var secList = JsonConvert.DeserializeObject<List<int>>(model.SectionIds);
                            foreach (var assignsection in secList)
                            {
                                var section = new StudentAssignmentSection
                                {
                                    StudentAssignmentId = cat.Id,
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

                                _context.StudentAssignmentSection.Add(section);
                            }

                            await _context.SaveChangesAsync();

                            var lstNotify = new List<Notification>();
                            var lstsavedsections = await _context.StudentAssignmentSection.AsNoTracking().Where(x => x.StudentAssignmentId == cat.Id).ToListAsync();

                            foreach (var sec in lstsavedsections)
                            {
                                var lststudentscurrentsection = await _context.VStudentCurrentLocation.AsNoTracking()
                                    .Where(x => x.SectionId == sec.SectionId && x.SessionYearId == sec.SessionYearId)
                                    .ToListAsync();

                                foreach (var student in lststudentscurrentsection)
                                {
                                    var submission = new StudentAssignmentStatus
                                    {
                                        StudentAssignmentId = cat.Id,
                                        SessionYearId = cat.SessionYearId,
                                        BranchClassId = cat.BranchClassId,
                                        BranchId = cat.BranchId,
                                        ClassId = cat.ClassId,
                                        ClassSubjectId = cat.ClassSubjectId,
                                        SubjectId = cat.SubjectId,
                                        SectionId = sec.SectionId,
                                        StudentId = student.StudentId,
                                        StatusId = ((int)AssignmentStatus.New).ToString(),
                                        IsActive = ((int)ActiveState.Active).ToString(),
                                        FileName = string.Empty,
                                        FilePath = Shared.GetNoImagePath(),
                                        CreatedDate = DateTime.Now,
                                        CreatedByUserId = userId
                                    };

                                    _context.StudentAssignmentStatus.Add(submission);
                                }

                                var lststudentids = lststudentscurrentsection.Select(x => x.StudentId).ToList();
                                foreach (var stud in lststudentids)
                                {
                                    var objNotify = await SetNotification(cat, userId, stud, cat.Title);
                                    lstNotify.Add(objNotify);
                                }
                            }

                            if (lstNotify.Any())
                            {
                                var notindex =  await _notificationService.SendDBNotification(lstNotify, (int)NotificationType.StudentAssignment, userId);
                                if (notindex != null)
                                {
                                    transaction.Commit();
                                    result = new ServiceResult
                                    {
                                        StatusId = (int)ServiceResultStatus.Added,
                                        RecordId = cat.Id
                                    };
                                }
                            }
                            else
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
                    catch (Exception)
                    {
                        transaction.Rollback();
                    }
                }
            }

            return result;
        }

        public async Task<Notification> SetNotification(StudentAssignment model, int fromUserId, int toPersonId, string message)
        {
            var objUser = await _context.VUserInfo.AsNoTracking().SingleOrDefaultAsync(x => x.Id == fromUserId);
            var objPerson = await _context.VUserInfo.AsNoTracking().SingleOrDefaultAsync(x => x.UserType == ((int)UserType.Student).ToString() && x.PersonId == toPersonId);

            return new Notification
            {
                BranchId = model.BranchId,
                SessionYearId = model.SessionYearId,
                RecordId = model.Id,
                ReadStatusId = ((int)ReadStatus.UnRead).ToString(),
                NotificationType = ((int)NotificationType.StudentAssignment).ToString(),
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

        public async Task<ServiceResult> DeleteStudentAssignment(int id, int userId)
        {
            var cat = await _context.StudentAssignment.SingleOrDefaultAsync(m => m.Id == id);
            if (cat == null) return null;

            cat.IsActive = ((int)ActiveState.InActive).ToString();
            cat.LastModifiedByUserId = userId;
            cat.LastModifiedDate = DateTime.Now;

            _context.Entry(cat).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return new ServiceResult
            {
                StatusId = (int)ServiceResultStatus.Deleted,
                RecordId = cat.Id
            };
        }

        public async Task<List<SelectListItem>> GetClassByBranch(int branchId)
        {
            var clsids = await _context.BranchClass.AsNoTracking().Where(x => x.BranchId == branchId).Select(x => x.ClassId).ToListAsync();
            var lstcls = await _context.Class.AsNoTracking().Where(x => x.IsActive == ((int)ActiveState.Active).ToString() && clsids.Contains(x.Id)).ToListAsync();

            return lstcls.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
        }

        public async Task<List<SelectListItem>> GetSectionByClass(int branchId, int classId)
        {
            var lstbrclsids = await _context.BranchClass.AsNoTracking().Where(x => x.BranchId == branchId && x.ClassId == classId).Select(x => x.Id).ToListAsync();
            var lstsec = await _context.Section.AsNoTracking().Where(x => x.IsActive == ((int)ActiveState.Active).ToString() && lstbrclsids.Contains(x.BranchClassId)).ToListAsync();

            return lstsec.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
        }

        public async Task<List<int>> GetSectionIdsByStudentAssignment(int studentAssignId)
        {
            return await _context.StudentAssignmentSection.AsNoTracking().Where(x => x.StudentAssignmentId == studentAssignId).Select(x => x.SectionId).ToListAsync();
        }

        public async Task<List<SelectListItem>> GetSubjectByClass(int classId, int teacherId)
        {
            var lstsubjids = await _context.BranchClassSectionSubjectTeacher.AsNoTracking().Where(x => x.ClassId == classId && x.TeacherId == teacherId).Select(x => x.SubjectId).ToListAsync();
            var lstsec = await _context.Subject.AsNoTracking().Where(x => x.IsActive == ((int)ActiveState.Active).ToString() && lstsubjids.Contains(x.Id)).ToListAsync();

            return lstsec.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
        }

        public async Task<int> GetBranchClassId(int branchId, int classId)
        {
            return _context.BranchClass.AsNoTracking().SingleOrDefaultAsync(x => x.BranchId == branchId && x.ClassId == classId)?.Id ?? 0;
        }

        public async Task<int> GetClassSubjectId(int classId, int subjectId)
        {
            return  _context.ClassSubject.AsNoTracking().SingleOrDefaultAsync(x => x.ClassId == classId && x.SubjectId == subjectId)?.Id ?? 0;
        }

        public async Task<List<VStudentAssignmentStatus>> GetAllStudentAssignStatus()
        {
            return await _context.VStudentAssignmentStatus.AsNoTracking().OrderBy(x => x.Id).ToListAsync();
        }

        public async Task<VStudentAssignmentStatus> ViewStudentAssignStatus(int id)
        {
            return  await _context.VStudentAssignmentStatus.AsNoTracking().SingleOrDefaultAsync(m => m.Id == id);
        }

        public async Task<StudentAssignStatus> GetStudentAssignStatus(int id)
        {
            var cat = await _context.VStudentAssignmentStatus.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
            if (cat == null) return new StudentAssignStatus();

            return new StudentAssignStatus
            {
                Id = cat.Id,
                StatusId = cat.StatusId == ((int)AssignmentStatus.New).ToString() ? ((int)AssignmentStatus.Accepted).ToString() : cat.StatusId,
                FeedBack = cat.FeedBack,
                SessionYearName = cat.SessionYearName,
                BranchName = cat.BranchName,
                ClassName = cat.ClassName,
                SectionName = cat.SectionName,
                SubjectName = cat.SubjectName,
                FullName = cat.FullName,
                FileName = cat.FileName,
                FilePath = cat.FilePath
            };
        }

        public async Task<ServiceResult> SaveStudentAssignStatus(StudentAssignStatus model, int userId)
        {
            var cat = await _context.StudentAssignmentStatus.SingleOrDefaultAsync(m => m.Id == model.Id && m.IsActive == ((int)ActiveState.Active).ToString());
            if (cat == null) return null;

            cat.StatusId = model.StatusId;
            cat.FeedBack = model.FeedBack;

            _context.Entry(cat).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return new ServiceResult
            {
                StatusId = (int)ServiceResultStatus.Updated,
                RecordId = cat.Id
            };
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
