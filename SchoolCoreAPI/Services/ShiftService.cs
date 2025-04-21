using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class ShiftService : IShiftService
    {
        private readonly APIContext _context;
        public ShiftService(APIContext context)
        {
            _context = context;
        }

        public async Task<List<VShifts>> GetAllShift()
        {
            return await _context.VShifts.AsNoTracking().OrderBy(x => x.Id).ToListAsync();
        }

        public async Task<Shift> ViewShift(int Id)
        {
            return await _context.Shift.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
        }

        public async Task<Shift> GetShift(int Id)
        {
            if (Id <= 0) return new Shift();

            var cat = await _context.Shift.AsNoTracking().SingleOrDefaultAsync(x => x.Id == Id && x.IsActive == ((int)ActiveState.Active).ToString());
            if (cat == null) return new Shift();

            return new Shift
            {
                Id = cat.Id,
                FromTime = cat.FromTime,
                ToTime = cat.ToTime,
                Description = cat.Description
            };
        }

        public async Task<bool> IsRecordExists(string fromtime, string totime, int Id)
        {
            var strFrom = $"{fromtime.Substring(0, 2)}:{fromtime.Substring(fromtime.Length - 2)}";
            var strTo = $"{totime.Substring(0, 2)}:{totime.Substring(totime.Length - 2)}";

            return Id == 0
                ? await _context.Shift.AsNoTracking().AnyAsync(e => e.FromTime == strFrom && e.ToTime == strTo && e.IsActive == ((int)ActiveState.Active).ToString())
                : await _context.Shift.AsNoTracking().AnyAsync(e => e.FromTime == strFrom && e.ToTime == strTo && e.Id != Id && e.IsActive == ((int)ActiveState.Active).ToString());
        }

        public async Task<ServiceResult> SaveShift(Shift model, int UserId)
        {
            var cat = new Shift();
            var result = new ServiceResult();
            var now = DateTime.Now;

            if (model.Id > 0)
            {
                cat = await _context.Shift.AsNoTracking().SingleOrDefaultAsync(m => m.Id == model.Id && m.IsActive == ((int)Shared.ActiveState.Active).ToString());
                if (cat == null) return null;

                cat.FromTime = model.FromTime;
                cat.ToTime = model.ToTime;
                cat.Description = model.Description;
                cat.LastModifiedDate = now;
                cat.LastModifiedByUserId = UserId;
                _context.Entry(cat).State = EntityState.Modified;
            }
            else
            {
                cat = new Shift
                {
                    FromTime = model.FromTime,
                    ToTime = model.ToTime,
                    Description = model.Description,
                    IsActive = ((int)Shared.ActiveState.Active).ToString(),
                    CreatedDate = now,
                    CreatedByUserId = UserId,
                    LastModifiedDate = now,
                    LastModifiedByUserId = UserId
                };
                _context.Shift.Add(cat);
                _context.Entry(cat).State = EntityState.Added;
            }

            var changes = await _context.SaveChangesAsync();
            if (changes == 0) return null;

            result.StatusId = model.Id > 0 ? (int)ServiceResultStatus.Updated : (int)ServiceResultStatus.Added;
            result.RecordId = cat.Id;
            return result;
        }

        public async Task<ServiceResult> DeleteShift(int Id, int UserId)
        {
            var cat = await _context.Shift.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
            if (cat == null) return null;

            cat.IsActive = ((int)Shared.ActiveState.InActive).ToString();
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
