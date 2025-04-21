using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface ITeacherService
    {        
        Task<List<VTeachers>> GetAllTeacher(int UserId);
        Task<VTeachers> ViewTeacher(int Id);
        Task<Personnel> GetTeacher(int Id);
        Task<PersonnelDetail> GetTeacherDetail(int Id, int BranchId);
        Task<ServiceResult> SaveTeacher(Personnel model, int UserId);
        Task<ServiceResult> DeleteTeacher(int Id, int UserId);       
        Task<bool> IsRecordExists(string fname, string lname, int Id);
        Task<TeacherTimeTableVM> GetTeacherTimeTable(int TeacherId);
        Task<TeacherTransTimeTableVM> GetTeacherTransTimeTableVM(int Id, int SessionYearId);
        Task<List<VTimeTable>> GetTimeTable(int TeacherId, int SessionYearId, int UserId);     
        Task<List<VSchedules>> GetTeacherSchedules(string teacherId);
        Task<List<VTeacherInvigilations>> GetInvigilationsByTeacherId(int teacherId, int scheduleId);
        Task<ScheduleTeacherProcedure> GetPivotScheduleByTeacher(string scheduleId, string teacherId);
        Task<TeacherPerformanceProcedure> GetTeacherPerformance(int ScheduleId);
        //  ClassSubjectVM GetTeacherClassSubject(int Id);
        //  ServiceResult UpdateTeacherClassSubject(ClassSubjectVM model, int UserId);
        // string GetSPTeacherSubjects(string teacherId);

        void Dispose();

    }
}
