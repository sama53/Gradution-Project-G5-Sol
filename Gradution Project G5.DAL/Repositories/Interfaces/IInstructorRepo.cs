using Gradution_Project_G5.DAL.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gradution_Project_G5.DAL.Repositories.Interfaces
{
    public interface IInstructorRepo : IGenericRepo<Instructor>
    {
        Task<IEnumerable<SelectListItem>> GetInstructorSelectListAsync();
          }
}