using Gradution_Project_G5.BLL.ViewModels;
using Gradution_Project_G5.DAL.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gradution_Project_G5.BLL.Interfaces
{
    public interface IInstructorServices
    {
        Task<Result<IEnumerable<SelectListItem>>> GetInstructorsSelectListAsync();
        Task<Result<Instructor>> GetInstructorById(int id);
    }
}