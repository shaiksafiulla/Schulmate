using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface ICalendarEventService
    {
        Task<List<CastCalender>> GetAllCalendarEvent(int BranchId, int SessionYearId);
        Task<List<CastCalender>> GetCalendarEventByMonth(int month, int BranchId, int SessionYearId);
        Task<CastCalender> ViewCalendarEvent(int Id);
        Task<CalendarEvent> GetCalendarEvent(int Id, int UserId);
        Task<ServiceResult> SaveCalendarEvent(CalendarEvent model, int UserId);
        Task<ServiceResult> DeleteCalendarEvent(int Id, int UserId);
        Task Dispose();
    }
}
