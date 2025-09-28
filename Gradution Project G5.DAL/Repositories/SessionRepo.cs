using Gradution_Project_G5.DAL.Entities;
using Gradution_Project_G5.DAL.Models.Data;
using Gradution_Project_G5.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gradution_Project_G5.DAL.Repositories
{
    public class SessionRepo : GenericRepo<Session>, ISessionRepo
    {
        public SessionRepo(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Session>> GetByCourseIdAsync(int courseId)
        {
            return await _context.Sessions
                .Where(s => s.CourseId == courseId)
                .Include(s => s.Course)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Session> Items, int TotalCount)> GetPagedByCourseAsync(int courseId, int page, int pageSize)
        {
            var query = _context.Sessions
                .Where(s => s.CourseId == courseId)
                .Include(s => s.Course);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(s => s.StartDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<bool> HasOverlappingSessionAsync(int courseId, DateTime startDate, DateTime endDate, int? excludeSessionId = null)
        {
            var query = _context.Sessions
                .Where(s => s.CourseId == courseId &&
                           s.StartDate < endDate &&
                           s.EndDate > startDate);

            if (excludeSessionId.HasValue)
                query = query.Where(s => s.Id != excludeSessionId.Value);

            return await query.AnyAsync();
        }
    }
}