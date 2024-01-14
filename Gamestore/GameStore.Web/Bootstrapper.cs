using GameStore.Application.IServices;
using GameStore.Application.Services;
using GameStore.Domain.IRepositories;
using GameStore.Domain.ISearchCriterias;
using GameStore.Infrastructure.Repositories;
using GameStore.Infrastructure.SearchCriteria;

namespace GameStore.Web;

internal static class Bootstrapper
{
    internal static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IGenreRepository, GenreRepository>();
        services.AddScoped<IPlatformRepository, PlatformRepository>();
        services.AddScoped<IPublisherRepository, PublisherRepository>();

        services.AddScoped<IGamesSearchCriteria, GamesSearchCirteria>();
        services.AddScoped<IGenresSearchCriteria, GenresSearchCriteria>();
        services.AddScoped<IPlatformsSearchCriteria, PlatformsSearchCriteria>();
        services.AddScoped<IPublisherSearchCriteria, PublisherSearchCriteria>();

        services.AddScoped<IGameService, GameService>();
        services.AddScoped<IGenreService, GenreService>();
        services.AddScoped<IPlatformService, PlatformService>();
        services.AddScoped<IPublisherService, PublisherService>();
    }
}
