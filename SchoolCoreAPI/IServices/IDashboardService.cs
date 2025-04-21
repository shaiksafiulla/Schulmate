using Newtonsoft.Json.Linq;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IDashboardService
    {       
        Task<List<vbranches>> GetAllBranch();
        Task<int> GetCurrentSessionYear();
        Task<OverAllPerformanceVM> GetOverAllPerformanceVM(int Id);
        Task<JObject> GetDashboardMetrics(int BranchId, int SessionYearId);
        void Dispose();      
   }

   
}
