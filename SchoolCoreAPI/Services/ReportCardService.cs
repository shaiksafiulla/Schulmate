using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Org.BouncyCastle.Asn1.Pkcs;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using System.Data;
using System.Dynamic;
using System.Globalization;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class ReportCardService : IReportCardService
    {
        private readonly APIContext _context;
        private readonly string _connectionString;

        public ReportCardService(APIContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<VReportCards>> GetAllReportCard(int branchId, int sessionYearId)
        {
            return await _context.VReportCards.AsNoTracking()
                .Where(x => x.BranchId == branchId && x.SessionYearId == sessionYearId)
                .OrderBy(x => x.Id)
                .ToListAsync();
        }

        public async Task<VReportCards> ViewReportCard(int id)
        {
            return await _context.VReportCards.AsNoTracking().SingleOrDefaultAsync(m => m.Id == id);
        }

        public async Task<ReportCard> GetReportCard(int id, int branchId, int sessionYearId)
        {
            var model = new ReportCard
            {
                BranchId = branchId,
                SessionYearId = sessionYearId,
                LstScheduleClass = await _context.VSchedules.AsNoTracking()
                    .Where(x => x.BranchId == branchId)
                    .Select(x => new ScheduleReportClass
                    {
                        Id = (int)x.Id,
                        Title = x.Title,
                        ExamTypeName = x.ExamTypeName,
                        IsSelected = false
                    })
                    .ToListAsync()
            };

            if (id > 0)
            {
                var cat = await _context.ReportCard.AsNoTracking()
                    .SingleOrDefaultAsync(x => x.Id == id && x.IsActive == ((int)ActiveState.Active).ToString());

                if (cat != null)
                {
                    model.Id = cat.Id;
                    model.Name = cat.Name;
                    model.Description = cat.Description;
                    model.FromDate = cat.FromDate;
                    model.ToDate = cat.ToDate;
                    model.BranchId = cat.BranchId;
                    model.SessionYearId = cat.SessionYearId;

                    var lstsched = await _context.ReportCardSchedule.AsNoTracking()
                        .Where(x => x.ReportCardId == id)
                        .Select(x => x.ScheduleId)
                        .ToListAsync();

                    foreach (var item in model.LstScheduleClass)
                    {
                        item.IsSelected = lstsched.Contains(item.Id);
                    }
                }
            }

            return model;
        }

        public async Task<ReportCardDetail> GetReportCardDetail(int id)
        {
            var lstrptcardscheduleids = await _context.ReportCardSchedule.AsNoTracking()
                .Where(x => x.ReportCardId == id)
                .Select(x => x.ScheduleId)
                .ToListAsync();

            var lstschbrclsids = await _context.VScheduleBranchClasses.AsNoTracking()
                .Where(x => lstrptcardscheduleids.Contains((int)x.ScheduleId))
                .Select(x => x.BranchClassId)
                .ToListAsync();

            var lstbrcls = await _context.VBranchClasses.AsNoTracking()
                .Where(x => lstschbrclsids.Contains((int)x.Id))
                .ToListAsync();

            var objsch = await _context.VReportCards.AsNoTracking().SingleOrDefaultAsync(m => m.Id == id);

            return new ReportCardDetail
            {
                Id = (int)objsch.Id,
                Name = objsch.Name,
                StrFromDate = objsch.StrFromDate,
                StrToDate = objsch.StrToDate,
                ScheduleCount = objsch.ScheduleCount.ToString(),
                Branch = objsch.BranchName,
                LstBranchClass = new List<VBranchClasses>(lstbrcls)
            };
        }

        public async Task<bool> IsRecordExists(string name, int id)
        {
            return id == 0
                ? await _context.ReportCard.AsNoTracking().AnyAsync(e => e.Name == name && e.IsActive == ((int)ActiveState.Active).ToString())
                : await _context.ReportCard.AsNoTracking().AnyAsync(e => e.Name == name && e.Id != id && e.IsActive == ((int)ActiveState.Active).ToString());
        }

        public async Task<ServiceResult> SaveReportCard(ReportCard model, int userId)
        {
            ServiceResult result = null;

            try
            {
                if (model.Id > 0)
                {
                    var cat = await _context.ReportCard.AsNoTracking()
                        .SingleOrDefaultAsync(m => m.Id == model.Id && m.IsActive == ((int)ActiveState.Active).ToString());

                    if (cat != null)
                    {
                        cat.SessionYearId = model.SessionYearId;
                        cat.BranchId = model.BranchId;
                        cat.Name = model.Name;
                        cat.FromDate = model.FromDate;
                        cat.ToDate = model.ToDate;
                        cat.Description = model.Description;
                        cat.LastModifiedDate = DateTime.Now;
                        cat.LastModifiedByUserId = userId;

                        var lstsched = await _context.ReportCardSchedule.AsNoTracking()
                            .Where(x => x.ReportCardId == model.Id)
                            .ToListAsync();

                        _context.ReportCardSchedule.RemoveRange(lstsched);

                        var lstadd = model.LstScheduleClass
                            .Where(x => x.IsSelected)
                            .Select(add => new ReportCardSchedule
                            {
                                SessionYearId = model.SessionYearId,
                                ReportCardId = model.Id,
                                ScheduleId = add.Id,
                                CreatedByUserId = userId,
                                CreatedDate = DateTime.Now
                            })
                            .ToList();

                        _context.ReportCardSchedule.AddRange(lstadd);

                        if (_context.SaveChanges() > 0)
                        {
                            result = new ServiceResult
                            {
                                StatusId = (int)ServiceResultStatus.Updated,
                                RecordId = cat.Id
                            };
                        }
                    }
                }
                else
                {
                    var cat = new ReportCard
                    {
                        SessionYearId = model.SessionYearId,
                        BranchId = model.BranchId,
                        Name = model.Name,
                        FromDate = model.FromDate,
                        ToDate = model.ToDate,
                        Description = model.Description,
                        IsActive = ((int)ActiveState.Active).ToString(),
                        CreatedDate = DateTime.Now,
                        CreatedByUserId = userId,
                        LastModifiedDate = DateTime.Now,
                        LastModifiedByUserId = userId
                    };

                    _context.ReportCard.Add(cat);

                    if (await _context.SaveChangesAsync() > 0)
                    {
                        var lstadd = model.LstScheduleClass
                            .Where(x => x.IsSelected)
                            .Select(add => new ReportCardSchedule
                            {
                                SessionYearId = model.SessionYearId,
                                ReportCardId = cat.Id,
                                ScheduleId = add.Id,
                                CreatedByUserId = userId,
                                CreatedDate = DateTime.Now
                            })
                            .ToList();

                        _context.ReportCardSchedule.AddRange(lstadd);

                        if (await _context.SaveChangesAsync() > 0)
                        {
                            result = new ServiceResult
                            {
                                StatusId = (int)ServiceResultStatus.Added,
                                RecordId = cat.Id
                            };
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Log exception
            }

            return result;
        }

        public async Task<ServiceResult> DeleteReportCard(int id, int userId)
        {
            var cat = await _context.ReportCard.AsNoTracking().SingleOrDefaultAsync(m => m.Id == id);

            if (cat != null)
            {
                cat.IsActive = "0";
                cat.LastModifiedByUserId = userId;
                cat.LastModifiedDate = DateTime.Now;

                _context.Entry(cat).State = EntityState.Modified;

                if (await _context.SaveChangesAsync() > 0)
                {
                    return new ServiceResult
                    {
                        StatusId = (int)ServiceResultStatus.Deleted,
                        RecordId = cat.Id
                    };
                }
            }

            return null;
        }

        public async Task<List<VBranchClasses>> GetBranchClassByReportCard(int reportCardId)
        {
            var lstrpcids = await _context.ReportCardSchedule.AsNoTracking()
                .Where(x => x.ReportCardId == reportCardId)
                .Select(x => x.ScheduleId)
                .ToListAsync();

            var lstbrclsids = await _context.ScheduleBranchClass.AsNoTracking()
                .Where(x => lstrpcids.Contains(x.ScheduleId))
                .Select(x => x.BranchClassId)
                .ToListAsync();

            return await _context.VBranchClasses.AsNoTracking()
                .Where(x => lstbrclsids.Contains((int)x.Id))
                .ToListAsync();
        }

        public async Task<Tuple<string, string>> GetSPReportCard(string reportCardId, string branchClassId)
        {
            try
            {
                int rpcId = int.Parse(reportCardId);
                int brclsId = int.Parse(branchClassId);

                var lstrpcids = await _context.ReportCardSchedule.AsNoTracking()
                    .Where(x => x.ReportCardId == rpcId)
                    .Select(x => x.ScheduleId)
                    .ToListAsync();

                if (lstrpcids.Count > 0)
                {
                    var lstexam = await _context.VExaminations.AsNoTracking()
                        .Where(x => lstrpcids.Contains((int)x.ScheduleId) && x.BranchClassId == brclsId)
                        .ToListAsync();

                    var lststr = lstexam
                        .Select(item => $"{item.StrExamDate} {item.SubjectName} ({item.Marks})")
                        .ToList();

                    string joinsubnames = "[" + string.Join("],[", lststr) + "]";

                    var lstheader = lstrpcids
                        .Select(schId => new SpReportHeader
                        {
                            Title = lstexam.FirstOrDefault(x => x.ScheduleId == schId)?.ExamTypeName + " Test " + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(lstexam.FirstOrDefault(x => x.ScheduleId == schId)?.ExamDate?.Month ?? 0),
                            ColCount = lstexam.Count(x => x.ScheduleId == schId).ToString()
                        })
                        .ToList();

                    DataTable dt2 = ListtoDataTableConverter.ToDataTable(lstheader);

          //          DataTable dt = new DataTable();

          //          var rawData = await _context.V_SP_RCCP
          //              .AsNoTracking()
          //   .Where(x => lstrpcids.Contains((int)x.ScheduleId) && x.BranchClassId == int.Parse(branchClassId))
          //.Select(e => new
          //{
          //    e.StudentId,
          //    e.RollNo,
          //    e.Name,
          //    e.TotalMarks,
          //    e.TotalMaxMarks,
          //    e.MrkPercent,
          //    e.Grade,
          //    e.PRank,
          //    e.SubjectName,
          //    e.Marks
          //})
          //.ToListAsync();

          //          // Step 2: Get unique SubjectNames for dynamic column creation
          //          var subjectNames = rawData.Select(x => x.SubjectName).Distinct().ToList();

          //          // Step 3: Group data by StudentId, RollNo, Name, and others
          //          var pivotedData = rawData
          //              .GroupBy(x => new { x.StudentId, x.RollNo, x.Name, x.TotalMarks, x.TotalMaxMarks, x.MrkPercent, x.Grade, x.PRank })
          //              .Select(g =>
          //              {
          //                  var pivot = new ExpandoObject() as IDictionary<string, Object>;
          //                  pivot["StudentId"] = g.Key.StudentId;
          //                  pivot["RollNo"] = g.Key.RollNo;
          //                  pivot["Name"] = g.Key.Name;
          //                  pivot["TotalMarks"] = g.Key.TotalMarks;
          //                  pivot["TotalMaxMarks"] = g.Key.TotalMaxMarks;
          //                  pivot["MrkPercent"] = g.Key.MrkPercent;
          //                  pivot["Grade"] = g.Key.Grade;
          //                  pivot["PRank"] = g.Key.PRank;

          //                  // Add columns dynamically for each SubjectName
          //                  foreach (var subjectName in subjectNames)
          //                  {
          //                      var subjectData = g.FirstOrDefault(x => x.SubjectName == subjectName);
          //                      pivot[subjectName] = subjectData?.Marks; // Assign Marks or null if not available
          //                  }

          //                  return pivot;
          //              })
          //              .ToList();

          //          // Step 4: Convert pivoted data to DataTable (if needed)
          //          dt = ConvertToDataTable(pivotedData);
                    

                    var fnresult = GetFn_RccP(string.Join(",", lstrpcids), branchClassId);
                    DataTable dt3 =  ConvertListToDataTable(fnresult.Result) ;//ListtoDataTableConverter.ToDataTable(fnresult.Result); 
                    DataTable dt4 = FormatFn_RccP(fnresult.Result);
                    string[] columnsToRemove = { "subjectName", "Marks" };
                    DataTable result = MergeDataTables(dt3, dt4, columnsToRemove);


                    return new Tuple<string, string>(DataTableToJSON(result), DataTableToJSON(dt2));
                }
            }
            catch (Exception ex)
            {
                // Log exception
            }

            return null;
        }
        public async Task<List<Fn_RccP>> GetFn_RccP(string scheduleIds, string branchClassId)
        {
            var studentResults = new List<Fn_RccP>();
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new NpgsqlCommand("SELECT * FROM fn_rccp(@ScheduleIds, @BranchClassId)", connection))
                    {
                        // Add parameters to the command
                        command.Parameters.AddWithValue("ScheduleIds", scheduleIds);
                        command.Parameters.AddWithValue("BranchClassId", branchClassId);

                        // Execute the query and read the results
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var grade = reader.IsDBNull(reader.GetOrdinal("Grade"))
    ? "No Grade"
    : reader.GetString(reader.GetOrdinal("Grade"));
                                var studentResult = new Fn_RccP
                                {
                                    StudentId = reader.GetInt32(reader.GetOrdinal("StudentId")),
                                    RollNo = reader.GetString(reader.GetOrdinal("RollNo")),
                                    FullName = reader.GetString(reader.GetOrdinal("FullName")),
                                    TotalMaxMarks = reader.GetInt32(reader.GetOrdinal("TotalMaxMarks")),
                                    TotalMarks = reader.GetInt32(reader.GetOrdinal("TotalMarks")),
                                    MrkPercent = reader.GetDecimal(reader.GetOrdinal("MrkPercent")),
                                    SubjectName = reader.GetString(reader.GetOrdinal("SubjectName")),
                                    Marks = reader.GetString(reader.GetOrdinal("Marks")),
                                  //  Grade = reader.GetString(reader.GetOrdinal("Grade")),
                                    Grade=grade,
                                    PRank = reader.GetInt32(reader.GetOrdinal("PRank"))
                                };

                                studentResults.Add(studentResult);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                
            }          

            return studentResults;
        }
        public DataTable FormatFn_RccP(List<Fn_RccP> studentResults)
        {
            List<dynamic> rows = new List<dynamic>();           

            foreach (var studentResult in studentResults)
            {
                string[] subjectArray = studentResult.SubjectName.Split(new string[] { " & " }, StringSplitOptions.None);
                string[] marksArray = studentResult.Marks.Split(new string[] { " & " }, StringSplitOptions.None);
                if (subjectArray.Length == marksArray.Length)
                {
                    for (int i = 0; i < subjectArray.Length; i++) {
                        rows.Add(CreateRow(studentResult.StudentId, subjectArray[i], marksArray[i]));
                    }
                }
               // rows.Add(CreateRow(studentResult.StudentId, studentResult.SubjectName, studentResult.Marks));
            }
            // Step 2: Get unique SubjectNames for dynamic column creation
            var subjectNames = rows.Select(x => x.SubjectName).Distinct().ToList();
            var pivotedData = rows
                       .GroupBy(x => new { x.StudentId})
                       .Select(g =>
                       {
                           var pivot = new ExpandoObject() as IDictionary<string, Object>;
                           pivot["StudentId"] = g.Key.StudentId;                         

                           // Add columns dynamically for each SubjectName
                           foreach (var subjectName in subjectNames)
                           {
                               var subjectData = g.FirstOrDefault(x => x.SubjectName == subjectName);
                               pivot[subjectName] = subjectData?.Marks; // Assign Marks or null if not available
                           }

                           return pivot;
                       })
                       .ToList();
            DataTable dt = ConvertToDataTable(pivotedData);
            return dt;
        }
       
        public DataTable MergeDataTables(DataTable dt1, DataTable dt2, string[] columnsToRemove)
        {
            // Step 3: Create a new DataTable with merged columns
            DataTable result = new DataTable();
            try
            {
                // Step 1: Remove unwanted columns from both DataTables
                RemoveColumns(dt1, columnsToRemove);
                RemoveColumns(dt2, columnsToRemove);

                // Step 2: Merge the DataTables based on StudentId
                var mergedTable = from table1 in dt1.AsEnumerable()
                                  join table2 in dt2.AsEnumerable()
                                  on table1.Field<int>("StudentId").ToString() equals table2.Field<string>("StudentId").ToString()
                                  select new { Row1 = table1, Row2 = table2 };

                // Add columns from the first table (dt1) and second table (dt2)
                foreach (DataColumn col in dt1.Columns)
                    result.Columns.Add(col.ColumnName, col.DataType);

                foreach (DataColumn col in dt2.Columns)
                {
                    // Skip adding columns that are already in the first table (to avoid duplicates)
                    if (!dt1.Columns.Contains(col.ColumnName))
                        result.Columns.Add(col.ColumnName, col.DataType);
                }

                // Step 4: Populate the result DataTable with merged rows
                foreach (var item in mergedTable)
                {
                    DataRow newRow = result.NewRow();

                    // Fill the new row with data from dt1
                    foreach (DataColumn col in dt1.Columns)
                    {
                        newRow[col.ColumnName] = item.Row1[col.ColumnName];
                    }

                    // Fill the new row with data from dt2 (skip already existing columns from dt1)
                    foreach (DataColumn col in dt2.Columns)
                    {
                        if (!dt1.Columns.Contains(col.ColumnName))  // To avoid duplicate columns
                        {
                            newRow[col.ColumnName] = item.Row2[col.ColumnName];
                        }
                    }

                    result.Rows.Add(newRow);
                }
                // Step 5: Sort the result DataTable by PRank
                result.DefaultView.Sort = "PRank ASC";  // Or "PRank DESC" for descending

                // Return the sorted DataTable
                result = result.DefaultView.ToTable();
            }
            catch (Exception ex)
            {
                // Handle exception
              //  Console.WriteLine(ex.Message);
            }
            return result;
        }


        // Helper function to remove columns
        private void RemoveColumns(DataTable dt, string[] columnsToRemove)
        {
            foreach (var column in columnsToRemove)
            {
                if (dt.Columns.Contains(column))
                {
                    dt.Columns.Remove(column);
                }
            }
        }
        


        static dynamic CreateRow(int studentid, string subject, string marks)
        {
            dynamic row = new System.Dynamic.ExpandoObject();
            row.StudentId = studentid;
            row.SubjectName = subject;
            row.Marks = marks;
            return row;
        }
        public async Task<List<VSchedules>> GetSchedulesByBranch(string branchId, string reportcardId)
        {
            if (string.IsNullOrEmpty(branchId) || string.IsNullOrEmpty(reportcardId))
            {
                return null;
            }

            int brId = int.Parse(branchId);
            int rcid = int.Parse(reportcardId);

            var lst = await _context.VSchedules.AsNoTracking().Where(x => x.BranchId == brId).ToListAsync();

            if (rcid > 0)
            {
                var lstids = lst.Select(x => x.Id).ToList();
                var lstrc = await _context.ReportCardSchedule.AsNoTracking()
                    .Where(x => lstids.Contains(x.ScheduleId) && x.ReportCardId == rcid)
                    .ToListAsync();

                foreach (var item in lst)
                {
                    item.IsSelected = lstrc.Any(x => x.ScheduleId == item.Id);
                }
            }

            return lst;
        }

        public async Task<RCCP_Student> GetRCCP_Student(string reportCardId, string studentId)
        {
            if (string.IsNullOrEmpty(reportCardId) || string.IsNullOrEmpty(studentId))
            {
                return null;
            }

            int rcpid = int.Parse(reportCardId);
            int studid = int.Parse(studentId);

            var lstrpcids = await _context.ReportCardSchedule.AsNoTracking()
                .Where(x => x.ReportCardId == rcpid)
                .Select(x => x.ScheduleId)
                .ToListAsync();

            if (lstrpcids.Count > 0)
            {
                var objstud = await _context.VStudents.AsNoTracking().SingleOrDefaultAsync(x => x.Id == studid);

                if (objstud != null)
                {
                    var brclsid = objstud.BranchClassId;

                    DataTable dt = new DataTable();
                    // Execute stored procedure and fill dt

                    var strResult = DataTableToJSON(dt);
                    return new RCCP_Student { StrResult = strResult };
                }
            }

            return null;
        }

        public async Task<List<SelectListItem>> GetBranches()
        {
            return await _context.Branch.AsNoTracking()
                .Where(x => x.IsActive == ((int)ActiveState.Active).ToString())
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                .ToListAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
