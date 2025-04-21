using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class HolidayService : IHolidayService
    {
        private readonly APIContext _context;
        public HolidayService(APIContext context)
        {
            _context = context;
        }

        public async Task<List<VHolidays>> GetAllHoliday()
        {
            return await _context.VHolidays.AsNoTracking().OrderBy(x => x.Id).ToListAsync();
        }

        public async Task<VHolidays> ViewHoliday(int Id)
        {
            return await _context.VHolidays.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
        }

        public async Task<Holiday> GetHoliday(int Id)
        {
            if (Id <= 0) return new Holiday();

            var cat = await _context.Holiday.AsNoTracking().SingleOrDefaultAsync(x => x.Id == Id && x.IsActive == ((int)ActiveState.Active).ToString());
            if (cat == null) return new Holiday();

            return new Holiday
            {
                Id = cat.Id,
                Title = cat.Title,
                Description = cat.Description,
                StartDate = cat.StartDate,
                EndDate = cat.EndDate
            };
        }

        public async Task<bool> IsRecordExists(string name, int Id)
        {
            return Id == 0
                ? await _context.Holiday.AnyAsync(e => e.Title == name && e.IsActive == ((int)ActiveState.Active).ToString())
                : await _context.Holiday.AnyAsync(e => e.Title == name && e.Id != Id && e.IsActive == ((int)ActiveState.Active).ToString());
        }

        public async Task<ServiceResult> SaveHoliday(Holiday model, int UserId)
        {
            var cat = new Holiday();
            var result = new ServiceResult();
            if (model.Id > 0)
            {
                cat = await _context.Holiday.AsNoTracking().SingleOrDefaultAsync(m => m.Id == model.Id && m.IsActive == ((int)ActiveState.Active).ToString());
                if (cat == null) return null;

                cat.Title = model.Title;
                cat.Description = model.Description;
                cat.StartDate = model.StartDate;
                cat.EndDate = model.EndDate;
                cat.LastModifiedDate = DateTime.Now;
                cat.LastModifiedByUserId = UserId;
                _context.Entry(cat).State = EntityState.Modified;
            }
            else
            {
                cat = new Holiday
                {
                    Title = model.Title,
                    Description = model.Description,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    IsActive = ((int)ActiveState.Active).ToString(),
                    CreatedDate = DateTime.Now,
                    CreatedByUserId = UserId,
                    LastModifiedDate = DateTime.Now,
                    LastModifiedByUserId = UserId
                };
                _context.Holiday.Add(cat);
            }

            var changes = await _context.SaveChangesAsync();
            if (changes > 0)
            {
                result.StatusId = model.Id > 0 ? (int)ServiceResultStatus.Updated : (int)ServiceResultStatus.Added;
                result.RecordId = cat.Id;
            }

            return result;
        }

        public async Task<ServiceResult> DeleteHoliday(int Id, int UserId)
        {
            var cat = await _context.Holiday.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
            if (cat == null) return null;

            cat.IsActive = ((int)ActiveState.InActive).ToString();
            cat.LastModifiedByUserId = UserId;
            cat.LastModifiedDate = DateTime.Now;
            _context.Entry(cat).State = EntityState.Modified;

            var changes = await _context.SaveChangesAsync();
            if (changes > 0)
            {
                return new ServiceResult
                {
                    StatusId = (int)ServiceResultStatus.Deleted,
                    RecordId = cat.Id
                };
            }

            return null;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
