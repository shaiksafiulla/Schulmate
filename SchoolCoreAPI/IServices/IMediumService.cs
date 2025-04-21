using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IMediumService
    {       
        Task<List<VMediums>> GetAllMedium();
        Task<VMediums> ViewMedium(int Id);
        Task<Medium> GetMedium(int Id);
        Task<ServiceResult> SaveMedium(Medium model, int UserId);
        Task<ServiceResult> DeleteMedium(int Id, int UserId);
        Task<bool> IsRecordExists(string name, int Id);
        void Dispose();

    }
}
