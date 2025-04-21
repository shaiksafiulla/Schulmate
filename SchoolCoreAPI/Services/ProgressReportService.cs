using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
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
    public class ProgressReportService : IProgressReportService
    {
        private readonly APIContext _context;
        private readonly string _connectionString;

        public ProgressReportService(APIContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

     public async Task<JObject> GetFn_ProgressReport(int rptcardId, int branchclassId, int studentId, int sessionYearId)
        {
            var tuple = await SetParams(rptcardId, branchclassId, studentId, sessionYearId);

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand("SELECT fn_progressreport(@p_schedule_ids,@p_branchclass_id,@p_sessionyear_id,@p_student_id,@p_reportcard_id)", connection);

            // Add parameter to the command           
            command.Parameters.AddWithValue("p_schedule_ids", tuple.Item1);
            command.Parameters.AddWithValue("p_branchclass_id", tuple.Item2);
            command.Parameters.AddWithValue("p_sessionyear_id", tuple.Item3);
            command.Parameters.AddWithValue("p_student_id", tuple.Item4);
            command.Parameters.AddWithValue("p_reportcard_id", tuple.Item5);

            var result = await command.ExecuteScalarAsync(); // Executes the function

            // The result will be in JSONB format
            if (result != null)
            {
                var jsonString = result.ToString();
                var json = JObject.Parse(jsonString); // Parse JSON into JObject
                return json;
            }

            return null;
        }
        public async Task<StudentScheduleVM> GetStudentScheduleVM(List<VStudentScheduleResults> reslst)
        {
            var revlst = reslst.OrderBy(x => x.ScheduleYearNo)
                .ThenBy(x => x.ScheduleMonthNo).ToList();
            var childVM = new StudentScheduleChildVM
            {
                label = "Performance",
                backgroundColor = new List<string> { "#6C63FF" },
                borderWidth = 1,
                fill = true,
                data = revlst.Select(item => new ScheduleLabel
                {
                    Strlabel = item.ScheduleMonth,
                    Percent = item.MrkPercent,
                    ScheduleId = item.ScheduleId
                }).ToList()
            };

            return new StudentScheduleVM
            {
                LstStudentScheduleResult = revlst,
                StudentScheduleGraph = new StudentScheduleGraphVM
                {
                    labels = revlst.Select(x => x.ScheduleMonth).ToList(),
                    datasets = new List<StudentScheduleChildVM> { childVM }
                }
            };
        }

        public async Task<Tuple<string,string,string, string, string>> SetParams(int rptcardId, int branchclassId, int studentId, int sessionYearId)
        {
            //scheduleids,studentid,reportcardid,sessionyearid
            var lstrptcardsched = await _context.ReportCardSchedule.AsNoTracking()
               .Where(x => x.ReportCardId == rptcardId && x.SessionYearId == sessionYearId)
               .Select(x => x.ScheduleId)
               .ToListAsync();

            return Tuple.Create(string.Join(",", lstrptcardsched), branchclassId.ToString(), sessionYearId.ToString(), studentId.ToString(), rptcardId.ToString());

        }





        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
