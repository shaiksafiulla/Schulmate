using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface ISectionService
    {        
        Task<List<VSections>> GetAllSection(int UserId);
        Task<VSections> ViewSection(int Id);
        Task<Section> GetSection(int Id);
        Task<ServiceResult> SaveSection(Section model, int UserId);
        Task<ServiceResult> DeleteSection(int Id, int UserId);
        Task<StudentVM> GetSectionStudent(int SectionId, int ClassId);
        Task<ServiceResult> UpdateSectionStudent(StudentVM model);
        Task<List<SelectListItem>> GetClassesByBranch(string branchId);
        Task<List<SelectListItem>> GetSectionsByBranchAndClass(string branchId, string classId);
        Task<bool> IsRecordExists(string name, int branchClassId, int Id);


        Task<SectionVM> GetSectionVM(int SectionId,int UserId);
        Task<SectionActivityVM> GetSectionActivityVM(int SectionId, int UserId);
        Task<ServiceResult> UpdateBranchClassSectionTeacher(SectionVM model, int UserId, int SessionYearId);
        Task<ServiceResult> UpdateBranchClassSectionActivityPersonnel(SectionActivityVM model, int UserId);
        Task<List<MbSection>> GetSectionsByPersonnelId(int PersonnelId, string PersonnelType, int BranchId, int SessionYearId);
        Task<List<MbBranchClass>> GetBranchClassesByPersonnelId(int PersonnelId, string PersonnelType, int SessionYearId);
        Task<List<MbSyllabus>> GetSubjectLessonsByClassId(int BranchClassId, int SessionYearId);
        Task<SectionTimeTableVM> GetSectionTimeTableVM(int Id, int UserId);
        Task<SectionTransTimeTableVM> GetSectionTransTimeTableVM(int Id, int SessionYearId);
        Task<List<VTimeTable>> GetTimeTable(int SectionId, int SessionYearId, int UserId);
        Task<ServiceResult> UpdateBranchClassSectionTimeTable(TimeTableDTO model, int UserId);
        void Dispose();
    }
}
