using BusinessLogic.Services.Interfaces;

namespace WebAPI.Middleware;

public class TotalGamesCountMiddleware(IServiceProvider serviceProvider) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
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
