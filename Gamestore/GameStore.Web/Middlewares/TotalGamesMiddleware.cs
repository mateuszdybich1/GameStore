using GameStore.Infrastructure.IRepositories;

namespace GameStore.Web.Middlewares;

public class TotalGamesMiddleware
{
    private readonly RequestDelegate _next;

    public TotalGamesMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IGameRepository gameRepository)
    {
        int totalGames = gameRepository.GetAllGames().Count;
        context.Response.OnStarting(() =>
        {
            context.Response.Headers.Add("x-total-numbers-of-games", totalGames.ToString());
            return Task.CompletedTask;
        });

        await _next(context);
    }
}
