using SchoolCoreAPI.Models;
using System.Data;

namespace SchoolCoreAPI.IServices
{
    public interface IScheduleResultService
    {
        Task<List<VScheduleResults>> GetAllScheduleResult(int BranchId);
        Task<VSchedules> GetStudentResult(int Id);
        Task<ScheduleResultDetail> GetScheduleResultDetail(int Id,int BranchId);
        
        Task<DataSet> GetExportData(int Id);
        Task<List<VScheduleBranchClasses>> GetStudentBranchClass(int Id);
        Task<Tuple<string, string>> GetSPExamResult(string branchClassId, string scheduleId);
        Task<int> UploadStudentResult(StudentResultUpload model, int UserId);
        Task<int> SaveStudentResult(StudentResultVM model, int UserId);
        Task<string> GetBranchClassResultStatus(string scheduleId, string branchClassId);
        void Dispose();      
   }

   
}
