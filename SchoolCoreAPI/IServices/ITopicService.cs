using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface ITopicService
    {
        
        Task<List<VTopics>> GetAllTopic();
        Task<VTopics> ViewTopic(int Id);
        Task<Topic> GetTopic(int Id);
        Task<ServiceResult> SaveTopic(Topic model, int UserId);
        Task<ServiceResult> DeleteTopic(int Id, int UserId);
        Task<bool> IsRecordExists(int lessonId, string name, int Id);
        Task<List<SelectListItem>> GetSubjectsByClass(int classId);
        Task<List<SelectListItem>> GetLessonsByClassSubject(int classId, int subjectId);
        void Dispose();

    }
}
