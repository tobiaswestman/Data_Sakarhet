using Microsoft.EntityFrameworkCore;
using Server_API.Contexts;
using Server_API.Helpers.Interfaces;
using System.Linq.Expressions;

namespace Server_API.Helpers.Repositories.BaseModels;

public abstract class Repo<TEntity> where TEntity : class
{
    private readonly DataContext _context;

    protected Repo(DataContext context)
    {
        _context = context;
    }

    // Adds a new entity to the database asynchronously.
    public virtual async Task<TEntity> AddAsync(TEntity entity)
    {
        _context.Set<TEntity>().Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    // Retrieves all entities from the database asynchronously.
    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _context.Set<TEntity>().ToListAsync();
    }

    // Retrieves a single entity from the database based on the provided predicate asynchronously.
    public virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _context.Set<TEntity>().FirstOrDefaultAsync(predicate);
    }

    // Retrieves a list of entities from the database based on the provided predicate asynchronously.
    public virtual async Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _context.Set<TEntity>().Where(predicate).ToListAsync();
    }

    // Updates the given entity in the database asynchronously.
    public virtual async Task<TEntity> UpdateAsync(TEntity entity)
    {
        _context.Set<TEntity>().Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    // Removes the given entity from the database asynchronously.
    public virtual async Task DeleteAsync(TEntity entity)
    {
        _context.Set<TEntity>().Remove(entity);
        await _context.SaveChangesAsync();
    }
}