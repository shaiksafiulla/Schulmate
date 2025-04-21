using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IQuestionBankService
    {        
        List<VQuestionBanks> GetAllQuestionBank();
        VQuestionBanks ViewQuestionBank(int Id);
        QuestionBank GetQuestionBank(int Id);
        ServiceResult SaveQuestionBank(QuestionBank model, int UserId);
        ServiceResult DeleteQuestionBank(int Id, int UserId);
        ServiceResult SaveQuestionEditor(string model, int UserId);
        string GetEditorContent(int Id);
        List<SelectListItem> GetSubjectsByClass(string classId);
        List<SelectListItem> GetTopicsBySubject(string classId,string subjectId);
        bool IsRecordExists(string name, int Id);
        void Dispose();
    }
}
