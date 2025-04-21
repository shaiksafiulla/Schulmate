using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface ISessionYearService
    {       
        Task<List<SessionYear>> GetAllSessionYear();
        Task<SessionYear> GetSessionYear(int Id);
        Task<ServiceResult> SaveSessionYear(SessionYear model, int UserId);
        Task<ServiceResult> DeleteSessionYear(int Id, int UserId);
        Task<bool> IsRecordExists(string name, int Id);
        void Dispose();       
    }

   
}
