using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IAdminService
    {
        Task<List<VAdmins>> GetAllAdmin(int UserId);
        Task<VAdmins> ViewAdmin(int Id);
        Task<Personnel> GetAdmin(int Id);
        Task<PersonnelDetail> GetAdminDetail(int Id);
        Task<ServiceResult> SaveAdmin(Personnel model, int UserId);
        Task<ServiceResult> DeleteAdmin(int Id, int UserId);
        Task<bool> IsRecordExists(string fname, string lname, int BranchId, int Id);
        void Dispose();
    }
}
