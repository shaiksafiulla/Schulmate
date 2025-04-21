using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IHolidayService
    {        
        Task<List<VHolidays>> GetAllHoliday();
        Task<VHolidays> ViewHoliday(int Id);
        Task<Holiday> GetHoliday(int Id);
        Task<ServiceResult> SaveHoliday(Holiday model, int UserId);
        Task<ServiceResult> DeleteHoliday(int Id, int UserId);
        Task<bool> IsRecordExists(string name, int Id);
        void Dispose();
    }
}
