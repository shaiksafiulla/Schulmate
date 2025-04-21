using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IAccountService
    {
        Task<List<VExpense>> GetAllExpense(int UserId);
        Task<VExpense> ViewExpense(int Id);
        Task<Expense> GetExpense(int Id, int UserId);
        Task<ServiceResult> SaveExpense(Expense model, int UserId);
        Task<ServiceResult> DeleteExpense(int Id, int UserId);

        Task<SearchExpenseParams> LoadExpenseParams();
        Task<List<VExpenseSummary>> GetExpenseSummary(int BranchId, int SessionYearId);
        Task<List<VExpenseSummary>> SearchExpenseSummary(SearchExpenseParams vM, int BranchId);
        Task<List<VExpense>> GetAllExpenseByDate(int Id);




        Task<SearchReceivableParams> LoadReceivableParams();
        Task<List<VReceivableSummary>> GetReceivableSummary(int BranchId, int SessionYearId);
        Task<List<VReceivableSummary>> SearchReceivableSummary(SearchReceivableParams vM, int BranchId);
        Task<List<VStudentPay>> GetAllReceivableByDate(int Id);

        // bool IsRecordExists(string name, int Id);
        void Dispose();

    }
}
