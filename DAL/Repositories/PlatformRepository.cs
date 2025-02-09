using DataAccess.Data;
using DataAccess.Entities;
using DataAccess.Repositories.Interfaces;

namespace DataAccess.Repositories;
public class PlatformRepository(GameStoreDbContext context)
    : RepositoryBase<Platform>(context), IPlatformRepository
{
}
