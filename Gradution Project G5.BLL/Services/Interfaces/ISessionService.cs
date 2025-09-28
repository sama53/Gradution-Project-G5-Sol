using Gradution_Project_G5.BLL.ViewModels.SessionsVM;
using Gradution_Project_G5.BLL.ViewModels;

namespace Gradution_Project_G5.BLL.Interfaces
{
    public interface ISessionServices
    {
        Task<Result<SessionVM>> GetSessionByIdAsync(int id);
        Task<Result<PagedResult<SessionVM>>> GetAllSessionsAsync(int page = 1, int pageSize = 10);
        Task<Result<PagedResult<SessionVM>>> SearchSessionsAsync(string searchTerm, int page = 1, int pageSize = 10);
        Task<Result<SessionVM>> CreateSessionAsync(SessionCreateVM createVM);
        Task<Result<SessionVM>> UpdateSessionAsync(int id, SessionEditVM editVM);
        Task<Result<bool>> DeleteSessionAsync(int id);
    }
}