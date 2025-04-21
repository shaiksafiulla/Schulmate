using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IQuestionTypeService
    {        
        List<VQuestionTypes> GetAllQuestionType();
        VQuestionTypes ViewQuestionType(int Id);
        QuestionType GetQuestionType(int Id);
        ServiceResult SaveQuestionType(QuestionType model, int UserId);
        ServiceResult DeleteQuestionType(int Id, int UserId);
        bool IsRecordExists(string name, int Id);
        void Dispose();
    }
}
