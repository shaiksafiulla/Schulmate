using System.ComponentModel.DataAnnotations;

namespace SchoolCoreMOB.Models
{
    public class MbPersonnelDetail
    {
        public int Id { get; set; }
        public string? PersonnelType { get; set; }
        public int SessionYearId { get; set; }
        public string FilePath { get; set; }
        public string FullName { get; set; }
        public string Age { get; set; }
        public string GenderName { get; set; }
        public string? Designation { get; set; }
        public string? Qualification { get; set; }
        public string? EmailAddress { get; set; }
        public string? BloodGroupName { get; set; }
        public string? DateOfBirth { get; set; }
        public string? FatherName { get; set; }
        public string? MotherName { get; set; }
        public string DefaultMobileNo { get; set; }
        public string? AlternateMobileNo { get; set; }
        public string? PermanentAddress { get; set; }
        public string? CurrentAddress { get; set; }
        public string BranchName { get; set; }
        public string? ClassName { get; set; }
        public string? SectionName { get; set; }
        public string AdmissionDate { get; set; }
        public string AdmissionNo { get; set; }
        public string EmployeeRollNo { get; set; }
        public string ActiveStatus { get; set; }
        public int? ClassTeacherCount { get; set; }
        public int? ClassSubjectTeacherCount { get; set; }
        public string? TeacherSubjectName { get; set; }
        public string? TeacherClassName { get; set; }
        public int? PresentDays { get; set; }
        public decimal? AttPercent { get; set; }
        public int? WorkingDays { get; set; }
    }
    public class VPersonnelAttendenceBenchmark
    {        
        public int Id { get; set; }
        public int PersonnelId { get; set; }
        public int SessionYearId { get; set; }
        public int WorkingDays { get; set; }
        public int? YearNo { get; set; }
        public int? MonthNo { get; set; }
        public string? MonthName { get; set; }
        public decimal? PresentDays { get; set; }
        public decimal? AbsentDays { get; set; }
        public decimal? LeaveDays { get; set; }
    }
    public class ScheduleTitle
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string StrStartDate { get; set; }
        public string StrEndDate { get; set; }
        public decimal ResultPercent { get; set; }
        public int StatusId { get; set; }
    }
    public class VTeacherInvigilations
    {
        [Key]
        public int Id { get; set; }
        public int? PersonnelId { get; set; }
        public int? ScheduleId { get; set; }
        public string? StrExamDate { get; set; }
        public string? ExamHallName { get; set; }

    }
    public class SectionTransTimeTableVM
    {
        public int SectionId { get; set; }
        public int SessionYearId { get; set; }
        public CastTransTimetable CastTransTimetable { get; set; }
    }
    public class CastTransTimetable
    {
        public List<Period> Periods { get; set; }  // List of time slots (rows)
        public List<WeekdayColumn> Weekdays { get; set; }  // List of weekdays (columns)
    }
    public class WeekdayColumn
    {
        public string Name { get; set; }
        public List<string> Subjects { get; set; }  // List of subjects for each period (row)
    }
    public class TeacherTransTimeTableVM
    {
        public int TeacherId { get; set; }
        public int SessionYearId { get; set; }
        public CastTransTimetable CastTransTimetable { get; set; }
    }
    public class MbSection
    {
        public int Id { get; set; }
        public string ClassSectionName { get; set; }
    }
    public class MbBranchClass
    {
        public int Id { get; set; }
        public string ClassName { get; set; }
    }
    public class MbSyllabus
    {
        public int Id { get; set; }
        public string SubjectName { get; set; }
        public List<VLesson> LstLesson { get; set; }
    }
    public class MbStudent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string MobileNo { get; set; }
        public string RollNo { get; set; }
        public string AdmissionNo { get; set; }
        public string FilePath { get; set; }
        public string ClassSectionName { get; set; }
    }
    public class MbReportCard
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string Name { get; set; }
    }
}
