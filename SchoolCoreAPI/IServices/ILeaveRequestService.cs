using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface ILeaveRequestService
    {       
        Task<List<VLeaveRequest>> GetAllLeaveRequest(int userId);
        Task<List<VLeaveRequest>> GetLeaveRequestByPersonnelId(int personnelId, string personnelType);
        Task<VLeaveRequest> ViewLeaveRequest(int Id);
        Task<LeaveRequest> GetLeaveRequest(int Id, int PersonId, string PersonType, int BranchId, int SessionYearId);
        Task<ServiceResult> SaveLeaveRequest(LeaveRequest model, int UserId);
        Task<ServiceResult> UpdateLeaveRequest(CastLeaveRequest model, int UserId);
        Task<ServiceResult> DeleteLeaveRequest(int Id, int UserId);
        Task<LeaveReport> GetLeaveReport();
        Task<SpLeaveReportResult> GetSPLeaveReport(int PersonId);
        void Dispose();

    }
}
