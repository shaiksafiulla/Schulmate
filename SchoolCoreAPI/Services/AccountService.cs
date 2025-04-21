using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class AccountService : IAccountService
    {
        private readonly APIContext _context;
        public AccountService(APIContext context)
        {
            _context = context;
        }

        public async Task<List<VExpense>> GetAllExpense(int UserId)
        {
            var objuser = await _context.VUserInfo.AsNoTracking().SingleOrDefaultAsync(x => x.Id == UserId);
            if (objuser == null)
            {
                return new List<VExpense>();
            }

            return objuser.UserType == ((int)UserType.SuperAdmin).ToString()
                ? await _context.VExpense.AsNoTracking().ToListAsync()
                : await _context.VExpense.AsNoTracking().Where(x => x.BranchId == objuser.BranchId).ToListAsync();
        }

        public async Task<VExpense> ViewExpense(int Id)
        {
            return await _context.VExpense.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
        }

        public async Task<Expense> GetExpense(int Id, int UserId)
        {
            var sheet = new List<SelectListItem>(GetExpenseCategory());
            var model = new Expense
            {
                ExpenseCategorySheet = sheet
            };
            //test
            if (Id > 0)
            {
                var cat = await _context.Expense.AsNoTracking().SingleOrDefaultAsync(x => x.Id == Id && x.IsActive == ((int)ActiveState.Active).ToString());
                if (cat != null)
                {
                    model = cat;
                    model.ExpenseCategorySheet = sheet;
                    var divcategoryItem = model.ExpenseCategorySheet.Find(p => p.Value == cat.CategoryId.ToString());
                    if (divcategoryItem != null)
                    {
                        divcategoryItem.Selected = true;
                    }
                }
            }
            else
            {
                model.ExpenseDate = DateTime.Now.Date;
                var objuser = await _context.VUserInfo.AsNoTracking().SingleOrDefaultAsync(x => x.Id == UserId);
                if (objuser != null)
                {
                    model.BranchId = (int)objuser.BranchId;
                }
                var objsession = await _context.SessionYear.AsNoTracking().SingleOrDefaultAsync(x => x.IsActive == ((int)ActiveState.Active).ToString() && x.IsDefault == true);
                if (objsession != null)
                {
                    model.SessionYearId = objsession.Id;
                }
            }
            return model;
        }

        public async Task<ServiceResult> SaveExpense(Expense model, int UserId)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                var now = DateTime.UtcNow;
                if (model.Id > 0)
                {
                    var cat = await _context.Expense.SingleOrDefaultAsync(m => m.Id == model.Id && m.IsActive == ((int)ActiveState.Active).ToString());
                    if (cat != null)
                    {
                        cat.SessionYearId = model.SessionYearId;
                        cat.BranchId = model.BranchId;
                        cat.CategoryId = model.CategoryId;
                        cat.Title = model.Title;
                        cat.ExpenseDate = model.ExpenseDate.ToUniversalTime();
                        cat.ReferenceNo = model.ReferenceNo;
                        cat.Amount = model.Amount;
                        cat.Description = model.Description;
                        cat.LastModifiedDate = now;
                        cat.LastModifiedByUserId = UserId;

                        _context.Entry(cat).State = EntityState.Modified;
                        if (await _context.SaveChangesAsync() > 0)
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
                    var cat = new Expense
                    {
                        SessionYearId = model.SessionYearId,
                        BranchId = model.BranchId,
                        CategoryId = model.CategoryId,
                        Title = model.Title,
                        ExpenseDate = model.ExpenseDate.ToUniversalTime(),
                        ReferenceNo = model.ReferenceNo,
                        Amount = model.Amount,
                        Description = model.Description,
                        IsActive = ((int)ActiveState.Active).ToString(),
                        CreatedDate = now,
                        CreatedByUserId = UserId,
                        LastModifiedDate = now,
                        LastModifiedByUserId = UserId
                    };

                    _context.Expense.Add(cat);
                    if (await _context.SaveChangesAsync() > 0)
                    {
                        result = new ServiceResult
                        {
                            StatusId = (int)ServiceResultStatus.Added,
                            RecordId = cat.Id
                        };
                    }
                }
            }
            catch (Exception)
            {
                // Log exception
            }

            return result;
        }

        public async Task<ServiceResult> DeleteExpense(int Id, int UserId)
        {
            ServiceResult result = new ServiceResult();
            var cat = await _context.Expense.SingleOrDefaultAsync(m => m.Id == Id);
            if (cat != null)
            {
                cat.IsActive = ((int)ActiveState.InActive).ToString();
                cat.LastModifiedByUserId = UserId;
                cat.LastModifiedDate = DateTime.UtcNow;
                _context.Entry(cat).State = EntityState.Modified;
                if (await _context.SaveChangesAsync() > 0)
                {
                    result = new ServiceResult
                    {
                        StatusId = (int)ServiceResultStatus.Deleted,
                        RecordId = cat.Id
                    };
                }
            }
            return result;
        }

        public List<SelectListItem> GetExpenseCategory()
        {
            return _context.ExpenseCategory
                .AsNoTracking()
                .Where(x => x.IsActive == ((int)ActiveState.Active).ToString())
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                .ToList();
        }

        public List<SelectListItem> GetSessionYearList()
        {
            return _context.SessionYear
                .AsNoTracking()
                .Where(x => x.IsActive == ((int)ActiveState.Active).ToString())
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                .ToList();
        }

        public async Task<SearchExpenseParams> LoadExpenseParams()
        {
            var vm = new SearchExpenseParams
            {
                FromDate = null,
                ToDate = null,
                SessionYearId = null,
                SessionYearSheet = new List<SelectListItem>(GetSessionYearList())
            };
            var selectedyear = await _context.SessionYear.AsNoTracking().FirstOrDefaultAsync(x => x.IsActive == ((int)ActiveState.Active).ToString() && x.IsDefault == true);
            if (selectedyear != null)
            {
                vm.SessionYearId = selectedyear.Id;
                var sessionItem = vm.SessionYearSheet.Find(p => p.Value == vm.SessionYearId.ToString());
                if (sessionItem != null)
                {
                    sessionItem.Selected = true;
                }
            }
            return vm;
        }       
        public async Task<List<VExpenseSummary>> GetExpenseSummary(int BranchId, int SessionYearId)
        {
            return await _context.VExpenseSummary
                .AsNoTracking()
                .Where(x => x.BranchId == BranchId && x.SessionYearId == SessionYearId)
                .OrderByDescending(x => x.Id)
                .ToListAsync();
        }
        public async Task<List<VExpenseSummary>> SearchExpenseSummary(SearchExpenseParams vM, int BranchId)
        {
            var builder = PredicateBuilder.True<VExpenseSummary>();
            if (!string.IsNullOrEmpty(vM.FromDate))
            {
                var frmdate = DateTime.Parse(vM.FromDate).ToUniversalTime();
                builder = builder.And(f => f.ExpenseDate >= frmdate);
            }
            if (!string.IsNullOrEmpty(vM.ToDate))
            {
                var todate = DateTime.Parse(vM.ToDate).AddHours(23).AddMinutes(59).AddSeconds(59).ToUniversalTime();
                builder = builder.And(f => f.ExpenseDate <= todate);
            }
            if (vM.SessionYearId.HasValue && vM.SessionYearId > 0)
            {
                builder = builder.And(f => f.SessionYearId == vM.SessionYearId);
            }
            if (BranchId > 0)
            {
                builder = builder.And(f => f.BranchId == BranchId);
            }
            return await _context.VExpenseSummary
                .AsNoTracking()
                .Where(builder)
                .OrderByDescending(x => x.Id)
                .ToListAsync();
        }
        public async Task<List<VExpense>> GetAllExpenseByDate(int Id)
        {
            var objSummary = await _context.VExpenseSummary.AsNoTracking().FirstOrDefaultAsync(x => x.Id == Id);
            if (objSummary == null)
            {
                return new List<VExpense>();
            }
            return await _context.VExpense
                .AsNoTracking()
                .Where(x => x.BranchId == objSummary.BranchId && x.SessionYearId == objSummary.SessionYearId && x.StrExpenseDate == objSummary.StrExpenseDate)
                .OrderByDescending(x => x.Id)
                .ToListAsync();
        }




        public async Task<SearchReceivableParams> LoadReceivableParams()
        {
            var vm = new SearchReceivableParams
            {
                FromDate = null,
                ToDate = null,
                SessionYearId = null,
                SessionYearSheet = new List<SelectListItem>(GetSessionYearList())
            };
            var selectedyear = await _context.SessionYear.AsNoTracking().FirstOrDefaultAsync(x => x.IsActive == ((int)ActiveState.Active).ToString() && x.IsDefault == true);
            if (selectedyear != null)
            {
                vm.SessionYearId = selectedyear.Id;
                var sessionItem = vm.SessionYearSheet.Find(p => p.Value == vm.SessionYearId.ToString());
                if (sessionItem != null)
                {
                    sessionItem.Selected = true;
                }
            }
            return vm;
        }
        public async Task<List<VReceivableSummary>> GetReceivableSummary(int BranchId, int SessionYearId)
        {
            return await _context.VReceivableSummary
                .AsNoTracking()
                .Where(x => x.BranchId == BranchId && x.SessionYearId == SessionYearId)
                .OrderByDescending(x => x.Id)
                .ToListAsync();
        }
        public async Task<List<VReceivableSummary>> SearchReceivableSummary(SearchReceivableParams vM, int BranchId)
        {
            var builder = PredicateBuilder.True<VReceivableSummary>();
            if (!string.IsNullOrEmpty(vM.FromDate))
            {
                var frmdate = DateTime.Parse(vM.FromDate).ToUniversalTime();
                builder = builder.And(f => f.CreatedDate >= frmdate);
            }
            if (!string.IsNullOrEmpty(vM.ToDate))
            {
                var todate = DateTime.Parse(vM.ToDate).AddHours(23).AddMinutes(59).AddSeconds(59).ToUniversalTime();
                builder = builder.And(f => f.CreatedDate <= todate);
            }
            if (vM.SessionYearId.HasValue && vM.SessionYearId > 0)
            {
                builder = builder.And(f => f.SessionYearId == vM.SessionYearId);
            }
            if (BranchId > 0)
            {
                builder = builder.And(f => f.BranchId == BranchId);
            }
            return await _context.VReceivableSummary
                .AsNoTracking()
                .Where(builder)
                .OrderByDescending(x => x.Id)
                .ToListAsync();
        }
        public async Task<List<VStudentPay>> GetAllReceivableByDate(int Id)
        {
            var objSummary = await _context.VReceivableSummary.AsNoTracking().FirstOrDefaultAsync(x => x.Id == Id);
            if (objSummary == null)
            {
                return new List<VStudentPay>();
            }
            return await _context.VStudentPay
                .AsNoTracking()
                .Where(x => x.BranchId == objSummary.BranchId && x.SessionYearId == objSummary.SessionYearId && x.StrCreatedDate == objSummary.StrCreatedDate)
                .OrderByDescending(x => x.Id)
                .ToListAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
