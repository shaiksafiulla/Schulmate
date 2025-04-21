using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IExpenseCategoryService
    {
        Task<List<VExpenseCategory>> GetAllExpenseCategory();
        Task<VExpenseCategory> ViewExpenseCategory(int Id);
        Task<ExpenseCategory> GetExpenseCategory(int Id);
        Task<ServiceResult> SaveExpenseCategory(ExpenseCategory model, int UserId);
        Task<ServiceResult> DeleteExpenseCategory(int Id, int UserId);
        Task<bool> IsRecordExists(string name, int Id);
        Task Dispose();
    }
}
