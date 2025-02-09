using BusinessLogic.Dto.Game;
using BusinessLogic.Dto.Genre;
using BusinessLogic.Dto.Platform;
using DataAccess.Entities;

namespace XUnitTests.Helpers;

public static class TestHelper
{
    public static Guid CreateGuid(int seed)
    {
        byte[] guidBytes = new byte[16];
        BitConverter.GetBytes(seed).CopyTo(guidBytes, 0);
        return new Guid(guidBytes);
    }

    public static IQueryable<Game> GetGames(int count)
    {
        var games = Enumerable
            .Range(0, count)
            .Select(num => new Game() { Id = CreateGuid(num + 1) })
            .AsQueryable();

        return games;
    }

    public static IQueryable<Genre> GetGenres(int count)
    {
        var genres = Enumerable
            .Range(0, count)
            .Select(num => new Genre() { Id = CreateGuid(num + 1) })
            .AsQueryable();

        return genres;
    }

    public static IQueryable<Platform> GetPlatforms(int count)
    {
        var platforms = Enumerable
            .Range(0, count)
            .Select(num => new Platform() { Id = CreateGuid(num + 1) })
            .AsQueryable();

        return platforms;
    }

    public static IQueryable<GameDto> GetGamesDtos(IQueryable<Game> games)
    {
        var gameDtos = games
            .Select(g => new GameDto() { Id = g.Id })
            .AsQueryable();

        return gameDtos;
    }

    public static IQueryable<GenreDto> GetGenresDtos(IQueryable<Genre> genres)
    {
        var genreDtos = genres
            .Select(g => new GenreDto() { Id = g.Id })
            .AsQueryable();

        return genreDtos;
    }

    public static IQueryable<PlatformDto> GetPlatformsDtos(IQueryable<Platform> platforms)
    {
        var platformDtos = platforms
            .Select(g => new PlatformDto() { Id = g.Id })
            .AsQueryable();

        return platformDtos;
    }
}
