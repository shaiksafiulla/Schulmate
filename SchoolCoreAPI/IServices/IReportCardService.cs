using Newtonsoft.Json.Linq;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IReportCardService
    {        
        Task<List<VReportCards>> GetAllReportCard(int branchId,int sessionYearId);
        Task<List<VBranchClasses>> GetBranchClassByReportCard(int reportCardId);
        Task<Tuple<string, string>> GetSPReportCard(string reportCardId, string branchClassId);
        Task<VReportCards> ViewReportCard(int Id);
        Task<ReportCard> GetReportCard(int Id, int BranchId, int SessionYearId);
        Task<ReportCardDetail> GetReportCardDetail(int Id);
        Task<ServiceResult> SaveReportCard(ReportCard model, int UserId);
        Task<ServiceResult> DeleteReportCard(int Id, int UserId);
        Task<List<VSchedules>> GetSchedulesByBranch(string branchId, string reportcardId);
        Task<RCCP_Student> GetRCCP_Student(string reportCardId, string StudentId);
        Task<bool> IsRecordExists(string name, int Id);

        
        void Dispose();
    }
}
