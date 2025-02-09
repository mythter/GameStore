using DataAccess.Data;
using DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;
public class RepositoryBase<T>(GameStoreDbContext context) : IRepositoryBase<T>
    where T : class
{
    protected DbSet<T> DbSet => context.Set<T>();

    public async Task<T> AddAsync(T entity)
    {
        var entry = await DbSet.AddAsync(entity);
        return entry.Entity;
    }

    public void Delete(T entity)
    {
        DbSet.Remove(entity);
    }

    public void DeleteRange(IEnumerable<T> entities)
    {
        DbSet.RemoveRange(entities);
    }

    public IQueryable<T> GetAll()
    {
        return DbSet.AsNoTracking();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await DbSet.FindAsync(id);
    }

    public void Update(T entity)
    {
        DbSet.Update(entity);
    }
}
