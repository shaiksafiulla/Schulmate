using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IDriverService
    {
        Task<List<VDrivers>> GetAllDriver(int UserId);
        Task<VDrivers> ViewDriver(int Id);
        Task<Personnel> GetDriver(int Id);
        Task<ServiceResult> SaveDriver(Personnel model, int UserId);
        Task<ServiceResult> DeleteDriver(int Id, int UserId);
        Task<bool> IsRecordExists(string fname, string lname, int BranchId, int Id);
        void Dispose();
    }
}
