using Gradution_Project_G5.BLL.ViewModels.CoursesVM;
using Gradution_Project_G5.BLL.ViewModels;

namespace Gradution_Project_G5.BLL.Interfaces
{
    public interface ICourseServices
    {
        Task<Result<CourseVM>> GetCourseByIdAsync(int id);
        Task<Result<PagedResult<CourseVM>>> GetAllCoursesAsync(int page = 1, int pageSize = 10);
        Task<Result<PagedResult<CourseVM>>> SearchCoursesAsync(string searchTerm, int page = 1, int pageSize = 10);
        Task<Result<CourseVM>> CreateCourseAsync(CourseCreateVM createVM);
        Task<Result<CourseVM>> UpdateCourseAsync(int id, CourseEditVM editVM);
        Task<Result<bool>> DeleteCourseAsync(int id);
        Task<Result<bool>> IsCourseNameUniqueAsync(string name, int? excludeId = null);
    }
}