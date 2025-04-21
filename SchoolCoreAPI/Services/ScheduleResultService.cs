using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using System.Data;
using System.Dynamic;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class ScheduleResultService : IScheduleResultService
    {
        private readonly APIContext _context;
        private readonly string _connectionString;

        public ScheduleResultService(APIContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<List<VScheduleResults>> GetAllScheduleResult(int BranchId)
        {
            return await _context.VScheduleResults.AsNoTracking().Where(x=>x.BranchId==BranchId).OrderBy(x => x.Id).ToListAsync();
        }

        public async Task<string> GetBranchClassResultStatus(string scheduleId, string branchClassId)
        {
            if (string.IsNullOrEmpty(scheduleId) || string.IsNullOrEmpty(branchClassId))
                return string.Empty;

            int schId = int.Parse(scheduleId);
            int brclsId = int.Parse(branchClassId);

            var objsyllabus = await _context.VScheduleResults.AsNoTracking().SingleOrDefaultAsync(x => x.Id == schId);
            if (objsyllabus != null && objsyllabus.StudentResultCount == objsyllabus.StudentCount)
            {
                var objsch = await _context.Schedule.SingleOrDefaultAsync(x => x.Id == schId);
                if (objsch != null)
                {
                    objsch.StatusId = (int)ScheduleStatus.Result;
                    _context.SaveChanges();
                }
            }

            var objbrcls = await _context.VScheduleBranchClasses.AsNoTracking().SingleOrDefaultAsync(x => x.ScheduleId == schId && x.BranchClassId == brclsId);
            return objbrcls?.ResultStatusColor ?? string.Empty;
        }

        
        public async Task<DataSet> GetExportData(int Id)
        {
            DataSet ds = new DataSet();
            try
            {
                var lstobjbrcls = await _context.VScheduleBranchClasses.AsNoTracking().Where(x => x.ScheduleId == Id).ToListAsync();
                var sheetindex = 1;
                foreach (var brcls in lstobjbrcls)
                {
                    DataTable dt = new DataTable(brcls.ClassName);
                    
                    var rawData = await _context.V_SP_PreExamResult
                         .AsNoTracking()
             .Where(x => x.ScheduleId == Id && x.BranchClassId== brcls.BranchClassId)
            .Select(e => new
            {
                e.StudentId,
                e.RollNo,
                e.Name,
                e.ExamId,
                e.SubjectName
               
                
            })
            .ToListAsync();

                    // Step 2: Get all unique SubjectNames (to create dynamic columns)
                    var subjectNames = rawData.Select(x => x.SubjectName).Distinct().ToList();

                    // Step 3: Group data by StudentId (and any other fields as needed)
                    var pivotedData = rawData
                        .GroupBy(x => new { x.StudentId, x.RollNo, x.Name })
                        .Select(g =>
                        {
                            var pivot = new ExpandoObject() as IDictionary<string, Object>;
                            pivot["Id"] = g.Key.StudentId;
                            pivot["RollNo"] = g.Key.RollNo;
                            pivot["Name"] = g.Key.Name;
                           

                            // Add SubjectName columns dynamically
                            foreach (var subjectName in subjectNames)
                            {
                                // Get the ExamId for the current SubjectName (if exists)
                                var exam = g.FirstOrDefault(x => x.SubjectName == subjectName);
                                pivot[subjectName] = exam?.ExamId; // Add ExamId or null if not available
                            }

                            return pivot;
                        })
                        .ToList();

                    // Step 4: Convert pivoted data to DataTable (if needed)
                    dt = ConvertToDataTable(pivotedData);
                    dt.TableName = brcls.ClassName;


                    //using (MySqlConnection conn = new MySqlConnection(_connectionString))
                    //{
                    //    MySqlCommand sqlComm = new MySqlCommand("SP_PreExamResult", conn);
                    //    sqlComm.Parameters.AddWithValue("@ScheduleId", Id);
                    //    sqlComm.Parameters.AddWithValue("@BranchClassId", brcls.BranchClassId);

                    //    sqlComm.CommandType = CommandType.StoredProcedure;
                    //    MySqlDataAdapter da = new MySqlDataAdapter();
                    //    da.SelectCommand = sqlComm;
                    //    da.Fill(dt);
                    //}

                    dt.DefaultView.Sort = "RollNo";
                    dt = dt.DefaultView.ToTable();
                    dt.Columns.Remove("Id");
                    foreach (DataRow row in dt.Rows)
                    {
                        foreach (DataColumn col in dt.Columns)
                        {
                            if (col.ColumnName != "RollNo" && col.ColumnName != "Name")
                            {
                                row[col.ColumnName] = DBNull.Value;
                            }
                        }
                    }
                    ds.Tables.Add(dt);
                    sheetindex++;
                }
            }
            catch (Exception ex)
            {
                // Log exception
            }
            return ds;
        }


        public async Task<Tuple<string, string>> GetSPExamResult(string branchClassId, string scheduleId)
        {
            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            try
            {
                var rawData =  _context.V_SP_PostExamResult
                    .AsNoTracking()
             .Where(x => x.ScheduleId == int.Parse(scheduleId) && x.BranchClassId==int.Parse(branchClassId))
           .Select(e => new
           {
               e.StudentId,
               e.RollNo,
               e.Name,
               e.SubjectName,
               e.Marks
           })
           .ToList();

                // Step 2: Get all unique SubjectNames (to create dynamic columns)
                var subjectNames = rawData.Select(x => x.SubjectName).Distinct().ToList();

                // Step 3: Group data by StudentId (and any other fields as needed)
                var pivotedData = rawData
                    .GroupBy(x => new { x.StudentId, x.RollNo, x.Name })
                    .Select(g =>
                    {
                        var pivot = new ExpandoObject() as IDictionary<string, Object>;
                        pivot["Id"] = g.Key.StudentId;
                        pivot["RollNo"] = g.Key.RollNo;
                        pivot["Name"] = g.Key.Name;

                        // Add SubjectName columns dynamically and assign Marks to each one
                        foreach (var subjectName in subjectNames)
                        {
                            // Get the Marks for the current SubjectName (if exists)
                            var subjectData = g.FirstOrDefault(x => x.SubjectName == subjectName);
                            pivot[subjectName] = subjectData?.Marks; // Add Marks or null if not available
                        }

                        return pivot;
                    })
                    .ToList();

                // Step 4: Convert pivoted data to DataTable (if needed)
                dt2 = ConvertToDataTable(pivotedData);
            }
            catch (Exception)
            {
                // Log exception
            }
            return new Tuple<string, string>(DataTableToJSON(dt), DataTableToJSON(dt2));
        }

        public async Task<List<VScheduleBranchClasses>> GetStudentBranchClass(int Id)
        {
            return await _context.VScheduleBranchClasses.AsNoTracking().Where(x => x.ScheduleId == Id).ToListAsync();
        }

        public async Task<VSchedules> GetStudentResult(int Id)
        {
            var objSched = await _context.VSchedules.AsNoTracking().SingleOrDefaultAsync(x => x.Id == Id);
            if (objSched != null)
            {
                objSched.LstScheduleBranchClass = await _context.VScheduleBranchClasses.AsNoTracking().Where(x => x.ScheduleId == Id).ToListAsync();
            }
            return objSched;
        }

        public async Task<ScheduleResultDetail> GetScheduleResultDetail(int Id, int BranchId)
        {
            var objsch = await _context.VSchedules.AsNoTracking().SingleOrDefaultAsync(m => m.Id == Id);
            if (objsch == null) return null;

            var lstschbrcls = await _context.VScheduleBranchClasses.AsNoTracking().Where(x => x.ScheduleId == Id).ToListAsync();
            var rpt = await _context.ReportSettings.AsNoTracking().FirstOrDefaultAsync(x=>x.BranchId==BranchId);

            return new ScheduleResultDetail
            {
                Id = (int)objsch.Id,
                StartDate = objsch.StrStartDate,
                EndDate = objsch.StrEndDate,
                Title = objsch.Title,
                ExamType = objsch.ExamTypeName,
                Branch = objsch.BranchName,
                ReportSettings = rpt,
                LstScheduleBranchClass = lstschbrcls
            };
        }

        public async Task<int> SaveStudentResult(StudentResultVM model, int UserId)
        {
            if (model == null || model.LstStudentResult.Count == 0) return 0;

            foreach (var item in model.LstStudentResult)
            {
                var objResult = await _context.StudentResult.SingleOrDefaultAsync(x => x.StudentId == item.StudentId && x.ExamId == item.ExamId);
                if (objResult != null)
                {
                    objResult.Marks = item.Marks;
                    objResult.LastModifiedByUserId = UserId;
                    objResult.LastModifiedDate = DateTime.Now;
                }
                else
                {
                    var result = new StudentResult
                    {
                        ExamId = item.ExamId,
                        StudentId = item.StudentId,
                        FeedType = ((int)FeedType.Manual).ToString(),
                        Marks = item.Marks,
                        CreatedByUserId = UserId,
                        CreatedDate = DateTime.Now,
                        IsActive = ((int)ActiveState.Active).ToString()
                    };
                    _context.StudentResult.Add(result);
                }
            }
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UploadStudentResult(StudentResultUpload model, int UserId)
        {
            if (model == null) return 0;

            var objbrcls = await _context.ScheduleBranchClass.AsNoTracking().SingleOrDefaultAsync(x => x.ScheduleId == model.ScheduleId && x.BranchClassId == model.BranchClassId);
            if (objbrcls == null) return 0;

            var lstexisting = await _context.StudentResult.Where(x => x.ScheduleId == model.ScheduleId && x.BranchClassId == model.BranchClassId).ToListAsync();
            _context.StudentResult.RemoveRange(lstexisting);

            var lstExam = await _context.VExaminations.AsNoTracking().Where(x => x.ScheduleId == model.ScheduleId && x.BranchClassId == model.BranchClassId).ToListAsync();
            foreach (var stud in model.LstExcelParams)
            {
                var objStud = await _context.VStudents.AsNoTracking().SingleOrDefaultAsync(x => x.RollNo == stud.RollNo && x.BranchClassId == model.BranchClassId);
                if (objStud == null) continue;

                var objExam = lstExam.SingleOrDefault(x => x.SubjectName + " (" + x.Marks + ")".ToUpper() == stud.Subject.ToUpper());
                if (objExam == null) continue;

                var result = new StudentResult
                {
                    SessionYearId = (int)objExam.SessionYearId,
                    ScheduleId = model.ScheduleId,
                    BranchClassId = model.BranchClassId,
                    BranchId = objbrcls.BranchId,
                    ClassId = objbrcls.ClassId,
                    SectionId = (int)objStud.SectionId,
                    ClassSubjectId = (int)objExam.ClassSubjectId,
                    SubjectId = (int)objExam.SubjectId,
                    ExamTypeId = (int)objExam.ExamTypeId,
                    ExamTimeId = (int)objExam.ExamTimeId,
                    ExamDate = (DateTime)objExam.ExamDate,
                    ExamId = (int)objExam.Id,
                    StudentId = (int)objStud.Id,
                    RollNo = objStud.RollNo,
                    FeedType = ((int)FeedType.Excel).ToString(),
                    MaxMarks = objExam.Marks,
                    Marks = stud.Marks,
                    IsActive = ((int)ActiveState.Active).ToString(),
                    CreatedByUserId = UserId,
                    CreatedDate = DateTime.Now
                };
                _context.StudentResult.Add(result);
            }
            return await _context.SaveChangesAsync();
        }
    }
}