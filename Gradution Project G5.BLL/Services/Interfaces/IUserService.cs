using Gradution_Project_G5.BLL.ViewModels;
using Gradution_Project_G5.BLL.ViewModels.UsersVM;

namespace Gradution_Project_G5.BLL.Interfaces
{
    public interface IUserServices
    {
        Task<Result<UserVM>> GetUserByIdAsync(int id);
        Task<Result<UserVM>> GetInstructorAsUserAsync(int id);
        Task<Result<PagedResult<UserVM>>> GetAllUsersAsync(int page = 1, int pageSize = 10);
        Task<Result<PagedResult<UserVM>>> GetUsersByRoleAsync(string role, int page = 1, int pageSize = 5);
        Task<Result<PagedResult<UserVM>>> SearchUsersAsync(string searchTerm, int page = 1, int pageSize = 5);
        Task<Result<UserVM>> CreateUserAsync(UserCreateVM createVM);
        Task<Result<UserVM>> UpdateUserAsync(int id, UserEditVM editVM);
        Task<Result<bool>> DeleteUserAsync(int id);
        Task<Result<bool>> DeleteInstructorAsync(int id);
        Task<Result<bool>> IsEmailUniqueAsync(string email, int? excludeId = null);
    }
}