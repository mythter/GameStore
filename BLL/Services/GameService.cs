using System.Text;
using System.Text.Json;
using AutoMapper;
using BusinessLogic.Dto.Game;
using BusinessLogic.Exceptions;
using BusinessLogic.Requests.Game;
using BusinessLogic.Services.Interfaces;
using DataAccess.Entities;
using DataAccess.Repositories.Interfaces;
using DataAccess.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services;
public class GameService(
    IUnitOfWork unitOfWork,
    IMapper mapper) : IGameService
{
    private IGameRepository GameRepository => unitOfWork.GameRepository;

    private IGenreRepository GenreRepository => unitOfWork.GenreRepository;

    private IGameGenreRepository GameGenreRepository => unitOfWork.GameGenreRepository;

    private IGamePlatformRepository GamePlatformRepository => unitOfWork.GamePlatformRepository;

    public async Task<GameDto> AddAsync(GameCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Game.Name))
        {
            throw new BadRequestException("Game name cannot be null or empty.");
        }

        var game = mapper.Map<Game>(request);
        game.Id = Guid.NewGuid();

        if (string.IsNullOrEmpty(game.Key))
        {
            game.Key = GenerateKeyFromName(game.Name);
        }

        var entity = await GameRepository.AddAsync(game);
        await AddRelatedGenresAsync(entity.Id, request.Genres);
        await AddRelatedPlatformsAsync(entity.Id, request.Platforms);

        await unitOfWork.SaveChangesAsync();

        return mapper.Map<GameDto>(entity);
    }

    public async Task DeleteByIdAsync(Guid id)
    {
        var entity = await GameRepository.GetByIdAsync(id);
        if (entity is not null)
        {
            GameRepository.Delete(entity);
            await unitOfWork.SaveChangesAsync();
        }
    }

    public async Task DeleteByKeyAsync(string key)
    {
        var entity = await GameRepository.GetByKeyAsync(key);
        if (entity is not null)
        {
            GameRepository.Delete(entity);
            await unitOfWork.SaveChangesAsync();
        }
    }

    public IQueryable<GameDto> GetAll()
    {
        var entities = GameRepository.GetAll();
        return mapper.ProjectTo<GameDto>(entities);
    }

    public async Task<GameDto> GetByIdAsync(Guid id)
    {
        var entity = await GameRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Game with id {id} was not found.");

        return mapper.Map<GameDto>(entity);
    }

    public async Task<GameDto> GetByKeyAsync(string key)
    {
        var entity = await GameRepository.GetByKeyAsync(key)
           ?? throw new NotFoundException($"Game with key {key} was not found.");

        return mapper.Map<GameDto>(entity);
    }

    public IQueryable<GameDto> GetAllByGenreId(Guid id)
    {
        var genreGames =
            from gg in GameGenreRepository.GetAll()
            where gg.GenreId == id
            join genre in GenreRepository.GetAll() on gg.GenreId equals genre.Id
            join game in GameRepository.GetAll() on gg.GameId equals game.Id
            select game;

        return mapper.ProjectTo<GameDto>(genreGames);
    }

    public async Task UpdateAsync(GameUpdateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Game.Name))
        {
            throw new BadRequestException("Game name cannot be null or empty.");
        }

        var entity = await GameRepository.GetByIdAsync(request.Game.Id)
            ?? throw new NotFoundException($"Game with id {request.Game.Id} was not found.");

        // update game
        mapper.Map(request.Game, entity);

        if (string.IsNullOrEmpty(entity.Key))
        {
            entity.Key = GenerateKeyFromName(entity.Name);
        }

        // delete old genres and platforms
        var existingGenres = GameGenreRepository.GetAll().Where(gg => gg.GameId == request.Game.Id);
        GameGenreRepository.DeleteRange(existingGenres);

        var existingPlatforms = GamePlatformRepository.GetAll().Where(gp => gp.GameId == request.Game.Id);
        GamePlatformRepository.DeleteRange(existingPlatforms);

        // link new genres and platforms
        await AddRelatedGenresAsync(entity.Id, request.Genres);
        await AddRelatedPlatformsAsync(entity.Id, request.Platforms);

        await unitOfWork.SaveChangesAsync();
    }

    public async Task<byte[]> GetGameFileAsync(string key)
    {
        var game = await GetByKeyAsync(key);

        var gameContent = JsonSerializer.Serialize(game);

        return Encoding.UTF8.GetBytes(gameContent);
    }

    public string GetGameFileName(string key)
    {
        return $"_{key}.txt";
    }

    public async Task<int> GetTotalGamesCountAsync()
    {
        return await GameRepository.GetAll().CountAsync();
    }

    private async Task AddRelatedGenresAsync(Guid gameId, IEnumerable<Guid>? genres)
    {
        if (genres is null)
        {
            return;
        }

        foreach (var genreId in genres)
        {
            var gameGenre = new GameGenre
            {
                GameId = gameId,
                GenreId = genreId,
            };
            await GameGenreRepository.AddAsync(gameGenre);
        }
    }

    private async Task AddRelatedPlatformsAsync(Guid gameId, IEnumerable<Guid>? platforms)
    {
        if (platforms is null)
        {
            return;
        }

        foreach (var platformId in platforms)
        {
            var gamePlatform = new GamePlatform
            {
                GameId = gameId,
                PlatformId = platformId,
            };
            await GamePlatformRepository.AddAsync(gamePlatform);
        }
    }

    private static string GenerateKeyFromName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be null or empty.", nameof(name));
        }

        char[] separator = [' ', '_', '-', '.', ',', ':', ';'];

        var words = name
            .Split(separator, StringSplitOptions.RemoveEmptyEntries)
            .Select(word => word.Trim().ToLowerInvariant())
            .Where(word => !string.IsNullOrEmpty(word))
            .ToArray();

        for (int i = 1; i < words.Length; i++)
        {
            words[i] = char.ToUpper(words[i][0]) + words[i][1..];
        }

        var camelCaseKey = string.Join(string.Empty, words);
        return camelCaseKey;
    }
}
