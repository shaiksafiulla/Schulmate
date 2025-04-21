using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface ITeacherTransferService
    {
        Task<TeacherTransferVM> GetTeacherTransferVM();
        Task<List<CastTeacherTransfer>> GetTeachers(int branchId);
        Task<ServiceResult> SaveTeacherTransfer(List<TeacherTransferBranch> model, int UserId);       
        void Dispose();       
    }

   
}
