using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class MbPersonnelService : IMbPersonnelService
    {
        private readonly APIContext _context;

        public MbPersonnelService(APIContext context)
        {
            _context = context;
        }

        public async Task<MbPersonnelDetail> GetPersonnelDetail(int Id, int SessionYearId, string PersonnelType)
        {
            if (Id <= 0 || SessionYearId <= 0 || string.IsNullOrEmpty(PersonnelType))
            {
                return new MbPersonnelDetail();
            }

            MbPersonnelDetail detail = new MbPersonnelDetail();
            try
            {
                bool isStudent = PersonnelType == ((int)UserType.Student).ToString();
                bool isTeacher = PersonnelType == ((int)UserType.Teacher).ToString();
                bool isAdmin = PersonnelType == ((int)UserType.Admin).ToString();

                if (isStudent)
                {
                    var objStudent = await _context.VStudents.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
                    if (objStudent != null)
                    {
                        objStudent.FilePath = string.IsNullOrEmpty(objStudent.FilePath) ? Shared.GetNoUserPath() : objStudent.FilePath;
                    }
                    var objStudentAdmission = await _context.StudentAdmission.AsNoTracking().SingleOrDefaultAsync(x => x.StudentId == Id);
                    var objStudentOtherDetail =await _context.StudentOtherDetail.AsNoTracking().SingleOrDefaultAsync(x => x.StudentId == Id);

                    detail = new MbPersonnelDetail
                    {
                        Id = objStudent?.Id ?? 0,
                        FilePath = objStudent?.FilePath,
                        SessionYearId = SessionYearId,
                        PersonnelType = PersonnelType,
                        FullName = objStudent?.FullName,
                        Age = objStudent?.Age,
                        GenderName = objStudent?.GenderName,
                        BloodGroupName = objStudent?.BloodGroupName,
                        DateOfBirth = objStudent?.DateOfBirth,
                        FatherName = objStudent?.FatherName,
                        MotherName = objStudent?.MotherName,
                        DefaultMobileNo = objStudent?.DefaultMobileNo,
                        BranchName = objStudent?.BranchName,
                        ClassName = objStudent?.ClassName,
                        SectionName = objStudent?.SectionName,
                        AdmissionDate = objStudent?.AdmissionDate,
                        AdmissionNo = objStudent?.AdmissionNo,
                        EmployeeRollNo = objStudent?.RollNo,
                        ActiveStatus = objStudent?.ActiveStatus,
                        AlternateMobileNo = objStudentOtherDetail?.OtherMobileNo,
                        PermanentAddress = objStudentOtherDetail?.PermanentAddress,
                        CurrentAddress = objStudentOtherDetail?.CurrentAddress,
                        PresentDays = objStudent?.PresentDays,
                        WorkingDays = objStudent?.WorkingDays,
                        AttPercent = objStudent?.AttPercent
                    };
                }
                else
                {
                    var objPersonnel = isTeacher
                        ? (object)await _context.VTeachers.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id)
                        : (object)await _context.VAdmins.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
                    if (objPersonnel != null)
                    {
                        var filePathProperty = objPersonnel.GetType().GetProperty("FilePath");
                        if (filePathProperty != null)
                        {
                            var filePath = (string)filePathProperty.GetValue(objPersonnel);
                            filePathProperty.SetValue(objPersonnel, string.IsNullOrEmpty(filePath) ? Shared.GetNoUserPath() : filePath);
                        }
                    }
                    var objPersonnelAdmission = _context.PersonnelAdmission.SingleOrDefault(x => x.PersonnelId == Id);
                    var objPersonnelOtherDetail = _context.PersonnelOtherDetail.SingleOrDefault(x => x.PersonnelId == Id);

                    detail = new MbPersonnelDetail
                    {
                        Id = (int)objPersonnel.GetType().GetProperty("Id").GetValue(objPersonnel),
                        FilePath = (string)objPersonnel.GetType().GetProperty("FilePath").GetValue(objPersonnel),
                        SessionYearId = SessionYearId,
                        PersonnelType = PersonnelType,
                        FullName = (string)objPersonnel.GetType().GetProperty("FullName").GetValue(objPersonnel),
                        Age = (string)objPersonnel.GetType().GetProperty("Age").GetValue(objPersonnel),
                        GenderName = (string)objPersonnel.GetType().GetProperty("GenderName").GetValue(objPersonnel),
                        BloodGroupName = (string)objPersonnel.GetType().GetProperty("BloodGroupName").GetValue(objPersonnel),
                        DateOfBirth = (string)objPersonnel.GetType().GetProperty("DateOfBirth").GetValue(objPersonnel),
                        FatherName = objPersonnelOtherDetail?.FatherName,
                        MotherName = objPersonnelOtherDetail?.MotherName,
                        DefaultMobileNo = (string)objPersonnel.GetType().GetProperty("DefaultMobileNo").GetValue(objPersonnel),
                        BranchName = (string)objPersonnel.GetType().GetProperty("BranchName").GetValue(objPersonnel),
                        ClassName = string.Empty,
                        SectionName = string.Empty,
                        AdmissionDate = objPersonnelAdmission?.AdmissionDate?.ToShortDateString(),
                        AdmissionNo = objPersonnelAdmission?.AdmissionNo,
                        EmployeeRollNo = (string)objPersonnel.GetType().GetProperty("EmployeeNo").GetValue(objPersonnel),
                        ActiveStatus = (string)objPersonnel.GetType().GetProperty("ActiveStatus").GetValue(objPersonnel),
                        AlternateMobileNo = objPersonnelOtherDetail?.OtherMobileNo,
                        PermanentAddress = objPersonnelOtherDetail?.PermanentAddress,
                        CurrentAddress = objPersonnelOtherDetail?.CurrentAddress,
                        Designation = isTeacher ? (string)objPersonnel.GetType().GetProperty("Designation").GetValue(objPersonnel) : string.Empty,
                        Qualification = isTeacher ? (string)objPersonnel.GetType().GetProperty("Qualification").GetValue(objPersonnel) : string.Empty,
                        EmailAddress = isTeacher ? (string)objPersonnel.GetType().GetProperty("EmailAddress").GetValue(objPersonnel) : string.Empty,
                        ClassTeacherCount = isTeacher ? (int?)objPersonnel.GetType().GetProperty("ClassTeacherCount").GetValue(objPersonnel) : null,
                        ClassSubjectTeacherCount = isTeacher ? (int?)objPersonnel.GetType().GetProperty("ClassSubjectTeacherCount").GetValue(objPersonnel) : null,
                        TeacherSubjectName = isTeacher ? (string)objPersonnel.GetType().GetProperty("TeacherSubjectName").GetValue(objPersonnel) : string.Empty,
                        TeacherClassName = isTeacher ? (string)objPersonnel.GetType().GetProperty("TeacherClassName").GetValue(objPersonnel) : string.Empty,
                        PresentDays = (int?)objPersonnel.GetType().GetProperty("PresentDays").GetValue(objPersonnel),
                        WorkingDays = (int?)objPersonnel.GetType().GetProperty("WorkingDays").GetValue(objPersonnel),
                        AttPercent = (decimal?)objPersonnel.GetType().GetProperty("AttPercent").GetValue(objPersonnel)
                    };
                }
            }
            catch (Exception ex)
            {
                // Log the exception
            }
            return detail;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
