using System.Linq.Expressions;

namespace Gradution_Project_G5.DAL.Repositories.Interfaces
{
    public interface IGenericRepo<T> where T : class
    {
        // Async methods
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);

        // Sync methods
        void Update(T entity);
        void Delete(T entity);
    }
}