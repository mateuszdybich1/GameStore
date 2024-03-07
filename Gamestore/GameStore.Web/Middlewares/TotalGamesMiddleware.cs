using GameStore.Domain.IRepositories;

namespace GameStore.Web.Middlewares;

public class TotalGamesMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context, Func<RepositoryTypes, IGameRepository> gameRepositoryFactory)
    {
        IGameRepository sqlGameRepository = gameRepositoryFactory(RepositoryTypes.Sql);
        IGameRepository mongoGameRepository = gameRepositoryFactory(RepositoryTypes.Mongo);
        var games = await sqlGameRepository.GetAllGames();
        var mongoGames = await mongoGameRepository.GetAllGames();
        mongoGames = mongoGames.Where(x => !games.Any(y => y.Key == x.Key));

        var totalGames = games.Count() + mongoGames.Count();
        context.Response.OnStarting(() =>
        {
            context.Response.Headers.Append("x-total-numbers-of-games", totalGames.ToString());
            return Task.CompletedTask;
        });

        await _next(context);
    }
}
