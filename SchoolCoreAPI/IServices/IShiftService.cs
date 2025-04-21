using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IShiftService
    {        
        Task<List<VShifts>> GetAllShift();
        Task<Shift> ViewShift(int Id);
        Task<Shift> GetShift(int Id);
        Task<ServiceResult> SaveShift(Shift model, int UserId);
        Task<ServiceResult> DeleteShift(int Id, int UserId);
        Task<bool> IsRecordExists(string fromtime, string totime, int Id);
        void Dispose();
    }
}
