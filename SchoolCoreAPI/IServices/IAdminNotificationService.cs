using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IAdminNotificationService
    {
        Task<List<VAdminNotification>> GetAllAdminNotification(int UserId, int SessionYearId);
        Task<VAdminNotification> ViewAdminNotification(int Id);
        Task<Tuple<byte[], string>> GetFileBytes(int Id);
        Task<List<SelectListItem>> GetUserTypes();
        Task<AdminNotification> GetAdminNotification(int Id, int BranchId, int SessionYearId, int UserId);
        Task<ServiceResult> SaveAdminNotification(AdminNotification model, int UserId);
        Task<ServiceResult> DeleteAdminNotification(int Id, int UserId);
        Task<List<CastStudPersonNotification>> GetStudPersonNotification(int BranchId, int SessionYearId, int PersonnelType);
        void Dispose();
    }
}
