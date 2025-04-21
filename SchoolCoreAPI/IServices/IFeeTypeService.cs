using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IFeeTypeService
    {
        Task<List<VFeeTypes>> GetAllFeeType();
        Task<VFeeTypes> ViewFeeType(int Id);
        Task<FeeType> GetFeeType(int Id);
        Task<ServiceResult> SaveFeeType(FeeType model, int UserId);
        Task<ServiceResult> DeleteFeeType(int Id, int UserId);
        Task<bool> IsRecordExists(string name, int Id);
        Task Dispose();
    }
}
