using DataAccess.Data;
using DataAccess.Entities;
using DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;
public class GameRepository(GameStoreDbContext context)
    : RepositoryBase<Game>(context), IGameRepository
{
    public async Task<Game> GetByKeyAsync(string key)
    {
        return await DbSet.SingleOrDefaultAsync(g => g.Key == key);
    }
}
