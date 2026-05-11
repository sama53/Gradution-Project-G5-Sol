using Gradution_Project_G5.DAL.Models;
using Gradution_Project_G5.DAL.Models.Data;
using Gradution_Project_G5.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Gradution_Project_G5.DAL.Repositories
{
    public class InstructorRepo : GenericRepo<Instructor>, IInstructorRepo
    {
        public InstructorRepo(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<SelectListItem>> GetInstructorSelectListAsync()
        {
            return await _context.Instructors
                .Where(i => i.IsActive)
                .Select(i => new SelectListItem
                {
                    Text = $"{i.FirstName} {i.LastName}",
                    Value = i.Id.ToString()
                })
                .ToListAsync();
        }
        public async Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null)
        {
            var query = _context.Instructors.Where(i => i.Email == email);

            if (excludeId.HasValue)
                query = query.Where(i => i.Id != excludeId.Value);

            return !await query.AnyAsync();
        }

    }
}