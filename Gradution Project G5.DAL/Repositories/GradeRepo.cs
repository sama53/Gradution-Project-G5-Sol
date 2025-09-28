using Gradution_Project_G5.DAL.Entities;
using Gradution_Project_G5.DAL.Models.Data;
using Gradution_Project_G5.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gradution_Project_G5.DAL.Repositories
{
    public class GradeRepo : GenericRepo<Grade>, IGradeRepo
    {
        public GradeRepo(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Grade>> GetBySessionIdAsync(int sessionId)
        {
            return await _context.Grades
                .Include(g => g.Trainee)
                .Include(g => g.Session)
                .ThenInclude(s => s.Course)
                .Where(g => g.SessionId == sessionId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Grade>> GetByTraineeIdAsync(int traineeId)
        {
            return await _context.Grades
                .Include(g => g.Session)
                .ThenInclude(s => s.Course)
                .Where(g => g.TraineeId == traineeId)
                .ToListAsync();
        }

         }
}