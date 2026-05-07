using Gradution_Project_G5.DAL.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gradution_Project_G5.DAL.Repositories.Interfaces
{
    public interface IInstructorRepo : IGenericRepo<Instructor>
    {
        Task<IEnumerable<SelectListItem>> GetInstructorSelectListAsync();
        Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null);
    }
}