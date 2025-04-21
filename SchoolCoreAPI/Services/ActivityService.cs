using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class ActivityService : IActivityService
    {
        private readonly APIContext _context;
        public ActivityService(APIContext context)
        {
            _context = context;
        }

        public async Task<List<VActivities>> GetAllActivity()
        {
            return await _context.VActivities.AsNoTracking().OrderByDescending(x => x.Id).ToListAsync();
        }

        public async Task<VActivities> ViewActivity(int Id)
        {
            return await _context.VActivities.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
        }

        public async Task<bool> IsRecordExists(string name, int Id)
        {
            return Id == 0
                ? await _context.Activity.AnyAsync(e => e.Name == name && e.IsActive == ((int)ActiveState.Active).ToString())
                : await _context.Activity.AnyAsync(e => e.Name == name && e.Id != Id && e.IsActive == ((int)ActiveState.Active).ToString());
        }

        public async Task<Activity> GetActivity(int Id)
        {
            if (Id <= 0) return new Activity();

            var cat = await _context.Activity.AsNoTracking().SingleOrDefaultAsync(x => x.Id == Id && x.IsActive == ((int)ActiveState.Active).ToString());
            if (cat == null) return new Activity();

            return new Activity
            {
                Id = cat.Id,
                Name = cat.Name,
                ShortName = cat.ShortName,
                Description = cat.Description,
                ActivityColor = cat.ActivityColor
            };
        }

        public async Task<ServiceResult> SaveActivity(Activity model, int UserId)
        {
            var cat = new Activity();
            var result = new ServiceResult();
            if (model.Id > 0)
            {
                cat = await _context.Activity.SingleOrDefaultAsync(m => m.Id == model.Id && m.IsActive == ((int)ActiveState.Active).ToString());
                if (cat == null) return null;

                cat.Name = model.Name;
                cat.ShortName = model.ShortName;
                cat.ActivityColor = model.ActivityColor;
                cat.Description = model.Description;
                cat.LastModifiedDate = DateTime.Now;
                cat.LastModifiedByUserId = UserId;
                _context.Entry(cat).State = EntityState.Modified;
            }
            else
            {
                cat = new Activity
                {
                    Name = model.Name,
                    ShortName = model.ShortName,
                    ActivityColor = model.ActivityColor,
                    Description = model.Description,
                    IsActive = ((int)ActiveState.Active).ToString(),
                    CreatedDate = DateTime.Now,
                    CreatedByUserId = UserId,
                    LastModifiedDate = DateTime.Now,
                    LastModifiedByUserId = UserId
                };
                _context.Activity.Add(cat);
            }

            var changes = await _context.SaveChangesAsync();
            if (changes == 0) return null;

            result.StatusId = model.Id > 0 ? (int)ServiceResultStatus.Updated : (int)ServiceResultStatus.Added;
            result.RecordId = cat.Id;
            return result;
        }

        public async Task<ServiceResult> DeleteActivity(int Id, int UserId)
        {
            var cat = await _context.Activity.SingleOrDefaultAsync(m => m.Id == Id);
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
