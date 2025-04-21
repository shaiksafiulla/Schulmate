using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class StudentEnquiryService : IStudentEnquiryService
    {
        private readonly APIContext _context;
        public StudentEnquiryService(APIContext context)
        {
            _context = context;
        }

        public async Task<List<VStudentEnquiry>> GetAllStudentEnquiry(int UserId)
        {
            var objuser = await _context.VUserInfo.AsNoTracking().SingleOrDefaultAsync(x => x.Id == UserId);
            if (objuser.UserType == ((int)UserType.SuperAdmin).ToString())
            {
                return await  _context.VStudentEnquiry.AsNoTracking().ToListAsync();
            }
            else
            {
                return  await _context.VStudentEnquiry.AsNoTracking().Where(x => x.BranchId == objuser.BranchId).ToListAsync();
            }
        }

        public async Task<List<SelectListItem>> GetClassByBranch(int branchId)
        {
            var lstbrclsids = await _context.BranchClass.AsNoTracking().Where(x => x.BranchId == branchId).Select(x => x.ClassId).ToListAsync();
            if (lstbrclsids.Any())
            {
                var lstcls = await _context.Class.AsNoTracking().Where(x => x.IsActive == ((int)ActiveState.Active).ToString() && lstbrclsids.Contains(x.Id)).ToListAsync();
                return lstcls.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
            }
            return new List<SelectListItem>();
        }

        public async Task<VStudentEnquiry> ViewStudentEnquiry(int Id)
        {
            return  await _context.VStudentEnquiry.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
        }

        public async Task<StudentEnquiry> GetStudentEnquiry(int Id, int UserId)
        {
            var model = new StudentEnquiry
            {
                ClassSheet = new List<SelectListItem>(),
                StatusSheet = await GetEnquiryStatus(),
                ReferenceSheet = await GetReference()
            };

            if (Id > 0)
            {
                var cat = await _context.StudentEnquiry.AsNoTracking().SingleOrDefaultAsync(x => x.Id == Id && x.IsActive == ((int)ActiveState.Active).ToString());
                if (cat != null)
                {
                    model = MapStudentEnquiry(cat, model);
                    if (cat.BranchId > 0)
                    {
                        model.BranchId = cat.BranchId;
                        model.ClassSheet = await GetClassByBranch(model.BranchId);
                        if (cat.ClassId > 0)
                        {
                            model.ClassId = cat.ClassId;
                            SetSelectedItem(model.ClassSheet, cat.ClassId);
                        }
                    }
                    if (cat.StatusId > 0)
                    {
                        model.StatusId = cat.StatusId;
                        SetSelectedItem(model.StatusSheet, cat.StatusId);
                    }
                    if (cat.ReferenceId > 0)
                    {
                        model.ReferenceId = cat.ReferenceId;
                        SetSelectedItem(model.ReferenceSheet, (int)cat.ReferenceId);
                    }
                }
            }
            else
            {
                model.Gender = "1";
                var defaultsession = await _context.SessionYear.AsNoTracking().SingleOrDefaultAsync(x => x.IsActive == ((int)ActiveState.Active).ToString() && x.IsDefault == true);
                model.SessionYearId = defaultsession.Id;

                var objuser = await _context.VUserInfo.AsNoTracking().SingleOrDefaultAsync(x => x.Id == UserId);
                model.BranchId = (int)objuser.BranchId;

                model.ClassSheet = await GetClassByBranch(model.BranchId);
            }
            return model;
        }

        public async Task<bool> IsRecordExists(string name, int Id)
        {
            return Id == 0
                ? await _context.StudentEnquiry.AsNoTracking().AnyAsync(e => e.Name == name && e.IsActive == ((int)ActiveState.Active).ToString())
                : await _context.StudentEnquiry.AsNoTracking().AnyAsync(e => e.Name == name && e.Id != Id && e.IsActive == ((int)ActiveState.Active).ToString());
        }

        public async Task<ServiceResult> SaveStudentEnquiry(StudentEnquiry model, int UserId)
        {
            ServiceResult result = null;
            if (model.Id > 0)
            {
                var cat = await _context.StudentEnquiry.SingleOrDefaultAsync(m => m.Id == model.Id && m.IsActive == ((int)ActiveState.Active).ToString());
                if (cat != null)
                {
                    UpdateStudentEnquiry(cat, model, UserId);
                    _context.Entry(cat).State = EntityState.Modified;
                    if (await _context.SaveChangesAsync() != 0)
                    {
                        result = new ServiceResult { StatusId = (int)ServiceResultStatus.Updated, RecordId = cat.Id };
                    }
                }
            }
            else
            {
                var cat = new StudentEnquiry();
                UpdateStudentEnquiry(cat, model, UserId);
                cat.IsActive = ((int)ActiveState.Active).ToString();
                cat.CreatedDate = DateTime.Now;
                cat.CreatedByUserId = UserId;
                _context.StudentEnquiry.Add(cat);
                _context.Entry(cat).State = EntityState.Added;
                if (await _context.SaveChangesAsync() != 0)
                {
                    result = new ServiceResult { StatusId = (int)ServiceResultStatus.Added, RecordId = cat.Id };
                }
            }
            return result;
        }

        public async Task<ServiceResult> DeleteStudentEnquiry(int Id, int UserId)
        {
            var cat = await _context.StudentEnquiry.SingleOrDefaultAsync(m => m.Id == Id);
            if (cat != null)
            {
                cat.IsActive = ((int)ActiveState.InActive).ToString();
                cat.LastModifiedByUserId = UserId;
                cat.LastModifiedDate = DateTime.Now;
                _context.Entry(cat).State = EntityState.Modified;
                if (await _context.SaveChangesAsync() != 0)
                {
                    return new ServiceResult { StatusId = (int)ServiceResultStatus.Deleted, RecordId = cat.Id };
                }
            }
            return null;
        }

        public async Task<List<SelectListItem>> GetBranches()
        {
            var lst = await _context.Branch.AsNoTracking().Where(x => x.IsActive == ((int)ActiveState.Active).ToString()).ToListAsync();
            return lst.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
        }

        public async Task<List<SelectListItem>> GetEnquiryStatus()
        {
            var lst = await _context.EnquiryStatus.AsNoTracking().Where(x => x.IsActive == ((int)ActiveState.Active).ToString()).ToListAsync();
            return lst.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
        }

        public async Task<List<SelectListItem>> GetReference()
        {
            var lst = await _context.ReferenceAdmission.AsNoTracking().Where(x => x.IsActive == ((int)ActiveState.Active).ToString()).ToListAsync();
            return lst.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        private void SetSelectedItem(List<SelectListItem> items, int id)
        {
            var item = items.Find(p => p.Value == id.ToString());
            if (item != null)
            {
                item.Selected = true;
            }
        }

        private StudentEnquiry MapStudentEnquiry(StudentEnquiry source, StudentEnquiry target)
        {
            target.Id = source.Id;
            target.BranchId = source.BranchId;
            target.ClassId = source.ClassId;
            target.SessionYearId = source.SessionYearId;
            target.StatusId = source.StatusId;
            target.ReferenceId = source.ReferenceId;
            target.Name = source.Name;
            target.FatherName = source.FatherName;
            target.MotherName = source.MotherName;
            target.MobileNo = source.MobileNo;
            target.PreviousSchool = source.PreviousSchool;
            target.Age = source.Age;
            target.Gender = source.Gender;
            target.Address = source.Address;
            target.Comments = source.Comments;
            return target;
        }

        private void UpdateStudentEnquiry(StudentEnquiry target, StudentEnquiry source, int userId)
        {
            target.BranchId = source.BranchId;
            target.ClassId = source.ClassId;
            target.SessionYearId = source.SessionYearId;
            target.StatusId = source.StatusId;
            target.ReferenceId = source.ReferenceId;
            target.Name = source.Name;
            target.FatherName = source.FatherName;
            target.MotherName = source.MotherName;
            target.MobileNo = source.MobileNo;
            target.PreviousSchool = source.PreviousSchool;
            target.Age = source.Age;
            target.Gender = source.Gender;
            target.Address = source.Address;
            target.Comments = source.Comments;
            target.LastModifiedDate = DateTime.Now;
            target.LastModifiedByUserId = userId;
        }
    }
}
