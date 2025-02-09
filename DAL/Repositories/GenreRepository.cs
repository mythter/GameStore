using DataAccess.Data;
using DataAccess.Entities;
using DataAccess.Repositories.Interfaces;

namespace DataAccess.Repositories;
public class GenreRepository(GameStoreDbContext context)
    : RepositoryBase<Genre>(context), IGenreRepository
{
}
