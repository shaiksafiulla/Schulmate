using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class BloodGroupService : IBloodGroupService
    {
        private readonly APIContext _context;
        public BloodGroupService(APIContext context)
        {
            _context = context;
        }

        public async Task<List<VBloodGroups>> GetAllBloodGroup()
        {
            return await _context.VBloodGroups.AsNoTracking().OrderBy(x => x.Id).ToListAsync();
        }

        public async Task<VBloodGroups> ViewBloodGroup(int Id)
        {
            return await _context.VBloodGroups.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
        }

        public async Task<BloodGroup> GetBloodGroup(int Id)
        {
            if (Id <= 0) return new BloodGroup();

            var cat = await _context.BloodGroup.AsNoTracking().SingleOrDefaultAsync(x => x.Id == Id && x.IsActive == ((int)ActiveState.Active).ToString());
            if (cat == null) return new BloodGroup();

            return new BloodGroup
            {
                Id = cat.Id,
                Name = cat.Name,
                Description = cat.Description
            };
        }

        public async Task<bool> IsRecordExists(string name, int Id)
        {
            return Id == 0
                ? await _context.BloodGroup.AnyAsync(e => e.Name == name && e.IsActive == ((int)ActiveState.Active).ToString())
                : await _context.BloodGroup.AnyAsync(e => e.Name == name && e.Id != Id && e.IsActive == ((int)ActiveState.Active).ToString());
        }

        public async Task<ServiceResult> SaveBloodGroup(BloodGroup model, int UserId)
        {
            var cat = new BloodGroup();
            var result = new ServiceResult();
            var now = DateTime.Now;

            if (model.Id > 0)
            {
                cat = await _context.BloodGroup.SingleOrDefaultAsync(m => m.Id == model.Id && m.IsActive == ((int)ActiveState.Active).ToString());
                if (cat == null) return result;

                cat.Name = model.Name;
                cat.Description = model.Description;
                cat.LastModifiedDate = now;
                cat.LastModifiedByUserId = UserId;
                _context.Entry(cat).State = EntityState.Modified;
            }
            else
            {
                cat = new BloodGroup
                {
                    Name = model.Name,
                    Description = model.Description,
                    IsActive = ((int)ActiveState.Active).ToString(),
                    CreatedDate = now,
                    CreatedByUserId = UserId,
                    LastModifiedDate = now,
                    LastModifiedByUserId = UserId
                };
                _context.BloodGroup.Add(cat);
            }

            if (await _context.SaveChangesAsync() > 0)
            {
                result.StatusId = model.Id > 0 ? (int)ServiceResultStatus.Updated : (int)ServiceResultStatus.Added;
                result.RecordId = cat.Id;
            }

            return result;
        }

        public async Task<ServiceResult> DeleteBloodGroup(int Id, int UserId)
        {
            var result = new ServiceResult();
            var cat = await _context.BloodGroup.SingleOrDefaultAsync(m => m.Id == Id);
            if (cat == null) return result;

            cat.IsActive = ((int)ActiveState.InActive).ToString();
            cat.LastModifiedByUserId = UserId;
            cat.LastModifiedDate = DateTime.Now;
            _context.Entry(cat).State = EntityState.Modified;

            if (await _context.SaveChangesAsync() > 0)
            {
                result.StatusId = (int)ServiceResultStatus.Deleted;
                result.RecordId = cat.Id;
            }

            return result;
        }

        public async Task Dispose()
        {
            await _context.DisposeAsync();
        }
    }
}
