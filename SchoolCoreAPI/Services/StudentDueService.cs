using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.Services
{
    public class StudentDueService : IStudentDueService, IDisposable
    {
        private readonly APIContext _context;

        public StudentDueService(APIContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<VStudentDues>> GetAllStudentDues(int BranchId)
        {
            try
            {
                return await _context.VStudentDues.AsNoTracking()
                    .Where(x => x.BranchId == BranchId)
                    .ToListAsync();
            }
            catch (Exception)
            {
                return new List<VStudentDues>();
            }
        }
        public async Task<List<VStudentDues>> GetAllStudentDuesByPercent(int BranchId, int SessionYearId, double duePercent)
        {

            var roundedDuePercent = Math.Round(duePercent, 2);

            var studentsWithDues = await _context.VStudentDues.AsNoTracking()
                .Where(s => s.BranchId==BranchId && s.SessionYearId==SessionYearId && s.DuePercent.HasValue && s.DuePercent.Value <= roundedDuePercent)  // Ensure DuePercent is not null and filter by rounded due percentage
                .ToListAsync();

            return studentsWithDues;

        }

        public async Task<VStudentDues> ViewStudentDue(int StudentId)
        {
            return await _context.VStudentDues.AsNoTracking()
                .SingleOrDefaultAsync(m => m.StudentId == StudentId);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
