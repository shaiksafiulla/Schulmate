using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IClassService
    {
        Task<List<VClasses>> GetAllClass();
        Task<VClasses> ViewClass(int Id);
        Task<Class> GetClass(int Id);
        Task<ServiceResult> SaveClass(Class model, int UserId);
        Task<ServiceResult> DeleteClass(int Id, int UserId);

        Task<SubjectVM> GetClassSubject(int Id);
        Task<ServiceResult> UpdateClassSubject(SubjectVM model, int UserId);
        Task<bool> IsRecordExists(string name, int MediumId, int Id);
        void Dispose();
    }
}
