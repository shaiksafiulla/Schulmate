using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SchoolCoreWEB.Helpers.Shared;

namespace SchoolCoreWEB.Models
{
    public class TimeTable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int SessionYearId { get; set; }
        public int BranchClassId { get; set; }
        public int ClassId { get; set; }
        public int SectionId { get; set; }
        public int? ClassSubjectId { get; set; }
        public int SubjectActivityId { get; set; }
        public int PersonnelId { get; set; }
        public string TimeTableType { get; set; }
        public int DayOfWeek { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }

        public string? IsActive { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
    }
    public class VTimeTable
    {
        [Key]
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int SessionYearId { get; set; }
        public int BranchClassId { get; set; }
        public int ClassId { get; set; }
        public int SectionId { get; set; }
        public int? ClassSubjectId { get; set; }
        public int SubjectActivityId { get; set; }
        public int PersonnelId { get; set; }
        public string TimeTableType { get; set; }
        public int DayOfWeek { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public string? PersonnelName { get; set; }
        public string? ClassSectionName { get; set; }
        public string? SubjectActivityName { get; set; }
        public string? SubjectActivityColor { get; set; }

    }
    public class CastSubjectTeacherTimeTable
    {
        public int Id { get; set; }
        public int? ClassSubjectId { get; set; }
        public int SubjectId { get; set; }
        public string? SubjectName { get; set; }
        public string? SubjectColor { get; set; }
        public int TeacherId { get; set; }
        public string? TeacherName { get; set; }
        //public string StrStartDate { get; set; }
        //public string StrEndDate { get; set; }
        //public DateTime? FromTime { get; set; }
        //public DateTime? ToTime { get; set; }
        //public string? StrFromTime { get; set; }
        //public string? StrToTime { get; set; }
        //public string? Title { get; set; }
        //public string? Start { get; set; }
        //public string? End { get; set; }
        //public string? BackgroundColor { get; set; }
        //public string? BorderColor { get; set; }

    }
    public class CastActivityPersonnelTimeTable
    {
        public int Id { get; set; }
        public int ActivityId { get; set; }
        public string? ActivityName { get; set; }
        public string? ActivityColor { get; set; }
        public int PersonnelId { get; set; }
        public string? PersonnelName { get; set; }
        //public string StrStartDate { get; set; }
        //public string StrEndDate { get; set; }
        //public DateTime? FromTime { get; set; }
        //public DateTime? ToTime { get; set; }
        //public string? StrFromTime { get; set; }
        //public string? StrToTime { get; set; }
        //public string? Title { get; set; }
        //public string? Start { get; set; }
        //public string? End { get; set; }
        //public string? BackgroundColor { get; set; }
        //public string? BorderColor { get; set; }

    }
    public class SectionTimeTableVM
    {
        public int BranchId { get; set; }
        public int SessionYearId { get; set; }
        public int BranchClassId { get; set; }
        public int ClassId { get; set; }
        public int SectionId { get; set; }
        public string? SectionName { get; set; }
        //public string? SlotDuration { get; set; }
        //public string? SlotMinTime { get; set; }
        //public string? SlotMaxTime { get; set; }
        //public string? StrStartDate { get; set; }
        //public string? StrEndDate { get; set; }
        public bool IsEdit { get; set; }
        public List<CastSubjectTeacherTimeTable>? LstCastSubjectTeacherTimeTable { get; set; }
        public List<CastActivityPersonnelTimeTable>? LstCastActivityPersonnelTimeTable { get; set; }
        //public List<CastSubjectTeacherTimeTable>? LstTimeTableResult { get; set; }
        public CastTimetable timetable { get; set; }

    }
    public class BranchTimeTableRPTVM
    {       
        public ReportSettings ReportSettings { get; set; }
        public List<BranchSectionTimeTableRPT> LstBranchSectionTimeTableRPT { get; set; }
    }
    public class BranchSectionTimeTableRPT
    {
        public int BranchId { get; set; }
        public int SessionYearId { get; set; }
        public int BranchClassId { get; set; }
        public int ClassId { get; set; }
        public int SectionId { get; set; }
        public string? ClassSectionName { get; set; }
        public CastTimetable timetable { get; set; }

    }
    public class TimeTableDTO
    {
        public int BranchId { get; set; }
        public int SessionYearId { get; set; }
        public int BranchClassId { get; set; }
        public int ClassId { get; set; }
        public int SectionId { get; set; }  
        public List<TimeTable>? LstTimeTable { get; set; }   

    }
    public class CastTimetable
    {
        public List<Weekday> Weekdays { get; set; }
        public List<Period> Periods { get; set; }
    }
    public class Break
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Description { get; set; }
    }
    public class Period
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsBreak { get; set; }
        public string Description { get; set; }
    }
    public class PeriodSlot
    {
        public Period Period { get; set; }
        public string Subject { get; set; }
    }
    public class Weekday
    {
        public string Name { get; set; }
        public List<PeriodSlot> PeriodSlots { get; set; }
    }
    //public class CastSectionTeacherTimeTable
    //{
    //    public int Id { get; set; }
    //    public int SubjectId { get; set; }
    //    public string? SubjectName { get; set; }
    //    public string? SubjectColor { get; set; }
    //    public int SectionId { get; set; }
    //    public string? SectionName { get; set; }
    //    public string StrStartDate { get; set; }
    //    public string StrEndDate { get; set; }
    //    public DateTime? FromTime { get; set; }
    //    public DateTime? ToTime { get; set; }
    //    public string? StrFromTime { get; set; }
    //    public string? StrToTime { get; set; }
    //    public string? Title { get; set; }
    //    public string? Start { get; set; }
    //    public string? End { get; set; }
    //    public string? BackgroundColor { get; set; }
    //    public string? BorderColor { get; set; }
    //}
    public class TeacherTimeTableVM
    {
        public int TeacherId { get; set; }
        public int SessionYearId { get; set; }
        public CastTimetable castTimetable { get; set; }
        //public string? TeacherName { get; set; }
        //public string? SlotDuration { get; set; }
        //public string? SlotMinTime { get; set; }
        //public string? SlotMaxTime { get; set; }
        //public string? StrStartDate { get; set; }
        //public string? StrEndDate { get; set; }
        //public List<CastSectionTeacherTimeTable>? LstTeacherTimeTable { get; set; }
    }

    //public class AdminNotification
    //{
    //    [Key]
    //    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //    public int Id { get; set; }
    //    public int SessionYearId { get; set; }
    //    public int BranchId { get; set; }
    //    public int UserTypeId { get; set; }
    //    public string Title { get; set; }
    //    public string Message { get; set; }
    //    [NotMapped]
    //    public IFormFile? NotificationFile { get; set; }
    //    public string? FileType { get; set; }
    //    public string? FileName { get; set; }
    //    public string? FilePath { get; set; }
    //    public string? IsActive { get; set; }
    //    public int? CreatedByUserId { get; set; }
    //    public DateTime? CreatedDate { get; set; }
    //    [NotMapped]
    //    public string? StrCreatedDate { get; set; }
    //    [NotMapped]
    //    public List<SelectListItem>? UserTypeSheet { get; set; }
    //    [NotMapped]
    //    public CastStudPersonNotification[]? LstCastStudPersonNotification { get; set; }
    //    [NotMapped]
    //    public string? StrCastStudPerson { get; set; }
    //    //[NotMapped]
    //    //public List<CastPersonnelNotification>? LstCastPersonnelNotification { get; set; }
    //}
    public class AdminNotification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SessionYearId { get; set; }
        public int BranchId { get; set; }
        public int? UserTypeId { get; set; }
        public string IsTag { get; set; }
        public int? TagTypeId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        [NotMapped]
        public IFormFile? NotificationFile { get; set; }
        public string? FileType { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public string? IsActive { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        [NotMapped]
        public string? StrCreatedDate { get; set; }
        [NotMapped]
        public List<SelectListItem>? UserTypeSheet { get; set; }
        [NotMapped]
        public List<SelectListItem>? TagTypeSheet { get; set; }
        [NotMapped]
        public List<CastStudPersonNotification>? LstCastStudPersonNotification { get; set; }
        [NotMapped]
        public string? StrCastStudPerson { get; set; }
        //[NotMapped]
        //public List<CastPersonnelNotification>? LstCastPersonnelNotification { get; set; }
    }
    public class VAdminNotification
    {
        [Key]
        public int Id { get; set; }
        public int SessionYearId { get; set; }
        public int BranchId { get; set; }
        public int? UserTypeId { get; set; }
        public string IsTag { get; set; }
        public int? TagTypeId { get; set; }
        public int? NotiPersonCount { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string? FileType { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public string? UserTypeName { get; set; }
        public string? TagTypeName { get; set; }
        public int? CreatedByUserId { get; set; }
        public string? StrCreatedDate { get; set; }

    }
    public class CastStudPersonNotification
    {
        public int StudPersonId { get; set; }
        public int PersonnelType { get; set; }
        public string RollEmployeeNo { get; set; }
        public string FullName { get; set; }
        public string? ClassName { get; set; }
        public string? SectionName { get; set; }
        public bool IsSelected { get; set; }
    }
    public class SectionVM
    {
        public int SectionId { get; set; }
        public List<string> SectionTeacherIds { get; set; }
        public List<SelectListItem>? SectionTeacherSheet { get; set; }
        public List<CastSectionSubjectTeacher>? LstSectionSubjectTeacher { get; set; }
    }
    public class SectionActivityVM
    {
        public int SectionId { get; set; }
        public List<CastSectionActivityPersonnel>? LstSectionActivityPersonnel { get; set; }
    }

    public class CastSectionActivityPersonnel
    {
        public int ActivityId { get; set; }
        public string? ActivityName { get; set; }
        public string? ActivityColor { get; set; }
        public List<string>? SectionActivityPersonnelIds { get; set; }
        public List<SelectListItem>? SectionActivityPersonnelSheet { get; set; }
    }
    public class BranchClassSectionActivityPersonnel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int BranchClassId { get; set; }
        public int BranchId { get; set; }
        public int ClassId { get; set; }
        public int SectionId { get; set; }
        public int ActivityId { get; set; }
        public int? PersonnelId { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? IsActive { get; set; }
    }
    public class CastSectionSubjectTeacher
    {
        public int ClassSubjectId { get; set; }
        public string? SubjectName { get; set; }
        public string? SubjectColor { get; set; }
        public List<string>? SectionSubjectTeacherIds { get; set; }
        public List<SelectListItem>? SectionSubjectTeacherSheet { get; set; }
    }
    public class AdminAnnouncement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SessionYearId { get; set; }
        public int BranchId { get; set; }
        public string Title { get; set; }
        [NotMapped]
        public IFormFile? AssignmentFile { get; set; }
        public string? FileType { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public string? Description { get; set; }
        public string? IsActive { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        [NotMapped]
        public List<SelectListItem>? ClassSectionSheet { get; set; }
        [NotMapped]
        public string? SectionIds { get; set; }
    }  
    public class AdminAnnouncementSection
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int AdminAnnouncementId { get; set; }
        public int SessionYearId { get; set; }
        public int BranchClassId { get; set; }
        public int BranchId { get; set; }
        public int ClassId { get; set; }
        public int SectionId { get; set; }
        public string? IsActive { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
    }    
    public class VAdminAnnouncement
    {
        [Key]
        public int Id { get; set; }
        public int SessionYearId { get; set; }
        public int BranchId { get; set; }
        public string Title { get; set; }
        public string? BranchName { get; set; }
        public string? ClassSectionName { get; set; }
        public string? Description { get; set; }
        public string? FileType { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public string? SessionYearName { get; set; }
    }
    public class LeaveReport
    {
        public List<SelectListItem> SessionYearList { get; set; }
        public int SessionYearId { get; set; }
    }
    public class SpLeaveReportResult
    {
        public string Result { get; set; }
    }
    public class CalendarEvent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int SessionYearId { get; set; }
        public string Title { get; set; }
        public string EventType { get; set; }
        public string IsPersonal { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
        public string? Color { get; set; }
        public string? IsActive { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        [NotMapped]
        public bool IsPersonalSelected { get; set; }
    }
    public class VCalendarEvent
    {
        [Key]
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int SessionYearId { get; set; }
        public string Title { get; set; }
        public string EventType { get; set; }
        public string IsPersonal { get; set; }
        public string? StrStartDate { get; set; }
        public string? StrEndDate { get; set; }
        public string? Description { get; set; }
        public string? Color { get; set; }
        public int? CreatedByUserId { get; set; }
        public int? LastModifiedByUserId { get; set; }
    }
    //public class CastCalender
    //{
    //    public int Id { get; set; }
    //    public string Title { get; set; }
    //    public string? Description { get; set; }
    //    public string Start { get; set; }
    //    public string End { get; set; }
    //    public string BackgroundColor { get; set; }
    //    public string BorderColor { get; set; }
    //}
    public class CastCalender
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int? Year { get; set; }
        public int? Month { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public string BackgroundColor { get; set; }
        public string BorderColor { get; set; }

    }
    public class FeeStructure
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int SessionYearId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }

        public decimal TotalAmount { get; set; }
        public DateTime DueDate { get; set; }
        public string DueChargeType { get; set; }
        public decimal DueCharge { get; set; }
        public string HasInstallment { get; set; }
        public string DiscountType { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmountAfterDiscount { get; set; }

        public string? IsActive { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        [NotMapped]
        public List<SelectListItem>? BranchSheet { get; set; }
        [NotMapped]
        public List<string>? ClassIds { get; set; }
        [NotMapped]
        public List<SelectListItem>? ClassSheet { get; set; }
        [NotMapped]
        public List<SelectListItem>? FeeTypeSheet { get; set; }
        [NotMapped]
        public List<FeeStructureDetail>? LstFeeStructureDetail { get; set; }
        [NotMapped]
        public List<FeeStructureInstallment>? LstFeeStructureInstallment { get; set; }
        //[NotMapped]
        //public CompulsoryFee? CompulsoryFee { get; set; }
        //[NotMapped]
        //public OptionalFee? OptionalFee { get; set; }
    }
    public class FeeStructureDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int FeeStructureId { get; set; }
        public int BranchId { get; set; }
        public int SessionYearId { get; set; }
        public int FeeTypeId { get; set; }
        public decimal Amount { get; set; }
        public string? IsActive { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
    }
    public class FeeStructureInstallment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int FeeStructureId { get; set; }
        public int BranchId { get; set; }
        public int SessionYearId { get; set; }
        public string InstallmentName { get; set; }
        public DateTime DueDate { get; set; }
        public string DueChargeType { get; set; }
        public decimal DueCharge { get; set; }
        public decimal Amount { get; set; }
        public string? IsActive { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
    }
    public class FeeStructureClass
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int FeeStructureId { get; set; }
        public int BranchId { get; set; }
        public int SessionYearId { get; set; }
        public int BranchClassId { get; set; }
        public int ClassId { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }

    }   
    public class CompulsoryFee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int FeeStructureId { get; set; }
        public int BranchId { get; set; }
        public int SessionYearId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime DueDate { get; set; }
        public string DueChargeType { get; set; }
        public decimal DueCharge { get; set; }
        public string HasInstallment { get; set; }
        public string DiscountType { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmountAfterDiscount { get; set; }
        public string? IsActive { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        [NotMapped]
        public List<CompulsoryFeeDetail>? LstCompulsoryFeeDetail { get; set; }
        [NotMapped]
        public List<CompulsoryFeeInstallment>? LstCompulsoryFeeInstallment { get; set; }
    }   
    public class CompulsoryFeeDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CompulsoryFeeId { get; set; }
        public int FeeStructureId { get; set; }
        public int BranchId { get; set; }
        public int SessionYearId { get; set; }
        public int FeeTypeId { get; set; }
        public decimal Amount { get; set; }
        public string? IsActive { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
    }  
    public class CompulsoryFeeInstallment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CompulsoryFeeId { get; set; }
        public int FeeStructureId { get; set; }
        public int BranchId { get; set; }
        public int SessionYearId { get; set; }
        public string InstallmentName { get; set; }
        public DateTime DueDate { get; set; }
        public string DueChargeType { get; set; }
        public decimal DueCharge { get; set; }
        public decimal Amount { get; set; }
        public string? IsActive { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
    }  
    public class OptionalFee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int FeeStructureId { get; set; }
        public int BranchId { get; set; }
        public int SessionYearId { get; set; }
        public decimal TotalAmount { get; set; }
        public string? IsActive { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        [NotMapped]
        public List<OptionalFeeDetail>? LstOptionalFeeDetail { get; set; }
    }  
    public class OptionalFeeDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int OptionalFeeId { get; set; }
        public int FeeStructureId { get; set; }
        public int BranchId { get; set; }
        public int SessionYearId { get; set; }
        public int FeeTypeId { get; set; }
        public decimal Amount { get; set; }
        public string? IsActive { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
    }  
    public class VFeeStructure
    {
        [Key]
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int SessionYearId { get; set; }
        public string Title { get; set; }
        public string HasInstallment { get; set; }
        public decimal TotalAmount { get; set; }
        //public decimal OptionalAmount { get; set; }
        public string? Description { get; set; }
        public string? BranchName { get; set; }
        public string? ClassName { get; set; }
        public string? SessionYearName { get; set; }
    }
    public class VNotification
    {
        [Key]
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int SessionYearId { get; set; }
        public string ReadStatusId { get; set; }
        public string NotificationType { get; set; }        
        public int RecordId { get; set; }
        public string Message { get; set; }
        public string FromUserType { get; set; }
        public string ToUserType { get; set; }
        public int FromUserId { get; set; }
        public int ToUserId { get; set; }
        public int FromPersonnelId { get; set; }
        public int ToPersonnelId { get; set; }
        public string? FromPersonnelName { get; set; }
        public string? Description { get; set; }
        public string? FromUserTypeName { get; set; }
        public string? NotificationTypeName { get; set; }
        public string? SessionYearName { get; set; }
        public string? StrCreatedDate { get; set; }
        public string? BranchName { get; set; }
        public string? ToUserTypeName { get; set; }

    }
    public class Personnel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PersonnelType { get; set; }      
        public string? Gender { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfBirth { get; set; }
        public int? BloodGroupId { get; set; }
        public string? EmailAddress { get; set; }
        public string? Designation { get; set; }
        public string? Qualification { get; set; }
        [NotMapped]
        public IFormFile? File { get; set; }
        [NotMapped]
        public int? BranchId { get; set; }
        [NotMapped]
        public List<SelectListItem>? BranchSheet { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }

        public string? IsActive { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [NotMapped]
        public List<SelectListItem>? BloodGroupSheet { get; set; }
        public PersonnelAdmission PersonnelAdmission { get; set; }
        public PersonnelOtherDetail PersonnelOtherDetail { get; set; }

    }
    public class PersonnelTransferBranch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SessionYearId { get; set; }
        public int PersonnelId { get; set; }
        public int FromBranchId { get; set; }
        public int ToBranchId { get; set; }
        public string EmployeeNo { get; set; }
        public string PersonnelType { get; set; }
        public string TransferType { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
    public class PersonnelAdmission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int PersonnelId { get; set; }
        public int BranchId { get; set; }
        public int SessionYearId { get; set; }
        public string PersonnelType { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? AdmissionDate { get; set; }
        public string? AdmissionNo { get; set; }
        //public string? EmployeeNo { get; set; }
        public string? IsActive { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        //[NotMapped]
        //public List<SelectListItem>? BranchSheet { get; set; }
    }   
    public class PersonnelOtherDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int PersonnelId { get; set; }
        public string PersonnelType { get; set; }
        public string? FatherName { get; set; }
        public string? MotherName { get; set; }
        public string? DefaultMobileNo { get; set; }
        public string? OtherMobileNo { get; set; }
        public string? CurrentAddress { get; set; }
        public string? PermanentAddress { get; set; }
        public string? IsActive { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
    public class Profile
    {
        [Key]
        public int Id { get; set; }       
        public string IsBranchAdmin { get; set; }
        public string? FullName { get; set; }
        public string? Age { get; set; }
        public string? BloodGroupName { get; set; }        
        public string? BranchName { get; set; }
        public string? DefaultMobileNo { get; set; }
        public string? Designation { get; set; }
        public string? EmailAddress { get; set; }
        public string? EmployeeNo { get; set; }
        public string? FilePath { get; set; }
        public string? GenderName { get; set; }
        public string? Qualification { get; set; }
        public string? TeacherClassName { get; set; }
    }
    public class VAdmin
    {
        [Key]
        public int Id { get; set; }
        public string PersonnelType { get; set; }
        public string TransferType { get; set; }
        public string IsBranchAdmin { get; set; }
        public string? FullName { get; set; }
        public string? Age { get; set; }
        public string? BloodGroupName { get; set; }
        public string? DateOfBirth { get; set; }
        public int? BranchId { get; set; }
        public string? BranchName { get; set; }
        public string? DefaultMobileNo { get; set; }
        public string? Designation { get; set; }
        public string? EmailAddress { get; set; }
        public string? EmployeeNo { get; set; }
        public string? FilePath { get; set; }
        public string? GenderName { get; set; }
        public string? Qualification { get; set; }
        public string ActiveStatus { get; set; }
        public int? PresentDays { get; set; }
        public decimal? AttPercent { get; set; }
        public int? WorkingDays { get; set; }
    }
    public class VStaff
    {
        [Key]
        public int Id { get; set; }
        public string PersonnelType { get; set; }
        public string TransferType { get; set; }
        public string? FullName { get; set; }
        public string? Age { get; set; }
        public string? BloodGroupName { get; set; }
        public int? BranchId { get; set; }
        public string? BranchName { get; set; }
        public string? DefaultMobileNo { get; set; }
        public string? Designation { get; set; }
        public string? EmailAddress { get; set; }
        public string? EmployeeNo { get; set; }
        public string? FilePath { get; set; }
        public string? GenderName { get; set; }
        public string? Qualification { get; set; }
    }  
    public class VDriver
    {
        [Key]
        public int Id { get; set; }
        public string PersonnelType { get; set; }
        public string TransferType { get; set; }
        public string? FullName { get; set; }
        public string? Age { get; set; }
        public string? BloodGroupName { get; set; }
        public int? BranchId { get; set; }
        public string? BranchName { get; set; }
        public string? DefaultMobileNo { get; set; }
        public string? Designation { get; set; }
        public string? EmailAddress { get; set; }
        public string? EmployeeNo { get; set; }
        public string? FilePath { get; set; }
        public string? GenderName { get; set; }
        public string? Qualification { get; set; }
    }
    public class StudentAssignment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SessionYearId { get; set; }
        public int BranchClassId { get; set; }
        public int BranchId { get; set; }
        public int ClassId { get; set; }
        public int ClasssubjectId { get; set; }
        public int SubjectId { get; set; }
        public string Title { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime SubmissionDate { get; set; }
        [NotMapped]
        public IFormFile? AssignmentFile { get; set; }
        public string? FileType { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public string? Description { get; set; }
        public string? IsActive { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        //[NotMapped]
        //public List<SelectListItem>? SessionYearSheet { get; set; }
        //[NotMapped]
        //public List<SelectListItem>? BranchSheet { get; set; }
        [NotMapped]
        public List<SelectListItem>? BranchClassSheet { get; set; }
        //[NotMapped]
        //public List<SelectListItem>? SectionSheet { get; set; }
        [NotMapped]
        public string? SectionIds { get; set; }
        [NotMapped]
        public List<SelectListItem>? SubjectSheet { get; set; }
    }     
    public class VStudentAssignment
    {
        [Key]
        public int Id { get; set; }
        public int SessionYearId { get; set; }
        public int BranchClassId { get; set; }
        public int BranchId { get; set; }
        public int ClassId { get; set; }
        public int ClasssubjectId { get; set; }
        public int SubjectId { get; set; }
        public string Title { get; set; }

        public string? BranchName { get; set; }
        public string? ClassName { get; set; }
        public string? Description { get; set; }
        public string? FileType { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public string? SessionYearName { get; set; }
        public string? SubjectName { get; set; }
        public string? SubmissionDate { get; set; }
        public string? SectionName { get; set; }

    }  
    public class StudentAssignmentSection
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int StudentAssignmentId { get; set; }
        public int SessionYearId { get; set; }
        public int BranchClassId { get; set; }
        public int BranchId { get; set; }
        public int ClassId { get; set; }
        public int ClasssubjectId { get; set; }
        public int SubjectId { get; set; }
        public int SectionId { get; set; }
        public string? IsActive { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }

    }   
    public class StudentAssignmentStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int StudentAssignmentId { get; set; }
        public int SessionYearId { get; set; }
        public int BranchClassId { get; set; }
        public int BranchId { get; set; }
        public int ClassId { get; set; }
        public int SectionId { get; set; }
        public int ClasssubjectId { get; set; }
        public int SubjectId { get; set; }
        public int StudentId { get; set; }
        public string StatusId { get; set; }
        public string? FeedBack { get; set; }
        public string? IsActive { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }

    }    
    public class VStudentAssignmentStatus
    {
        [Key]
        public int Id { get; set; }
        public int StudentAssignmentId { get; set; }
        public int SessionYearId { get; set; }
        public int BranchClassId { get; set; }
        public int BranchId { get; set; }
        public int ClassId { get; set; }
        public int SectionId { get; set; }
        public int ClasssubjectId { get; set; }
        public int SubjectId { get; set; }
        public int StudentId { get; set; }
        public string StatusId { get; set; }
        public string? FilePath { get; set; }
        public string? FileName { get; set; }
        public string? SubmitFilePath { get; set; }
        public string? SubmitFileName { get; set; }
        public string? FullName { get; set; }
        public string? SubmissionDate { get; set; }
        public string? FeedBack { get; set; }
        public string? SectionName { get; set; }
        public string? SessionYearName { get; set; }
        public string? ClassName { get; set; }
        public string? BranchName { get; set; }
        public string? SubjectName { get; set; }
        public string? Title { get; set; }

    }
    public class StudentAssignStatus
    {
        public int Id { get; set; }
        public string StatusId { get; set; }
        public string? FeedBack { get; set; }
        public string? SessionYearName { get; set; }
        public string? BranchName { get; set; }
        public string? ClassName { get; set; }
        public string? SectionName { get; set; }
        public string? SubjectName { get; set; }
        public string? FullName { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
    }

   
    public class TeacherAnnouncement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SessionYearId { get; set; }
        public int BranchClassId { get; set; }
        public int BranchId { get; set; }
        public int ClassId { get; set; }
        public int ClasssubjectId { get; set; }
        public int SubjectId { get; set; }
        public string Title { get; set; }
        [NotMapped]
        public IFormFile? AssignmentFile { get; set; }
        public string? FileType { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public string? Description { get; set; }
        public string? IsActive { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        //[NotMapped]
        //public List<SelectListItem>? SessionYearSheet { get; set; }
        //[NotMapped]
        //public List<SelectListItem>? BranchSheet { get; set; }
        [NotMapped]
        public List<SelectListItem>? BranchClassSheet { get; set; }
        [NotMapped]
        public string? SectionIds { get; set; }
        [NotMapped]
        public List<SelectListItem>? SubjectSheet { get; set; }
    }   
    public class TeacherAnnouncementSection
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int TeacherAnnouncementId { get; set; }
        public int SessionYearId { get; set; }
        public int BranchClassId { get; set; }
        public int BranchId { get; set; }
        public int ClassId { get; set; }
        public int ClasssubjectId { get; set; }
        public int SubjectId { get; set; }
        public int SectionId { get; set; }
        public string? IsActive { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }

    }    
    public class VTeacherAnnouncement
    {
        [Key]
        public int Id { get; set; }
        public int SessionYearId { get; set; }
        public int BranchClassId { get; set; }
        public int BranchId { get; set; }
        public int ClassId { get; set; }
        public int ClasssubjectId { get; set; }
        public int SubjectId { get; set; }
        public string Title { get; set; }
        public string? StrCreatedDate { get; set; }
        public string? BranchName { get; set; }
        public string? ClassName { get; set; }
        public string? Description { get; set; }
        public string? FileType { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public string? SessionYearName { get; set; }
        public string? SubjectName { get; set; }
        public string? SectionName { get; set; }

    }
    public class Expense
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int SessionYearId { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public string? ReferenceNo { get; set; }
        public string? Description { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal Amount { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ExpenseDate { get; set; }
        public string? IsActive { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        //[NotMapped]
        //public List<SelectListItem>? BranchSheet { get; set; }
        [NotMapped]
        public List<SelectListItem>? ExpenseCategorySheet { get; set; }
        //[NotMapped]
        //public List<SelectListItem>? SessionYearSheet { get; set; }

    }
    public class VExpense
    {
        [Key]
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int SessionYearId { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public decimal Amount { get; set; }
        public string? StrExpenseDate { get; set; }
        public string? SessionYearName { get; set; }
        public string? ReferenceNo { get; set; }
        public string? Description { get; set; }
        public string? CategoryName { get; set; }
        public string? BranchName { get; set; }
    }
    public class SearchExpenseParams
    {
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public int? SessionYearId { get; set; }
        public List<SelectListItem>? SessionYearSheet { get; set; }
    }
    public class SearchReceivableParams
    {
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public int? SessionYearId { get; set; }
        public List<SelectListItem>? SessionYearSheet { get; set; }
    }
    public class ReceivableSummaryId
    {        
        public int Id { get; set; }
       
    }

    public class LeaveRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SessionYearId { get; set; }
        public int BranchId { get; set; }
        public int PersonnelId { get; set; }
        public string LeaveRequestType { get; set; }
        public string PersonnelType { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FromDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ToDate { get; set; }
        public string StatusId { get; set; }
        public string Reason { get; set; }
        public string? Comment { get; set; }
        public string? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        [NotMapped]
        public List<LeaveRequestDate>? LstLeaveRequestDate { get; set; }
    }   
    public class LeaveRequestDate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int LeaveRequestId { get; set; }
        public string LeaveType { get; set; }
        public DateTime LeaveDate { get; set; }
        public string? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
    }  
    public class VLeaveRequest
    {
        [Key]
        public int? Id { get; set; }
        public int? SessionYearId { get; set; }
        public string? SessionYearName { get; set; }
        public int? PersonnelId { get; set; }
        public int? CreatedByUserId { get; set; }
        public string? LeaveRequestType { get; set; }
        public string? LeaveRequestTypeName { get; set; }
        public string? PersonnelType { get; set; }
        public string? PersonnelTypeName { get; set; }
        public int? BranchId { get; set; }
        public string? BranchName { get; set; }
        public string? StatusId { get; set; }
        public int? LeaveRequestCount { get; set; }
        public decimal? DayCount { get; set; }
        public string? StrCreatedDate { get; set; }
        public string? StrFromDate { get; set; }
        public string? StrToDate { get; set; }
        public string? StrModifiedDate { get; set; }
        public string? Reason { get; set; }
        public string? Comment { get; set; }
        public string? StatusColor { get; set; }
        [NotMapped]
        public string? StatusName { get; set; }
        [NotMapped]
        public int? RequestId { get; set; }
        [NotMapped]
        public int? NotificationId { get; set; }
    }
    public class VLeaveRequestDate
    {
        [Key]
        public int Id { get; set; }
        public int LeaveRequestId { get; set; }
        public string LeaveType { get; set; }
        public int? BranchId { get; set; }
        public int? PersonnelId { get; set; }
        public string? PersonnelType { get; set; }
        public string? StatusId { get; set; }
        public string? StrLeaveDate { get; set; }
        public int? SessionYearId { get; set; }
        public string? Reason { get; set; }
    }
    public class CastLeaveRequest
    {
        [Key]
        public int? RequestId { get; set; }
        public int? NotificationId { get; set; }
        public string? StatusId { get; set; }
        public string? Comment { get; set; }
    }
    public class FeeType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
    }   
    public class VFeeType
    {
        [Key]
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? FeeCount { get; set; }
        public string? IsDelete { get; set; }
    }   
    public class ExpenseCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
    }   
    public class VExpenseCategory
    {
        [Key]
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? ExpenseCount { get; set; }
        public string? IsDelete { get; set; }
    }
    public class Lesson
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ClassSubjectId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        [NotMapped]
        public int? ClassId { get; set; }
        [NotMapped]
        public int? SubjectId { get; set; }
        [NotMapped]
        public List<SelectListItem>? ClassSheet { get; set; }

        [NotMapped]
        public List<SelectListItem>? SubjectSheet { get; set; }
        [NotMapped]
        public bool IsSelected { get; set; }
    }
    public class VLesson
    {

        [Key]
        public int? Id { get; set; }
        public int? SessionYearId { get; set; }
        public int? ClassSubjectId { get; set; }
        public int? SubjectId { get; set; }
        public int? ClassId { get; set; }
        public int? ExaminationCount { get; set; }
        public int? QuestionBankCount { get; set; }
        public int? TopicCount { get; set; }
        public string? Name { get; set; }
        public string? ClassName { get; set; }
        public string? SubjectName { get; set; }
        public string? Description { get; set; }
        public string? IsDelete { get; set; }
        [NotMapped]
        public bool IsSelected { get; set; }
    }
    public class EnquiryStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Color { get; set; }
        public string? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
    }  
    public class VEnquiryStatus
    {
        [Key]
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Color { get; set; }
        public int? EnquiryCount { get; set; }
        public string? IsDelete { get; set; }
    }
    public class ReferenceAdmission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
    }
    public class VReferenceAdmission
    {
        [Key]
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? StudentEnquiryCount { get; set; }
        //public int? StudentCount { get; set; }
        public string? IsDelete { get; set; }
    }    
    public class StudentEnquiry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int ClassId { get; set; }
        public int SessionYearId { get; set; }
        public int StatusId { get; set; }
        public int? ReferenceId { get; set; }
        public string Name { get; set; }
        public string? FatherName { get; set; }
        public string? MotherName { get; set; }
        public string? MobileNo { get; set; }
        public string? Age { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? Comments { get; set; }
        public string? PreviousSchool { get; set; }
        public string? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        //[NotMapped]
        //public List<SelectListItem>? BranchSheet { get; set; }
        [NotMapped]
        public List<SelectListItem>? ClassSheet { get; set; }
        [NotMapped]
        public List<SelectListItem>? StatusSheet { get; set; }
        [NotMapped]
        public List<SelectListItem>? ReferenceSheet { get; set; }
    }   
    public class VStudentEnquiry
    {
        [Key]
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int ClassId { get; set; }
        public int SessionYearId { get; set; }
        public int StatusId { get; set; }
        public int? ReferenceId { get; set; }
        public string Name { get; set; }
        public string? FatherName { get; set; }
        public string? MotherName { get; set; }
        public string? MobileNo { get; set; }
        public string? Age { get; set; }
        public string? GenderName { get; set; }
        public string? EnquiryStatusName { get; set; }
        public string? EnquiryStatusColor { get; set; }
        public string? EnquiryReferenceName { get; set; }
        public string? EnquiryDate { get; set; }
        public string? BranchName { get; set; }
        public string? ClassName { get; set; }
        public string? SessionYearName { get; set; }
        public string? Address { get; set; }
        public string? Comments { get; set; }
    }
    public class StudentTransferSection
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SessionYearId { get; set; }
        public int StudentId { get; set; }
        public int FromBranchId { get; set; }
        public int FromClassId { get; set; }
        public int FromSectionId { get; set; }
        public int ToBranchId { get; set; }
        public int ToClassId { get; set; }
        public int ToSectionId { get; set; }
        public string RollNo { get; set; }
        public string TransferType { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
    public class CastStudentTransfer
    {
        public int StudentId { get; set; }
        public string? StudentName { get; set; }
        public string? RollNo { get; set; }
        public bool IsSelected { get; set; }
    }
    public class StudentTransferVM
    {
        public int SessionYearId { get; set; }
        public List<SelectListItem> FromBranchSheet { get; set; }
        public List<SelectListItem> FromClassSheet { get; set; }
        public List<SelectListItem> FromSectionSheet { get; set; }
        public List<SelectListItem> ToBranchSheet { get; set; }
        public List<SelectListItem> ToClassSheet { get; set; }
        public List<SelectListItem> ToSectionSheet { get; set; }
    }
    public class TeacherTransferBranch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SessionYearId { get; set; }
        public int TeacherId { get; set; }
        public int FromBranchId { get; set; }
        public int ToBranchId { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
    public class CastTeacherTransfer
    {
        public int TeacherId { get; set; }
        public string? TeacherName { get; set; }
        public string? EmployeeNo { get; set; }
        public bool IsSelected { get; set; }
    }
    public class TeacherTransferVM
    {
        public int SessionYearId { get; set; }
        public List<SelectListItem> FromBranchSheet { get; set; }
        public List<SelectListItem> ToBranchSheet { get; set; }
    }
    public class Attachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        [NotMapped]
        public IFormFile? File { get; set; }
        public string? FileType { get; set; }
        public string? FilePath { get; set; }
        public string? FileName { get; set; }
        public string? AttachType { get; set; }
        public int? ReferenceId { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string? IsActive { get; set; }
    }
    public class VAttachment
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? FileTypeName { get; set; }
        public string? FileName { get; set; }
        public string? AttachType { get; set; }
        public int? ReferenceId { get; set; }
    }
    public class SessionYear
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]        
        public DateTime FromDate { get; set; }
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ToDate { get; set; }
        public string? Description { get; set; }
        public string? IsActive { get; set; }
        public bool IsDefault { get; set; }
        [NotMapped]
        public bool IsDefaultExist { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        [NotMapped]
        public string? StrFromDate { get; set; }
        [NotMapped]
        public string? StrToDate { get; set; }
    }
    public class LoginUser
    {
        public string UserName { get; set; }
        public string Password { get; set; }       
    }
    public class VUserInfo
    {
        [Key]
        public int Id { get; set; }
        public int PersonId { get; set; }
        public int? SessionYearId { get; set; }
        public string UserType { get; set; }
        public string UserName { get; set; }
        public int? BranchId { get; set; }
        public int? BranchClassId { get; set; }
        public string? BranchName { get; set; }
        public int? ClassId { get; set; }
        public string? ClassName { get; set; }
        public int? SectionId { get; set; }
        public string? SectionName { get; set; }
        public string? EmployeeNo { get; set; }
        public string? FullName { get; set; }
        public string? UserTypeName { get; set; }
        public string? FilePath { get; set; }
        public DateTime? LastLoggedIn { get; set; }
        public string? HasLogin { get; set; }
        [NotMapped]
        public List<string> LstPages { get; set; }
        [NotMapped]
        public int? NotiCount { get; set; }
        [NotMapped]
        public string Vapidkey { get; set; }
    }
    public class PushPayLoad
    {
        public string Title { get; set; }
        public string Message { get; set; }
    }
    public class UserParam
    {       
        public int UserId { get; set; }
        public int PersonId { get; set; }
        public int SessionYearId { get; set; }
        public string UserType { get; set; }       
        public int BranchId { get; set; }       
        public int ClassId { get; set; }       
        public int SectionId { get; set; }       

    }
    public class VBranch
    {
        [Key]
        public int? Id { get; set; }
        public int? AdminId { get; set; }
        public string? AdminName { get; set; }
        public string? PhoneNo { get; set; }
        public string? EmailAddress { get; set; }
        public string? Name { get; set; }
        public string? Theme { get; set; }
        public string? Address { get; set; }
        public int? ClassCount { get; set; }
        public int? SectionCount { get; set; }
        public int? StudentCount { get; set; }
        public int? TeacherCount { get; set; }
        public int? ScheduleCount { get; set; }
        public string? IsDelete { get; set; }
        [NotMapped]
        public List<VBranchClass> LstBranchClass { get; set; }
    }
    public class CastTeacher
    {
        public int TeacherId { get; set; }
        public string? TeacherName { get; set; }
        public bool IsSelected { get; set; }
    }
    public class CastSubjectTeacher
    {
        public int ClassSubjectId { get; set; }
        public string? SubjectName { get; set; }
        public string? SubjectColor { get; set; }
        public int? TeacherId { get; set; }
        public List<SelectListItem>? TeacherSheet { get; set; }
    }
    public class BranchClassVM
    {
        public int BranchClassId { get; set; }
        public List<CastTeacher> LstCastTeacher { get; set; }
        public List<CastSubjectTeacher> LstCastSubjectTeacher { get; set; }
    }

  
    public class Branch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int? AdminId { get; set; }
        public string? PhoneNo { get; set; }
        public string? EmailAddress { get; set; }
        public string? Theme { get; set; }
        public string? Address { get; set; }
        public string? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        [NotMapped]
        public List<SelectListItem>? AdminSheet { get; set; }

    }

    public class ClassVM
    {
        public int? BranchId { get; set; }
        public List<CastClass> LstClass { get; set; }
    }
    public class CastClass
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool IsSelected { get; set; }
        public string? IsDelete { get; set; }
    }
    public class Class
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int MediumId { get; set; }       
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string? Description { get; set; }
        public string? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        [NotMapped]
        public bool? IsSelected { get; set; }
        [NotMapped]
        public string? IsDelete { get; set; }
        [NotMapped]
        public List<SelectListItem>? MediumSheet { get; set; }       

    }

    public class Organization
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }

        [NotMapped]
        public IFormFile? LogoFile { get; set; }
        [NotMapped]
        public IFormFile? HeaderFile { get; set; }

        //[NotMapped]
        //public byte[] LogoFileData { get; set; }
        //[NotMapped]
        //public byte[] HeaderFileData { get; set; }
        public string? Theme { get; set; }
        public string? Address { get; set; }
        public string? LogoPhotoPath { get; set; }
        public string? LogoFileName { get; set; }
        public string? HeaderPhotoPath { get; set; }
        public string? HeaderFileName { get; set; }
        public int? CreatedByUserId { get; set; }
        // 
        public DateTime? CreatedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        // 
        public DateTime? LastModifiedDate { get; set; }
        public string? IsActive { get; set; }
    }    
    public class VClass
    {
        [Key]
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? ShortName { get; set; }
        public int? MediumId { get; set; }
        public string? MediumName { get; set; }       
        public string? Description { get; set; }
        public int? SubjectCount { get; set; }
        public int? BranchClassCount { get; set; }
        public string IsDelete { get; set; }

    }
    public class Activity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string ActivityColor { get; set; }
        public string? Description { get; set; }
        public string? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        [NotMapped]
        public bool IsSelected { get; set; }
        [NotMapped]
        public string? IsDelete { get; set; }

    }
    public class PeriodBreak
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string BreakColor { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public string? Description { get; set; }
        public string? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        [NotMapped]
        public bool IsSelected { get; set; }
        [NotMapped]
        public string? IsDelete { get; set; }

    }
    public class Subject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public int MediumId { get; set; }
        public string SubjectType { get; set; }
        public string SubjectColor { get; set; }

        public string? SubjectCode { get; set; }
        public string? Description { get; set; }
        public string? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        [NotMapped]
        public bool IsSelected { get; set; }
        [NotMapped]
        public string? IsDelete { get; set; }
        [NotMapped]
        public List<SelectListItem>? MediumSheet { get; set; }
    }
    public class SubjectVM
    {
        public int? ClassId { get; set; }
        public List<CastSubject> LstSubject { get; set; }
    }
    public class CastSubject
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? SubjectColor { get; set; }
        public bool IsSelected { get; set; }
        public string? IsDelete { get; set; }

    }
    public class Section
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int BranchClassId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public int? MaxCount { get; set; }
        public string? Description { get; set; }
        public string? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        [NotMapped]
        public int? BranchId { get; set; }
        [NotMapped]
        public int? ClassId { get; set; }
        [NotMapped]
        public List<SelectListItem>? BranchSheet { get; set; }
        [NotMapped]
        public List<SelectListItem>? ClassSheet { get; set; }

    }
    public class VSection
    {
        [Key]
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? ShortName { get; set; }
        public int? BranchClassId { get; set; }
        public int? MaxCount { get; set; }
        public int? BranchId { get; set; }
        public string? BranchName { get; set; }
        public int? ClassId { get; set; }
        public string? ClassName { get; set; }
        public string? ClassShortName { get; set; }
        public string? Description { get; set; }
        public int? StudentCount { get; set; }
        public int? ClassSubjectCount { get; set; }
        public int? SubjectCount { get; set; }
        public int? TimeTableCount { get; set; }
        public string? ClassTeachers { get; set; }
        public string? IsDelete { get; set; }
        public string? SlotDuration { get; set; }
        public string? FromTime { get; set; }
        public string? ToTime { get; set; }
        [NotMapped]
        public bool IsSelected { get; set; }

    }
    public class Student
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PersonnelType { get; set; }

        public string? Gender { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfBirth { get; set; }
        public int? BloodGroupId { get; set; }

        [NotMapped]
        public int? BranchId { get; set; }
        [NotMapped]
        public int? BranchClassId { get; set; }
        [NotMapped]
        public int? ClassId { get; set; }
        [NotMapped]
        public int? SectionId { get; set; }
        [NotMapped]
        public IFormFile? File { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }

        public string? IsActive { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        [NotMapped]
        public List<SelectListItem>? BloodGroupSheet { get; set; }
        public StudentAdmission StudentAdmission { get; set; }
        public StudentOtherDetail StudentOtherDetail { get; set; }

        [NotMapped]
        public List<SelectListItem>? ClassSheet { get; set; }
        [NotMapped]
        public List<SelectListItem>? SectionSheet { get; set; }
        //[NotMapped]
        //public List<SelectListItem>? BranchSheet { get; set; }
        [NotMapped]
        public string? IsDelete { get; set; }
        [NotMapped]
        public bool IsSelected { get; set; }
    }
    public class StudentAdmission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int BranchId { get; set; }
        public int BranchClassId { get; set; }
        public int ClassId { get; set; }
        public int SectionId { get; set; }
        public int SessionYearId { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? AdmissionDate { get; set; }
        public string? AdmissionNo { get; set; }
        public string? RollNo { get; set; }
        public string? IsActive { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }

    }
    public class StudentOtherDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string? FatherName { get; set; }
        public string? MotherName { get; set; }
        public string? DefaultMobileNo { get; set; }
        public string? OtherMobileNo { get; set; }
        public string? CurrentAddress { get; set; }
        public string? PermanentAddress { get; set; }
        public string? IsActive { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }    
    public class StudentFee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int FeeStructureId { get; set; }
        public int BranchId { get; set; }
        public int SessionYearId { get; set; }
        public int BranchClassId { get; set; }
        public int ClassId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? RequestDiscountAmount { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string? IsActive { get; set; }

    }
    public class PayMode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string? IsActive { get; set; }
    }
    public class VPayMode
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string IsDelete { get; set; }
        public string? Description { get; set; }
        public int? PayModeCount { get; set; }
    }
    public class StudentPay
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? ReceiptNo { get; set; }
        public int StudentId { get; set; }
        public int StudentFeeId { get; set; }
        public int FeeStructureId { get; set; }
        public int BranchId { get; set; }
        public int SessionYearId { get; set; }
        public int BranchClassId { get; set; }
        public int ClassId { get; set; }
        public decimal Amount { get; set; }
        public int PayModeId { get; set; }
        public string? ReferenceNo { get; set; }
        public string? Remarks { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        [NotMapped]
        public string? StudentName { get; set; }
        [NotMapped]
        public string? AdmissionNo { get; set; }
        [NotMapped]
        public string? RollNo { get; set; }
        [NotMapped]
        public string? Age { get; set; }
        [NotMapped]
        public string? Gender { get; set; }
        [NotMapped]
        public string? MobileNo { get; set; }
        [NotMapped]
        public string? BranchName { get; set; }
        [NotMapped]
        public string? ClassName { get; set; }
        [NotMapped]
        public string? SectionName { get; set; }
        [NotMapped]
        public decimal? DueAmount { get; set; }
        [NotMapped]
        public List<SelectListItem>? PayModeSheet { get; set; }

    }
    public class MbStudentPay
    {
        public int StudentId { get; set; }
        public decimal? FinalAmount { get; set; }
        public decimal? PaidAmount { get; set; }
        public decimal? DueAmount { get; set; }
        public List<VStudentPay> LstStudentPay { get; set; }
    }
    public class VStudentPay
    {       
        public int Id { get; set; }
        public string ReceiptNo { get; set; }
        public int StudentId { get; set; }
        public int StudentFeeId { get; set; }
        public int FeeStructureId { get; set; }
        public int BranchId { get; set; }
        public int SessionYearId { get; set; }
        public int BranchClassId { get; set; }
        public int ClassId { get; set; }
        public decimal Amount { get; set; }
        public decimal DueAmount { get; set; }
        public int PayModeId { get; set; }
        public string? ReferenceNo { get; set; }
        public string? Remarks { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? PayModeName { get; set; }
        public string? StrCreatedDate { get; set; }

    }
    public class StudentVM
    {
        public int? SectionId { get; set; }
        public List<Student> LstStudent { get; set; }
    }
    public class ExamTime
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? FromTime { get; set; }
        public string? ToTime { get; set; }
        public string? Description { get; set; }
        public string? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        [NotMapped]
        public bool IsSelected { get; set; }
    }
    public class VExamTime
    {
        [Key]
        public int? Id { get; set; }
        public string? FromTime { get; set; }
        public string? ToTime { get; set; }
        public string? Description { get; set; }
        public int? ScheduleCount { get; set; }
        public string IsDelete { get; set; }
    }
    public class ExamType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? IsActive { get; set; }        
        public DateTime? CreatedDate { get; set; }
        public int? CreatedByUserId { get; set; }        
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
    }
    public class VExamType
    {
        [Key]
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? ScheduleCount { get; set; }
        public string IsDelete { get; set; }
    }
    public class BloodGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
    }
    public class VBloodGroup
    {
        [Key]
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? StudentCount { get; set; }
        public int? TeacherCount { get; set; }
        public string IsDelete { get; set; }
    }
    public class Grade
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal MinPercent { get; set; }
        public decimal MaxPercent { get; set; }
        public string? Color { get; set; }
        public string? Description { get; set; }
        public int? CreatedByUserId { get; set; }        
        public DateTime? CreatedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string? IsActive { get; set; }
    }
    public class Holiday
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        public string? IsActive { get; set; }
        [NotMapped]
        public List<SelectListItem>? ExamTypeSheet { get; set; }
        [NotMapped]
        public List<VBranch>? LstBranch { get; set; }
    }
    public class VHoliday
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? StrStartDate { get; set; }
        public string? StrEndDate { get; set; }
    }
    public class Topic
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ClassSubjectId { get; set; }
        public int LessonId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        public string? IsActive { get; set; }
        [NotMapped]
        public int? ClassId { get; set; }
        [NotMapped]
        public int? SubjectId { get; set; }
        [NotMapped]
        public List<SelectListItem>? ClassSheet { get; set; }
        [NotMapped]
        public List<SelectListItem>? SubjectSheet { get; set; }
        [NotMapped]
        public List<SelectListItem>? LessonSheet { get; set; }
        [NotMapped]
        public bool IsSelected { get; set; }
    }
    public class VTopic
    {
        [Key]
        public int Id { get; set; }
        public int LessonId { get; set; }
        public string Name { get; set; }
        public int? ClassSubjectId { get; set; }
        public int? ClassId { get; set; }
        public int? SubjectId { get; set; }
        public string? LessonName { get; set; }
        public string? ClassName { get; set; }
        public string? SubjectName { get; set; }
        public string? Description { get; set; }

    }
    public class VActivities
    {
        [Key]
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? ShortName { get; set; }
        public string? ActivityColor { get; set; }
        public string? Description { get; set; }
        public int? BranchActivityCount { get; set; }
        public string IsDelete { get; set; }

    }
    public class VPeriodBreaks
    {
        [Key]
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? ShortName { get; set; }
        public string? BreakColor { get; set; }
        public string? FromTime { get; set; }
        public string? ToTime { get; set; }
        public string? Description { get; set; }
        public int? BranchPeriodBreakCount { get; set; }
        public string IsDelete { get; set; }

    }
    public class VSubject
    {
        [Key]
        public int? Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public int? MediumId { get; set; }
        public string? MediumName { get; set; }
        public string? Description { get; set; }
        public string? SubjectType { get; set; }
        public string? SubjectCode { get; set; }
        public string? SubjectColor { get; set; }
        public string? IsDelete { get; set; }
        public int? ClassSubjectCount { get; set; }
    }
    public class Shift
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public string? Description { get; set; }
        public string? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        [NotMapped]
        public bool IsSelected { get; set; }

    }
    public class VShift
    {
        [Key]
        public int? Id { get; set; }
        public string? FromTime { get; set; }
        public string? ToTime { get; set; }
        public string? Description { get; set; }
        public int? ClassCount { get; set; }
        public string? IsDelete { get; set; }
    }
    public class Medium
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? IsActive { get; set; }     
        public DateTime? CreatedDate { get; set; }
        public int? CreatedByUserId { get; set; }      
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
    }
    public class VMedium
    {
        [Key]
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? ClassCount { get; set; }
        public int? SubjectCount { get; set; }
        public string? IsDelete { get; set; }
    }
    public class QuestionType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? IsActive { get; set; }       
        public DateTime? CreatedDate { get; set; }
        public int? CreatedByUserId { get; set; }   
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }

    }
    public class VQuestionType
    {
        [Key]
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? QuestionBankCount { get; set; }
        public string? IsDelete { get; set; }
    }
    public class QuestionDifficulty
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedByUserId { get; set; } 
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
    }
    public class VQuestionDifficulty
    {
        [Key]
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? QuestionBankCount { get; set; }
        public string IsDelete { get; set; }
    }
    public class QuestionCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedByUserId { get; set; }   
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
    }
    public class VQuestionCategory
    {
        [Key]
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? QuestionBankCount { get; set; }
        public string IsDelete { get; set; }
    }
    public class VStudentId
    {
        public int Id { get; set; }
    }
    public class AttachModel
    {
        public int Type { get; set; }
        public int ReferId { get; set; }
    }
    public class VTeacherId
    {
        public int Id { get; set; }
    }
    public class VStudent
    {
        [Key]
        public int? Id { get; set; }
        public int SessionYearId { get; set; }
        public string ActiveStatus { get; set; }
        public string? AdmissionDate { get; set; }
        public string? AdmissionNo { get; set; }
        public int? BranchClassId { get; set; }
        public int? BranchId { get; set; }
        public string? BranchName { get; set; }
        public string? RollNo { get; set; }
        public string? FullName { get; set; }
        public string? FatherName { get; set; }
        public string? MotherName { get; set; }
        public string? Age { get; set; }
        public string? GenderName { get; set; }
        public string? DefaultMobileNo { get; set; }
        public string? BloodGroupName { get; set; }
        public string? DateOfBirth { get; set; }
        public int? ClassId { get; set; }
        public string? ClassName { get; set; }
        public int? SectionId { get; set; }
        public int? FeeStructureId { get; set; }
        public int? StudentFeeId { get; set; }
        public string? SectionName { get; set; }
        public string? FilePath { get; set; }
        public int? ScheduleCount { get; set; }
        public int? WorkingDays { get; set; }
        public int? PresentDays { get; set; }
        public decimal? DueAmount { get; set; }
        public decimal? AttPercent { get; set; }
        public string? IsDelete { get; set; }
        public string? StrSpark { get; set; }
        [NotMapped]
        public List<decimal?> LstSpark { get; set; }
        [NotMapped]
        public bool IsSelected { get; set; }
        [NotMapped]
        public string? Remarks { get; set; }
    }
    
    public class VExamTimeTable
    {
        [Key]
        public int Id { get; set; }
        public int SessionYearId { get; set; }
        public int ScheduleId { get; set; }
        public int BranchClassId { get; set; }
        public int ClassId { get; set; }
        public int SubjectId { get; set; }
        public string? ClassName { get; set; }
        public string? SubjectName { get; set; }
        public string? StrExamDate { get; set; }
        public string? StrExamTime { get; set; }
        [NotMapped]
        public List<VExaminationLessons> LstExaminationLesson { get; set; }

    }
    public class VExaminationLessons
    {
        [Key]
        public int Id { get; set; }
        public int SessionYearId { get; set; }
        public int ExamId { get; set; }

        public int LessonId { get; set; }

        public string LessonName { get; set; }

    }
    public class VStudentMarkResult
    {

        [Key]
        public int Id { get; set; }
        public int SessionYearId { get; set; }
        public int StudentId { get; set; }
        public string RollNo { get; set; }
        public string? FullName { get; set; }
        public int ScheduleId { get; set; }
        public int BranchClassId { get; set; }
        public int BranchId { get; set; }
        public int ClassId { get; set; }
        public int SectionId { get; set; }
        public int SubjectId { get; set; }
        public int ExamId { get; set; }
        public int? YearNo { get; set; }
        public int? MonthNo { get; set; }
        public string Marks { get; set; }
        public string? SubjectName { get; set; }
        public string MaxMarks { get; set; }
        public string? StrExamDate { get; set; }
        public string? StrExamMonth { get; set; }
        public decimal? MrkPercent { get; set; }
        public string? GradeColor { get; set; }

        [NotMapped]
        public List<VExaminationLessons> LstExaminationLesson { get; set; }

    }
    public class StudentMarkGraphVM
    {
        public List<string> labels { get; set; }
        public List<StudentMarkChildVM> datasets { get; set; }
    }
    public class StudentMarkChildVM
    {
        public string label { get; set; }
        public List<string> backgroundColor { get; set; }
        public int borderWidth { get; set; }
        public bool fill { get; set; }
        public List<int?> data { get; set; }
    }
    public class VStudentScheduleResult
    {

        [Key]
        public int Id { get; set; }
        public int SessionYearId { get; set; }
        public int StudentId { get; set; }        
        public int ScheduleId { get; set; }
        public int? ScheduleYearNo { get; set; }
        public int? ScheduleMonthNo { get; set; }
        public string? ExamTypeName { get; set; }

        public string? ScheduleMonth { get; set; }

        public double? Marks { get; set; }

        public double? MaxMarks { get; set; }

        public decimal? MrkPercent { get; set; }

        public string? GradeName { get; set; }
        public string? GradeColor { get; set; }

        [NotMapped]
        public List<VStudentMarkResult> LstStudentMarkResult { get; set; }
        [NotMapped]
        public StudentMarkGraphVM StudentMarkGraph { get; set; }

    }
    public class ScheduleLabel
    {
        public decimal? Percent { get; set; }
        public string Strlabel { get; set; }
        public int? ScheduleId { get; set; }

    }
    public class StudentScheduleChildVM
    {
        public string label { get; set; }
        public List<string> backgroundColor { get; set; }
        public int borderWidth { get; set; }
        public bool fill { get; set; }
        // public List<decimal?> data { get; set; }
        public List<ScheduleLabel> data { get; set; }
    }
    public class StudentScheduleGraphVM
    {
        public List<string> labels { get; set; }
        public List<StudentScheduleChildVM> datasets { get; set; }
    }
    public class StudentScheduleVM
    {
        public List<VStudentScheduleResult> LstStudentScheduleResult { get; set; }
        public StudentScheduleGraphVM StudentScheduleGraph { get; set; }
    }
    public class StudentScheduleResultVM
    {
        public List<VStudentScheduleResult> LstStudentScheduleResult { get; set; }
        public StudentScheduleGraphVM StudentScheduleGraph { get; set; }
    }
    public class StudentMarkVM
    {
        public VStudentScheduleResult StudentScheduleResult { get; set; }
        //public StudentMarkGraphVM StudentMarkGraph { get; set; }
    }
    public class StudentMarkSubjectGraphVM
    {
        public List<string> labels { get; set; }
        public List<StudentMarkSubjectChildVM> datasets { get; set; }
    }
    public class StudentMarkSubjectChildVM
    {
        public string label { get; set; }
        public List<string> backgroundColor { get; set; }
        public int borderWidth { get; set; }
        public bool fill { get; set; }
        public List<decimal?> data { get; set; }
    }
    public class StudentMarkSubjectVM
    {
        public List<VStudentMarkResult> LstStudentMarkResult { get; set; }
        public StudentMarkSubjectGraphVM StudentMarkSubjectGraph { get; set; }
    }
    //public class Teacher
    //{
    //    [Key]
    //    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //    public int Id { get; set; }
    //    public int BranchId { get; set; }
    //    public string? EmployeeNo { get; set; }
    //    public string FirstName { get; set; }
    //    public string LastName { get; set; }
    //    public string? Age { get; set; }
    //    public string? Gender { get; set; }
    //    public string? MobileNo { get; set; }
    //    public string? Designation { get; set; }
    //    public string? Address { get; set; }
    //    //[NotMapped]
    //    //public IFormFile File { get; set; }
    //    [NotMapped]
    //    public byte[] PhotoFileData { get; set; }
    //    public string? PhotoPath { get; set; }
    //    public string? FileName { get; set; }
    //    public int? CreatedByUserId { get; set; }
    //    // 
    //    public DateTime? CreatedDate { get; set; }
    //    public int? ModifiedByUserId { get; set; }
    //    // 
    //    public DateTime? ModifiedDate { get; set; }
    //    public string? IsActive { get; set; }
    //    [NotMapped]
    //    public List<SelectListItem> BranchSheet { get; set; }
    //}
    public class VTeacher
    {
        [Key]
        public int? Id { get; set; }
        public string? PersonnelType { get; set; }
        public string TransferType { get; set; }
        public string? TeacherClassName { get; set; }
        public string? TeacherSubjectName { get; set; }
        public int? BranchId { get; set; }
        public string? BranchName { get; set; }
        public string? EmployeeNo { get; set; }
        public string? FullName { get; set; }
        public string? FilePath { get; set; }
        public string? Age { get; set; }
        public string? BloodGroupName { get; set; }
        public string? DateOfBirth { get; set; }
        public string? GenderName { get; set; }
        public string? DefaultMobileNo { get; set; }
        public string? Qualification { get; set; }
        public string? Designation { get; set; }
        public string? EmailAddress { get; set; }

        public int? ClassSubjectTeacherCount { get; set; }
        public int? ClassTeacherCount { get; set; }
        public int? ScheduleCount { get; set; }
        public string? IsDelete { get; set; }
        public string ActiveStatus { get; set; }
        public int? PresentDays { get; set; }
        public decimal? AttPercent { get; set; }
        public int? WorkingDays { get; set; }


        [NotMapped]
        public int? ScheduleId { get; set; }
        [NotMapped]
        public int? ScheduleExamDateId { get; set; }
        [NotMapped]
        public int? ScheduleSectionId { get; set; }
        [NotMapped]
        public List<SelectListItem> ScheduleSectionSheet { get; set; }
        [NotMapped]
        public bool IsSelected { get; set; }
    }
    public class VClassSubject
    {
        [Key]
        public int? Id { get; set; }
        public int? ClassId { get; set; }
        public int? SubjectId { get; set; }
        public string? ClassName { get; set; }
        public string? SubjectName { get; set; }
        public string? SubjectColor { get; set; }
        public string? IsDelete { get; set; }
        [NotMapped]
        public List<VLesson>? LstLesson { get; set; }
        [NotMapped]
        public string? StrExamDate { get; set; }
        [NotMapped]
        public int? ScheduleId { get; set; }
        [NotMapped]
        public int? BranchClassId { get; set; }
        [NotMapped]
        public List<SelectListItem>? ExamDateSheet { get; set; }
        [NotMapped]
        public List<SelectListItem>? ExamTimeSheet { get; set; }
        [NotMapped]
        public int? ExamTimeId { get; set; }
        //[NotMapped]
        //public string StrFromTime { get; set; }
        //[NotMapped]
        //public string StrToTime { get; set; }
        //[NotMapped]
        //public string StrMarks { get; set; }
        [NotMapped]
        public bool IsSelected { get; set; }
    }
    public class BranchClass
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int ClassId { get; set; }
        public int? ShiftId { get; set; }
        public string? SlotDuration { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        [NotMapped]
        public List<SelectListItem>? ShiftSheet { get; set; }
    }
    public class VBranchClass
    {
        [Key]
        public int? Id { get; set; }
        public int? BranchId { get; set; }
        public int? ClassId { get; set; }
        public int? SectionCount { get; set; }
        public int? StudentCount { get; set; }
        public int? PeriodBreakCount { get; set; }
        //public int? ScheduleCount { get; set; }
        //public int? ClassTeacherCount { get; set; }
        //public int? SubjectTeacherCount { get; set; }
        public string? BranchName { get; set; }
        public string? IsDelete { get; set; }

        public string? ClassName { get; set; }
        public int? ShiftId { get; set; }
        public string? ShiftName { get; set; }
        public string? SlotDuration { get; set; }

        [NotMapped]
        public bool? IsSelected { get; set; }
       
        [NotMapped]
        public List<VClassSubject>? LstClassSubject { get; set; }
        //[NotMapped]
        //public string Marks { get; set; }
        [NotMapped]
        public string? AttStatusName { get; set; }
        [NotMapped]
        public string? AttStatusColor { get; set; }

    }
    public class PeriodBreakVM
    {
        public int? BranchId { get; set; }
        public int? BranchClassId { get; set; }
        public int? ClassId { get; set; }
        public List<CastPeriodBreak> LstPeriodBreak { get; set; }
    }
    public class CastPeriodBreak
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? SubjectColor { get; set; }
        public bool IsSelected { get; set; }
        public string? IsDelete { get; set; }

    }
    public class ClassSubjectVM
    {
        public int? TeacherId { get; set; }
        public List<VBranchClass> LstBranchClass { get; set; }
    }
    public class VScheduleBranchClass
    {
        [Key]
        public int? Id { get; set; }
        public int? SessionYearId { get; set; }
        public int? ScheduleId { get; set; }
        public int? BranchClassId { get; set; }
        public int? BranchId { get; set; }
        public int? ClassId { get; set; }
        public string? ClassName { get; set; }
        public double? Marks { get; set; }
        public int? StudentCount { get; set; }
        public int? StudentResultCount { get; set; }
        public int? ResultStatusId { get; set; }
        public string? ResultStatusColor { get; set; }
        public int? SubjectCount { get; set; }
        public int? QPaperCount { get; set; }
        public string? QPaperStatusColor { get; set; }

    }
    public class ScheduleBranchClassGroup
    {        
        public int Id { get; set; }
        public int ScheduleId { get; set; }
        public int BranchClassId { get; set; }
        public int BranchId { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }        
        public int StudentCount { get; set; }        
    }
    public class ScheduleBranchClassStudent
    {
        public int StudentId { get; set; }
        public int ScheduleId { get; set; }
        public int BranchClassId { get; set; }
        public int BranchId { get; set; }
        public int ClassId { get; set; }
        public int SectionId { get; set; }
        public string? FullName { get; set; }
        public string? RollNo { get; set; }       
    }
    public class ExamHall
    {
        public int Id { get; set; }
        public int ScheduleId { get; set; }
        public int RowCount { get; set; }
        public int ColumnCount { get; set; }
        public string Name { get; set; }
        public List<Seat> Seats { get; set; }
    }
    public class Seat
    {
        public int Id { get; set; }        
        public string SeatNumber { get; set; }
        public bool IsAllocated { get; set; }
        public ScheduleBranchClassStudent? ScheduleBranchClassStudent { get; set; }
    }
    public class ClassAllocation
    {
        public int ScheduleId { get; set; }
        public List<int> ScheduleBranchClassIds { get; set; }
    }
    public class CastScheduleExamHall
    {
        public int Id { get; set; }
        public int ScheduleId { get; set; }
        public int RowCount { get; set; }
        public int ColumnCount { get; set; }
        public string Name { get; set; }
        public List<CastExamHallSeat> LstCastExamHallSeat { get; set; }
        public List<CastStudentAllocation> LstCastStudentAllocation { get; set; }
    }
    public class CastExamHallSeat
    {
        public int Id { get; set; }
        public int ExamHallId { get; set; }
        public bool IsAllocated { get; set; }
        public string SeatNumber { get; set; }
    }
    public class CastStudentAllocation
    {
        public int StudentId { get; set; }
        public int ExamHallId { get; set; }
        public int SeatId { get; set; }
        public string SeatNumber { get; set; }
        public string Name { get; set; }
        public string RollNo { get; set; }
    }
    public class VBranchcls
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public BranchBranchCls BranchBranchCls { get; set; }
        public BranchSectionCls BranchSectionCls { get; set; }
    }
    public class BranchSectionCls
    {
        public int BranchId { get; set; }
        public string Name { get; set; }
        public List<VSection> LstSection { get; set; }
    }
    public class BranchBranchCls
    {
        public int BranchId { get; set; }
        public string Name { get; set; }
        public List<VBranchClass> LstBranchClass { get; set; }
    }
    public class SearchScheduleDate
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string ScheduleId { get; set; }
        
    }
    public class ScheduleClassAndSectionAndTeacher
    {
        public string StrClass { get; set; }
        public string StrSection { get; set; }
        public string StrTeacher { get; set; }
    }
    public class ScheduleStrDate
    {
        public string ExamDate { get; set; }
        public string? Comment { get; set; }
        public string? DateColor { get; set; }
        public bool IsSelected { get; set; }
    }
    public class ScheduleClass
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int ClassId { get; set; }
        public bool IsSelected { get; set; }
        //public string Marks { get; set; }
    }
    public class ScheduleSections
    {
        public int Id { get; set; }
        public bool IsSelected { get; set; }
    }
    public class ScheduleTeachers
    {
        public int Id { get; set; }
        public bool IsSelected { get; set; }
    }
    public class Schedule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SessionYearId { get; set; }
        public int BranchId { get; set; }
        public int StatusId { get; set; }
        public string Title { get; set; }
        public string ExamCount { get; set; }
        public int ExamTypeId { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        // 
        public DateTime? StartDate { get; set; }
        // 
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }
        // 
        public DateTime? CreatedDate { get; set; }
        //public string FromTimeOne { get; set; }
        //public string ToTimeOne { get; set; }
        //public string FromTimeTwo { get; set; }
        //public string ToTimeTwo { get; set; }

        public int? CreatedByUserId { get; set; }
        // 
        public DateTime? LastModifiedDate { get; set; }

        public int? LastModifiedByUserId { get; set; }
        public string Description { get; set; }
        public string? IsActive { get; set; }

        [NotMapped]
        public List<SelectListItem>? ExamTypeSheet { get; set; }
        [NotMapped]
        public List<SelectListItem>? BranchSheet { get; set; }
        [NotMapped]
        public List<VBranchcls>? LstBranch { get; set; }
        [NotMapped]
        public List<ScheduleStrDate>? LstStrDate { get; set; }
        [NotMapped]
        public List<ExamTime>? LstExamTime { get; set; }
        [NotMapped]
        public string? StrDate { get; set; }
        [NotMapped]
        public string? StrTime { get; set; }
        [NotMapped]
        public List<ScheduleClass>? LstScheduleClass { get; set; }
        //[NotMapped]
        //public List<ScheduleSections>? LstScheduleSections { get; set; }
        [NotMapped]
        public List<ScheduleTeachers>? LstScheduleTeachers { get; set; }

    }
    public class ScheduleBranchClassSubjectVM
    {
        public int ScheduleId { get; set; }
        public List<ScheduleBranchClassSubject> LstScheduleBranchClassSubject { get; set; }
    }
    public class ScheduleBranchClassSubject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SessionYearId { get; set; }
        public int ScheduleId { get; set; }
        public int BranchClassId { get; set; }
        public int ClassSubjectId { get; set; }
        public int? BranchId { get; set; }
        public int? ClassId { get; set; }
        public int? SubjectId { get; set; }
        public string Marks { get; set; }
        [NotMapped]
        public bool IsSelected { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }

    }
    public class ScheduleBrclsSubject
    {
        public int ScheduleId { get; set; }
        // public int BranchId { get; set; }
        public List<VBranchClass> LstBranchClass { get; set; }
    }
    public class ScheduleExamProcedure
    {
        public string StrResult { get; set; }
    }
    public class ScheduleTeacherProcedure
    {
        public string StrResult { get; set; }
    }
    public class SpScheduleTeacher
    {
        public string PreResult { get; set; }
        public string PostResult { get; set; }
    }
    public class SpScheduleExam
    {
        public string PreResult { get; set; }
        public string PostResult { get; set; }
    }
    public class SpScheduleStudent
    {
        public string PreResult { get; set; }
        public string PostResult { get; set; }
    }
    //public class VScheduleSection
    //{
    //    [Key]
    //    public int Id { get; set; }
    //    public int ScheduleId { get; set; }
    //    public int BranchId { get; set; }
    //    public int? BranchClassId { get; set; }
    //    public int SectionId { get; set; }
    //    public string SectionName { get; set; }

    //}
    public class SpScheduleExamDateTime
    {
        public string StrDate { get; set; }
        public string StrTime { get; set; }
    }
    public class ScheduleTeacherExam
    {
        public int ScheduleId { get; set; }
        // public int BranchId { get; set; }
        //public List<VScheduleExamDate> LstScheduleExamDate { get; set; }
        public List<CastExamHall> LstCastExamHall { get; set; }
    }
    public class VScheduleTeacherExamHall
    {
        [Key]
        public int Id { get; set; }
        public int SessionYearId { get; set; }
        public int ScheduleId { get; set; }
        public int ScheduleExamHallId { get; set; }
        public int ScheduleExamDateId { get; set; }
        public int? TeacherId { get; set; }
        public string? HallName { get; set; }
        public string? StrExamDate { get; set; }
        public string? ExamDay { get; set; }
        [NotMapped]
        public List<SelectListItem>? TeacherSheet { get; set; }

    }
    public class CastExamHall
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<CastExamDate> LstCastExamDate { get; set; }
    }
    public class CastExamDate
    {
        public int Id { get; set; }
        public int? TeacherId { get; set; }
        public string StrExamDate { get; set; }
        public List<SelectListItem>? TeacherSheet { get; set; }
    }
    public class ScheduleExamHall
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SessionYearId { get; set; }
        public int ScheduleId { get; set; }
        public int RowCount { get; set; }
        public int ColumnCount { get; set; }
        public string Name { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        [NotMapped]
        public List<VScheduleExamDate>? LstScheduleExamDates { get; set; }

    }
    public class VScheduleStudentAllocation
    {
        [Key]
        public int Id { get; set; }
        public int SessionYearId { get; set; }
        public int ScheduleId { get; set; }
        public int StudentId { get; set; }
        public int BranchClassId { get; set; }
        public int BranchId { get; set; }
        public int ClassId { get; set; }
        public int SectionId { get; set; }
        public int ExamHallId { get; set; }
        public int SeatId { get; set; }
        public string? ExamTitle { get; set; }
        public string? FullName { get; set; }
        public string? RollNo { get; set; }
        public string? ClassSectionName { get; set; }
        public string? ExamHallName { get; set; }
        public string? SeatNumber { get; set; }
        [NotMapped]
        public int? SerialNo { get; set; }

    }
    public class VScheduleExamDate
    {
        [Key]
        public int Id { get; set; }
        public int SessionYearId { get; set; }
        public int ScheduleId { get; set; }
        // 
        public DateTime ExamDate { get; set; }
        public string StrExamDate { get; set; }
        public string ExamDay { get; set; }
        [NotMapped]
        public int TeacherId { get; set; }
        [NotMapped]
        public List<SelectListItem>? TeacherSheet { get; set; }

    }
    public class VScheduleTeacher
    {
        [Key]
        public int Id { get; set; }
        public int SessionYearId { get; set; }
        public int ScheduleId { get; set; }
        public int? BranchId { get; set; }
        public int TeacherId { get; set; }
        public string TeacherName { get; set; }
        [NotMapped]
        public int? ScheduleExamDateId { get; set; }
        [NotMapped]
        public int? ScheduleSectionId { get; set; }
        [NotMapped]
        public List<SelectListItem> ScheduleSectionSheet { get; set; }

    }
    //public class TeacherSection
    //{
    //    public int ScheduleId { get; set; }
    //    public List<ScheduleTeacherSection> LstScheduleTeacherSection { get; set; }
    //}
    //public class ScheduleTeacherSection
    //{
    //    [Key]
    //    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //    public int Id { get; set; }
    //    public int ScheduleId { get; set; }
    //    public int TeacherId { get; set; }
    //    public int ScheduleSectionId { get; set; }
    //    public int ScheduleExamDateId { get; set; }

    //    // 
    //    public DateTime? CreatedDate { get; set; }
    //    public int? CreatedByUserId { get; set; }

    //}
    public class ScheduleExam
    {
        public int ScheduleId { get; set; }
        public List<Examination> LstScheduleExamination { get; set; }
    }
    public class Examination
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SessionYearId { get; set; }
        public int ScheduleId { get; set; }
        public int BranchId { get; set; }
        public int? BranchClassId { get; set; }
        public int? ClassSubjectId { get; set; }
        public int ClassId { get; set; }
        public int SubjectId { get; set; }
        public int? ExamTimeId { get; set; }
        public int? ExamTypeId { get; set; }
        [NotMapped]
        public string StrExamDate { get; set; }

        public DateTime? ExamDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        public string? IsActive { get; set; }
        public string? FilePath { get; set; }
        public string? FileName { get; set; }
        public string? QPaperStatusId { get; set; }
        public string? QPaperType { get; set; }

        [NotMapped]
        public List<SelectListItem> BranchSheet { get; set; }
        [NotMapped]
        public List<SelectListItem> ClassSheet { get; set; }

        [NotMapped]
        public List<SelectListItem> SubjectSheet { get; set; }
    }
    //public class ScheduleRollNo
    //{
    //    public int ScheduleId { get; set; }
    //    public List<ScheduleBranchClassRollNo> LstScheduleBranchClassRollNo { get; set; }
    //}
    //public class ScheduleBranchClassRollNo
    //{
    //    [Key]
    //    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //    public int Id { get; set; }
    //    public int ScheduleId { get; set; }
    //    public int BranchClassId { get; set; }
    //    public int SectionId { get; set; }
    //    public int? FromRollNo { get; set; }
    //    public int? ToRollNo { get; set; }
    //    public int? CreatedByUserId { get; set; }
    //    public DateTime? CreatedDate { get; set; }
    //}
    public class ScheduleStudentExam
    {
        public int ScheduleId { get; set; }
        // public int BranchId { get; set; }
        public List<ScheduleStudent> LstScheduleStudent { get; set; }
    }
    public class ScheduleStudent
    {
        public int Id { get; set; }
        public int? SerialNo { get; set; }
        public int? RegisterNo { get; set; }
        public string FullName { get; set; }
        public string GenderName { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        [NotMapped]
        public int? ScheduleId { get; set; }
        [NotMapped]
        public int? ScheduleSectionId { get; set; }
        public string ScheduleSectionName { get; set; }
        public string TicketNo { get; set; }
        [NotMapped]
        public List<SelectListItem> ScheduleSectionSheet { get; set; }


    }
    //public class VScheduleStudentSection
    //{
    //    [Key]
    //    public int Id { get; set; }
    //    public int ScheduleId { get; set; }
    //    public int StudentId { get; set; }
    //    public int ScheduleSectionId { get; set; }
    //    public string TicketNo { get; set; }
    //    public int? BranchId { get; set; }
    //    public int? BranchClassId { get; set; }

    //    public string SectionName { get; set; }
    //    public string FullName { get; set; }
    //    public int? RegisterNo { get; set; }
    //    public string ClassName { get; set; }
    //    [NotMapped]
    //    public int? SerialNo { get; set; }
    //}
    public class VExamination
    {
        [Key]
        public int? Id { get; set; }
        public int? SessionYearId { get; set; }
        public int? ScheduleId { get; set; }
        public int? BranchId { get; set; }
        public int? BranchClassId { get; set; }
        public int? ClassSubjectId { get; set; }
        public int? SubjectId { get; set; }
        public int? ClassId { get; set; }
        public int? ExamTypeId { get; set; }
        public int? ExamTimeId { get; set; }
        public string? ExamTypeName { get; set; }
        public string? BranchName { get; set; }
        public string? ClassName { get; set; }
        public string? SubjectName { get; set; }
        public string? Marks { get; set; }
        public string? QPaperType { get; set; }
        public string? QPaperStatusId { get; set; }
        public DateTime? ExamDate { get; set; }
        public string? ExamTime { get; set; }
        public string? StrExamDate { get; set; }
        public string? FilePath { get; set; }
        public string? FileName { get; set; }
        public int LessonCount { get; set; }
        [NotMapped]
        public DateTime? ScheduleDate { get; set; }
        [NotMapped]
        public string? StrScheduleDate { get; set; }
        [NotMapped]
        public string? QPaperStatusColor { get; set; }
    }
    public class VSchedule
    {
        [Key]
        public int? Id { get; set; }
        public int? SessionYearId { get; set; }
        public int? BranchId { get; set; }
        public int? StatusId { get; set; }
        public string? BranchName { get; set; }
        public string? StatusName { get; set; }
        public string? Title { get; set; }
        public string? ExamCount { get; set; }
        public int? ExamTypeId { get; set; }
        public string? ExamTypeName { get; set; }
        // [Column(TypeName = "datetime2")]
        public DateTime? StartDate { get; set; }
        // [Column(TypeName = "datetime2")]
        public DateTime? EndDate { get; set; }
        public string? StrStartDate { get; set; }
        public string? StrEndDate { get; set; }
        public string? Description { get; set; }
        public int? ExamHallCount { get; set; }
        public int? ClassCount { get; set; }
        public int? TeacherCount { get; set; }
        public int? StudentCount { get; set; }
        public decimal? ResultPercent { get; set; }
        [NotMapped]
        public List<VScheduleBranchClass> LstScheduleBranchClass { get; set; }
        [NotMapped]
        public bool IsSelected { get; set; }
    }
    public class SpScheduleSubject
    {
        public string PreResult { get; set; }
        public string PostResult { get; set; }
    }
    public class SpScheduleStudentSection
    {
        public string StrResult { get; set; }
    }
    public class UserInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string UserType { get; set; }
        public int PersonId { get; set; }
        [NotMapped]
        public bool IsSelected { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        [NotMapped]
        public string? NewPassword { get; set; }
        public DateTime? LastLoggedIn { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [NotMapped]
        public bool HasLoginSelected { get; set; }
        public string? IsActive { get; set; }
        public string? HasLogin { get; set; }
    }

    public class UserRoleVM
    {
        public int UserId { get; set; }
        public List<Role> LstRole { get; set; }
    }
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int SessionYearId { get; set; }
        public string RoleType { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int? CreatedByUserId { get; set; }      
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedByUserId { get; set; }      
        public DateTime? ModifiedDate { get; set; }
        public string? IsActive { get; set; }
        [NotMapped]
        public bool IsSelected { get; set; }
        [NotMapped]
        public List<SelectListItem>? BranchSheet { get; set; }
        [NotMapped]
        public List<SelectListItem>? SessionYearSheet { get; set; }
    }
    public class VRole
    {
        [Key]
        public int? Id { get; set; }
        public string? RoleType { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? BranchId { get; set; }
        public string? BranchName { get; set; }
        public int? SessionYearId { get; set; }
        public string? SessionYearName { get; set; }
        public int? UserCount { get; set; }
        public string IsDelete { get; set; }
    }
    public class PageVM
    {
        public int? RoleId { get; set; }
        public List<VUserTypePage> LstUserTypePage { get; set; }
    }
    public class Page
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        [NotMapped]
        public bool IsSelected { get; set; }
        public string Description { get; set; }
        public int? CreatedByUserId { get; set; }
        // [Column(TypeName = "datetime2")]
        public DateTime? CreatedDate { get; set; }

        public int? ModifiedByUserId { get; set; }
        // [Column(TypeName = "datetime2")]
        public DateTime? ModifiedDate { get; set; }

        public string IsActive { get; set; }

    }
    public class UserTypePage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int PageId { get; set; }
        public string UserType { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
    }   
    public class VUserTypePage
    {
        [Key]
        public int Id { get; set; }
        public int PageId { get; set; }
        public string UserType { get; set; }
        public string? Screen { get; set; }
        [NotMapped]
        public bool IsSelected { get; set; }
    }
    public class RoleUserVM
    {
        public int RoleId { get; set; }
        public List<UserInfo> LstUser { get; set; }
    }
    public class PageFunctionProcedure
    {
        public string StrResult { get; set; }
    }
    public class VBranchClassAttendence
    {
        [Key]
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int ClassId { get; set; }
        public int? SectionCount { get; set; }
        public int? StudentCount { get; set; }
        public int? ScheduleCount { get; set; }
        public string BranchName { get; set; }
        public string IsDelete { get; set; }
        public string ClassName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? WorkingDate { get; set; }
        public string StrWorkingDate { get; set; }
        public int? AttendanceStudentCount { get; set; }
        public string StatusColor { get; set; }
        public string StatusName { get; set; }

    }
    public class Attendence
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SessionYearId { get; set; }
        public int BranchId { get; set; }
        public DateTime WorkingDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        public string? IsActive { get; set; }
        [NotMapped]
        public int PersonnelType { get; set; }
        [NotMapped]
        public List<CastStudentAttendence>? LstStudentAttendence { get; set; }
        [NotMapped]
        public List<CastPersonnelAttendence>? LstPersonnelAttendence { get; set; }

    }
    public class PersonnelMonthlyAttendence
    {
        public int YearId { get; set; }
        public int MonthId { get; set; }
        public string MonthName { get; set; }
        public int WorkingDaysCount { get; set; }
        public List<HeatMapPersonnelAttendence> LstHeatMapPersonnelAttendence { get; set; }
    }
    //public class HeatMapPersonnelAttendence
    //{
    //    public int PersonnelId { get; set; }
    //    public int PersonnelType { get; set; }
    //    public DateTime AttendenceDate { get; set; }
    //    public int YearId { get; set; }
    //    public int MonthId { get; set; }
    //    public string Status { get; set; }
    //}
    public class SectionAttendence
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AttStatusName { get; set; }
        public string AttStatusColor { get; set; }
    }
    public class CastStudentAttendence
    {
        public int StudentId { get; set; }
        public int SessionYearId { get; set; }
        public int BranchId { get; set; }
        public int BranchClassId { get; set; }
        public int ClassId { get; set; }
        public int SectionId { get; set; }
        public string? StudentName { get; set; }
        public string? RollNo { get; set; }
        public bool IsSelected { get; set; }
        public string? LeaveType { get; set; }
        public string? Remarks { get; set; }
    }
    //public class PersonnelTotalAttendenceGraphVM
    //{
    //    public List<string> labels { get; set; }
    //    public List<PersonnelTotalAttendenceChildVM> datasets { get; set; }
    //}
    //public class PersonnelTotalAttendenceChildVM
    //{
    //    public string label { get; set; }
    //    public List<string> backgroundColor { get; set; }
    //    public int borderWidth { get; set; }
    //    public bool fill { get; set; }
    //    public List<decimal?> data { get; set; }
    //}
    //public class PersonnelTotalAttendence
    //{
    //    public string TotalWorkingDays { get; set; }
    //    public string TotalPresentDays { get; set; }
    //    public string TotalAbsentDays { get; set; }
    //    public string TotalLeaveDays { get; set; }
    //    public List<VPersonnelAttendenceBenchmark> LstPersonnelAttendenceBenchmark { get; set; }
    //    public PersonnelTotalAttendenceGraphVM PersonnelTotalAttendenceGraphVM { get; set; }
    //}

    public class VPersonnelAttendenceBenchmark
    {
        [Key]
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
    
    public class HeatMapPersonnelAttendence
    {
        public int PersonnelId { get; set; }
        public int PersonnelType { get; set; }
        public DateTime AttendenceDate { get; set; }
        public int YearId { get; set; }
        public int MonthId { get; set; }
        public string Status { get; set; }
    }
    public class CastPersonnelAttendence
    {
        public int PersonnelId { get; set; }
        public int SessionYearId { get; set; }
        public int BranchId { get; set; }
        public string PersonnelType { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeNo { get; set; }
        public bool IsSelected { get; set; }
        public string? LeaveType { get; set; }
        public string? Remarks { get; set; }
    }
    public class VAttendence
    {
        [Key]
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int SessionYearId { get; set; }
        public decimal? StudentResultPercent { get; set; }
        public string? StrWorkingDate { get; set; }
        public string? StrTodayDate { get; set; }
        public string? IsEdit { get; set; }
        public string? AdminStatusColor { get; set; }
        public string? AdminStatusName { get; set; }
        public string? DriverStatusName { get; set; }
        public string? DriverStatusColor { get; set; }
        public string? StaffStatusName { get; set; }
        public string? StaffStatusColor { get; set; }
        public string? StudentStatusName { get; set; }
        public string? StudentStatusColor { get; set; }
        public string? TeacherStatusName { get; set; }
        public string? TeacherStatusColor { get; set; }


    }
    public class AttendenceVM
    {
        public List<VAttendence> LstAttendence { get; set; }
        public bool IsAttendenceExist { get; set; }
    }
    public class StudentAttendance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int AttendenceId { get; set; }
        public int StudentId { get; set; }
        public int BranchId { get; set; }
        public int SessionYearId { get; set; }
        public int BranchClassId { get; set; }
        public int ClassId { get; set; }
        public int SectionId { get; set; }
        public DateTime WorkingDate { get; set; }
        public string IsPresent { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        public string? IsActive { get; set; }
        public string? Remarks { get; set; }
    }
    public class VDailyAttendance
    {
        [Key]
        public int Id { get; set; }
        public int BranchId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime WorkingDate { get; set; }
        public string StrWorkingDate { get; set; }
        public string BranchName { get; set; }
        public int? BranchClassCount { get; set; }
        public int? AttendanceClassCount { get; set; }
        public string StatusColor { get; set; }
        public string StatusName { get; set; }
        public decimal? ResultPercent { get; set; }
    }
    public class AttendenceClass
    {
        public int AttendenceId { get; set; }
        public int BranchId { get; set; }
        public int BranchClassId { get; set; }
        public string StrWorkingDate { get; set; }
        public List<VStudent> LstStudent { get; set; }
    }
    public class ReportCard
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SessionYearId { get; set; }
        public int BranchId { get; set; }
        public string Name { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        // [Column(TypeName = "datetime2")]
        public DateTime? FromDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        // [Column(TypeName = "datetime2")]
        public DateTime? ToDate { get; set; }

        public string? Description { get; set; }

        public string? IsActive { get; set; }
        // [Column(TypeName = "datetime2")]
        public DateTime? CreatedDate { get; set; }

        public int? CreatedByUserId { get; set; }
        // [Column(TypeName = "datetime2")]
        public DateTime? LastModifiedDate { get; set; }

        public int? LastModifiedByUserId { get; set; }
        //[NotMapped]
        //public List<SelectListItem> BranchSheet { get; set; }
        [NotMapped]
        public List<ScheduleReportClass> LstScheduleClass { get; set; }

    }
    public class ScheduleReportClass
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? ExamTypeName { get; set; }
        public bool IsSelected { get; set; }
    }
    public class VReportCard
    {
        [Key]
        public int? Id { get; set; }
        public int? BranchId { get; set; }
        public int? SessionYearId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? BranchName { get; set; }
        public int? ScheduleCount { get; set; }
        public string IsDelete { get; set; }

        // [Column(TypeName = "datetime2")]
        public DateTime? FromDate { get; set; }

        // [Column(TypeName = "datetime2")]
        public DateTime? ToDate { get; set; }
        public string? StrFromDate { get; set; }
        public string? StrToDate { get; set; }
    }
    public class SpReportCardResult
    {
        public string PreResult { get; set; }
        public string PostResult { get; set; }
    }
    public class RCCP_Student
    {
        public string StrResult { get; set; }
    }
    public class QuestionBank
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int LessonId { get; set; }
        public int QuestionTypeId { get; set; }
        public int QuestionDifficultyId { get; set; }
        public int QuestionCategoryId { get; set; }
        public string Question { get; set; }
        public string DeltaQuestion { get; set; }
        public string? Marks { get; set; }
        public string? Description { get; set; }
        public string? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        [NotMapped]
        public int? ClassId { get; set; }
        [NotMapped]
        public int? SubjectId { get; set; }
        [NotMapped]
        public List<SelectListItem>? ClassSheet { get; set; }
        [NotMapped]
        public List<SelectListItem>? SubjectSheet { get; set; }
        [NotMapped]
        public List<SelectListItem>? LessonSheet { get; set; }
        [NotMapped]
        public List<SelectListItem>? QuestionTypeSheet { get; set; }
        [NotMapped]
        public List<SelectListItem>? QuestionDifficultySheet { get; set; }
        [NotMapped]
        public List<SelectListItem>? QuestionCategorySheet { get; set; }

    }
    public class VQuestionBank
    {
        [Key]
        public int? Id { get; set; }
        public int? LessonId { get; set; }
        public int? ClassSubjectId { get; set; }
        public int? ClassId { get; set; }
        public int? SubjectId { get; set; }
        public string? ClassName { get; set; }
        public string? SubjectName { get; set; }
        public string? LessonName { get; set; }
        public int? QuestionTypeId { get; set; }
        public string? QuestionTypeName { get; set; }
        public int? QuestionDifficultyId { get; set; }
        public string? QuestionDifficultyName { get; set; }
        public int? QuestionCategoryId { get; set; }
        public string? QuestionCategoryName { get; set; }
        public string? Question { get; set; }
        public string? Marks { get; set; }
        public int? ExamQuestionCount { get; set; }
        public string? IsDelete { get; set; }
        [NotMapped]
        public string? QuestionColor { get; set; }
    }
    public class OverAllPerformanceVM
    {
        public List<VRCOverAllPerformance> LstOverAllPerformance { get; set; }
        public OverAllPerformanceGraphVM OverAllPerformanceGraph { get; set; }
        public OverAllPerformanceClassVM OverAllPerformanceClass { get; set; }
        public OverAllPerformanceGenderVM OverAllPerformanceGender { get; set; }
    }
    public class VRCOverAllPerformance
    {
        [Key]
        public int? Id { get; set; }
        public int ScheduleId { get; set; }
        public int? BranchId { get; set; }
        public string ExamTypeName { get; set; }
        public string ScheduleMonth { get; set; }
        public decimal? Marks { get; set; }
        public decimal? MaxMarks { get; set; }
        public decimal? MrkPercent { get; set; }
        public string Grade { get; set; }
    }
    public class OverAllPerformanceGraphVM
    {
        public List<string> labels { get; set; }
        public List<OverAllPerformanceChildVM> datasets { get; set; }
    }
    public class OverAllPerformanceChildVM
    {
        public string label { get; set; }
        public List<string> backgroundColor { get; set; }
        public int borderWidth { get; set; }
        public bool fill { get; set; }
        // public List<decimal?> data { get; set; }
        public List<OverAllPerformanceLabel> data { get; set; }
    }
    public class OverAllPerformanceLabel
    {
        public decimal? Percent { get; set; }
        public string Strlabel { get; set; }
        public int? ScheduleId { get; set; }

    }
    public class OverAllPerformanceClassVM
    {
        public List<VRCOverAllPerformanceClass> LstOverAllPerformanceClass { get; set; }
        public OverAllPerformanceClassGraphVM OverAllPerformanceClassGraph { get; set; }
    }
    public class VRCOverAllPerformanceClass
    {
        [Key]
        public int? Id { get; set; }
        public int? BranchClassId { get; set; }
        public int? BranchId { get; set; }
        public string ClassName { get; set; }
        public decimal? Marks { get; set; }
        public decimal? MaxMarks { get; set; }
        public decimal? MrkPercent { get; set; }
        public string GradeColor { get; set; }
    }
    public class OverAllPerformanceClassGraphVM
    {
        public List<string> labels { get; set; }
        public List<OverAllPerformanceClassChildVM> datasets { get; set; }
    }
    public class OverAllPerformanceClassChildVM
    {
        public string label { get; set; }
        public List<string> backgroundColor { get; set; }
        public int borderWidth { get; set; }
        public bool fill { get; set; }
        public List<decimal?> data { get; set; }
        // public List<OverAllPerformanceClassLabel> data { get; set; }
    }
    public class OverAllPerformanceClassLabel
    {
        public decimal? Percent { get; set; }
        public string Strlabel { get; set; }
        public int? ScheduleId { get; set; }

    }
    public class OverAllPerformanceGenderVM
    {
        public List<VRCOverAllPerformanceGender> LstOverAllPerformanceGender { get; set; }
        public OverAllPerformanceGenderGraphVM OverAllPerformanceGenderGraph { get; set; }
    }
    public class VRCOverAllPerformanceGender
    {
        [Key]
        public int? Id { get; set; }
        public int? BranchId { get; set; }
        public string Gender { get; set; }
        public string GenderName { get; set; }
        public decimal? Marks { get; set; }
        public decimal? MaxMarks { get; set; }
        public decimal? MrkPercent { get; set; }
    }
    public class OverAllPerformanceGenderGraphVM
    {
        public List<string> labels { get; set; }
        public List<OverAllPerformanceGenderChildVM> datasets { get; set; }
    }
    public class OverAllPerformanceGenderChildVM
    {
        public string label { get; set; }
        public List<string> backgroundColor { get; set; }
        public int borderWidth { get; set; }
        public bool fill { get; set; }
        public List<decimal?> data { get; set; }
        //public List<OverAllPerformanceGenderLabel> data { get; set; }
    }
    public class OverAllPerformanceGenderLabel
    {
        public decimal? Percent { get; set; }
        public string Strlabel { get; set; }
        public int? ScheduleId { get; set; }

    }
    public class ReportSettings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int BranchId { get; set; }
        public string? AFourHeaderType { get; set; }
        public string? AFourFooterType { get; set; }
        public int? AFourHeaderSpace { get; set; }
        public int? AFourFooterSpace { get; set; }

        public string? AFiveHeaderType { get; set; }
        public string? AFiveFooterType { get; set; }
        public int? AFiveHeaderSpace { get; set; }
        public int? AFiveFooterSpace { get; set; }
        public string? BarcodeType { get; set; }
        public string? EsignType { get; set; }

        [NotMapped]
        public IFormFile? AFourHeaderFile { get; set; }
        //public byte[] AFourHeaderFileData { get; set; }
        [NotMapped]
        public IFormFile? AFourFooterFile { get; set; }
        //public byte[] AFourFooterFileData { get; set; }
        [NotMapped]
        public IFormFile? AFiveHeaderFile { get; set; }
        //public byte[] AFiveHeaderFileData { get; set; }
        [NotMapped]
        public IFormFile? AFiveFooterFile { get; set; }
        // public byte[] AFiveFooterFileData { get; set; }

        public string? AFourHeaderPhotoPath { get; set; }
        public string? AFourHeaderFileName { get; set; }
        public string? AFourFooterPhotoPath { get; set; }
        public string? AFourFooterFileName { get; set; }

        public string? AFiveHeaderPhotoPath { get; set; }
        public string? AFiveHeaderFileName { get; set; }
        public string? AFiveFooterPhotoPath { get; set; }
        public string? AFiveFooterFileName { get; set; }

        public int? PreparedById { get; set; }
        public int? VerifiedById { get; set; }
        public int? CreatedByUserId { get; set; }

        public DateTime? CreatedDate { get; set; }
        public int? ModifiedByUserId { get; set; }

        public DateTime? ModifiedDate { get; set; }
        public string? IsActive { get; set; }
        [NotMapped]
        public List<SelectListItem>? LstPreparedBy { get; set; }
        [NotMapped]
        public List<SelectListItem>? LstVerifiedBy { get; set; }
        [NotMapped]
        public string? PreparedByESignPath { get; set; }
        [NotMapped]
        public string? PreparedByName { get; set; }
        [NotMapped]
        public string? PreparedByDesignation { get; set; }
        [NotMapped]
        public string? VerifiedByESignPath { get; set; }
        [NotMapped]
        public string? VerifiedByName { get; set; }
        [NotMapped]
        public string? VerifiedByDesignation { get; set; }
    }
    public class TimeTableVM
    {
        public List<VBranch> LstBranch { get; set; }
        public VSchedule Schedule { get; set; }
    }
    public class LicenseKey
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public int NoOfMonths { get; set; }
        public string Key { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
    public class VLicenseKey
    {
        [Key]
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public int NoOfMonths { get; set; }
        public string CreatedDate { get; set; }
        public string ExpirationDate { get; set; }
        public string CreatedBy { get; set; }

    }
    public class ProgressReport
    {
        public int ReportCardId { get; set; }
        public int BranchClassId { get; set; }
        public int StudentId { get; set; }
        public ReportSettings ReportSettings { get; set; }
        public VReportCard ReportCard { get; set; }
    }
    public class FeeReceipt
    {
        public int PaymentId { get; set; }       
    }

    public class HallTicket
    {
        public ReportSettings ReportSettings { get; set; }
        //public ScheduleStudent scheduleStudent { get; set; }
        public VScheduleStudentAllocation ScheduleStudentAllocation { get; set; }
        public List<HallTicketSubject> LstHallTicketSubject { get; set; }
    }
    public class ScheduleDetail
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ExamType { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Branch { get; set; }
        public ReportSettings ReportSettings { get; set; }
    }
    public class ScheduleSyllabusDetail
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ExamType { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Branch { get; set; }
        public ReportSettings ReportSettings { get; set; }        
    }
    public class ScheduleResultDetail
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ExamType { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Branch { get; set; }
        public ReportSettings ReportSettings { get; set; }
        public List<VScheduleBranchClass> LstScheduleBranchClass { get; set; }
    }
    public class StudentDetail
    {
        public int Id { get; set; }
        public int SessionYearId { get; set; }
        public string FilePath { get; set; }
        public string FullName { get; set; }
        public string Age { get; set; }
        public string GenderName { get; set; }
        public string? FatherName { get; set; }
        public string? MotherName { get; set; }
        public string DefaultMobileNo { get; set; }
        public string? AlternateMobileNo { get; set; }
        public string? PermanentAddress { get; set; }
        public string? CurrentAddress { get; set; }
        public string BranchName { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        public string AdmissionDate { get; set; }
        public string AdmissionNo { get; set; }
        public string RollNo { get; set; }
        public string ActiveStatus { get; set; }
        public ReportSettings ReportSettings { get; set; }
    }
    public class PersonnelDetail
    {
        public int Id { get; set; }
        public int SessionYearId { get; set; }
        public string FilePath { get; set; }
        public string FullName { get; set; }
        public string Age { get; set; }
        public string GenderName { get; set; }
        public string? FatherName { get; set; }
        public string? MotherName { get; set; }
        public string DefaultMobileNo { get; set; }
        public string? AlternateMobileNo { get; set; }
        public string? PermanentAddress { get; set; }
        public string? CurrentAddress { get; set; }
        public string BranchName { get; set; }
        //public string ClassName { get; set; }
        //public string SectionName { get; set; }
        public string AdmissionDate { get; set; }
        public string AdmissionNo { get; set; }
        public string EmployeeNo { get; set; }
        public string ActiveStatus { get; set; }

        public int? ClassSubjectTeacherCount { get; set; }
        public string? TeacherSubjectName { get; set; }
        public ReportSettings ReportSettings { get; set; }
    }
    public class ReportCardDetail
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public string ScheduleCount { get; set; }
        public string Branch { get; set; }
        public List<VBranchClass> LstBranchClass { get; set; }
    }
    public class HallTicketSubject
    {
        public string SubjectName { get; set; }
        public string InvSignature { get; set; }
    }
    public class ExaminationPaper
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ExamId { get; set; }
        public string Question { get; set; }
        public string DeltaQuestion { get; set; }
        public string IsActive { get; set; }
        // 
        public DateTime? CreatedDate { get; set; }
        public int? CreatedByUserId { get; set; }
        // 
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }

    }
    public class SpSyllabusTopic
    {
        public string StrResult { get; set; }
    }
    public class QuestionPaper
    {
        public int Id { get; set; }
        public string QPaperType { get; set; }
        public string BranchName { get; set; }
        public string ExamTypeName { get; set; }
        public string ScheduleMonth { get; set; }
        public string ClassName { get; set; }
        public string SubjectName { get; set; }
        public string ExamDate { get; set; }
        public string ExamTime { get; set; }
        public string Marks { get; set; }
        public List<VExaminationQuestion> LstExaminationQuestion { get; set; }
        public ExaminationPaper ExaminationPaper { get; set; }
        public ViewAttach Attach { get; set; }
    }
    public class VExaminationQuestion
    {
        [Key]
        public int Id { get; set; }
        public int ExamId { get; set; }
        public int QuestionBankId { get; set; }
        public int? QuestionTypeId { get; set; }
        public string Question { get; set; }
        public string DeltaQuestion { get; set; }
    }
    public class ViewAttach
    {
        public string url { get; set; }
    }
    public class ExamSubject
    {
        public int ExamId { get; set; }
        public string SubjectName { get; set; }
        public string QPaperStatusColor { get; set; }
    }
    public class TopicVM
    {
        // public int? ClassSubjectId { get; set; }
        public int? ExamId { get; set; }
        public List<Topic> LstTopic { get; set; }
    }
    public class LessonVM
    {
        // public int? ClassSubjectId { get; set; }
        public int ExamId { get; set; }
        public List<CastLesson> LstLesson { get; set; }
    }
    public class CastLesson
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool IsSelected { get; set; }
    }
    public class ExamQuestionVM
    {
        public VExamination Examination { get; set; }
        public List<VQuestionBank> LstQuestionBank { get; set; }
        public List<VQuestionBank> LstFilterQuestionBank { get; set; }

        public List<ExamQuestionAlgo> LstExamQuestionAlgo { get; set; }
    }
    public class ExamQuestionAlgo
    {
        public bool IsSelected { get; set; }
        public int? QuestionTypeId { get; set; }
        public string QuestionTypeName { get; set; }
        public int? QuestionDifficultyId { get; set; }
        public List<SelectListItem> QuestionDifficultySheet { get; set; }
        public int? QuestionCategoryId { get; set; }
        public List<SelectListItem> QuestionCategorySheet { get; set; }
        public int? QuestionCount { get; set; }
        public string Marks { get; set; }

    }
    public class UploadExamQPaper
    {
        public int ExamId { get; set; }
        public string QPaperFileName { get; set; }
        public byte[] QPaperFileData { get; set; }
    }
    public class SpExamResult
    {
        public string PreResult { get; set; }
        public string PostResult { get; set; }
    }
    public class StudentResultUpload
    {
        public int ScheduleId { get; set; }
        public int BranchClassId { get; set; }
        // public int ClassId { get; set; }
        public List<ExcelParams> LstExcelParams { get; set; }

    }
    public class ExcelParams
    {
        public string RollNo { get; set; }
        public string Subject { get; set; }
        public string Marks { get; set; }
    }
    public class StudentResultVM
    {
        public List<StudentResult> LstStudentResult { get; set; }
    }
    public class StudentResult
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SessionYearId { get; set; }
        public int ExamId { get; set; }
        public string FeedType { get; set; }

        public int StudentId { get; set; }

        public string Marks { get; set; }
        // 
        public DateTime? CreatedDate { get; set; }

        public int? CreatedByUserId { get; set; }
        // 
        public DateTime? LastModifiedDate { get; set; }

        public int? LastModifiedByUserId { get; set; }

        public string IsActive { get; set; }

    }
    public class VScheduleSyllabus
    {
        [Key]
        public int Id { get; set; }
        public int SessionYearId { get; set; }
        public int BranchId { get; set; }

        public string BranchName { get; set; }

        public int StatusId { get; set; }

        public string StatusName { get; set; }

        public string Title { get; set; }

        public int ExamTypeId { get; set; }

        public string ExamTypeName { get; set; }

        public string ExamCount { get; set; }
        // 
        public DateTime? StartDate { get; set; }
        // 
        public DateTime? EndDate { get; set; }

        public string StrStartDate { get; set; }

        public string StrEndDate { get; set; }

        public string Description { get; set; }

        public int? SubjectCount { get; set; }

        public int? QPaperCount { get; set; }

        public decimal? ResultPercent { get; set; }

    }
    public class VScheduleResult
    {
        [Key]
        public int Id { get; set; }
        public int SessionYearId { get; set; }
        public int BranchId { get; set; }

        public string BranchName { get; set; }

        public int StatusId { get; set; }

        public string StatusName { get; set; }

        public string Title { get; set; }

        public int ExamTypeId { get; set; }

        public string ExamTypeName { get; set; }

        public string ExamCount { get; set; }
        // 
        public DateTime? StartDate { get; set; }
        // 
        public DateTime? EndDate { get; set; }

        public string StrStartDate { get; set; }

        public string StrEndDate { get; set; }

        public string Description { get; set; }

        public int? StudentCount { get; set; }

        public int? StudentResultCount { get; set; }

        public decimal? ResultPercent { get; set; }

    }
    public class ExcelDownload
    {
        public int Id { get; set; }
    }
    public class ChangePwd
    {
        public string Password { get; set; }
        public string NewPassword { get; set; }
    }
}