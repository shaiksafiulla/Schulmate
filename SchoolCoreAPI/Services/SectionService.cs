using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class SectionService : ISectionService
    {
        private readonly APIContext _context;
        public SectionService(APIContext context)
        {
            _context = context;
        }
        public async Task<List<VSections>> GetAllSection(int UserId)
        {
            var objuser = await _context.VUserInfo.SingleOrDefaultAsync(x => x.Id == UserId);
            if (objuser.UserType == ((int)UserType.SuperAdmin).ToString())
            {
                return await _context.VSections.AsNoTracking().ToListAsync();
            }
            return await _context.VSections.AsNoTracking().Where(x => x.BranchId == objuser.BranchId).ToListAsync();
        }
        public async Task<VSections> ViewSection(int Id)
        {
            return await _context.VSections.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
        }
        public async Task<Section> GetSection(int Id)
        {
            var model = new Section
            {
                BranchSheet = new List<SelectListItem>(await GetBranches()),
                ClassSheet = new List<SelectListItem>()
            };

            if (Id > 0)
            {
                var cat = await _context.Section.AsNoTracking().SingleOrDefaultAsync(x => x.Id == Id && x.IsActive == ((int)Shared.ActiveState.Active).ToString());
                if (cat != null)
                {
                    model.Id = cat.Id;
                    model.Name = cat.Name;
                    model.ShortName = cat.ShortName;
                    model.MaxCount = cat.MaxCount;
                    model.Description = cat.Description;
                    if (cat.BranchClassId > 0)
                    {
                        model.BranchClassId = cat.BranchClassId;
                        var objclssubject = _context.BranchClass.SingleOrDefault(x => x.Id == cat.BranchClassId);
                        if (objclssubject != null)
                        {
                            model.BranchId = objclssubject.BranchId;
                            var divselectedItem = model.BranchSheet.Find(p => p.Value == objclssubject.BranchId.ToString());
                            if (divselectedItem != null)
                            {
                                divselectedItem.Selected = true;
                                model.ClassId = objclssubject.ClassId;

                                var subids = await _context.BranchClass.AsNoTracking().Where(x => x.BranchId == objclssubject.BranchId).Select(x => x.ClassId).ToListAsync();
                                var lst = await _context.Class.AsNoTracking().Where(x => x.IsActive == ((int)Shared.ActiveState.Active).ToString() && subids.Contains(x.Id)).ToListAsync();
                                model.ClassSheet = lst.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();

                                var catselectedItem = model.ClassSheet.Find(p => p.Value == model.ClassId.ToString());
                                if (catselectedItem != null)
                                {
                                    catselectedItem.Selected = true;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                model.BranchClassId = 0;
            }
            return model;
        }
        public async Task<bool> IsRecordExists(string name, int branchClassId, int Id)
        {
            try
            {
                return Id == 0
                    ? await _context.VSections.AnyAsync(e => e.Name == name && e.BranchClassId == branchClassId)
                    : await _context.VSections.AnyAsync(e => e.Name == name && e.BranchClassId == branchClassId && e.Id != Id);
            }
            catch
            {
                return false;
            }
        }
        public async Task<ServiceResult> SaveSection(Section model, int UserId)
        {
            if (model == null) return null;

            if (model.BranchId > 0 && model.ClassId > 0)
            {
                var objclasssubj = await _context.BranchClass.AsNoTracking().SingleOrDefaultAsync(x => x.BranchId == model.BranchId && x.ClassId == model.ClassId);
                if (objclasssubj != null)
                {
                    model.BranchClassId = objclasssubj.Id;
                }
            }

            Section cat;
            if (model.Id > 0)
            {
                cat = await _context.Section.AsNoTracking().SingleOrDefaultAsync(m => m.Id == model.Id && m.IsActive == ((int)Shared.ActiveState.Active).ToString());
                if (cat == null) return null;

                cat.Name = model.Name;
                cat.ShortName = model.ShortName;
                cat.BranchClassId = model.BranchClassId;
                cat.MaxCount = model.MaxCount;
                cat.Description = model.Description;
                cat.LastModifiedDate = DateTime.Now;
                cat.LastModifiedByUserId = UserId;
                _context.Entry(cat).State = EntityState.Modified;
            }
            else
            {
                cat = new Section
                {
                    Name = model.Name,
                    ShortName = model.ShortName,
                    BranchClassId = model.BranchClassId,
                    MaxCount = model.MaxCount,
                    Description = model.Description,
                    IsActive = ((int)Shared.ActiveState.Active).ToString(),
                    CreatedDate = DateTime.Now,
                    CreatedByUserId = UserId
                };
                _context.Section.Add(cat);
                _context.Entry(cat).State = EntityState.Added;
            }

            var changes = await _context.SaveChangesAsync();
            if (changes == 0) return null;

            return new ServiceResult
            {
                StatusId = model.Id > 0 ? (int)ServiceResultStatus.Updated : (int)ServiceResultStatus.Added,
                RecordId = cat.Id
            };
        }
        public async Task<ServiceResult> DeleteSection(int Id, int UserId)
        {
            var cat = await _context.Section.SingleOrDefaultAsync(m => m.Id == Id);
            if (cat == null) return null;

            cat.IsActive = ((int)Shared.ActiveState.InActive).ToString();
            cat.LastModifiedByUserId = UserId;
            cat.LastModifiedDate = DateTime.Now;

            var changes = await _context.SaveChangesAsync();
            if (changes == 0) return null;

            return new ServiceResult
            {
                StatusId = (int)ServiceResultStatus.Deleted,
                RecordId = cat.Id
            };
        }
        public async Task<StudentVM> GetSectionStudent(int SectionId, int ClassId)
        {
            try
            {
                var lstvstudent = await _context.VStudents.AsNoTracking().Where(x => x.ClassId == ClassId && (x.SectionId == SectionId || x.SectionId == null)).ToListAsync();
                var lstvstudentids = lstvstudent.Select(x => x.Id).ToList();
                var lststudent = await _context.Student.AsNoTracking().Where(x => x.IsActive == ((int)Shared.ActiveState.Active).ToString() && lstvstudentids.Contains(x.Id)).ToListAsync();

                if (lststudent.Any())
                {
                    lststudent.ForEach(x => x.IsDelete = ((int)Shared.ActiveState.Active).ToString());
                    var objsec = await _context.VSections.SingleOrDefaultAsync(x => x.Id == SectionId);
                    if (objsec != null && objsec.Id > 0)
                    {
                        foreach (var item in lststudent)
                        {
                            if (item.SectionId == objsec.Id)
                            {
                                item.IsSelected = true;
                                item.IsDelete = objsec.IsDelete;
                            }
                        }
                    }
                }

                return new StudentVM { LstStudent = lststudent, SectionId = SectionId };
            }
            catch
            {
                return null;
            }
        }
        public async Task<ServiceResult> UpdateSectionStudent(StudentVM model)
        {
            try
            {
                var lststudent = await _context.Student.AsNoTracking().Where(x => x.IsActive == ((int)Shared.ActiveState.Active).ToString() && x.SectionId == model.SectionId).ToListAsync();
                if (lststudent.Any())
                {
                    lststudent.ForEach(x => x.SectionId = null);
                    await _context.SaveChangesAsync();
                }

                var selectedStudents = model.LstStudent.Where(x => x.IsSelected).ToList();
                if (selectedStudents.Any())
                {
                    foreach (var item in selectedStudents)
                    {
                        var objStudent = await _context.Student.SingleOrDefaultAsync(x => x.Id == item.Id);
                        if (objStudent != null)
                        {
                            objStudent.SectionId = model.SectionId;
                        }
                    }
                    var changes = await _context.SaveChangesAsync();
                    if (changes != 0)
                    {
                        return new ServiceResult
                        {
                            StatusId = (int)ServiceResultStatus.Updated,
                            RecordId = (int)model.SectionId
                        };
                    }
                }
            }
            catch
            {
                // Handle exception
            }

            return null;
        }

        public async Task<List<SelectListItem>> GetBranches()
        {
            var lst = await _context.Branch.AsNoTracking().Where(x => x.IsActive == ((int)Shared.ActiveState.Active).ToString()).ToListAsync();
            return lst.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
        }
        public async Task<List<SelectListItem>> GetClassesByBranch(string branchId)
        {
            if (string.IsNullOrEmpty(branchId)) return new List<SelectListItem>();

            int clsId = int.Parse(branchId);
            var subids = await _context.BranchClass.AsNoTracking().Where(x => x.BranchId == clsId).Select(x => x.ClassId).ToListAsync();
            var lst = await _context.Class.AsNoTracking().Where(x => x.IsActive == ((int)Shared.ActiveState.Active).ToString() && subids.Contains(x.Id)).ToListAsync();
            return lst.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
        }
        public async Task<List<SelectListItem>> GetSectionsByBranchAndClass(string branchId, string classId)
        {
            if (string.IsNullOrEmpty(branchId) || string.IsNullOrEmpty(classId)) return new List<SelectListItem>();

            int brId = int.Parse(branchId);
            int clsId = int.Parse(classId);
            var objBrCls = await _context.BranchClass.SingleOrDefaultAsync(x => x.BranchId == brId && x.ClassId == clsId);
            if (objBrCls == null) return new List<SelectListItem>();

            var lst = await _context.Section.AsNoTracking().Where(x => x.IsActive == ((int)Shared.ActiveState.Active).ToString() && x.BranchClassId == objBrCls.Id).ToListAsync();
            return lst.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
        }


        public async Task<SectionVM> GetSectionVM(int SectionId, int UserId)
        {
            SectionVM result = new SectionVM();
            try
            {
                result.SectionId = SectionId;
                var objsection = await _context.VSections.AsNoTracking().SingleOrDefaultAsync(x => x.Id == SectionId);
                if (objsection == null) return result;

                var lst = await _context.VPersonnelCurrentLocation.AsNoTracking()
                    .Where(x => x.BranchId == objsection.BranchId && x.PersonnelType == ((int)UserType.Teacher).ToString())
                    .Select(x => new SelectListItem { Text = x.FullName, Value = x.PersonnelId.ToString() })
                    .ToListAsync();
                result.SectionTeacherSheet = lst;

                result.SectionTeacherIds = await _context.BranchClassSectionTeacher.AsNoTracking()
                    .Where(x => x.SectionId == SectionId && x.IsActive == ((int)ActiveState.Active).ToString())
                    .Select(x => x.TeacherId.ToString())
                    .ToListAsync();

                var lstsub = _context.VClassSubjects
                    .Where(x => x.ClassId == objsection.ClassId)
                    .Select(cs => new CastSectionSubjectTeacher
                    {
                        ClassSubjectId = (int)cs.Id,
                        SubjectName = cs.SubjectName,
                        SubjectColor = cs.SubjectColor,
                        SectionSubjectTeacherIds = _context.BranchClassSectionSubjectTeacher
                            .Where(x => x.ClassSubjectId == cs.Id && x.SectionId == SectionId)
                            .Select(x => x.TeacherId.ToString())
                            .ToList(),
                        SectionSubjectTeacherSheet = lst
                    })
                    .ToList();

                result.LstSectionSubjectTeacher = lstsub;
            }
            catch (Exception ex)
            {
                // Log exception
            }
            return result;
        }
        public async Task<List<MbSection>> GetSectionsByPersonnelId(int PersonnelId, string PersonnelType, int BranchId, int SessionYearId)
        {
            List<MbSection> result = new List<MbSection>();
            if (PersonnelId <= 0 || SessionYearId <= 0) return result;

            List<int> clsids = new List<int>();
            if (PersonnelType == ((int)UserType.Teacher).ToString())
            {
                clsids.AddRange(await _context.BranchClassSectionTeacher.AsNoTracking()
                    .Where(x => x.TeacherId == PersonnelId && x.BranchId == BranchId && x.SessionYearId == SessionYearId && x.IsActive == ((int)ActiveState.Active).ToString())
                    .Select(x => x.SectionId)
                    .ToListAsync());

                clsids.AddRange(await _context.BranchClassSectionSubjectTeacher.AsNoTracking()
                    .Where(x => x.TeacherId == PersonnelId && x.BranchId == BranchId && x.SessionYearId == SessionYearId && x.IsActive == ((int)ActiveState.Active).ToString())
                    .Select(x => x.SectionId)
                    .ToListAsync());
            }
            else
            {
                clsids.AddRange(await _context.BranchClassSectionTeacher.AsNoTracking()
                    .Where(x => x.BranchId == BranchId && x.SessionYearId == SessionYearId && x.IsActive == ((int)ActiveState.Active).ToString())
                    .Select(x => x.SectionId)
                    .ToListAsync());
            }

            result = await _context.VSections.AsNoTracking()
                .Where(x => clsids.Contains((int)x.Id))
                .Select(c => new MbSection
                {
                    Id = (int)c.Id,
                    ClassSectionName = c.ClassShortName + " " + c.ShortName
                })
                .ToListAsync();

            return result;
        }
        public async Task<List<MbBranchClass>> GetBranchClassesByPersonnelId(int PersonnelId, string PersonnelType, int SessionYearId)
        {
            List<MbBranchClass> result = new List<MbBranchClass>();
            if (PersonnelId <= 0 || SessionYearId <= 0) return result;

            List<int> clsids = new List<int>();
            if (PersonnelType == ((int)UserType.Teacher).ToString())
            {
                clsids.AddRange(await _context.BranchClassSectionTeacher.AsNoTracking()
                    .Where(x => x.TeacherId == PersonnelId && x.SessionYearId == SessionYearId && x.IsActive == ((int)ActiveState.Active).ToString())
                    .Select(x => x.BranchClassId)
                    .ToListAsync());

                clsids.AddRange(await _context.BranchClassSectionSubjectTeacher.AsNoTracking()
                    .Where(x => x.TeacherId == PersonnelId && x.SessionYearId == SessionYearId && x.IsActive == ((int)ActiveState.Active).ToString())
                    .Select(x => x.BranchClassId)
                    .ToListAsync());
            }
            else
            {
                clsids.AddRange(await _context.BranchClassSectionTeacher.AsNoTracking()
                    .Where(x => x.SessionYearId == SessionYearId && x.IsActive == ((int)ActiveState.Active).ToString())
                    .Select(x => x.BranchClassId)
                    .ToListAsync());
            }

            var lstbranchclass = await _context.BranchClass.AsNoTracking()
                .Where(x => clsids.Contains((int)x.Id))
                .ToListAsync();

            var lstclass = await _context.Class.AsNoTracking()
                .Where(x => lstbranchclass.Select(bc => bc.ClassId).Contains((int)x.Id))
                .ToListAsync();

            result = lstbranchclass
                .Select(c => new MbBranchClass
                {
                    Id = (int)c.Id,
                    ClassName = lstclass.SingleOrDefault(x => x.Id == c.ClassId)?.Name
                })
                .ToList();

            return result;
        }
        public async Task<List<MbSyllabus>> GetSubjectLessonsByClassId(int BranchClassId, int SessionYearId)
        {
            var objbrcls = await _context.BranchClass.AsNoTracking().SingleOrDefaultAsync(x => x.Id == BranchClassId);
            if (objbrcls == null) return new List<MbSyllabus>();

            var lstlesson = _context.VLessons.AsNoTracking()
                .Where(x => x.ClassId == objbrcls.ClassId && x.SessionYearId == SessionYearId)
                .ToList();

            var lstsubject = lstlesson
                .GroupBy(order => new { order.SubjectId, order.SubjectName })
                .Select(group => new { group.Key.SubjectId, group.Key.SubjectName })
                .ToList();

            return lstsubject
                .Select(grp => new MbSyllabus
                {
                    Id = (int)grp.SubjectId,
                    SubjectName = grp.SubjectName,
                    LstLesson = lstlesson.Where(x => x.SubjectId == grp.SubjectId).ToList()
                })
                .ToList();
        }
        public async Task<SectionActivityVM> GetSectionActivityVM(int SectionId, int UserId)
        {
            SectionActivityVM result = new SectionActivityVM();
            try
            {
                result.SectionId = SectionId;
                var objsection = await _context.VSections.AsNoTracking().SingleOrDefaultAsync(x => x.Id == SectionId);
                if (objsection == null) return result;

                var lst = await _context.VPersonnelCurrentLocation.AsNoTracking()
                    .Where(x => x.BranchId == objsection.BranchId && (x.PersonnelType == ((int)UserType.Teacher).ToString() || x.PersonnelType == ((int)UserType.Staff).ToString()))
                    .Select(x => new SelectListItem { Text = x.FullName, Value = x.PersonnelId.ToString() })
                    .ToListAsync();

                var lstsub = await _context.VActivities.AsNoTracking()
                    .Select(cs => new CastSectionActivityPersonnel
                    {
                        ActivityId = (int)cs.Id,
                        ActivityName = cs.ShortName + " (" + cs.Name + ")",
                        ActivityColor = cs.ActivityColor,
                        SectionActivityPersonnelIds = _context.BranchClassSectionActivityPersonnel
                            .Where(x => x.SectionId == SectionId && x.ActivityId == cs.Id)
                            .Select(x => x.PersonnelId.ToString())
                            .ToList(),
                        SectionActivityPersonnelSheet = lst
                    })
                    .ToListAsync();

                result.LstSectionActivityPersonnel = lstsub;
            }
            catch (Exception ex)
            {
                // Log exception
            }
            return result;
        }
        public async Task<ServiceResult> UpdateBranchClassSectionTeacher(SectionVM model, int UserId, int SessionYearId)
        {
            ServiceResult result = null;
            try
            {
                var lstbrclstea = await _context.BranchClassSectionTeacher
                    .Where(x => x.SectionId == model.SectionId && x.SessionYearId == SessionYearId && x.IsActive == ((int)Shared.ActiveState.Active).ToString())
                    .ToListAsync();

                if (lstbrclstea.Any())
                {
                    _context.BranchClassSectionTeacher.RemoveRange(lstbrclstea);
                }

                if (model.SectionTeacherIds.Any())
                {
                    var objsection = await _context.VSections.AsNoTracking().SingleOrDefaultAsync(x => x.Id == model.SectionId);
                    if (objsection != null)
                    {
                        var newTeachers = model.SectionTeacherIds.Select(modelid => new BranchClassSectionTeacher
                        {
                            SessionYearId = SessionYearId,
                            BranchClassId = (int)objsection.BranchClassId,
                            SectionId = (int)objsection.Id,
                            BranchId = (int)objsection.BranchId,
                            ClassId = (int)objsection.ClassId,
                            TeacherId = int.Parse(modelid),
                            IsActive = ((int)ActiveState.Active).ToString(),
                            CreatedByUserId = UserId,
                            CreatedDate = DateTime.Now
                        }).ToList();

                        _context.BranchClassSectionTeacher.AddRange(newTeachers);
                    }
                }

                var secteacherindex = await _context.SaveChangesAsync();
                if (secteacherindex != 0)
                {
                    var lstbrclssecsubtea = await _context.BranchClassSectionSubjectTeacher
                        .Where(x => x.SectionId == model.SectionId && x.SessionYearId == SessionYearId && x.IsActive == ((int)Shared.ActiveState.Active).ToString())
                        .ToListAsync();

                    if (lstbrclssecsubtea.Any())
                    {
                        _context.BranchClassSectionSubjectTeacher.RemoveRange(lstbrclssecsubtea);
                    }

                    if (model.LstSectionSubjectTeacher != null && model.LstSectionSubjectTeacher.Any())
                    {
                        var objsection = await _context.VSections.AsNoTracking().SingleOrDefaultAsync(x => x.Id == model.SectionId);
                        if (objsection != null)
                        {
                            var newSubjectTeachers = model.LstSectionSubjectTeacher
                                .SelectMany(sectionsubjTeacher => sectionsubjTeacher.SectionSubjectTeacherIds.Select(modelsubid => new BranchClassSectionSubjectTeacher
                                {
                                    SessionYearId = SessionYearId,
                                    BranchClassId = (int)objsection.BranchClassId,
                                    SectionId = (int)objsection.Id,
                                    BranchId = (int)objsection.BranchId,
                                    ClassId = (int)objsection.ClassId,
                                    ClassSubjectId = sectionsubjTeacher.ClassSubjectId,
                                    SubjectId = (int)_context.VClassSubjects.SingleOrDefault(x => x.Id == sectionsubjTeacher.ClassSubjectId)?.SubjectId,
                                    TeacherId = int.Parse(modelsubid),
                                    IsActive = ((int)ActiveState.Active).ToString(),
                                    CreatedByUserId = UserId,
                                    CreatedDate = DateTime.Now
                                }))
                                .ToList();

                            _context.BranchClassSectionSubjectTeacher.AddRange(newSubjectTeachers);
                        }
                    }

                    var index = await _context.SaveChangesAsync();
                    if (index != 0 || index == 0)
                    {
                        result = new ServiceResult
                        {
                            StatusId = (int)ServiceResultStatus.Updated,
                            RecordId = (int)model.SectionId
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                // Log exception
            }
            return result;
        }

        public async Task<ServiceResult> UpdateBranchClassSectionActivityPersonnel(SectionActivityVM model, int UserId)
        {
            ServiceResult result = null;
            try
            {
                var lstbrclssecsubtea = await _context.BranchClassSectionActivityPersonnel
                    .Where(x => x.SectionId == model.SectionId && x.IsActive == ((int)Shared.ActiveState.Active).ToString())
                    .ToListAsync();

                if (lstbrclssecsubtea.Any())
                {
                    _context.BranchClassSectionActivityPersonnel.RemoveRange(lstbrclssecsubtea);
                }

                if (model.LstSectionActivityPersonnel != null && model.LstSectionActivityPersonnel.Any())
                {
                    var objsection = await _context.VSections.AsNoTracking().SingleOrDefaultAsync(x => x.Id == model.SectionId);
                    if (objsection != null)
                    {
                        var newActivityPersonnel = model.LstSectionActivityPersonnel
                            .SelectMany(sectionsubjTeacher => sectionsubjTeacher.SectionActivityPersonnelIds.Select(modelsubid => new BranchClassSectionActivityPersonnel
                            {
                                BranchClassId = (int)objsection.BranchClassId,
                                SectionId = (int)objsection.Id,
                                BranchId = (int)objsection.BranchId,
                                ClassId = (int)objsection.ClassId,
                                ActivityId = sectionsubjTeacher.ActivityId,
                                PersonnelId = int.Parse(modelsubid),
                                IsActive = ((int)ActiveState.Active).ToString(),
                                CreatedByUserId = UserId,
                                CreatedDate = DateTime.Now
                            }))
                            .ToList();

                        _context.BranchClassSectionActivityPersonnel.AddRange(newActivityPersonnel);
                    }
                }

                var changes = await _context.SaveChangesAsync();
                if (changes > 0)
                {
                    result = new ServiceResult
                    {
                        StatusId = (int)ServiceResultStatus.Updated,
                        RecordId = (int)model.SectionId
                    };
                }
            }
            catch (Exception ex)
            {
                // Log exception
            }
            return result;
        }
        public async Task<SectionTimeTableVM> GetSectionTimeTableVM(int Id, int UserId)
        {
            var secobj = await _context.VSections.AsNoTracking().SingleOrDefaultAsync(s => s.Id == Id);
            var objsession = await _context.SessionYear.AsNoTracking().SingleOrDefaultAsync(x => x.IsActive == ((int)ActiveState.Active).ToString() && x.IsDefault == true);

            if (secobj == null || objsession == null)
            {
                return null;
            }

            var lstsectionsubteachers = await _context.BranchClassSectionSubjectTeacher.AsNoTracking()
                .Where(x => x.BranchId == secobj.BranchId && x.BranchClassId == secobj.BranchClassId && x.ClassId == secobj.ClassId && x.SectionId == secobj.Id)
                .ToListAsync();

            var lstcastsubjectteacher = lstsectionsubteachers
                .Select((c, index) => new CastSubjectTeacherTimeTable
                {
                    Id = index,
                    ClassSubjectId = c.ClassSubjectId,
                    SubjectId = c.SubjectId,
                    TeacherId = (int)c.TeacherId,
                    SubjectColor =  _context.Subject.AsNoTracking().SingleOrDefault(x => x.Id == c.SubjectId && x.IsActive == ((int)ActiveState.Active).ToString())
                    ?.SubjectColor,
                    SubjectName =  _context.Subject.AsNoTracking().SingleOrDefault(x => x.Id == c.SubjectId && x.IsActive == ((int)ActiveState.Active).ToString())
                    ?.ShortName,
                    TeacherName =  _context.Personnel.AsNoTracking().SingleOrDefault(x => x.Id == c.TeacherId && x.IsActive == ((int)ActiveState.Active).ToString())
                    ?.FirstName + " " + _context.Personnel.SingleOrDefault(x => x.Id == c.TeacherId && x.IsActive == ((int)ActiveState.Active).ToString())?.LastName
                })
                .ToList();

            var lstsectionactivitypers = await _context.BranchClassSectionActivityPersonnel.AsNoTracking()
                .Where(x => x.BranchId == secobj.BranchId && x.BranchClassId == secobj.BranchClassId && x.ClassId == secobj.ClassId && x.SectionId == secobj.Id)
                .ToListAsync();

            var lstcastactivityperson = lstsectionactivitypers
                .Select((c, index) => new CastActivityPersonnelTimeTable
                {
                    Id = index,
                    ActivityId = c.ActivityId,
                    PersonnelId = (int)c.PersonnelId,
                    ActivityColor = _context.Activity.AsNoTracking().SingleOrDefault(x => x.Id == c.ActivityId && x.IsActive == ((int)ActiveState.Active).ToString())
                    ?.ActivityColor,
                    ActivityName = _context.Activity.AsNoTracking().SingleOrDefault(x => x.Id == c.ActivityId && x.IsActive == ((int)ActiveState.Active).ToString())
                    ?.ShortName,
                    PersonnelName = _context.Personnel.AsNoTracking().SingleOrDefault(x => x.Id == c.PersonnelId && x.IsActive == ((int)ActiveState.Active).ToString())
                    ?.FirstName + " " + _context.Personnel.SingleOrDefault(x => x.Id == c.PersonnelId && x.IsActive == ((int)ActiveState.Active).ToString())?.LastName
                })
                .ToList();

            var sectionDurBreak = GetClassDurationAndBreaks(Id);

            return new SectionTimeTableVM
            {
                SessionYearId = objsession.Id,
                BranchId = (int)secobj.BranchId,
                BranchClassId = (int)secobj.BranchClassId,
                ClassId = (int)secobj.ClassId,
                SectionId = (int)secobj.Id,
                SectionName = secobj.Name,
                IsEdit = false,
                LstCastSubjectTeacherTimeTable = lstcastsubjectteacher,
                LstCastActivityPersonnelTimeTable = lstcastactivityperson,
                timetable = GenerateTimetable(sectionDurBreak.Result.Duration, sectionDurBreak.Result.LstBreak, sectionDurBreak.Result.ClassStartTime, sectionDurBreak.Result.ClassEndTime)
            };
        }
        public async Task<SectionTransTimeTableVM> GetSectionTransTimeTableVM(int Id, int SessionYearId)
        {
            var timetablesection = await _context.TimeTable.AsNoTracking().FirstOrDefaultAsync(x => x.SectionId == Id && x.SessionYearId == SessionYearId && x.IsActive == ((int)ActiveState.Active).ToString());
            if (timetablesection == null)
            {
                return new SectionTransTimeTableVM { CastTransTimetable = new CastTransTimetable() };
            }

            var sectionDurBreak = await GetClassDurationAndBreaks(Id);

            return new SectionTransTimeTableVM
            {
                SectionId = Id,
                SessionYearId = SessionYearId,
                CastTransTimetable = GenerateTransTimetable(sectionDurBreak.Duration, sectionDurBreak.LstBreak, sectionDurBreak.ClassStartTime, sectionDurBreak.ClassEndTime)
            };
        }
        public async Task<List<VTimeTable>> GetTimeTable(int SectionId, int SessionYearId, int UserId)
        {
            return await _context.VTimeTable.AsNoTracking().Where(x => x.SectionId == SectionId && x.SessionYearId == SessionYearId).ToListAsync();
        }
        public async Task<ClassDurationAndBreaks> GetClassDurationAndBreaks(int sectionId)
        {
            var objsection = await _context.Section.AsNoTracking().SingleOrDefaultAsync(x => x.Id == sectionId && x.IsActive == ((int)ActiveState.Active).ToString());
            if (objsection == null)
            {
                return new ClassDurationAndBreaks();
            }

            var objbrcls = await _context.BranchClass.AsNoTracking().SingleOrDefaultAsync(x => x.Id == objsection.BranchClassId);
            var objshift = await _context.Shift.AsNoTracking().SingleOrDefaultAsync(x => x.Id == objbrcls.ShiftId && x.IsActive == ((int)ActiveState.Active).ToString());

            var lstperiodbreakids = await _context.BranchClassPeriodBreak.AsNoTracking()
                .Where(x => x.BranchClassId == objsection.BranchClassId)
                .Select(x => x.PeriodBreakId)
                .ToListAsync();

            var lstbrk = await _context.PeriodBreak.AsNoTracking()
                .Where(x => lstperiodbreakids.Contains(x.Id) && x.IsActive == ((int)ActiveState.Active).ToString())
                .Select(c => new Break
                {
                    StartTime = GetTimeSpan(c.FromTime),
                    EndTime = GetTimeSpan(c.ToTime),
                    Name = c.ShortName
                })
                .ToListAsync();

            return new ClassDurationAndBreaks
            {
                ClassStartTime = GetTimeSpan(objshift.FromTime),
                ClassEndTime = GetTimeSpan(objshift.ToTime),
                Duration = int.Parse(objbrcls.SlotDuration),
                LstBreak = lstbrk
            };
        }
        public DateTime convertTo24HrFormat(string str12hour)
        {
            return DateTime.ParseExact(str12hour, "hh:mm tt", CultureInfo.InvariantCulture);
        }
        public async Task<ServiceResult> UpdateBranchClassSectionTimeTable(TimeTableDTO model, int UserId)
        {
            ServiceResult result = null;
            try
            {
                var lstbrclstea = await _context.TimeTable
                    .Where(x => x.SectionId == model.SectionId && x.IsActive == ((int)Shared.ActiveState.Active).ToString())
                    .ToListAsync();

                if (lstbrclstea.Any())
                {
                    lstbrclstea.ForEach(sectea =>
                    {
                        sectea.IsActive = ((int)Shared.ActiveState.InActive).ToString();
                        sectea.LastModifiedByUserId = UserId;
                        sectea.LastModifiedDate = DateTime.Now;
                    });
                }

                if (model.LstTimeTable.Any())
                {
                    var newTimeTables = model.LstTimeTable.Select(secitem => new TimeTable
                    {
                        BranchId = secitem.BranchId,
                        SessionYearId = secitem.SessionYearId,
                        BranchClassId = secitem.BranchClassId,
                        ClassId = secitem.ClassId,
                        SectionId = secitem.SectionId,
                        ClassSubjectId = secitem.ClassSubjectId,
                        SubjectActivityId = secitem.SubjectActivityId,
                        TimeTableType = secitem.TimeTableType,
                        PersonnelId = secitem.PersonnelId,
                        DayOfWeek = secitem.DayOfWeek,
                        FromTime = secitem.FromTime,
                        ToTime = secitem.ToTime,
                        IsActive = ((int)ActiveState.Active).ToString(),
                        CreatedByUserId = UserId,
                        CreatedDate = DateTime.Now
                    }).ToList();

                    _context.TimeTable.AddRange(newTimeTables);
                }

                var changes = await _context.SaveChangesAsync();
                if (changes > 0)
                {
                    result = new ServiceResult
                    {
                        StatusId = (int)ServiceResultStatus.Updated,
                        RecordId = (int)model.SectionId
                    };
                }
            }
            catch (Exception ex)
            {
                // Log exception
            }
            return result;
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
