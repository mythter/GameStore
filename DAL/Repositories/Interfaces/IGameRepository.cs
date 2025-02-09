using DataAccess.Entities;

namespace DataAccess.Repositories.Interfaces;

public interface IGameRepository : IRepositoryBase<Game>
{
    Task<Game> GetByKeyAsync(string key);
}
