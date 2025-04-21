using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IQuestionCategoryService
    {
        
        List<VQuestionCategories> GetAllQuestionCategory();
        VQuestionCategories ViewQuestionCategory(int Id);
        QuestionCategory GetQuestionCategory(int Id);
        ServiceResult SaveQuestionCategory(QuestionCategory model, int UserId);
        ServiceResult DeleteQuestionCategory(int Id, int UserId);
        bool IsRecordExists(string name, int Id);
        void Dispose();
    }
}
