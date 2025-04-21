using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IStudentEnquiryService
    {
        Task<List<VStudentEnquiry>> GetAllStudentEnquiry(int UserId);
        Task<List<SelectListItem>> GetClassByBranch(int branchId);
        Task<VStudentEnquiry> ViewStudentEnquiry(int Id);
        Task<StudentEnquiry> GetStudentEnquiry(int Id, int UserId);
        Task<ServiceResult> SaveStudentEnquiry(StudentEnquiry model, int UserId);
        Task<ServiceResult> DeleteStudentEnquiry(int Id, int UserId);
        Task<bool> IsRecordExists(string name, int Id);
        void Dispose();

    }
}
