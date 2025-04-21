using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IEnquiryStatusService
    {
        Task<List<VEnquiryStatus>> GetAllEnquiryStatus();
        Task<VEnquiryStatus> ViewEnquiryStatus(int Id);
        Task<EnquiryStatus> GetEnquiryStatus(int Id);
        Task<ServiceResult> SaveEnquiryStatus(EnquiryStatus model, int UserId);
        Task<ServiceResult> DeleteEnquiryStatus(int Id, int UserId);
        Task<bool> IsRecordExists(string name, int Id);
        Task Dispose();
    }
}
