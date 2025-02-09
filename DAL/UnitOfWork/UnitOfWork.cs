using DataAccess.Data;
using DataAccess.Repositories;
using DataAccess.Repositories.Interfaces;

namespace DataAccess.UnitOfWork;

public class UnitOfWork(GameStoreDbContext context) : IUnitOfWork
{
    private IGameRepository _gameRepository;

    private IGenreRepository _genreRepository;

    private IPlatformRepository _platformRepository;

    private IGameGenreRepository _gameGenreRepository;

    private IGamePlatformRepository _gamePlatformRepository;

    public IGameRepository GameRepository
    {
        get
        {
            _gameRepository ??= new GameRepository(context);

            return _gameRepository;
        }
    }

    public IPlatformRepository PlatformRepository
    {
        get
        {
            _platformRepository ??= new PlatformRepository(context);

            return _platformRepository;
        }
    }

    public IGenreRepository GenreRepository
    {
        get
        {
            _genreRepository ??= new GenreRepository(context);

            return _genreRepository;
        }
    }

    public IGameGenreRepository GameGenreRepository
    {
        get
        {
            _gameGenreRepository ??= new GameGenreRepository(context);

            return _gameGenreRepository;
        }
    }

    public IGamePlatformRepository GamePlatformRepository
    {
        get
        {
            _gamePlatformRepository ??= new GamePlatformRepository(context);

            return _gamePlatformRepository;
        }
    }

    public int SaveChanges()
    {
        return context.SaveChanges();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }
}
