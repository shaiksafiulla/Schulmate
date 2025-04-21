using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface ILessonService
    {       
        Task<List<VLessons>> GetAllLesson();
        Task<VLessons> ViewLesson(int Id);
        Task<Lesson> GetLesson(int Id);
        Task<ServiceResult> SaveLesson(Lesson model, int UserId);
        Task<ServiceResult> DeleteLesson(int Id, int UserId);
        Task<bool> IsRecordExists(int classId, int subjectId, string name, int Id);
        void Dispose();

    }
}
