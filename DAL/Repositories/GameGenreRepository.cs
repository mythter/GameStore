using DataAccess.Data;
using DataAccess.Entities;
using DataAccess.Repositories.Interfaces;

namespace DataAccess.Repositories;

public class GameGenreRepository(GameStoreDbContext context)
    : RepositoryBase<GameGenre>(context), IGameGenreRepository
{
}
