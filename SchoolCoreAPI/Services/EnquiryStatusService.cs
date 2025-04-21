using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class EnquiryStatusService : IEnquiryStatusService
    {
        private readonly APIContext _context;
        public EnquiryStatusService(APIContext context)
        {
            _context = context;
        }

        public async Task<List<VEnquiryStatus>> GetAllEnquiryStatus()
        {
            return await _context.VEnquiryStatus.AsNoTracking().OrderBy(x => x.Id).ToListAsync();
        }

        public async Task<VEnquiryStatus> ViewEnquiryStatus(int Id)
        {
            return await _context.VEnquiryStatus.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
        }

        public async Task<EnquiryStatus> GetEnquiryStatus(int Id)
        {
            if (Id <= 0) return new EnquiryStatus();

            var cat = await _context.EnquiryStatus.AsNoTracking().SingleOrDefaultAsync(x => x.Id == Id && x.IsActive == ((int)ActiveState.Active).ToString());
            if (cat == null) return new EnquiryStatus();

            return new EnquiryStatus
            {
                Id = cat.Id,
                Name = cat.Name,
                Description = cat.Description,
                Color = cat.Color
            };
        }

        public async Task<bool> IsRecordExists(string name, int Id)
        {
            return Id == 0
                ? await _context.EnquiryStatus.AnyAsync(e => e.Name == name && e.IsActive == ((int)ActiveState.Active).ToString())
                : await _context.EnquiryStatus.AnyAsync(e => e.Name == name && e.Id != Id && e.IsActive == ((int)ActiveState.Active).ToString());
        }

        public async Task<ServiceResult> SaveEnquiryStatus(EnquiryStatus model, int UserId)
        {
            var cat = new EnquiryStatus();
            var result = new ServiceResult();
            if (model.Id > 0)
            {
                cat = await _context.EnquiryStatus.SingleOrDefaultAsync(m => m.Id == model.Id && m.IsActive == ((int)ActiveState.Active).ToString());
                if (cat == null) return result;

                cat.Name = model.Name;
                cat.Color = model.Color;
                cat.Description = model.Description;
                cat.LastModifiedDate = DateTime.Now;
                cat.LastModifiedByUserId = UserId;
                _context.Entry(cat).State = EntityState.Modified;
            }
            else
            {
                cat = new EnquiryStatus
                {
                    Name = model.Name,
                    Color = model.Color,
                    Description = model.Description,
                    IsActive = ((int)ActiveState.Active).ToString(),
                    CreatedDate = DateTime.Now,
                    CreatedByUserId = UserId,
                    LastModifiedDate = DateTime.Now,
                    LastModifiedByUserId = UserId
                };
                _context.EnquiryStatus.Add(cat);
            }

            var changes = await _context.SaveChangesAsync();
            if (changes > 0)
            {
                result.StatusId = model.Id > 0 ? (int)ServiceResultStatus.Updated : (int)ServiceResultStatus.Added;
                result.RecordId = cat.Id;
            }

            return result;
        }

        public async Task<ServiceResult> DeleteEnquiryStatus(int Id, int UserId)
        {
            var result = new ServiceResult();
            var cat = await _context.EnquiryStatus.SingleOrDefaultAsync(m => m.Id == Id);
            if (cat == null) return result;

            cat.IsActive = ((int)ActiveState.InActive).ToString();
            cat.LastModifiedByUserId = UserId;
            cat.LastModifiedDate = DateTime.Now;
            _context.Entry(cat).State = EntityState.Modified;

            var changes = await _context.SaveChangesAsync();
            if (changes > 0)
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
