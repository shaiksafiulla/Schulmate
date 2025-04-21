using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IReportSettingsService
    {       
        Task<ReportSettings> GetReportSettings(int UserId);
        Task<ReportSettings> GetReportSettingsByBranch(int BranchId);
        Task<ServiceResult> SaveReportSettings(ReportSettings model, int UserId);
        void Dispose();      
   }

   
}
