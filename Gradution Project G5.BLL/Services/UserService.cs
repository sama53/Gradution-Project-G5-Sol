using Gradution_Project_G5.BLL.Interfaces;
using Gradution_Project_G5.BLL.ViewModels;
using Gradution_Project_G5.BLL.ViewModels.UsersVM;
using Gradution_Project_G5.DAL.Entities;
using Gradution_Project_G5.DAL.Models;
using Gradution_Project_G5.DAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Gradution_Project_G5.BLL.Services
{
    public class UserServices : IUserServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserServices> _logger;

        public UserServices(IUnitOfWork unitOfWork, ILogger<UserServices> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<Result<UserVM>> GetInstructorAsUserAsync(int id)
        {
            try
            {
                var instructor = await _unitOfWork.Instructors.GetByIdAsync(id);
                if (instructor == null)
                    return ResultHelper.Failure<UserVM>("Instructor not found");

                return ResultHelper.Success(new UserVM
                {
                    Id = instructor.Id,
                    Name = $"{instructor.FirstName} {instructor.LastName}",
                    Email = instructor.Email,
                    Role = UserRole.Instructor,
                    IsActive = instructor.IsActive,
                    IsInstructor = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting instructor as user: {Id}", id);
                return ResultHelper.Failure<UserVM>("An error occurred");
            }
        }
        public async Task<Result<UserVM>> GetUserByIdAsync(int id)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user == null)
                    return ResultHelper.Failure<UserVM>("User not found"); 

                var userVM = MapToVM(user);
                return ResultHelper.Success(userVM); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID: {Id}", id);
                return ResultHelper.Failure<UserVM>("An error occurred while retrieving the user"); 
            }
        }

        public async Task<Result<PagedResult<UserVM>>> GetAllUsersAsync(int page = 1, int pageSize = 5)
        {
            try
            {
                var users = await _unitOfWork.Users.GetAllAsync();
                var activeUsers = users.Where(u => u.IsActive).ToList();

                var instructors = await _unitOfWork.Instructors.GetAllAsync();
                var activeInstructors = instructors.Where(i => i.IsActive).ToList();

                var instructorVMs = activeInstructors.Select(i => new UserVM
                {
                    Id = i.Id,
                    Name = $"{i.FirstName} {i.LastName}",
                    Email = i.Email,
                    Role = UserRole.Instructor,
                    IsActive = i.IsActive,
                    IsInstructor = true 
                }).ToList();

                var allVMs = activeUsers.Select(MapToVM)
                    .Concat(instructorVMs)
                    .ToList();

                var totalCount = allVMs.Count;

                var pagedResult = new PagedResult<UserVM>
                {
                    Items = allVMs.Skip((page - 1) * pageSize).Take(pageSize),
                    TotalCount = totalCount,
                    PageNumber = page,
                    PageSize = pageSize
                };

                return ResultHelper.Success(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return ResultHelper.Failure<PagedResult<UserVM>>("An error occurred while retrieving users");
            }
        }

        public async Task<Result<PagedResult<UserVM>>> GetUsersByRoleAsync(string role, int page = 1, int pageSize = 5)
        {
            try
            {
                if (!Enum.TryParse<UserRole>(role, true, out var userRole))
                    return ResultHelper.Failure<PagedResult<UserVM>>("Invalid user role"); 

                var users = await _unitOfWork.Users.GetByRoleAsync(userRole);
                var totalCount = users.Count();

                var pagedUsers = users
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(MapToVM);

                var pagedResult = new PagedResult<UserVM>
                {
                    Items = pagedUsers,
                    TotalCount = totalCount,
                    PageNumber = page,
                    PageSize = pageSize
                };

                return ResultHelper.Success(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users by role: {Role}", role);
                return ResultHelper.Failure<PagedResult<UserVM>>("An error occurred while retrieving users"); 
            }
        }

        public async Task<Result<PagedResult<UserVM>>> SearchUsersAsync(string searchTerm, int page = 1, int pageSize = 5)
        {
            try
            {
                var allUsers = await _unitOfWork.Users.GetAllAsync();
                var filteredUsers = allUsers
                    .Where(u => u.IsActive &&
                               (u.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                u.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                u.Role.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));

                var totalCount = filteredUsers.Count();

                var pagedUsers = filteredUsers
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(MapToVM);

                var pagedResult = new PagedResult<UserVM>
                {
                    Items = pagedUsers,
                    TotalCount = totalCount,
                    PageNumber = page,
                    PageSize = pageSize
                };

                return ResultHelper.Success(pagedResult); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching users with term: {SearchTerm}", searchTerm);
                return ResultHelper.Failure<PagedResult<UserVM>>("An error occurred while searching users");
            }
        }

        public async Task<Result<UserVM>> CreateUserAsync(UserCreateVM createVM)
        {
            try
            {
                var isUnique = await _unitOfWork.Users.IsEmailUniqueAsync(createVM.Email);
                if (!isUnique)
                    return ResultHelper.Failure<UserVM>("Email already exists");

                if (createVM.Role == UserRole.Instructor)
                {
                    var nameParts = createVM.Name.Trim().Split(' ', 2);
                    var instructor = new Instructor
                    {
                        FirstName = nameParts[0],
                        LastName = nameParts.Length > 1 ? nameParts[1] : "",
                        Email = createVM.Email,
                        Specialization = Specialization.SoftwareDevelopment,
                        IsActive = true
                    };

                    await _unitOfWork.Instructors.AddAsync(instructor);
                    await _unitOfWork.SaveChangesAsync();

                    return ResultHelper.Success(new UserVM
                    {
                        Id = instructor.Id,
                        Name = $"{instructor.FirstName} {instructor.LastName}",
                        Email = instructor.Email,
                        Role = UserRole.Instructor,
                        IsActive = instructor.IsActive
                    });
                }

                var user = new User
                {
                    Name = createVM.Name,
                    Email = createVM.Email,
                    Role = createVM.Role,
                    IsActive = true
                };

                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                return ResultHelper.Success(MapToVM(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return ResultHelper.Failure<UserVM>("An error occurred while creating the user");
            }
        }

        public async Task<Result<UserVM>> UpdateUserAsync(int id, UserEditVM editVM)
        {
            try
            {
                if (editVM.IsInstructor)
                {
                    var instructor = await _unitOfWork.Instructors.GetByIdAsync(id);
                    if (instructor == null)
                        return ResultHelper.Failure<UserVM>("Instructor not found");

                    var isInstructorEmailUnique = await _unitOfWork.Instructors.IsEmailUniqueAsync(editVM.Email, id);
                    if (!isInstructorEmailUnique)
                        return ResultHelper.Failure<UserVM>("Email already exists");

                    var nameParts = editVM.Name.Trim().Split(' ', 2);
                    instructor.FirstName = nameParts[0];
                    instructor.LastName = nameParts.Length > 1 ? nameParts[1] : "";
                    instructor.Email = editVM.Email;

                    _unitOfWork.Instructors.Update(instructor);
                    await _unitOfWork.SaveChangesAsync();

                    return ResultHelper.Success(new UserVM
                    {
                        Id = instructor.Id,
                        Name = $"{instructor.FirstName} {instructor.LastName}",
                        Email = instructor.Email,
                        Role = UserRole.Instructor,
                        IsActive = instructor.IsActive,
                        IsInstructor = true
                    });
                }

                // باقي الـ Users عادي
                var user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user == null)
                    return ResultHelper.Failure<UserVM>("User not found");

                var isUnique = await _unitOfWork.Users.IsEmailUniqueAsync(editVM.Email, id);
                if (!isUnique)
                    return ResultHelper.Failure<UserVM>("Email already exists");

                user.Name = editVM.Name;
                user.Email = editVM.Email;
                user.Role = editVM.Role;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return ResultHelper.Success(MapToVM(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID: {Id}", id);
                return ResultHelper.Failure<UserVM>("An error occurred while updating the user");
            }
        }
        public async Task<Result<bool>> DeleteInstructorAsync(int id)
        {
            try
            {
                var instructor = await _unitOfWork.Instructors.GetByIdAsync(id);
                if (instructor == null)
                    return ResultHelper.Failure<bool>("Instructor not found");

                // Unassign instructor from all related courses
                var courses = await _unitOfWork.Courses.FindAsync(c => c.InstructorId == id);
                foreach (var course in courses)
                {
                    course.InstructorId = null;
                    _unitOfWork.Courses.Update(course);
                }

                instructor.IsActive = false;
                _unitOfWork.Instructors.Update(instructor);
                await _unitOfWork.SaveChangesAsync();

                return ResultHelper.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting instructor with ID: {Id}", id);
                return ResultHelper.Failure<bool>("An error occurred while deleting the instructor");
            }
        }
        public async Task<Result<bool>> DeleteUserAsync(int id)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user == null)
                    return ResultHelper.Failure<bool>("User not found");

                if (user.Role == UserRole.Trainee)
                {
                    var grades = await _unitOfWork.Grades.GetByTraineeIdAsync(id);
                    foreach (var grade in grades)
                        _unitOfWork.Grades.Delete(grade);
                }

                user.IsActive = false;
                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return ResultHelper.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID: {Id}", id);
                return ResultHelper.Failure<bool>("An error occurred while deleting the user");
            }
        }

        public async Task<Result<bool>> IsEmailUniqueAsync(string email, int? excludeId = null)
        {

            try
            {

                var isUnique = await _unitOfWork.Users.IsEmailUniqueAsync(email, excludeId);
                return ResultHelper.Success(isUnique); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking email uniqueness");
                return ResultHelper.Failure<bool>("An error occurred while checking email uniqueness"); 
            }
        }

        private UserVM MapToVM(User user)
        {
            return new UserVM
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive
            };
        }
    }
}