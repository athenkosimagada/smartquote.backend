using Microsoft.EntityFrameworkCore;
using smartquote.api.Entities;
using smartquote.api.Repositories.Interfaces;
using System.Linq.Expressions;

namespace smartquote.api.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly DbContext _context;

    public Repository(DbContext context)
    {
        _context = context;
    }

    public async Task<TEntity?> GetByIdAsync(int id, bool includeItems = false)
    {
        var query = _context.Set<TEntity>().AsQueryable();

        if (includeItems && typeof(TEntity) == typeof(Quote))
        {
            query = query.Include("Items");
        }

        return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(int pageNumber, int pageSize, bool includeItems = false)
    {
        var query = _context.Set<TEntity>().AsQueryable();

        if (includeItems && typeof(TEntity) == typeof(Quote))
        {
            query = query.Include("Items");
        }

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _context.Set<TEntity>().Where(predicate).ToListAsync();
    }

    public async Task AddAsync(TEntity entity)
    {
        await _context.Set<TEntity>().AddAsync(entity);
    }
    public async Task AddRangeAsync(IEnumerable<TEntity> entities)
    {
        await _context.Set<TEntity>().AddRangeAsync(entities);
    }

    public void Remove(TEntity entity)
    {
        _context.Set<TEntity>().Remove(entity);
    }

    public void RemoveRange(IEnumerable<TEntity> entities)
    {
        _context.Set<TEntity>().RemoveRange(entities);
    }
}
