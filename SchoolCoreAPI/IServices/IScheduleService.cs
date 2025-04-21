using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IScheduleService
    {
        Task<List<VSchedules>> GetAllSchedule(int BranchId, int SessionYearId);
        Task<ScheduleDetail> ViewSchedule(int Id,int BranchId);
        Task<Schedule> GetSchedule(int Id, int BranchId, int SessionYearId);
        Task<int> SaveSchedule(Schedule model, int UserId);
        Task<bool> DeleteSchedule(int Id, int UserId);

        Task<HallTicket> GetHallTicket(string scheduleId, string studentId, int BranchId);
        Task<int> SaveScheduleBranchClassSubject(ScheduleBranchClassSubjectVM model, int UserId);
        //int SaveScheduleBranchClassSubjectTopic(ScheduleBrclsSubject model, int UserId);
        Task<int> SaveScheduleExamAlgo(string ScheduleId, int UserId);
        Task<int> UpdateScheduleTimeTableStatus(string ScheduleId, int UserId);
        Task<int> UpdateScheduleTeacherStatus(string ScheduleId, int UserId);
        Task<int> UpdateScheduleFinishStatus(string ScheduleId, int UserId);
        Task<int> SaveScheduleTeacherExamDateAlgo(string ScheduleId, int UserId);

        Task<ScheduleExamProcedure> GetPivotTimeTable(string ScheduleId, int UserId);
        Task<ScheduleTeacherProcedure> GetPivotScheduleTeacher(string ScheduleId, int UserId);
        Task<List<ScheduleBranchClassGroup>> GetScheduleBranchClassGroup(string ScheduleId, int UserId);
        Task<List<ScheduleBranchClassStudent>> GetScheduleBranchClassStudent(ClassAllocation model, int UserId);
        Task<int> SaveScheduleExamHall(List<ExamHall> lstModel, int UserId);
        Task<int> GetScheduleStatus(string ScheduleId);
        Task<int> SaveScheduleExam(ScheduleBrclsSubject model, int UserId);
        Task<SpScheduleTeacher> GetSpScheduleTeacher(string scheduleId);
        Task<SpScheduleExam> GetSpScheduleExam(string scheduleId);
        //SpScheduleStudent GetSpScheduleStudent(string scheduleId);
        // List<VScheduleSections> GetScheduleSection(string scheduleId);
        Task<SpScheduleExamDateTime> GetScheduleExamDateTime(string scheduleId);
        Task<int> SaveScheduleTeacherExamDate(ScheduleTeacherExam model, int UserId);
        // int SaveTeacherSection(TeacherSection model, int UserId);
        Task<int> SaveScheduleExamination(ScheduleExam model, int UserId);

        //int SaveScheduleStudent(ScheduleStudentExam model, int UserId);
        Task<List<ScheduleStrDate>> GetScheduleDate(string startDate, string endDate, string scheduleId);
        Task<List<VBranchClasses>> GetScheduleBranchClassExam(string scheduleId);
        Task<List<VScheduleBranchClasses>> GetScheduleClassByBranch(string scheduleId, string branchId);
        Task<List<CastExamHall>> GetScheduleTeacherExamDate(string scheduleId);
        Task<List<CastScheduleExamHall>> GetCastScheduleExamHall(string scheduleId);

        //List<ScheduleStudent> GetScheduleStudent(string scheduleId);
        Task<List<VScheduleStudentAllocation>> GetScheduleStudentInfo(string scheduleId, string branchClassId);
        Task<List<VExaminations>> GetScheduleExamination(string scheduleId, string branchId);
        Task<TimeTableVM> GetTimeTable(int ScheduleId);
        //List<vbranches> GetBranchBySchedule(int Id);
        Task<List<VScheduleBranchClasses>> GetBranchClassBySchedule(int scheduleId);
        Task<Tuple<string, string>> GetSPBranchClassSubject(string scheduleId);
        Task<List<VBranchClasses>> GetBranchClassSubjectTopicBySchedule(string scheduleId, string branchId);
        Task<TimeTableSubjectVM> GetSubjectsByBranchClassId(string branchClassId, string scheduleId);
        Task<List<VExaminations>> GetExaminationByBranchClassId(string branchClassId, string scheduleId);
        Task<int> SaveScheduleTimeTable(TimeTableSubjectVM model, int UserId);
        Task<int> SaveQuestionEditor(string model, int UserId);
        Task<string> GetEditorContent(int Id);
        Task<TimeTableVM> ViewTimeTable(int ScheduleId);
        Task<TimeTableVM> GetExamResultTable(int ScheduleId);
        Task<AutoScheduleVM> GetAutoScheduleVM(int Id);
        Task<string> GetAutoScheduleClass(string branchId);
        Task<ScheduleStudentSectionVM> GetScheduleStudentSection(int Id);
        Task<Tuple<string, string>> GetScheduleClassAndSectionByBranch(string branchId, string scheduleId);
        Task<ScheduleTeacherSectionVM> GetScheduleTeacherSection(int Id);
        Task<string> GetSPScheduleSection(int scheduleId);

        Task<List<ScheduleTitle>> GetSchedulesByPersonnelId(int PersonnelId, string PersonnelType, int SessionYearId, int BranchId);
        Task<List<VExamTimeTable>> GetExamTimeTableByScheduleAndClass(int ScheduleId, int BranchClassId, int SessionYearId);
        Task<List<MbSyllabus>> GetExamLessonByScheduleAndClass(int ScheduleId, int BranchClassId, int SessionYearId);
        Task<MBScheduleDetail> GetMBScheduleById(int ScheduleId);
        Task<List<MbBranchClass>> GetBranchClassesByScheduleId(int ScheduleId, int PersonnelId, string PersonnelType, int SessionYearId);

        Task<List<MbSubject>> GetSubjectByScheduleAndClass(int ScheduleId, int BranchClassId, int SessionYearId);
        Task<List<MbStudentMark>> GetStudentsByScheduleAndClassAndSubject(int ScheduleId, int BranchClassId, int SessionYearId, int SubjectId);
        void Dispose();
    }
}
