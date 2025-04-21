using Newtonsoft.Json.Linq;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IProgressReportService
    {     
        Task<JObject> GetFn_ProgressReport(int rptcardId, int branchclassId, int studentId, int sessionYearId);
        Task<StudentScheduleVM> GetStudentScheduleVM(List<VStudentScheduleResults> revlst);

       
        void Dispose();
    }
}
