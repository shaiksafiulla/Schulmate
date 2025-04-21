using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class PeriodBreakService : IPeriodBreakService
    {
        private readonly APIContext _context;
        public PeriodBreakService(APIContext context)
        {
            _context = context;
        }

        public async Task<List<VPeriodBreaks>> GetAllPeriodBreak()
        {
            return await _context.VPeriodBreaks.AsNoTracking().OrderByDescending(x => x.Id).ToListAsync();
        }

        public async Task<VPeriodBreaks> ViewPeriodBreak(int Id)
        {
            return  await _context.VPeriodBreaks.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
        }

        public async Task<bool> IsRecordExists(string name, int Id)
        {
            return Id == 0
                ? await _context.PeriodBreak.AnyAsync(e => e.Name == name && e.IsActive == ((int)ActiveState.Active).ToString())
                : await _context.PeriodBreak.AnyAsync(e => e.Name == name && e.Id != Id && e.IsActive == ((int)ActiveState.Active).ToString());
        }

        public async Task<PeriodBreak> GetPeriodBreak(int Id)
        {
            if (Id <= 0) return new PeriodBreak();

            var cat = await _context.PeriodBreak.AsNoTracking().SingleOrDefaultAsync(x => x.Id == Id && x.IsActive == ((int)ActiveState.Active).ToString());
            if (cat == null) return new PeriodBreak();

            return new PeriodBreak
            {
                Id = cat.Id,
                Name = cat.Name,
                ShortName = cat.ShortName,
                FromTime = cat.FromTime,
                ToTime = cat.ToTime,
                Description = cat.Description,
                BreakColor = cat.BreakColor
            };
        }

        public async Task<ServiceResult> SavePeriodBreak(PeriodBreak model, int UserId)
        {
            var cat = new PeriodBreak();
            var result = new ServiceResult();
            if (model.Id > 0)
            {
                cat = await _context.PeriodBreak.SingleOrDefaultAsync(m => m.Id == model.Id && m.IsActive == ((int)ActiveState.Active).ToString());
                if (cat == null) return null;

                cat.Name = model.Name;
                cat.ShortName = model.ShortName;
                cat.BreakColor = model.BreakColor;
                cat.FromTime = model.FromTime;
                cat.ToTime = model.ToTime;
                cat.Description = model.Description;
                cat.LastModifiedDate = DateTime.Now;
                cat.LastModifiedByUserId = UserId;
                _context.Entry(cat).State = EntityState.Modified;
            }
            else
            {
                cat = new PeriodBreak
                {
                    Name = model.Name,
                    ShortName = model.ShortName,
                    BreakColor = model.BreakColor,
                    FromTime = model.FromTime,
                    ToTime = model.ToTime,
                    Description = model.Description,
                    IsActive = ((int)ActiveState.Active).ToString(),
                    CreatedDate = DateTime.Now,
                    CreatedByUserId = UserId,
                    LastModifiedDate = DateTime.Now,
                    LastModifiedByUserId = UserId
                };
                _context.PeriodBreak.Add(cat);
                _context.Entry(cat).State = EntityState.Added;
            }

            var changes = await _context.SaveChangesAsync();
            if (changes == 0) return null;

            result.StatusId = model.Id > 0 ? (int)ServiceResultStatus.Updated : (int)ServiceResultStatus.Added;
            result.RecordId = cat.Id;
            return result;
        }

        public async Task<ServiceResult> DeletePeriodBreak(int Id, int UserId)
        {
            var cat = await _context.PeriodBreak.SingleOrDefaultAsync(m => m.Id == Id);
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
