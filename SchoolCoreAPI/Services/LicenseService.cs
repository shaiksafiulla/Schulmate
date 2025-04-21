using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class LicenseService : ILicenseService
    {
        private readonly APIContext _context;

        public LicenseService(APIContext context)
        {
            _context = context;
        }

        public async Task<bool> ValidateLicenseKeyAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.LicenseKey.AsNoTracking()
                .OrderByDescending(x => x.Id)
                .AnyAsync(k => k.ExpirationDate > now);
        }

        public async Task<List<VLicensekeys>> GetLicenses()
        {
            try
            {
                return await _context.VLicenseKeys.AsNoTracking()
                    .OrderByDescending(x => x.Id)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<LicenseKey> GetLicense(int Id)
        {
            return new LicenseKey
            {
                OrganizationId = 0,
                Key = "testkey",
                ExpirationDate = DateTime.Now
            };
        }

        public async Task<VLicensekeys> ViewLicense(int Id)
        {
            return await _context.VLicenseKeys.AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<ServiceResult> SaveLicense(LicenseKey model, int UserId)
        {
            var objLicense = new LicenseKey
            {
                NoOfMonths = model.NoOfMonths,
                OrganizationId =  _context.Organization.AsNoTracking().SingleOrDefaultAsync()?.Id ?? 0,
                Key = Guid.NewGuid().ToString(),
                ExpirationDate = DateTime.Now.AddMonths(model.NoOfMonths),
                CreatedDate = DateTime.Now,
                CreatedByUserId = UserId
            };

            _context.LicenseKey.Add(objLicense);
            _context.Entry(objLicense).State = EntityState.Added;

            var addedIndex = await _context.SaveChangesAsync();
            if (addedIndex == 0) return null;

            return new ServiceResult
            {
                StatusId = (int)ServiceResultStatus.Added,
                RecordId = objLicense.Id
            };
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}