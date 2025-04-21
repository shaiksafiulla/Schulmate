using Newtonsoft.Json.Linq;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IAttendenceService
    {
        Task<AttendenceVM> GetAllAttendence(int UserId);
        Task<MBDailyAttendenceVM> GetDailyAttendence(int BranchId, int SessionYearId);
        Task<Attendence> GetAttendence(int Id, int UserId);
        Task<Attendence> ViewAttendence(int Id);
        Task<List<SectionAttendence>> GetSectionAttendence(int Id, int BranchId, int SessionYearId);
        Task<List<MBPersonnelAttendence>> GetAttendenceDetailByPersonnelId(int PersonnelId, int PersonnelType, int SessionYearId);
        Task<List<CastStudentAttendence>> GetStudentAttendence(int Id, int SectionId, int SessionYearId);
        Task<List<PersonnelMonthlyAttendence>> GetAttendenceByPersonnelId(int PersonnelId, int PersonnelType, int SessionYearId);
        Task<List<VPersonnelAttendenceBenchmark>> GetTotalAttendenceByPersonnelId(int PersonnelId, int PersonnelType, int SessionYearId);
        Task<List<CastPersonnelAttendence>> GetPersonnelAttendence(int Id, int BranchId, int SessionYearId, int PersonnelType);
        Task<ServiceResult> SaveAttendence(Attendence model, int UserId);
        Task<SectionAttendenceVM> GetMbSectionAttendence(int Id, int TeacherId, int branchId, int SessionYearId);


       
        Task<JObject> GetFn_AttendenceMetric(int PersonnelId, int PersonnelType, int SessionYearId);
        void Dispose();
    }
}
