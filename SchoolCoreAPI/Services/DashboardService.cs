using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Npgsql;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly APIContext _context;
        private readonly string _connectionString;
        public DashboardService(APIContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<JObject> GetDashboardMetrics(int BranchId, int SessionYearId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new NpgsqlCommand("SELECT fn_dashboardmetrics(@p_branch_id,@p_sessionyear_id)", connection);

            // Add parameter to the command           
            command.Parameters.AddWithValue("p_branch_id", BranchId.ToString());        
            command.Parameters.AddWithValue("p_sessionyear_id", SessionYearId.ToString());          

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
        public async Task<List<vbranches>> GetAllBranch()
        {
            try
            {
                return await _context.vbranches.AsNoTracking().OrderByDescending(x => x.Id).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<int> GetCurrentSessionYear()
        {
            try
            {
                return _context.SessionYear.AsNoTracking().Where(x => x.IsDefault == true).FirstOrDefault().Id;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<OverAllPerformanceVM> GetOverAllPerformanceVM(int Id)
        {
            var vm = new OverAllPerformanceVM();
            try
            {
                var revlst = await _context.VRCOverAllPerformances
                    .Where(x => x.BranchId == Id)
                    .OrderBy(x => x.ScheduleId)
                    .ToListAsync();

                vm.LstOverAllPerformance = revlst;
                vm.OverAllPerformanceGraph = new OverAllPerformanceGraphVM
                {
                    labels = revlst.Select(x => x.ScheduleMonth).ToList(),
                    datasets = new List<OverAllPerformanceChildVM>
                            {
                                new OverAllPerformanceChildVM
                                {
                                    label = "Performance",
                                    backgroundColor = new List<string> { "#0eb5ed" },
                                    borderWidth = 1,
                                    fill = true,
                                    data = revlst.Select(item => new OverAllPerformanceLabel
                                    {
                                        Strlabel = item.ScheduleMonth,
                                        Percent = item.MrkPercent,
                                        ScheduleId = item.ScheduleId
                                    }).ToList()
                                }
                            }
                };

                vm.OverAllPerformanceClass = await GetOverAllPerformanceClassVMAsync(Id);
                vm.OverAllPerformanceGender = await GetOverAllPerformanceGenderVMAsync(Id);
            }
            catch (Exception)
            {
                // Log exception
            }
            return vm;
        }
       

        public async Task<OverAllPerformanceClassVM> GetOverAllPerformanceClassVMAsync(int Id)
        {
            var vm = new OverAllPerformanceClassVM();
            try
            {
                var revlst = await _context.VRCOverAllPerformanceClasses
                    .Where(x => x.BranchId == Id)
                    .OrderByDescending(x => x.MrkPercent)
                    .ToListAsync();

                vm.LstOverAllPerformanceClass = revlst;
                vm.OverAllPerformanceClassGraph = new OverAllPerformanceClassGraphVM
                {
                    labels = revlst.Select(x => x.ClassName).ToList(),
                    datasets = new List<OverAllPerformanceClassChildVM>
                            {
                                new OverAllPerformanceClassChildVM
                                {
                                    label = "% Marks",
                                    backgroundColor = revlst.Select(x => x.GradeColor).ToList(),
                                    borderWidth = 1,
                                    fill = true,
                                    data = revlst.Select(x => x.MrkPercent).ToList()
                                }
                            }
                };
            }
            catch (Exception)
            {
                // Log exception
            }
            return vm;
        }

        public async Task<OverAllPerformanceGenderVM> GetOverAllPerformanceGenderVMAsync(int Id)
        {
            var vm = new OverAllPerformanceGenderVM();
            try
            {
                var revlst = await _context.VRCOverAllPerformanceGenders
                    .Where(x => x.BranchId == Id)
                    .OrderByDescending(x => x.MrkPercent)
                    .ToListAsync();

                vm.LstOverAllPerformanceGender = revlst;
                vm.OverAllPerformanceGenderGraph = new OverAllPerformanceGenderGraphVM
                {
                    labels = revlst.Select(x => x.GenderName).ToList(),
                    datasets = new List<OverAllPerformanceGenderChildVM>
                            {
                                new OverAllPerformanceGenderChildVM
                                {
                                    label = "Boys vs Girls",
                                    backgroundColor = revlst.Select(_ => $"#{new Random().Next(0x1000000):X6}").ToList(),
                                    borderWidth = 1,
                                    fill = true,
                                    data = revlst.Select(x => x.MrkPercent).ToList()
                                }
                            }
                };
            }
            catch (Exception)
            {
                // Log exception
            }
            return vm;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}