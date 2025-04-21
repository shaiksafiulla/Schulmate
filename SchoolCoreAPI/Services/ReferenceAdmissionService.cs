using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class ReferenceAdmissionService : IReferenceAdmissionService
    {
        private readonly APIContext _context;
        public ReferenceAdmissionService(APIContext context)
        {
            _context = context;
        }

        public List<VReferenceAdmission> GetAllReferenceAdmission()
        {
            return _context.VReferenceAdmission.AsNoTracking().OrderBy(x => x.Id).ToList();
        }

        public VReferenceAdmission ViewReferenceAdmission(int Id)
        {
            return _context.VReferenceAdmission.AsNoTracking().SingleOrDefault(m => m.Id == Id);
        }

        public ReferenceAdmission GetReferenceAdmission(int Id)
        {
            if (Id <= 0) return new ReferenceAdmission();

            var cat = _context.ReferenceAdmission.AsNoTracking()
                .SingleOrDefault(x => x.Id == Id && x.IsActive == ((int)ActiveState.Active).ToString());

            if (cat == null) return new ReferenceAdmission();

            return new ReferenceAdmission
            {
                Id = cat.Id,
                Name = cat.Name,
                Description = cat.Description
            };
        }

        public bool IsRecordExists(string name, int Id)
        {
            return Id == 0
                ? _context.ReferenceAdmission.AsNoTracking().Any(e => e.Name == name && e.IsActive == ((int)ActiveState.Active).ToString())
                : _context.ReferenceAdmission.AsNoTracking().Any(e => e.Name == name && e.Id != Id && e.IsActive == ((int)ActiveState.Active).ToString());
        }

        public ServiceResult SaveReferenceAdmission(ReferenceAdmission model, int UserId)
        {
            var cat = new ReferenceAdmission();
            var result = new ServiceResult();
            var now = DateTime.Now;

            if (model.Id > 0)
            {
                cat = _context.ReferenceAdmission
                    .SingleOrDefault(m => m.Id == model.Id && m.IsActive == ((int)ActiveState.Active).ToString());

                if (cat == null) return null;

                cat.Name = model.Name;
                cat.Description = model.Description;
                cat.LastModifiedDate = now;
                cat.LastModifiedByUserId = UserId;

                _context.Entry(cat).State = EntityState.Modified;
            }
            else
            {
                cat = new ReferenceAdmission
                {
                    Name = model.Name,
                    Description = model.Description,
                    IsActive = ((int)ActiveState.Active).ToString(),
                    CreatedDate = now,
                    CreatedByUserId = UserId,
                    LastModifiedDate = now,
                    LastModifiedByUserId = UserId
                };

                _context.ReferenceAdmission.Add(cat);
            }

            var changes = _context.SaveChanges();
            if (changes > 0)
            {
                result.StatusId = model.Id > 0 ? (int)ServiceResultStatus.Updated : (int)ServiceResultStatus.Added;
                result.RecordId = cat.Id;
            }

            return result;
        }

        public ServiceResult DeleteReferenceAdmission(int Id, int UserId)
        {
            var cat = _context.ReferenceAdmission.SingleOrDefault(m => m.Id == Id);
            if (cat == null) return null;

            cat.IsActive = ((int)ActiveState.InActive).ToString();
            cat.LastModifiedByUserId = UserId;
            cat.LastModifiedDate = DateTime.Now;

            _context.Entry(cat).State = EntityState.Modified;
            var changes = _context.SaveChanges();

            if (changes == 0) return null;

            return new ServiceResult
            {
                StatusId = (int)ServiceResultStatus.Deleted,
                RecordId = cat.Id
            };
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
