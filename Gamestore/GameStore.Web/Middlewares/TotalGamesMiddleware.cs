using GameStore.Domain.IRepositories;

namespace GameStore.Web.Middlewares;

public class TotalGamesMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context, Func<RepositoryTypes, IGameRepository> gameRepositoryFactory)
    {
        var sqlGameRepository = gameRepositoryFactory(RepositoryTypes.Sql);
        var mongoGameRepository = gameRepositoryFactory(RepositoryTypes.Mongo);

        var tasks = new List<Task<int>>
        {
        sqlGameRepository.GetAllGamesCount(),
        mongoGameRepository.GetAllGamesCount(),
        };

        await Task.WhenAll(tasks);

        var games = tasks[0].Result;
        var mongoGames = tasks[1].Result;

        var totalGames = games + mongoGames;
        context.Response.OnStarting(() =>
        {
            context.Response.Headers.Append("x-total-numbers-of-games", totalGames.ToString());
            return Task.CompletedTask;
        });

        await _next(context);
    }
}
