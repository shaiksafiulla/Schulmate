using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IQuestionDifficultyService
    {
       
        List<VQuestionDifficulties> GetAllQuestionDifficulty();
        VQuestionDifficulties ViewQuestionDifficulty(int Id);
        QuestionDifficulty GetQuestionDifficulty(int Id);
        ServiceResult SaveQuestionDifficulty(QuestionDifficulty model, int UserId);
        ServiceResult DeleteQuestionDifficulty(int Id, int UserId);
        bool IsRecordExists(string name, int Id);
        void Dispose();
    }
}
