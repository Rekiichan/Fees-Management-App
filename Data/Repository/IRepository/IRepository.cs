using System.Linq.Expressions;

namespace FeeCollectorApplication.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter, bool tracked = true);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null);
        Task<IEnumerable<T>> GetLimitAsync(Expression<Func<T, bool>> filter = null, int numberLimit = 10);
        Task Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
    }
}
