using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface INotificationService
    {
        Task<List<VNotification>> GetAllNotification(int toUserId);
        Task<VNotification> ViewNotification(int Id);
        Task<int> UpdateNotificationStatus(int Id, int userId);
        Task<int> UpdateActionTakenStatus(int Id, int userId);
        Task<ServiceResult> SendNotification(Notification model, int userId);
        Task<ServiceResult> SendDBNotification(List<Notification> model, int notificationType, int userId);
    }
}
