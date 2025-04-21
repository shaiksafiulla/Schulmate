using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IGradeService
    {
        Task<List<Grade>> GetAllGrade();
        Task<Grade> ViewGrade(int Id);
        Task<Grade> GetGrade(int Id);
        Task<ServiceResult> SaveGrade(Grade model, int UserId);
        Task<ServiceResult> DeleteGrade(int Id, int UserId);
        Task<bool> IsRecordExists(string name, int Id);
        void Dispose();
    }
}
