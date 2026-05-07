using Gradution_Project_G5.DAL.Entities;
using Gradution_Project_G5.DAL.Models;
using Gradution_Project_G5.DAL.Models.Data;
using Gradution_Project_G5.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gradution_Project_G5.DAL.Repositories
{
    public class CourseRepo : GenericRepo<Course>, ICourseRepo
    {
        public CourseRepo(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Course>> GetAllWithIncludesAsync()
            => await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Sessions)
                .ToListAsync();

        public async Task<Course?> GetByIdWithIncludesAsync(int id)
            => await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Sessions)
                .FirstOrDefaultAsync(c => c.Id == id);

        public async Task<bool> IsCourseNameUniqueAsync(string name, int? excludeId = null)
        {
            var query = _context.Courses.Where(c => c.Name == name);
            if (excludeId.HasValue)
                query = query.Where(c => c.Id != excludeId.Value);
            return !await query.AnyAsync();
        }
    }

}
