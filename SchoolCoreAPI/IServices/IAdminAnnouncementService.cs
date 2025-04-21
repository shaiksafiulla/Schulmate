using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IAdminAnnouncementService
    {
        Task<List<VAdminAnnouncement>> GetAllAdminAnnouncement(int UserId);
        Task<VAdminAnnouncement> ViewAdminAnnouncement(int Id);
        Task<AdminAnnouncement> GetAdminAnnouncement(int Id, int UserId);
        Task<ServiceResult> SaveAdminAnnouncement(AdminAnnouncement model, int UserId);
        Task<ServiceResult> DeleteAdminAnnouncement(int Id, int UserId);
        Task<List<int>> GetSectionIdsByAdminAnnouncement(int AdminAnnounceId);

        void Dispose();
    }
}
