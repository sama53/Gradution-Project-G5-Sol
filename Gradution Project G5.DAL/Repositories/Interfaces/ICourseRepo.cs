using Gradution_Project_G5.DAL.Entities;
using Gradution_Project_G5.DAL.Models;

namespace Gradution_Project_G5.DAL.Repositories.Interfaces
{
    public interface ICourseRepo : IGenericRepo<Course>
    {
        Task<bool> IsCourseNameUniqueAsync(string name, int? excludeId = null);
        Task<IEnumerable<Course>> GetAllWithIncludesAsync();
        Task<Course?> GetByIdWithIncludesAsync(int id);
    }
}