using Gradution_Project_G5.BLL.Interfaces;
using Gradution_Project_G5.BLL.ViewModels;
using Gradution_Project_G5.BLL.ViewModels.CoursesVM;
using Gradution_Project_G5.DAL.Entities;
using Gradution_Project_G5.DAL.Models;
using Gradution_Project_G5.DAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Gradution_Project_G5.BLL.Services
{
    public class CourseServices : ICourseServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CourseServices> _logger;

        public CourseServices(IUnitOfWork unitOfWork, ILogger<CourseServices> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<CourseVM>> GetCourseByIdAsync(int id)
        {
            try
            {
                var course = await _unitOfWork.Courses.GetByIdAsync(id);
                if (course == null)
                    return ResultHelper.Failure<CourseVM>("Course not found"); 

                var courseVM = MapToVM(course);
                return ResultHelper.Success(courseVM); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting course by ID: {Id}", id);
                return ResultHelper.Failure<CourseVM>("An error occurred while retrieving the course"); 
            }
        }

        public async Task<Result<PagedResult<CourseVM>>> GetAllCoursesAsync(int page = 1, int pageSize = 10)
        {
            try
            {
                var courses = await _unitOfWork.Courses.GetAllAsync();
                var totalCount = courses.Count();

                var pagedCourses = courses
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(MapToVM);

                var pagedResult = new PagedResult<CourseVM>
                {
                    Items = pagedCourses,
                    TotalCount = totalCount,
                    PageNumber = page,
                    PageSize = pageSize
                };

                return ResultHelper.Success(pagedResult); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all courses");
                return ResultHelper.Failure<PagedResult<CourseVM>>("An error occurred while retrieving courses"); 
            }
        }

        public async Task<Result<PagedResult<CourseVM>>> SearchCoursesAsync(string searchTerm, int page = 1, int pageSize = 10)
        {
            try
            {
                var allCourses = await _unitOfWork.Courses.GetAllAsync();
                var filteredCourses = allCourses
                    .Where(c => c.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                               c.Category.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

                var totalCount = filteredCourses.Count();

                var pagedCourses = filteredCourses
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(MapToVM);

                var pagedResult = new PagedResult<CourseVM>
                {
                    Items = pagedCourses,
                    TotalCount = totalCount,
                    PageNumber = page,
                    PageSize = pageSize
                };

                return ResultHelper.Success(pagedResult); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching courses with term: {SearchTerm}", searchTerm);
                return ResultHelper.Failure<PagedResult<CourseVM>>("An error occurred while searching courses"); 
            }
        }

        public async Task<Result<CourseVM>> CreateCourseAsync(CourseCreateVM createVM)
        {
            try
            {
                
                var isUnique = await _unitOfWork.Courses.IsCourseNameUniqueAsync(createVM.Name);
                if (!isUnique)
                    return ResultHelper.Failure<CourseVM>("Course name already exists"); 

                var course = new Course
                {
                    Name = createVM.Name,
                    Description = createVM.Description,
                    Category = createVM.Category,
                    InstructorId = createVM.InstructorId,
                    IsActive = true
                };

                await _unitOfWork.Courses.AddAsync(course);
                await _unitOfWork.SaveChangesAsync();

                var courseVM = MapToVM(course);
                return ResultHelper.Success(courseVM); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating course");
                return ResultHelper.Failure<CourseVM>("An error occurred while creating the course"); 
            }
        }

        public async Task<Result<CourseVM>> UpdateCourseAsync(int id, CourseEditVM editVM)
        {
            try
            {
                var course = await _unitOfWork.Courses.GetByIdAsync(id);
                if (course == null)
                    return ResultHelper.Failure<CourseVM>("Course not found"); 

               
                var isUnique = await _unitOfWork.Courses.IsCourseNameUniqueAsync(editVM.Name, id);
                if (!isUnique)
                    return ResultHelper.Failure<CourseVM>("Course name already exists"); 

                course.Name = editVM.Name;
                course.Description = editVM.Description;
                course.Category = editVM.Category;
                course.InstructorId = editVM.InstructorId;

                _unitOfWork.Courses.Update(course);
                await _unitOfWork.SaveChangesAsync();

                var courseVM = MapToVM(course);
                return ResultHelper.Success(courseVM); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating course with ID: {Id}", id);
                return ResultHelper.Failure<CourseVM>("An error occurred while updating the course"); 
            }
        }

        public async Task<Result<bool>> DeleteCourseAsync(int id)
        {
            try
            {
                var course = await _unitOfWork.Courses.GetByIdAsync(id);
                if (course == null)
                    return ResultHelper.Failure<bool>("Course not found");

                course.IsActive = false;
                _unitOfWork.Courses.Update(course);
                await _unitOfWork.SaveChangesAsync();

                return ResultHelper.Success(true); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting course with ID: {Id}", id);
                return ResultHelper.Failure<bool>("An error occurred while deleting the course"); 
            }
        }


        public async Task<Result<bool>> IsCourseNameUniqueAsync(string name, int? excludeId = null)
        {
            try
            {
                var isUnique = await _unitOfWork.Courses.IsCourseNameUniqueAsync(name, excludeId);
                return ResultHelper.Success(isUnique); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking course name uniqueness");
                return ResultHelper.Failure<bool>("An error occurred while checking course name uniqueness"); 
            }
        }

        private CourseVM MapToVM(Course course)
        {
            return new CourseVM
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description,
                Category = course.Category,
                IsActive = course.IsActive,
                InstructorId = course.InstructorId,
                InstructorName = course.Instructor != null ?
                    $"{course.Instructor.FirstName} {course.Instructor.LastName}" : "Not Assigned"
            };
        }
    }
}