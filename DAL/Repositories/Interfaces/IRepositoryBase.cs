namespace DataAccess.Repositories.Interfaces;
public interface IRepositoryBase<T>
    where T : class
{
    Task<T?> GetByIdAsync(Guid id);

    IQueryable<T> GetAll();

    Task<T> AddAsync(T entity);

    void Update(T entity);

    void Delete(T entity);

    void DeleteRange(IEnumerable<T> entities);
}
