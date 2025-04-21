using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IPeriodBreakService
    {     
        Task<List<VPeriodBreaks>> GetAllPeriodBreak();
        Task<VPeriodBreaks> ViewPeriodBreak(int Id);
        Task<PeriodBreak> GetPeriodBreak(int Id);
        Task<ServiceResult> SavePeriodBreak(PeriodBreak model, int UserId);
        Task<ServiceResult> DeletePeriodBreak(int Id, int UserId);
        Task<bool> IsRecordExists(string name,  int Id);
        void Dispose();
    }
}
