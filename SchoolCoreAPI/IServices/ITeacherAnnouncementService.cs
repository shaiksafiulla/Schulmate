using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface ITeacherAnnouncementService
    {       
        Task<List<VTeacherAnnouncement>> GetAllTeacherAnnouncement(int TeacherId, int SessionYearId);
        Task<List<VTeacherAnnouncementSection>> GetTeacherAnnouncementSections(int SectionId, int SessionYearId);
        Task<VTeacherAnnouncement> ViewTeacherAnnouncement(int Id);
        Task<TeacherAnnouncement> GetTeacherAnnouncement(int Id, int PersonId, int BranchId, int SessionYearId);
        Task<ServiceResult> SaveTeacherAnnouncement(TeacherAnnouncement model, int UserId);
        Task<Tuple<byte[], string>> GetFileBytes(int Id);
        Task<ServiceResult> DeleteTeacherAnnouncement(int Id, int UserId);
        Task<List<SelectListItem>> GetClassByBranch(int BranchId);
        Task<List<SelectListItem>> GetSectionByClassAndSubjectTeacher(int BranchClassId, int TeacherId, int SessionYearId);
        Task<List<int>> GetSectionIdsByTeacherAnnouncement(int StudentAssignId);
        Task<List<SelectListItem>> GetSubjectByClass(int BranchClassId,int TeacherId, int SessionYearId);      
        void Dispose();

    }
}
