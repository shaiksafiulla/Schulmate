using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IPayModeService
    {        
        Task<List<VPayMode>> GetAllPayMode();
        Task<VPayMode> ViewPayMode(int Id);
        Task<PayMode> GetPayMode(int Id);
        Task<ServiceResult> SavePayMode(PayMode model, int UserId);
        Task<ServiceResult> DeletePayMode(int Id, int UserId);
        Task<bool> IsRecordExists(string name, int Id);
        void Dispose();
    }
}
