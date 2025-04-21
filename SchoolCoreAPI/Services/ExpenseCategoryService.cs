using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class ExpenseCategoryService : IExpenseCategoryService
    {
        private readonly APIContext _context;
        public ExpenseCategoryService(APIContext context)
        {
            _context = context;
        }

        public async Task<List<VExpenseCategory>> GetAllExpenseCategory()
        {
            return await _context.VExpenseCategory.AsNoTracking().OrderBy(x => x.Id).ToListAsync();
        }

        public async Task<VExpenseCategory> ViewExpenseCategory(int Id)
        {
            return await _context.VExpenseCategory.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
        }

        public async Task<ExpenseCategory> GetExpenseCategory(int Id)
        {
            if (Id <= 0) return new ExpenseCategory();

            var cat = await _context.ExpenseCategory.AsNoTracking().SingleOrDefaultAsync(x => x.Id == Id && x.IsActive == ((int)ActiveState.Active).ToString());
            if (cat == null) return new ExpenseCategory();

            return new ExpenseCategory
            {
                Id = cat.Id,
                Name = cat.Name,
                Description = cat.Description
            };
        }

        public async Task<bool> IsRecordExists(string name, int Id)
        {
            return Id == 0
                ? await _context.ExpenseCategory.AnyAsync(e => e.Name == name && e.IsActive == ((int)ActiveState.Active).ToString())
                : await _context.ExpenseCategory.AnyAsync(e => e.Name == name && e.Id != Id && e.IsActive == ((int)ActiveState.Active).ToString());
        }

        public async Task<ServiceResult> SaveExpenseCategory(ExpenseCategory model, int UserId)
        {
            var cat = new ExpenseCategory();
            var result = new ServiceResult();
            if (model.Id > 0)
            {
                cat = await _context.ExpenseCategory.SingleOrDefaultAsync(m => m.Id == model.Id && m.IsActive == ((int)ActiveState.Active).ToString());
                if (cat == null) return null;

                cat.Name = model.Name;
                cat.Description = model.Description;
                cat.LastModifiedDate = DateTime.Now;
                cat.LastModifiedByUserId = UserId;
                _context.Entry(cat).State = EntityState.Modified;
            }
            else
            {
                cat = new ExpenseCategory
                {
                    Name = model.Name,
                    Description = model.Description,
                    IsActive = ((int)ActiveState.Active).ToString(),
                    CreatedDate = DateTime.Now,
                    CreatedByUserId = UserId,
                    LastModifiedDate = DateTime.Now,
                    LastModifiedByUserId = UserId
                };
                _context.ExpenseCategory.Add(cat);
            }

            var changes = await _context.SaveChangesAsync();
            if (changes > 0)
            {
                result.StatusId = model.Id > 0 ? (int)ServiceResultStatus.Updated : (int)ServiceResultStatus.Added;
                result.RecordId = cat.Id;
            }

            return result;
        }

        public async Task<ServiceResult> DeleteExpenseCategory(int Id, int UserId)
        {
            var cat = await _context.ExpenseCategory.SingleOrDefaultAsync(m => m.Id == Id);
            if (cat == null) return null;

            cat.IsActive = ((int)ActiveState.InActive).ToString();
            cat.LastModifiedByUserId = UserId;
            cat.LastModifiedDate = DateTime.Now;
            _context.Entry(cat).State = EntityState.Modified;

            var changes = await _context.SaveChangesAsync();
            if (changes > 0)
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
