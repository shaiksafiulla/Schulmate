using Microsoft.EntityFrameworkCore;

namespace SchoolCoreAPI.Models
{
    public class APIContext : DbContext
    {
        public APIContext(DbContextOptions<APIContext> options) : base(options)
        {
        }
        public virtual DbSet<Branch> Branch { get; set; }
        public virtual DbSet<vbranches> vbranches { get; set; }
        public virtual DbSet<Class> Class { get; set; }
        public virtual DbSet<VClassSubjects> VClassSubjects { get; set; }
        public virtual DbSet<VBranchClasses> VBranchClasses { get; set; }
        public virtual DbSet<VTopics> VTopics { get; set; }
        public virtual DbSet<BranchClass> BranchClass { get; set; }
        public virtual DbSet<PeriodBreak> PeriodBreak { get; set; }
        public virtual DbSet<Activity> Activity { get; set; }
        public virtual DbSet<Subject> Subject { get; set; }
        public virtual DbSet<ClassSubject> ClassSubject { get; set; }
        public virtual DbSet<BranchClassPeriodBreak> BranchClassPeriodBreak { get; set; }
        public virtual DbSet<VBranchClassPeriodBreak> VBranchClassPeriodBreak { get; set; }

        public virtual DbSet<Topic> Topic { get; set; }
        public virtual DbSet<Section> Section { get; set; }
        public virtual DbSet<VClasses> VClasses { get; set; }
        public virtual DbSet<VSections> VSections { get; set; }
        public virtual DbSet<Student> Student { get; set; }
        public virtual DbSet<StudentAdmission> StudentAdmission { get; set; }
        public virtual DbSet<StudentOtherDetail> StudentOtherDetail { get; set; }
        public virtual DbSet<StudentFee> StudentFee { get; set; }
        public virtual DbSet<PayMode> PayMode { get; set; }
        public virtual DbSet<VPayMode> VPayMode { get; set; }
        public virtual DbSet<StudentPay> StudentPay { get; set; }
        public virtual DbSet<VStudents> VStudents { get; set; }

        public virtual DbSet<ExamType> ExamType { get; set; }
        public virtual DbSet<Schedule> Schedule { get; set; }
        public virtual DbSet<Examination> Examination { get; set; }
        public virtual DbSet<ExaminationLesson> ExaminationLesson { get; set; }
        public virtual DbSet<VSchedules> VSchedules { get; set; }
        public virtual DbSet<VExaminations> VExaminations { get; set; }


        public virtual DbSet<StudentResult> StudentResult { get; set; }
        public virtual DbSet<Grade> Grade { get; set; }
        public virtual DbSet<Organization> Organization { get; set; }
        public virtual DbSet<VStudentMarkResults> VStudentMarkResults { get; set; }
        public virtual DbSet<VStudentScheduleResults> VStudentScheduleResults { get; set; }
        public virtual DbSet<VExaminationLessons> VExaminationLessons { get; set; }
        
        public virtual DbSet<VTeachers> VTeachers { get; set; }
       
        public virtual DbSet<ScheduleBranchClass> ScheduleBranchClass { get; set; }
        //public virtual DbSet<ScheduleStudentSection> ScheduleStudentSection { get; set; }
        public virtual DbSet<Holiday> Holiday { get; set; }
        public virtual DbSet<VHolidays> VHolidays { get; set; }

        public virtual DbSet<ScheduleExamDate> ScheduleExamDate { get; set; }
        //public virtual DbSet<ScheduleSection> ScheduleSection { get; set; }
        public virtual DbSet<VScheduleBranchClasses> VScheduleBranchClasses { get; set; }
        public virtual DbSet<ScheduleBranchClassSubject> ScheduleBranchClassSubject { get; set; }
        public virtual DbSet<VScheduleBranchClassSubjects> VScheduleBranchClassSubjects { get; set; }
        public virtual DbSet<ExamTime> ExamTime { get; set; }
        public virtual DbSet<ScheduleExamTime> ScheduleExamTime { get; set; }
        //public virtual DbSet<ScheduleBranchClassSubjectTopic> ScheduleBranchClassSubjectTopic { get; set; }
        //public virtual DbSet<VScheduleBranchClassSubjectTopics> VScheduleBranchClassSubjectTopics { get; set; }
        public virtual DbSet<VScheduleExamDates> VScheduleExamDates { get; set; }
        //public virtual DbSet<VScheduleSections> VScheduleSections { get; set; }
        //public virtual DbSet<ScheduleTeacherSection> ScheduleTeacherSection { get; set; }
        //public virtual DbSet<VScheduleTeacherSections> VScheduleTeacherSections { get; set; }
        //public virtual DbSet<VScheduleStudentSections> VScheduleStudentSections { get; set; }
        public virtual DbSet<Question> Question { get; set; }
        public virtual DbSet<QuestionDifficulty> QuestionDifficulty { get; set; }
        public virtual DbSet<QuestionType> QuestionType { get; set; }
        public virtual DbSet<QuestionCategory> QuestionCategory { get; set; }
        public virtual DbSet<QuestionBank> QuestionBank { get; set; }
        public virtual DbSet<VQuestionBanks> VQuestionBanks { get; set; }
        public virtual DbSet<ExaminationQuestion> ExaminationQuestion { get; set; }
        public virtual DbSet<VScheduleResults> VScheduleResults { get; set; }
        public virtual DbSet<VScheduleSyllabus> VScheduleSyllabus { get; set; }

        public virtual DbSet<userinfo> userinfo { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Page> Page { get; set; }
        public virtual DbSet<RolePage> RolePage { get; set; }
        public virtual DbSet<UserTypePage> UserTypePage { get; set; }
        public virtual DbSet<VUserTypePage> VUserTypePage { get; set; }
        
        public virtual DbSet<UserRole> UserRole { get; set; }
        public virtual DbSet<VUserInfo> VUserInfo { get; set; }
        public virtual DbSet<VRoles> VRoles { get; set; }
        public virtual DbSet<vuserpermissions> vuserpermissions { get; set; }
        public virtual DbSet<VPeriodBreaks> VPeriodBreaks { get; set; }
        public virtual DbSet<VActivities> VActivities { get; set; }
        public virtual DbSet<VSubjects> VSubjects { get; set; }
        public virtual DbSet<VQuestionTypes> VQuestionTypes { get; set; }
        public virtual DbSet<VQuestionDifficulties> VQuestionDifficulties { get; set; }
        public virtual DbSet<VQuestionCategories> VQuestionCategories { get; set; }
        public virtual DbSet<VExamTypes> VExamTypes { get; set; }
        public virtual DbSet<VExamTimes> VExamTimes { get; set; }

        public virtual DbSet<ReportCard> ReportCard { get; set; }
        public virtual DbSet<VReportCards> VReportCards { get; set; }
        public virtual DbSet<ReportCardSchedule> ReportCardSchedule { get; set; }
        public virtual DbSet<VExaminationQuestions> VExaminationQuestions { get; set; }
        public virtual DbSet<VRCOverAllPerformances> VRCOverAllPerformances { get; set; }
        public virtual DbSet<VRCOverAllPerformanceClasses> VRCOverAllPerformanceClasses { get; set; }
        public virtual DbSet<VRCOverAllPerformanceGenders> VRCOverAllPerformanceGenders { get; set; }
        public virtual DbSet<ExaminationPaper> ExaminationPaper { get; set; }
        public virtual DbSet<ScheduleTeacher> ScheduleTeacher { get; set; }
        public virtual DbSet<VScheduleTeachers> VScheduleTeachers { get; set; }
        public virtual DbSet<VScheduleExamTimes> VScheduleExamTimes { get; set; }
        //public virtual DbSet<ScheduleBranchClassRollNo> ScheduleBranchClassRollNo { get; set; }
        public virtual DbSet<Attendence> Attendence { get; set; }
        public virtual DbSet<StudentAttendence> StudentAttendence { get; set; }
        public virtual DbSet<PersonnelAttendence> PersonnelAttendence { get; set; }
        //public virtual DbSet<VDailyAttendence> VDailyAttendence { get; set; }
        public virtual DbSet<VAttendence> VAttendence { get; set; }
        public virtual DbSet<VStudentAttendance> VStudentAttendance { get; set; }
        //public virtual DbSet<VBranchClassAttendence> VBranchClassAttendence { get; set; }

        public virtual DbSet<LicenseKey> LicenseKey { get; set; }
        public virtual DbSet<VLicensekeys> VLicenseKeys { get; set; }
        public virtual DbSet<ReportSettings> ReportSettings { get; set; }
        public virtual DbSet<pushsubscriptions> pushsubscriptions { get; set; }
        public virtual DbSet<Medium> Medium { get; set; }
        public virtual DbSet<Shift> Shift { get; set; }
        public virtual DbSet<VShifts> VShifts { get; set; }
        public virtual DbSet<VMediums> VMediums { get; set; }
        public virtual DbSet<SessionYear> SessionYear { get; set; }
        public virtual DbSet<Attachment> Attachment { get; set; }
        public virtual DbSet<VAttachment> VAttachment { get; set; }
        public virtual DbSet<StudentTransferSection> StudentTransferSection { get; set; }
        public virtual DbSet<VStudentCurrentLocation> VStudentCurrentLocation { get; set; }
        public virtual DbSet<BloodGroup> BloodGroup { get; set; }
        public virtual DbSet<VBloodGroups> VBloodGroups { get; set; }
        public virtual DbSet<VTeacherCurrentLocation> VTeacherCurrentLocation { get; set; }
        public virtual DbSet<TeacherTransferBranch> TeacherTransferBranch { get; set; }

        public virtual DbSet<FeeType> FeeType { get; set; }
        public virtual DbSet<VFeeTypes> VFeeTypes { get; set; }

        public virtual DbSet<Lesson> Lesson { get; set; }
        public virtual DbSet<VLessons> VLessons { get; set; }

        public virtual DbSet<EnquiryStatus> EnquiryStatus { get; set; }
        public virtual DbSet<VEnquiryStatus> VEnquiryStatus { get; set; }

        public virtual DbSet<ExpenseCategory> ExpenseCategory { get; set; }
        public virtual DbSet<VExpenseCategory> VExpenseCategory { get; set; }

        public virtual DbSet<ReferenceAdmission> ReferenceAdmission { get; set; }
        public virtual DbSet<VReferenceAdmission> VReferenceAdmission { get; set; }
        public virtual DbSet<StudentEnquiry> StudentEnquiry { get; set; }
        public virtual DbSet<VStudentEnquiry> VStudentEnquiry { get; set; }
        public virtual DbSet<LeaveRequest> LeaveRequest { get; set; }
        public virtual DbSet<LeaveRequestDate> LeaveRequestDate { get; set; }
        public virtual DbSet<VLeaveRequest> VLeaveRequest { get; set; }
        public virtual DbSet<VLeaveRequestDate> VLeaveRequestDate { get; set; }
        

        public virtual DbSet<Expense> Expense { get; set; }
        public virtual DbSet<VExpense> VExpense { get; set; }
        public virtual DbSet<VExpenseSummary> VExpenseSummary { get; set; }
        public virtual DbSet<StudentAssignment> StudentAssignment { get; set; }
        public virtual DbSet<VStudentAssignment> VStudentAssignment { get; set; }
        public virtual DbSet<StudentAssignmentSection> StudentAssignmentSection { get; set; }
        public virtual DbSet<StudentAssignmentStatus> StudentAssignmentStatus { get; set; }
        public virtual DbSet<VStudentAssignmentStatus> VStudentAssignmentStatus { get; set; }
        public virtual DbSet<TeacherAnnouncement> TeacherAnnouncement { get; set; }
        public virtual DbSet<VTeacherAnnouncement> VTeacherAnnouncement { get; set; }
        public virtual DbSet<TeacherAnnouncementSection> TeacherAnnouncementSection { get; set; }


        public virtual DbSet<Personnel> Personnel { get; set; }
        public virtual DbSet<PersonnelAdmission> PersonnelAdmission { get; set; }
        public virtual DbSet<PersonnelTransferBranch> PersonnelTransferBranch { get; set; }
        public virtual DbSet<PersonnelOtherDetail> PersonnelOtherDetail { get; set; }
        public virtual DbSet<VAdmins> VAdmins { get; set; }
        public virtual DbSet<VStaff> VStaff { get; set; }
        public virtual DbSet<VDrivers> VDrivers { get; set; }
        public virtual DbSet<Notification> Notification { get; set; }
        public virtual DbSet<VNotification> VNotification { get; set; }


        public virtual DbSet<FeeStructure> FeeStructure { get; set; }
        public virtual DbSet<FeeStructureClass> FeeStructureClass { get; set; }
        public virtual DbSet<FeeStructureDetail> FeeStructureDetail { get; set; }
        public virtual DbSet<FeeStructureInstallment> FeeStructureInstallment { get; set; }


        public virtual DbSet<CompulsoryFee> CompulsoryFee { get; set; }
        public virtual DbSet<CompulsoryFeeDetail> CompulsoryFeeDetail { get; set; }
        public virtual DbSet<CompulsoryFeeInstallment> CompulsoryFeeInstallment { get; set; }
        public virtual DbSet<OptionalFee> OptionalFee { get; set; }
        public virtual DbSet<OptionalFeeDetail> OptionalFeeDetail { get; set; }


        public virtual DbSet<VFeeStructure> VFeeStructure { get; set; }
        public virtual DbSet<CalendarEvent> CalendarEvent { get; set; }
        public virtual DbSet<VCalendarEvent> VCalendarEvent { get; set; }
        public virtual DbSet<AdminAnnouncement> AdminAnnouncement { get; set; }
        public virtual DbSet<AdminAnnouncementSection> AdminAnnouncementSection { get; set; }
        public virtual DbSet<VAdminAnnouncement> VAdminAnnouncement { get; set; }
        public virtual DbSet<VPersonnelCurrentLocation> VPersonnelCurrentLocation { get; set; }

        public virtual DbSet<BranchClassSectionTeacher> BranchClassSectionTeacher { get; set; }
        public virtual DbSet<BranchClassSectionSubjectTeacher> BranchClassSectionSubjectTeacher { get; set; }
        public virtual DbSet<BranchClassSectionActivityPersonnel> BranchClassSectionActivityPersonnel { get; set; }
        
        public virtual DbSet<AdminNotification> AdminNotification { get; set; }
        public virtual DbSet<TimeTable> TimeTable { get; set; }
        public virtual DbSet<VTimeTable> VTimeTable { get; set; }
        public virtual DbSet<ScheduleExamHall> ScheduleExamHall { get; set; }
        public virtual DbSet<ScheduleExamHallSeat> ScheduleExamHallSeat { get; set; }
        public virtual DbSet<ScheduleStudentAllocation> ScheduleStudentAllocation { get; set; }
        public virtual DbSet<ScheduleTeacherExamHall> ScheduleTeacherExamHall { get; set; }
        public virtual DbSet<VScheduleTeacherExamHall> VScheduleTeacherExamHall { get; set; }
        public virtual DbSet<VScheduleStudentAllocation> VScheduleStudentAllocation { get; set; }
        public virtual DbSet<VStudentPay> VStudentPay { get; set; }
        public virtual DbSet<VStudentAttendenceBenchmark> VStudentAttendenceBenchmark { get; set; }
        public virtual DbSet<VPersonnelAttendenceBenchmark> VPersonnelAttendenceBenchmark { get; set; }
        public virtual DbSet<VTeacherInvigilations> VTeacherInvigilations { get; set; }
        public virtual DbSet<VExamTimeTable> VExamTimeTable { get; set; }
        //public virtual DbSet<Credential> Credential { get; set; }
        //public virtual DbSet<User> User { get; set; }
        public virtual DbSet<VAdminNotification> VAdminNotification { get; set; }
        public virtual DbSet<VTeacherAnnouncementSection> VTeacherAnnouncementSection { get; set; }
        public virtual DbSet<VStudentAttendence> VStudentAttendence { get; set; }


        public virtual DbSet<Documents> Documents { get; set; }
        public virtual DbSet<QueryLogs> QueryLogs { get; set; }
        public virtual DbSet<VStudentDues> VStudentDues { get; set; }
        public virtual DbSet<V_SP_PreSchedSubject> V_SP_PreSchedSubject { get; set; }
        public virtual DbSet<V_SP_ScheduleExamination> V_SP_ScheduleExamination { get; set; }
        public virtual DbSet<V_SP_ScheduleTeacher> V_SP_ScheduleTeacher { get; set; }
        public virtual DbSet<V_SP_PreExamResult> V_SP_PreExamResult { get; set; }
        public virtual DbSet<V_SP_PostExamResult> V_SP_PostExamResult { get; set; }
        public virtual DbSet<V_SP_RCCP> V_SP_RCCP { get; set; }
        public virtual DbSet<V_SP_ScheduleTopic> V_SP_ScheduleTopic { get; set; }
        public virtual DbSet<VStudentFeeReceipt> VStudentFeeReceipt { get; set; }
        public virtual DbSet<VReceivableSummary> VReceivableSummary { get; set; }
        public virtual DbSet<V_SP_TeacherPerformance> V_SP_TeacherPerformance { get; set; }
        public virtual DbSet<V_SP_LeaveSummary> V_SP_LeaveSummary { get; set; }
        

        //public virtual DbSet<Credentials> Credentials { get; set; }
        //public virtual DbSet<Users> Users { get; set; }


    }
}
