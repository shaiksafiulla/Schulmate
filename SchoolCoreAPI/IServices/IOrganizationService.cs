using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IOrganizationService
    {        
        Task<Organization> GetOrganization();
        Task<ServiceResult> SaveOrganization(Organization model, int UserId);
        void Dispose();
    }
}
