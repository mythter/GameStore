using DataAccess.Data;
using DataAccess.Entities;
using DataAccess.Repositories.Interfaces;

namespace DataAccess.Repositories;

public class GamePlatformRepository(GameStoreDbContext context)
    : RepositoryBase<GamePlatform>(context), IGamePlatformRepository
{
}