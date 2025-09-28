using Gradution_Project_G5.DAL.Entities;
using Gradution_Project_G5.DAL.Models;
using Gradution_Project_G5.DAL.Models.Data;
using Gradution_Project_G5.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gradution_Project_G5.DAL.Repositories
{
    public class UserRepo : GenericRepo<User>, IUserRepo
    {
        public UserRepo(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<User>> GetByRoleAsync(UserRole role)
        {
            return await _context.Users
                .Where(u => u.Role == role && u.IsActive)
                .ToListAsync();
        }

        public async Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null)
        {
            var query = _context.Users.Where(u => u.Email == email);

            if (excludeId.HasValue)
                query = query.Where(u => u.Id != excludeId.Value);

            return !await query.AnyAsync();
        }

       }
}