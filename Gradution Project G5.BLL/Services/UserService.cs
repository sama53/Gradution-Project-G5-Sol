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

        public async Task<Result<PagedResult<UserVM>>> GetAllUsersAsync(int page = 1, int pageSize = 10)
        {
            try
            {
                var users = await _unitOfWork.Users.GetAllAsync();
                var activeUsers = users.Where(u => u.IsActive).ToList();
                var totalCount = activeUsers.Count;

                var pagedUsers = activeUsers
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
                _logger.LogError(ex, "Error getting all users");
                return ResultHelper.Failure<PagedResult<UserVM>>("An error occurred while retrieving users"); 
            }
        }

        public async Task<Result<PagedResult<UserVM>>> GetUsersByRoleAsync(string role, int page = 1, int pageSize = 10)
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

        public async Task<Result<PagedResult<UserVM>>> SearchUsersAsync(string searchTerm, int page = 1, int pageSize = 10)
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

                var user = new User
                {
                    Name = createVM.Name,
                    Email = createVM.Email,
                    Role = createVM.Role,
                    IsActive = true
                };

                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                var userVM = MapToVM(user);
                return ResultHelper.Success(userVM); 
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

                var userVM = MapToVM(user);
                return ResultHelper.Success(userVM); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID: {Id}", id);
                return ResultHelper.Failure<UserVM>("An error occurred while updating the user");
            }
        }

        public async Task<Result<bool>> DeleteUserAsync(int id)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user == null)
                    return ResultHelper.Failure<bool>("User not found"); 

              
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