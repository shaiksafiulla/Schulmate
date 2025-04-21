using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IScheduleSyllabusService
    {
        Task<List<VScheduleSyllabus>> GetAllScheduleSyllabus(int BranchId);
        Task<ExaminationPaper> GetExaminationPaper(int Id);
        Task<string> SaveExaminationPaper(ExaminationPaper model, int UserId);
        Task<VSchedules> GetScheduleSyllabus(int Id);
        Task<ScheduleSyllabusDetail> GetScheduleSyllabusDetail(int Id, int BranchId);
        Task<string> GetSPSyllabusTopic(int scheduleId);
        Task<List<VExaminations>> GetExaminationByScheduleId(int scheduleId);
        Task<QuestionPaper> GetQuestionPaper(int examId);
        Task<List<ExamSubject>> GetExamSubjectByBranchClass(string scheduleId, string branchclassId);
        Task<string> GetBranchClassQuestionStatus(string scheduleId, string branchClassId);
        Task<LessonVM> GetExamTopic(string examId);
        Task<int> UpdateExamTopic(LessonVM model);
        //string UploadExamPaper(int ExamId, File file, int UserId, string rootpath);
        Task<string> UploadQPaper(UploadExamQPaper model, int UserId);
        Task<ExamQuestionVM> GetExamQuestion(int Id);
        Task<string> UpdateExamQuestion(ExamQuestionVM model, int UserId);
        Task<List<VQuestionBanks>> ProcessQuestionAlgo(ExamQuestionVM mddel);
        Task<List<VExaminations>> GetAllExamination();
        Task<VExaminations> ViewExamination(int Id);
        Task<Examination> GetExamination(int Id);
        Task<int> SaveExamination(Examination model, int UserId);
        Task<bool> DeleteExamination(int Id, int UserId);
        Task<ViewAttach> ViewExamPaper(int ExamId);
        Task<Tuple<byte[], string>> GetFileBytes(int Id, string rootpath);
        void Dispose();      
   }

   
}
