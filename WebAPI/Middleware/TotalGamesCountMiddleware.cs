using BusinessLogic.Services.Interfaces;

namespace WebAPI.Middleware;

public class TotalGamesCountMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
{
    public async Task InvokeAsync(HttpContext context)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var gameService = scope.ServiceProvider.GetRequiredService<IGameService>();
            int gamesCount = await gameService.GetTotalGamesCountAsync();
            context.Response.Headers.Append("x-total-number-of-games", gamesCount.ToString());
        }

        await next(context);
    }
}
