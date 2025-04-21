using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface ILicenseService
    {
        //  Task<string> GenerateLicenseKeyAsync(DateTime expirationDate);
        Task<bool> ValidateLicenseKeyAsync();
        Task<List<VLicensekeys>> GetLicenses();
        Task<VLicensekeys> ViewLicense(int Id);
        Task<LicenseKey> GetLicense(int Id);
        Task<ServiceResult> SaveLicense(LicenseKey model, int UserId);
        void Dispose();
    }
}
