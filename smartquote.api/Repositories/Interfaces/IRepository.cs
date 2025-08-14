using System.Linq.Expressions;

namespace smartquote.api.Repositories.Interfaces;

public interface IRepository<TEntity> where TEntity : class 
{
    Task<TEntity?> GetByIdAsync(int id, bool includeItems = false);
    Task<IEnumerable<TEntity>> GetAllAsync(int pageNumber, int pageSize, bool includeItems = false);
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);

    Task AddAsync(TEntity entity);
    Task AddRangeAsync(IEnumerable<TEntity> entities);

    void Remove(TEntity entity);
    void RemoveRange(IEnumerable<TEntity> entities);
}
