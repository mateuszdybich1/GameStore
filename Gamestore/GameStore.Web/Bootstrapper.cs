﻿using GameStore.Application.IServices;
using GameStore.Application.Services;
using GameStore.Infrastructure.IRepositories;
using GameStore.Infrastructure.ISearchCriterias;
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

        services.AddScoped<IGamesSearchCriteria, GamesSearchCirteria>();
        services.AddScoped<IGenresSearchCriteria, GenresSearchCriteria>();
        services.AddScoped<IPlatformsSearchCriteria, PlatformsSearchCriteria>();

        services.AddScoped<IGameService, GameService>();
        services.AddScoped<IGenreService, GenreService>();
        services.AddScoped<IPlatformService, PlatformService>();
    }
}
