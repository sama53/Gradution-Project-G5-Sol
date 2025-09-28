using Gradution_Project_G5.DAL.Entities;
using Gradution_Project_G5.DAL.Models;

namespace Gradution_Project_G5.DAL.Repositories.Interfaces
{
    public interface IUserRepo : IGenericRepo<User>
    {
        Task<IEnumerable<User>> GetByRoleAsync(UserRole role);
        Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null);
   }
}