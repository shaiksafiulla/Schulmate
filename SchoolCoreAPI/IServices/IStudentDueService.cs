using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IStudentDueService
    {        
        Task<List<VStudentDues>> GetAllStudentDues(int BranchId);
        Task<List<VStudentDues>> GetAllStudentDuesByPercent(int BranchId, int SessionYearId, double duePercent);
        Task<VStudentDues> ViewStudentDue(int StudentId);        
        void Dispose();
    }
}
