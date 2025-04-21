using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IActivityService
    {
        Task<List<VActivities>> GetAllActivity();
        Task<VActivities> ViewActivity(int Id);
        Task<Activity> GetActivity(int Id);
        Task<ServiceResult> SaveActivity(Activity model, int UserId);
        Task<ServiceResult> DeleteActivity(int Id, int UserId);
        Task<bool> IsRecordExists(string name, int Id);
        void Dispose();
    }
}
