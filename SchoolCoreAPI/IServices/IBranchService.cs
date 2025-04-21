using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IBranchService
    {
        Task<List<vbranches>> GetAllBranch();
        Task<vbranches> ViewBranch(int Id);
        BranchDTO GetBranch(int Id);
        Task<ServiceResult> SaveBranch(BranchDTO model, int UserId);
        Task<ServiceResult> DeleteBranch(int Id, int UserId);
        Task<ClassVM> GetBranchClass(int Id);
        Task<List<BranchSectionTimeTableRPT>> GetBranchSectionTimeTableRPT(int Id, int UserId);
        Task<List<VTimeTable>> GetTimeTable(int BranchId, int SessionYearId, int UserId);
        Task<ServiceResult> UpdateBranchClass(ClassVM model, int UserId);
        Task<bool> IsRecordExists(string name, int Id);
        Task Dispose();
    }

   
}
