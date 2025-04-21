using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IExamTimeService
    {
        Task<List<VExamTimes>> GetAllExamTime();
        Task<ExamTime> ViewExamTime(int Id);
        Task<ExamTime> GetExamTime(int Id);
        Task<ServiceResult> SaveExamTime(ExamTime model, int UserId);
        Task<ServiceResult> DeleteExamTime(int Id, int UserId);
        Task<bool> IsRecordExists(string fromtime, string totime, int Id);
        Task Dispose();
    }
}
