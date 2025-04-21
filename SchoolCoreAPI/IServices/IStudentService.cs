using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.IServices
{
    public interface IStudentService
    {
        Task<List<MbStudent>> SearchMbStudents(int PersonnelId, int BranchId, string PersonnelType, int SessionYearId, string query);
        Task<List<MbStudent>> LoadDefaultMbStudents(int PersonnelId, int BranchId, string PersonnelType, int SessionYearId);
        Task<List<VStudents>> GetAllStudent(int UserId);
        Task<VStudents> ViewStudent(int Id);
        Task<StudentDetail> GetStudentDetail(int Id, int BranchId);
        Task<Student> GetStudent(int Id, int UserId);
        Task<ServiceResult> SaveStudent(Student model, int UserId);
        Task<ServiceResult> DeleteStudent(int Id, int UserId);
        Task<bool> IsRecordExists(string fname, string lname, int Id);
        Task<List<SelectListItem>> GetClassesByBranch(int branchId);
        Task<List<SelectListItem>> GetSectionsByBranchAndClass(int branchId, int classId);
        Task<StudentPay> GetStudentPay(int StudentId);
        Task<ServiceResult> SaveStudentPay(StudentPay model, int UserId);
        Task<VStudentFeeReceipt> GetStudentFeeReceipt(int PaymentId);
        Task<MbStudentPay> GetAllPaymentByStudentId(int StudentId, int UserId);
        Task<StudentScheduleVM> GetStudentScheduleVM(int Id, int SessionYearId);
        Task<StudentScheduleResultVM> GetStudentScheduleResultVM(int Id, int ReportCardId, int SessionYearId);
        Task<StudentMarkVM> GetStudentMarkVM(int Id, int ScheduleId);

        Task<List<SelectListItem>> GetStudentSubjects(int StudentId, int SessionYearId);
        Task<StudentMarkSubjectGraphVM> GetStudentSubjectGraphVM(int StudentId, int SubjectId, int SessionYearId);
        Task<StudentMarkSubjectVM> GetStudentMarkSubjectVM(int Id, string Subject, int SessionYearId);
        Task<List<MbReportCard>> GetStudentMbReportCardList(int StudentId, int SessionYearId);
        void Dispose();
    }
}
