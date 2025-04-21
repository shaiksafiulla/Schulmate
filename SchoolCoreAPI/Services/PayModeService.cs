using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class PayModeService : IPayModeService
    {
        private readonly APIContext _context;
        public PayModeService(APIContext context)
        {
            _context = context;
        }

        public async Task<List<VPayMode>> GetAllPayMode()
        {
            return await _context.VPayMode.AsNoTracking().OrderBy(x => x.Id).ToListAsync();
        }

        public async Task<VPayMode> ViewPayMode(int Id)
        {
            return await _context.VPayMode.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
        }

        public async Task<PayMode> GetPayMode(int Id)
        {
            if (Id <= 0) return new PayMode();

            var cat = await _context.PayMode.AsNoTracking().SingleOrDefaultAsync(x => x.Id == Id && x.IsActive == ((int)ActiveState.Active).ToString());
            if (cat == null) return new PayMode();

            return new PayMode
            {
                Id = cat.Id,
                Name = cat.Name,
                Description = cat.Description
            };
        }

        public async Task<bool> IsRecordExists(string name, int Id)
        {
            return Id == 0
                ? await _context.PayMode.AsNoTracking().AnyAsync(e => e.Name == name && e.IsActive == ((int)ActiveState.Active).ToString())
                : await _context.PayMode.AsNoTracking().AnyAsync(e => e.Name == name && e.Id != Id && e.IsActive == ((int)ActiveState.Active).ToString());
        }

        public async Task<ServiceResult> SavePayMode(PayMode model, int UserId)
        {
            var cat = new PayMode();
            ServiceResult result = null;
            if (model.Id > 0)
            {
                cat = await _context.PayMode.AsNoTracking().SingleOrDefaultAsync(m => m.Id == model.Id && m.IsActive == ((int)ActiveState.Active).ToString());
                if (cat != null)
                {
                    cat.Name = model.Name;
                    cat.Description = model.Description;
                    cat.LastModifiedDate = DateTime.Now;
                    cat.LastModifiedByUserId = UserId;
                    _context.Entry(cat).State = EntityState.Modified;
                    if (await _context.SaveChangesAsync() > 0)
                    {
                        result = new ServiceResult
                        {
                            StatusId = (int)ServiceResultStatus.Updated,
                            RecordId = cat.Id
                        };
                    }
                }
            }
            else
            {
                cat = new PayMode
                {
                    Name = model.Name,
                    Description = model.Description,
                    IsActive = ((int)ActiveState.Active).ToString(),
                    CreatedDate = DateTime.Now,
                    CreatedByUserId = UserId
                };
                _context.PayMode.Add(cat);
                if (await _context.SaveChangesAsync() > 0)
                {
                    result = new ServiceResult
                    {
                        StatusId = (int)ServiceResultStatus.Added,
                        RecordId = cat.Id
                    };
                }
            }
            return result;
        }

        public async Task<ServiceResult> DeletePayMode(int Id, int UserId)
        {
            var cat = await _context.PayMode.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
            if (cat == null) return null;

            cat.IsActive = ((int)ActiveState.InActive).ToString();
            cat.LastModifiedByUserId = UserId;
            cat.LastModifiedDate = DateTime.Now;
            _context.Entry(cat).State = EntityState.Modified;
            if (await _context.SaveChangesAsync() > 0)
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
