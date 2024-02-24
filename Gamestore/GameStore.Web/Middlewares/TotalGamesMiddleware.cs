using GameStore.Domain.IRepositories;

namespace GameStore.Web.Middlewares;

public class TotalGamesMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context, IGameRepository gameRepository)
    {
        int totalGames = await gameRepository.GetAllGamesCount();
        context.Response.OnStarting(() =>
        {
            context.Response.Headers.Append("x-total-numbers-of-games", totalGames.ToString());
            return Task.CompletedTask;
        });

        await _next(context);
    }
}
