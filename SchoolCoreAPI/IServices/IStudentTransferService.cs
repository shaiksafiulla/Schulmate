using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IStudentTransferService
    {
        Task<StudentTransferVM> GetStudentTransferVM();
        Task<List<SelectListItem>> GetClassByBranch(int branchId);
        Task<List<SelectListItem>> GetSectionByClass(int branchId, int classId);
        Task<List<CastStudentTransfer>> GetStudents(int sectionId);
        Task<ServiceResult> SaveStudentTransfer(List<StudentTransferSection> model, int UserId);       
        void Dispose();       
    }

   
}
