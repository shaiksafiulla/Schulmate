using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IBranchClassService
    {
        Task<List<VBranchClasses>> GetAllBranchClass(int UserId);
        Task<VBranchClasses> ViewBranchClass(int Id);
        Task<BranchClass> GetBranchClass(int Id);
        Task<ServiceResult> SaveBranchClass(BranchClass model, int UserId);
        Task<BranchClassVM> GetBranchClassVM(int BranchId, int BranchClassId, int ClassId);
        // Task<ServiceResult> UpdateBranchClassTeacher(BranchClassVM model, int UserId);
        Task<PeriodBreakVM> GetPeriodBreak(int Id);
        Task<ServiceResult> UpdatePeriodBreak(PeriodBreakVM model, int UserId);
        Task Dispose();
    }

   
}
