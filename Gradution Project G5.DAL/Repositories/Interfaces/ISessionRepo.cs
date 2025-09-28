using Gradution_Project_G5.DAL.Entities;

namespace Gradution_Project_G5.DAL.Repositories.Interfaces
{
    public interface ISessionRepo : IGenericRepo<Session>
    {
        Task<IEnumerable<Session>> GetByCourseIdAsync(int courseId);
        Task<(IEnumerable<Session> Items, int TotalCount)> GetPagedByCourseAsync(int courseId, int page, int pageSize);
        Task<bool> HasOverlappingSessionAsync(int courseId, DateTime startDate, DateTime endDate, int? excludeSessionId = null);
    }
}