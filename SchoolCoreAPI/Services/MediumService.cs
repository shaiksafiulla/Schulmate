using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class MediumService : IMediumService
    {
        private readonly APIContext _context;
        public MediumService(APIContext context)
        {
            _context = context;
        }

        public async Task<List<VMediums>> GetAllMedium()
        {
            return await _context.VMediums.AsNoTracking().OrderBy(x => x.Id).ToListAsync();
        }

        public async Task<VMediums> ViewMedium(int Id)
        {
            return await _context.VMediums.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
        }

        public async Task<Medium> GetMedium(int Id)
        {
            if (Id <= 0) return new Medium();

            var cat = await _context.Medium.AsNoTracking().SingleOrDefaultAsync(x => x.Id == Id && x.IsActive == ((int)ActiveState.Active).ToString());
            if (cat == null) return new Medium();

            return new Medium
            {
                Id = cat.Id,
                Name = cat.Name,
                Description = cat.Description
            };
        }

        public async Task<bool> IsRecordExists(string name, int Id)
        {
            return Id == 0
                ? await _context.Medium.AsNoTracking().AnyAsync(e => e.Name == name && e.IsActive == ((int)ActiveState.Active).ToString())
                : await _context.Medium.AsNoTracking().AnyAsync(e => e.Name == name && e.Id != Id && e.IsActive == ((int)ActiveState.Active).ToString());
        }

        public async Task<ServiceResult> SaveMedium(Medium model, int UserId)
        {
            var cat = new Medium();
            var result = new ServiceResult();
            if (model.Id > 0)
            {
                cat = await _context.Medium.AsNoTracking().SingleOrDefaultAsync(m => m.Id == model.Id && m.IsActive == ((int)ActiveState.Active).ToString());
                if (cat == null) return null;

                cat.Name = model.Name;
                cat.Description = model.Description;
                cat.LastModifiedDate = DateTime.Now;
                cat.LastModifiedByUserId = UserId;
                _context.Entry(cat).State = EntityState.Modified;
            }
            else
            {
                cat = new Medium
                {
                    Name = model.Name,
                    Description = model.Description,
                    IsActive = ((int)ActiveState.Active).ToString(),
                    CreatedDate = DateTime.Now,
                    CreatedByUserId = UserId,
                    LastModifiedDate = DateTime.Now,
                    LastModifiedByUserId = UserId
                };
                _context.Medium.Add(cat);
            }

            var changes = await _context.SaveChangesAsync();
            if (changes == 0) return null;

            result.StatusId = model.Id > 0 ? (int)ServiceResultStatus.Updated : (int)ServiceResultStatus.Added;
            result.RecordId = cat.Id;
            return result;
        }

        public async Task<ServiceResult> DeleteMedium(int Id, int UserId)
        {
            var cat = await _context.Medium.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
            if (cat == null) return null;

            cat.IsActive = ((int)ActiveState.InActive).ToString();
            cat.LastModifiedByUserId = UserId;
            cat.LastModifiedDate = DateTime.Now;
            _context.Entry(cat).State = EntityState.Modified;

            var changes = await _context.SaveChangesAsync();
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
