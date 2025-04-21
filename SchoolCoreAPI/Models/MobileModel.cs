using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SchoolCoreAPI.Models
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
    public class MbSubject
    {
        public int Id { get; set; }
        public string SubjectName { get; set; }
    }
    public class MbStudentMark
    {
        public int Id { get; set; }
        public string StudentName { get; set; }
        public string MaxMarks { get; set; }
        public string Marks { get; set; }
        public string GradeColor { get; set; }
    }
    public class MbSyllabus
    {
        public int Id { get; set; }
        public string SubjectName { get; set; }
        public List<VLessons> LstLesson { get; set; }
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
    [Table("documents")]
    public class Documents
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }       
        public string? Subject { get; set; }
        public string? Chapter { get; set; }
        public string? FileContent { get; set; }
        public string? FilePath { get; set; }
        [NotMapped]
        public IFormFile? DocumentFile { get; set; }
    }
    [Table("querylogs")]
    public class QueryLogs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? UserQuery { get; set; }
        public string? Response { get; set; }
        public DateTime? Timestamp { get; set; }        
    }

    [Table("vstudentdues")]
    public class VStudentDues
    {
        [Key]      
        public int? Id { get; set; }
        public int? StudentId { get; set; }       
        public int? BranchId { get; set; }
        public int? SessionYearId { get; set; }
        public int? BranchClassId { get; set; }
        public int? ClassId { get; set; }       
        public double? FinalAmount { get; set; }       
        public double? DueAmount { get; set; }
        public double? DuePercent { get; set; }
        public string? FullName { get; set; }
        public string? ClassName { get; set; }
    }

    //[Table("Users")]
    //public class Users
    //{
    //    [Key]
    //    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //    public int Id { get; set; }
    //    public string Username { get; set; }
    //    public string DisplayName { get; set; }
    //    public byte[] UserId { get; set; }
    //    public DateTime CreatedAt { get; set; }
    //}
    //[Table("Credentials")]
    //public class Credentials
    //{
    //    [Key]
    //    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //    public int Id { get; set; }
    //    public int UserId { get; set; }
    //    public byte[] CredentialId { get; set; }  // Store the credential ID as a binary field
    //    public byte[] PublicKey { get; set; }     // Store the public key as a binary field

    //}





}
