using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IStudentAssignmentService
    {       
        Task<List<VStudentAssignment>> GetAllStudentAssignment();
        Task<VStudentAssignment> ViewStudentAssignment(int Id);
        Task<StudentAssignment> GetStudentAssignment(int Id, int PersonId, int BranchId, int SessionYearId);
        Task<ServiceResult> SaveStudentAssignment(StudentAssignment model, int UserId);
        Task<ServiceResult> DeleteStudentAssignment(int Id, int UserId);
        Task<List<SelectListItem>> GetClassByBranch(int BranchId);
        Task<List<SelectListItem>> GetSectionByClass(int BranchId, int ClassId);

        Task<List<int>> GetSectionIdsByStudentAssignment(int StudentAssignId);
        Task<List<SelectListItem>> GetSubjectByClass(int ClassId, int TeacherId);



        // bool IsRecordExists(string name, int Id);
        Task<List<VStudentAssignmentStatus>> GetAllStudentAssignStatus();
        Task<VStudentAssignmentStatus> ViewStudentAssignStatus(int Id);
        Task<StudentAssignStatus> GetStudentAssignStatus(int Id);
        Task<ServiceResult> SaveStudentAssignStatus(StudentAssignStatus model, int UserId);
        void Dispose();

    }
}
