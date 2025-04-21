using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IExamTypeService
    {
        Task<List<VExamTypes>> GetAllExamType();
        Task<VExamTypes> ViewExamType(int Id);
        Task<ExamType> GetExamType(int Id);
        Task<ServiceResult> SaveExamType(ExamType model, int UserId);
        Task<ServiceResult> DeleteExamType(int Id, int UserId);
        Task<bool> IsRecordExists(string name, int Id);
        Task Dispose();
    }
}
