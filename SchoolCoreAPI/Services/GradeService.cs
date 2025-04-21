using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class GradeService : IGradeService
    {
        private readonly APIContext _context;
        public GradeService(APIContext context)
        {
            _context = context;
        }

        public async Task<List<Grade>> GetAllGrade()
        {
            return await _context.Grade
                .Where(x => x.IsActive == ((int)ActiveState.Active).ToString())
                .OrderBy(x => x.Id)
                .ToListAsync();
        }

        public async Task<Grade> ViewGrade(int Id)
        {
            return await _context.Grade
                .SingleOrDefaultAsync(m => m.Id == Id && m.IsActive == ((int)ActiveState.Active).ToString());
        }

        public async Task<Grade> GetGrade(int Id)
        {
            if (Id <= 0) return new Grade();

            var cat = await _context.Grade
                .SingleOrDefaultAsync(x => x.Id == Id && x.IsActive == ((int)ActiveState.Active).ToString());

            if (cat == null) return new Grade();

            return new Grade
            {
                Id = cat.Id,
                Name = cat.Name,
                Color = cat.Color,
                MinPercent = cat.MinPercent,
                MaxPercent = cat.MaxPercent,
                Description = cat.Description
            };
        }

        public async Task<bool> IsRecordExists(string name, int Id)
        {
            return Id == 0
                ? await _context.Grade.AnyAsync(e => e.Name == name && e.IsActive == ((int)ActiveState.Active).ToString())
                : await _context.Grade.AnyAsync(e => e.Name == name && e.Id != Id && e.IsActive == ((int)ActiveState.Active).ToString());
        }

        public async Task<ServiceResult> SaveGrade(Grade model, int UserId)
        {
            var cat = new Grade();
            var result = new ServiceResult();
            var now = DateTime.Now;

            if (model.Id > 0)
            {
                cat = await _context.Grade
                    .SingleOrDefaultAsync(m => m.Id == model.Id && m.IsActive == ((int)ActiveState.Active).ToString());

                if (cat == null) return null;

                cat.Name = model.Name;
                cat.Color = model.Color;
                cat.MinPercent = model.MinPercent;
                cat.MaxPercent = model.MaxPercent;
                cat.LastModifiedDate = now;
                cat.LastModifiedByUserId = UserId;

                _context.Entry(cat).State = EntityState.Modified;
            }
            else
            {
                cat = new Grade
                {
                    Name = model.Name,
                    MinPercent = model.MinPercent,
                    MaxPercent = model.MaxPercent,
                    Color = model.Color,
                    IsActive = ((int)ActiveState.Active).ToString(),
                    CreatedDate = now,
                    CreatedByUserId = UserId,
                    LastModifiedDate = now,
                    LastModifiedByUserId = UserId
                };

                _context.Grade.Add(cat);
                _context.Entry(cat).State = EntityState.Added;
            }

            var changes = await _context.SaveChangesAsync();
            if (changes == 0) return null;

            result.StatusId = model.Id > 0 ? (int)ServiceResultStatus.Updated : (int)ServiceResultStatus.Added;
            result.RecordId = cat.Id;

            return result;
        }

        public async Task<ServiceResult> DeleteGrade(int Id, int UserId)
        {
            var cat = await _context.Grade.SingleOrDefaultAsync(m => m.Id == Id);
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
