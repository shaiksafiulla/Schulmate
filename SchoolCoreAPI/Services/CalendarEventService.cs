using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class CalendarEventService : ICalendarEventService
    {
        private readonly APIContext _context;
        public CalendarEventService(APIContext context)
        {
            _context = context;
        }

        public async Task<List<CastCalender>> GetAllCalendarEvent(int BranchId, int SessionYearId)
        {
            return await _context.VCalendarEvent
                .Where(x => x.BranchId == BranchId && x.SessionYearId == SessionYearId)
                .OrderBy(x => x.Id)
                .Select(c => new CastCalender
                {
                    Id = c.Id,
                    Year = c.Year,
                    Month = c.Month,
                    Title = c.Title,
                    Description = c.Description,
                    Start = c.StrStartDate,
                    End = c.StrEndDate,
                    BackgroundColor = c.Color,
                    BorderColor = c.Color
                })
                .ToListAsync();
        }

        public async Task<List<CastCalender>> GetCalendarEventByMonth(int month, int BranchId, int SessionYearId)
        {
            return await _context.VCalendarEvent
                .Where(x => x.Month == month && x.BranchId == BranchId && x.SessionYearId == SessionYearId)
                .OrderBy(x => x.Id)
                .Select(c => new CastCalender
                {
                    Id = c.Id,
                    Year = c.Year,
                    Month = c.Month,
                    Title = c.Title,
                    Description = c.Description,
                    Start = c.StrStartDate,
                    End = c.StrEndDate,
                    BackgroundColor = c.Color,
                    BorderColor = c.Color
                })
                .ToListAsync();
        }

        public async Task<CastCalender> ViewCalendarEvent(int Id)
        {
            var obj = await _context.VCalendarEvent.SingleOrDefaultAsync(m => m.Id == Id);
            if (obj == null) return null;

            return new CastCalender
            {
                Id = obj.Id,
                Year = obj.Year,
                Month = obj.Month,
                Title = obj.Title,
                Start = obj.StrStartDate,
                End = obj.StrEndDate,
                BackgroundColor = obj.Color,
                BorderColor = obj.Color
            };
        }

        public async Task<CalendarEvent> GetCalendarEvent(int Id, int UserId)
        {
            if (Id > 0)
            {
                var cat = await _context.CalendarEvent.SingleOrDefaultAsync(x => x.Id == Id && x.IsActive == ((int)ActiveState.Active).ToString());
                if (cat != null)
                {
                    return new CalendarEvent
                    {
                        Id = cat.Id,
                        BranchId = cat.BranchId,
                        SessionYearId = cat.SessionYearId,
                        Title = cat.Title,
                        EventType = cat.EventType,
                        IsPersonal = cat.IsPersonal,
                        IsPersonalSelected = cat.IsPersonal == ((int)ActiveState.Active).ToString(),
                        Description = cat.Description,
                        StartDate = cat.StartDate,
                        EndDate = cat.EndDate?.AddDays(-1),
                        Color = cat.Color
                    };
                }
            }
            else
            {
                var objsession = await _context.SessionYear.SingleOrDefaultAsync(x => x.IsActive == ((int)ActiveState.Active).ToString() && x.IsDefault == true);
                var objUser = await _context.VUserInfo.SingleOrDefaultAsync(x => x.Id == UserId);

                return new CalendarEvent
                {
                    SessionYearId = objsession?.Id ?? 0,
                    BranchId = objUser?.BranchId ?? 0,
                    EventType = ((int)EventType.Event).ToString(),
                    IsPersonal = ((int)ActiveState.InActive).ToString(),
                    IsPersonalSelected = false
                };
            }

            return null;
        }

        public async Task<ServiceResult> SaveCalendarEvent(CalendarEvent model, int UserId)
        {
            ServiceResult result = null;
            try
            {
                if (model.Id > 0)
                {
                    var cat = await _context.CalendarEvent.SingleOrDefaultAsync(m => m.Id == model.Id && m.IsActive == ((int)ActiveState.Active).ToString());
                    if (cat != null)
                    {
                        cat.Title = model.Title;
                        cat.EventType = model.EventType;
                        cat.IsPersonal = model.IsPersonalSelected ? ((int)ActiveState.Active).ToString() : ((int)ActiveState.InActive).ToString();
                        cat.Description = model.Description;
                        cat.StartDate = model.StartDate;
                        cat.EndDate = model.EndDate?.AddDays(1);
                        cat.Color = model.Color;
                        cat.LastModifiedDate = DateTime.Now;
                        cat.LastModifiedByUserId = UserId;

                        _context.Entry(cat).State = EntityState.Modified;
                        if (await _context.SaveChangesAsync() != 0)
                        {
                            result = new ServiceResult
                            {
                                StatusId = (int)ServiceResultStatus.Updated,
                                RecordId = cat.Id
                            };
                        }
                    }
                }
                else
                {
                    var cat = new CalendarEvent
                    {
                        Title = model.Title,
                        EventType = model.EventType,
                        IsPersonal = model.IsPersonalSelected ? ((int)ActiveState.Active).ToString() : ((int)ActiveState.InActive).ToString(),
                        BranchId = model.BranchId,
                        SessionYearId = model.SessionYearId,
                        Description = model.Description,
                        StartDate = model.StartDate,
                        EndDate = model.EndDate,
                        Color = model.Color,
                        IsActive = ((int)ActiveState.Active).ToString(),
                        CreatedDate = DateTime.Now,
                        CreatedByUserId = UserId,
                        LastModifiedDate = DateTime.Now,
                        LastModifiedByUserId = UserId
                    };

                    _context.CalendarEvent.Add(cat);
                    _context.Entry(cat).State = EntityState.Added;
                    if (await _context.SaveChangesAsync() != 0)
                    {
                        result = new ServiceResult
                        {
                            StatusId = (int)ServiceResultStatus.Added,
                            RecordId = cat.Id
                        };
                    }
                }
            }
            catch (Exception ex)
            {

                
            }   

            return result;
        }

        public async Task<ServiceResult> DeleteCalendarEvent(int Id, int UserId)
        {
            var cat = await _context.CalendarEvent.SingleOrDefaultAsync(m => m.Id == Id);
            if (cat == null) return null;

            cat.IsActive = ((int)ActiveState.InActive).ToString();
            cat.LastModifiedByUserId = UserId;
            cat.LastModifiedDate = DateTime.Now;

            _context.Entry(cat).State = EntityState.Modified;
            if (await _context.SaveChangesAsync() != 0)
            {
                return new ServiceResult
                {
                    StatusId = (int)ServiceResultStatus.Deleted,
                    RecordId = cat.Id
                };
            }

            return null;
        }

        public async Task Dispose()
        {
            await _context.DisposeAsync();
        }
    }
}
