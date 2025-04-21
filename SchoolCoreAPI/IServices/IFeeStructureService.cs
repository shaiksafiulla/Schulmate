using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IFeeStructureService
    {
        Task<List<VFeeStructure>> GetAllFeeStructure(int UserId);
        Task<VFeeStructure> ViewFeeStructure(int Id);
        Task<FeeStructure> GetFeeStructure(int Id, int UserId);
        Task<ServiceResult> SaveFeeStructure(FeeStructure model, int UserId);
        Task<ServiceResult> DeleteFeeStructure(int Id, int UserId);
        Task<List<SelectListItem>> GetClassesByBranch(int branchId, int sessionyearId);
        Task<bool> IsRecordExists(string name, int Id);
        Task Dispose();
    }
}
