using DataAccess.Data;
using DataAccess.Entities;

namespace WebAPI.Extensions;

public static class SeedDataExtension
{
    public static async Task SeedData(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GameStoreDbContext>();

        if (!dbContext.Platforms.Any())
        {
            await dbContext.Platforms.AddRangeAsync(
                new Platform { Type = "Mobile" },
                new Platform { Type = "Browser" },
                new Platform { Type = "Desktop" },
                new Platform { Type = "Console" });
        }

        if (!dbContext.Genres.Any())
        {
            var strategy = await dbContext.Genres.AddAsync(new Genre { Name = "Strategy" });
            await dbContext.Genres.AddRangeAsync(
                new Genre { Name = "RTS", ParentGenreId = strategy.Entity.Id },
                new Genre { Name = "TBS", ParentGenreId = strategy.Entity.Id });

            await dbContext.Genres.AddAsync(new Genre { Name = "RPG" });

            var sports = await dbContext.Genres.AddAsync(new Genre { Name = "Sports" });
            await dbContext.Genres.AddRangeAsync(
                new Genre { Name = "Races", ParentGenreId = sports.Entity.Id },
                new Genre { Name = "Rally", ParentGenreId = sports.Entity.Id },
                new Genre { Name = "Arcade", ParentGenreId = sports.Entity.Id },
                new Genre { Name = "Formula", ParentGenreId = sports.Entity.Id },
                new Genre { Name = "Off-road", ParentGenreId = sports.Entity.Id });

            var action = await dbContext.Genres.AddAsync(new Genre { Name = "Action" });
            await dbContext.Genres.AddRangeAsync(
                new Genre { Name = "FPS", ParentGenreId = action.Entity.Id },
                new Genre { Name = "TPS", ParentGenreId = action.Entity.Id });

            await dbContext.Genres.AddAsync(new Genre { Name = "Adventure" });

            await dbContext.Genres.AddAsync(new Genre { Name = "Puzzle & Skill" });
        }
    }
}
