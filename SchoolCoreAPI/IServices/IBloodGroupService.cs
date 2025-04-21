using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IBloodGroupService
    {
        Task<List<VBloodGroups>> GetAllBloodGroup();
        Task<VBloodGroups> ViewBloodGroup(int Id);
        Task<BloodGroup> GetBloodGroup(int Id);
        Task<ServiceResult> SaveBloodGroup(BloodGroup model, int UserId);
        Task<ServiceResult> DeleteBloodGroup(int Id, int UserId);
        Task<bool> IsRecordExists(string name, int Id);
        Task Dispose();
    }
}
