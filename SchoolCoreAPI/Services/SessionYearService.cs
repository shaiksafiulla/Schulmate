using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using System.Globalization;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class SessionYearService : ISessionYearService
    {
        private readonly APIContext _context;
        public SessionYearService(APIContext context)
        {
            _context = context;
        }

        public async Task<List<SessionYear>> GetAllSessionYear()
        {
            var lst = await _context.SessionYear.AsNoTracking().ToListAsync();
            foreach (var x in lst)
            {
                x.StrFromDate = x.FromDate.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
                x.StrToDate = x.ToDate.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
            }
            return lst;
        }

        public async Task<SessionYear> GetSessionYear(int Id)
        {
            if (Id <= 0) return new SessionYear { IsDefaultExist = await isDefaultSessionExist() };

            var cat = await _context.SessionYear.AsNoTracking().SingleOrDefaultAsync(x => x.Id == Id && x.IsActive == ((int)Shared.ActiveState.Active).ToString());
            if (cat == null) return new SessionYear { IsDefaultExist = await isDefaultSessionExist() };

            return new SessionYear
            {
                Id = cat.Id,
                Name = cat.Name,
                FromDate = cat.FromDate,
                ToDate = cat.ToDate,
                IsDefault = cat.IsDefault,
                Description = cat.Description,
                StrFromDate = cat.FromDate.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture),
                StrToDate = cat.ToDate.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture),
                IsDefaultExist = await isDefaultSessionExist()
            };
        }

        public async Task<bool> isDefaultSessionExist()
        {
            return await _context.SessionYear.AsNoTracking().AnyAsync(x => x.IsDefault);
        }

        public async Task<bool> IsRecordExists(string name, int Id)
        {
            return Id == 0
                ? await _context.SessionYear.AsNoTracking().AnyAsync(e => e.Name == name && e.IsActive == ((int)ActiveState.Active).ToString())
                : await _context.SessionYear.AsNoTracking().AnyAsync(e => e.Name == name && e.Id != Id && e.IsActive == ((int)ActiveState.Active).ToString());
        }

        public async Task<ServiceResult> SaveSessionYear(SessionYear model, int UserId)
        {
            try
            {
                if (model.Id > 0)
                {
                    var cat = await _context.SessionYear.AsNoTracking().SingleOrDefaultAsync(m => m.Id == model.Id && m.IsActive == ((int)ActiveState.Active).ToString());
                    if (cat == null) return null;

                    cat.Name = model.Name;
                    cat.FromDate = model.FromDate;
                    cat.ToDate = model.ToDate;
                    cat.IsDefault = model.IsDefault;
                    cat.Description = model.Description;
                    cat.LastModifiedDate = DateTime.Now;
                    cat.LastModifiedByUserId = UserId;
                    _context.Entry(cat).State = EntityState.Modified;
                }
                else
                {
                    var cat = new SessionYear
                    {
                        Name = model.Name,
                        FromDate = model.FromDate,
                        ToDate = model.ToDate,
                        IsDefault = model.IsDefault,
                        Description = model.Description,
                        IsActive = ((int)Shared.ActiveState.Active).ToString(),
                        CreatedDate = DateTime.Now,
                        CreatedByUserId = UserId,
                        LastModifiedDate = DateTime.Now,
                        LastModifiedByUserId = UserId
                    };
                    _context.SessionYear.Add(cat);
                }

                var changes = await _context.SaveChangesAsync();
                if (changes == 0) return null;

                return new ServiceResult
                {
                    StatusId = model.Id > 0 ? (int)ServiceResultStatus.Updated : (int)ServiceResultStatus.Added,
                    RecordId = model.Id > 0 ? model.Id : _context.SessionYear.OrderByDescending(s => s.Id).First().Id
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<ServiceResult> DeleteSessionYear(int Id, int UserId)
        {
            var cat = await _context.SessionYear.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
            if (cat == null) return null;

            cat.IsActive = ((int)Shared.ActiveState.InActive).ToString();
            cat.LastModifiedByUserId = UserId;
            cat.LastModifiedDate = DateTime.Now;
            _context.Entry(cat).State = EntityState.Modified;

            var changes = await _context.SaveChangesAsync();
            if (changes == 0) return null;

            return new ServiceResult
            {
                StatusId = (int)ServiceResultStatus.Deleted,
                RecordId = cat.Id
            };
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}