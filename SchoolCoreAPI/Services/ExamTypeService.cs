using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class ExamTypeService : IExamTypeService
    {
        private readonly APIContext _context;
        public ExamTypeService(APIContext context)
        {
            _context = context;
        }

        public async Task<List<VExamTypes>> GetAllExamType()
        {
            return await _context.VExamTypes.AsNoTracking().OrderBy(x => x.Id).ToListAsync();
        }

        public async Task<VExamTypes> ViewExamType(int Id)
        {
            return await _context.VExamTypes.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
        }

        public async Task<ExamType> GetExamType(int Id)
        {
            if (Id <= 0) return new ExamType();

            var cat = await _context.ExamType.AsNoTracking().SingleOrDefaultAsync(x => x.Id == Id && x.IsActive == ((int)ActiveState.Active).ToString());
            if (cat == null) return new ExamType();

            return new ExamType
            {
                Id = cat.Id,
                Name = cat.Name,
                Description = cat.Description
            };
        }

        public async Task<bool> IsRecordExists(string name, int Id)
        {
            return Id == 0
                ? await _context.ExamType.AnyAsync(e => e.Name == name && e.IsActive == ((int)ActiveState.Active).ToString())
                : await _context.ExamType.AnyAsync(e => e.Name == name && e.Id != Id && e.IsActive == ((int)ActiveState.Active).ToString());
        }

        public async Task<ServiceResult> SaveExamType(ExamType model, int UserId)
        {
            var cat = new ExamType();
            var result = new ServiceResult();
            if (model.Id > 0)
            {
                cat = await _context.ExamType.SingleOrDefaultAsync(m => m.Id == model.Id && m.IsActive == ((int)ActiveState.Active).ToString());
                if (cat == null) return result;

                cat.Name = model.Name;
                cat.Description = model.Description;
                cat.LastModifiedDate = DateTime.Now;
                cat.LastModifiedByUserId = UserId;
                _context.Entry(cat).State = EntityState.Modified;
            }
            else
            {
                cat = new ExamType
                {
                    Name = model.Name,
                    Description = model.Description,
                    IsActive = ((int)ActiveState.Active).ToString(),
                    CreatedDate = DateTime.Now,
                    CreatedByUserId = UserId,
                    LastModifiedDate = DateTime.Now,
                    LastModifiedByUserId = UserId
                };
                _context.ExamType.Add(cat);
            }

            if (await _context.SaveChangesAsync() > 0)
            {
                result.StatusId = model.Id > 0 ? (int)ServiceResultStatus.Updated : (int)ServiceResultStatus.Added;
                result.RecordId = cat.Id;
            }

            return result;
        }

        public async Task<ServiceResult> DeleteExamType(int Id, int UserId)
        {
            var result = new ServiceResult();
            var cat = await _context.ExamType.SingleOrDefaultAsync(m => m.Id == Id);
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
