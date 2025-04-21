using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IStaffService
    {
        Task<List<VStaff>> GetAllStaff(int UserId);
        Task<VStaff> ViewStaff(int Id);
        Task<Personnel> GetStaff(int Id);
        Task<ServiceResult> SaveStaff(Personnel model, int UserId);
        Task<ServiceResult> DeleteStaff(int Id, int UserId);
        Task<bool> IsRecordExists(string fname, string lname, int BranchId, int Id);
        void Dispose();       

    }
}
