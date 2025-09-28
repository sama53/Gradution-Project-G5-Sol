using Gradution_Project_G5.BLL.Interfaces;
using Gradution_Project_G5.BLL.Validation;
using Gradution_Project_G5.BLL.ViewModels;
using Gradution_Project_G5.BLL.ViewModels.CoursesVM;
using Gradution_Project_G5.BLL.ViewModels.SessionsVM;
using Gradution_Project_G5.DAL.Entities;
using Gradution_Project_G5.DAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Gradution_Project_G5.BLL.Services
{
    public class SessionServices : ISessionServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SessionServices> _logger;
        private readonly DateValidation _dateValidation;

        public SessionServices(IUnitOfWork unitOfWork, ILogger<SessionServices> logger, DateValidation dateValidation)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _dateValidation = dateValidation;
        }

        public async Task<Result<SessionVM>> GetSessionByIdAsync(int id)
        {
            try
            {
                var session = await _unitOfWork.Sessions.GetByIdAsync(id);
                if (session == null)
                    return ResultHelper.Failure<SessionVM>("Session not found"); 

                var sessionVM = await MapToVM(session);
                return ResultHelper.Success(sessionVM); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting session by ID: {Id}", id);
                return ResultHelper.Failure<SessionVM>("An error occurred while retrieving the session"); 
            }
        }

        public async Task<Result<PagedResult<SessionVM>>> SearchSessionsAsync(string searchTerm, int page = 1, int pageSize = 10)
        {
            try
            {
                var allSessions = await _unitOfWork.Sessions.GetAllAsync();
                var filteredSessions = allSessions
                    .Where(c => c.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

                var totalCount = filteredSessions.Count();

                var pagedSessions = filteredSessions
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(MapToVM)
                    .Select(x => x.Result);

                var pagedResult = new PagedResult<SessionVM>
                {
                    Items = pagedSessions,
                    TotalCount = totalCount,
                    PageNumber = page,
                    PageSize = pageSize
                };

                return ResultHelper.Success(pagedResult); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching sessions with term: {SearchTerm}", searchTerm);
                return ResultHelper.Failure<PagedResult<SessionVM>>("An error occurred while searching sessions"); 
            }
        }

        public async Task<Result<PagedResult<SessionVM>>> GetAllSessionsAsync(int page = 1, int pageSize = 10)
        {
            try
            {
                var sessions = await _unitOfWork.Sessions.GetAllAsync();
                var totalCount = sessions.Count();

                var sessionVMs = new List<SessionVM>();
                foreach (var session in sessions.Skip((page - 1) * pageSize).Take(pageSize))
                {
                    sessionVMs.Add(await MapToVM(session));
                }

                var pagedResult = new PagedResult<SessionVM>
                {
                    Items = sessionVMs,
                    TotalCount = totalCount,
                    PageNumber = page,
                    PageSize = pageSize
                };

                return ResultHelper.Success(pagedResult); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all sessions");
                return ResultHelper.Failure<PagedResult<SessionVM>>("An error occurred while retrieving sessions"); 
            }
        }

        public async Task<Result<SessionVM>> CreateSessionAsync(SessionCreateVM createVM)
        {
            try
            {
            
                var dateValidation = await _dateValidation.ValidateSessionDatesAsync(
                    createVM.CourseId, createVM.StartDate, createVM.EndDate);

                if (!dateValidation.Success)
                    return ResultHelper.Failure<SessionVM>(dateValidation.Message); 

                var session = new Session
                {
                    Title = createVM.Title,
                    StartDate = createVM.StartDate,
                    EndDate = createVM.EndDate,
                    CourseId = createVM.CourseId
                };

                await _unitOfWork.Sessions.AddAsync(session);
                await _unitOfWork.SaveChangesAsync();

                var sessionVM = await MapToVM(session);
                return ResultHelper.Success(sessionVM); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating session");
                return ResultHelper.Failure<SessionVM>("An error occurred while creating the session"); 
            }
        }

        public async Task<Result<SessionVM>> UpdateSessionAsync(int id, SessionEditVM editVM)
        {
            try
            {
                var session = await _unitOfWork.Sessions.GetByIdAsync(id);
                if (session == null)
                    return ResultHelper.Failure<SessionVM>("Session not found"); 

                session.Title = editVM.Title;
                session.StartDate = editVM.StartDate;
                session.EndDate = editVM.EndDate;
                session.CourseId = editVM.CourseId;

                _unitOfWork.Sessions.Update(session);
                await _unitOfWork.SaveChangesAsync();

                var sessionVM = await MapToVM(session);
                return ResultHelper.Success(sessionVM); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating session with ID: {Id}", id);
                return ResultHelper.Failure<SessionVM>("An error occurred while updating the session"); 
            }
        }

        public async Task<Result<bool>> DeleteSessionAsync(int id)
        {
            try
            {
                var session = await _unitOfWork.Sessions.GetByIdAsync(id);
                if (session == null)
                    return ResultHelper.Failure<bool>("Session not found"); 

                _unitOfWork.Sessions.Delete(session);
                await _unitOfWork.SaveChangesAsync();

                return ResultHelper.Success(true); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting session with ID: {Id}", id);
                return ResultHelper.Failure<bool>("An error occurred while deleting the session"); 
            }
        }

        private async Task<SessionVM> MapToVM(Session session)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(session.CourseId);

            return new SessionVM
            {
                Id = session.Id,
                Title = session.Title,
                StartDate = session.StartDate,
                EndDate = session.EndDate,
                CourseId = session.CourseId,
                CourseName = course?.Name ?? "Unknown Course",
                Duration = (session.EndDate - session.StartDate).TotalHours
            };
        }
    }
}