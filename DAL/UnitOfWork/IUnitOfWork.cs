using DataAccess.Repositories.Interfaces;

namespace DataAccess.UnitOfWork;

public interface IUnitOfWork
{
    IGameRepository GameRepository { get; }

    IGenreRepository GenreRepository { get; }

    IPlatformRepository PlatformRepository { get; }

    IGameGenreRepository GameGenreRepository { get; }

    IGamePlatformRepository GamePlatformRepository { get; }

    int SaveChanges();

    Task<int> SaveChangesAsync();
}
