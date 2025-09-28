using Gradution_Project_G5.DAL.Entities;

namespace Gradution_Project_G5.DAL.Repositories.Interfaces
{
    public interface IGradeRepo : IGenericRepo<Grade>
    {
        Task<IEnumerable<Grade>> GetBySessionIdAsync(int sessionId);
        Task<IEnumerable<Grade>> GetByTraineeIdAsync(int traineeId);
       
    }
}