using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface ISubjectService
    {     
        Task<List<VSubjects>> GetAllSubject();
        Task<VSubjects> ViewSubject(int Id);
        Task<Subject> GetSubject(int Id);
        Task<ServiceResult> SaveSubject(Subject model, int UserId);
        Task<ServiceResult> DeleteSubject(int Id, int UserId);
        Task<bool> IsRecordExists(string name, int MediumId, int Id);
        void Dispose();
    }
}
