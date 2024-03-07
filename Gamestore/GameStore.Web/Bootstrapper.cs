using GameStore.Application.IServices;
using GameStore.Application.Services;
using GameStore.Domain.IRepositories;
using GameStore.Domain.ISearchCriterias;
using GameStore.Infrastructure.MongoRepositories;
using GameStore.Infrastructure.Repositories;
using GameStore.Infrastructure.SearchCriteria;
using MongoDB.Driver;

namespace GameStore.Web;

internal static class Bootstrapper
{
    internal static void RegisterServices(this IServiceCollection services)
    {
        services.AddSingleton<IMongoClient, MongoClient>();

        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IGameRepository, MongoGameRepository>();
        services.AddTransient<Func<RepositoryTypes, IGameRepository>>(provider => key =>
        {
            var allservices = provider.GetServices<IGameRepository>();
            return key switch
            {
                RepositoryTypes.Sql => allservices.First(o => o.GetType() == typeof(GameRepository)),
                RepositoryTypes.Mongo => allservices.First(o => o.GetType() == typeof(MongoGameRepository)),
                _ => throw new ArgumentException($"Unknown key: {key}"),
            };
        });

        services.AddScoped<IGenreRepository, GenreRepository>();
        services.AddScoped<IGenreRepository, MongoGenreRepository>();
        services.AddTransient<Func<RepositoryTypes, IGenreRepository>>(provider => key =>
        {
            var allservices = provider.GetServices<IGenreRepository>();
            return key switch
            {
                RepositoryTypes.Sql => allservices.First(o => o.GetType() == typeof(GenreRepository)),
                RepositoryTypes.Mongo => allservices.First(o => o.GetType() == typeof(MongoGenreRepository)),
                _ => throw new ArgumentException($"Unknown key: {key}"),
            };
        });

        services.AddScoped<IPlatformRepository, PlatformRepository>();

        services.AddScoped<IPublisherRepository, PublisherRepository>();
        services.AddScoped<IPublisherRepository, MongoPublisherRepository>();
        services.AddTransient<Func<RepositoryTypes, IPublisherRepository>>(provider => key =>
        {
            var allservices = provider.GetServices<IPublisherRepository>();
            return key switch
            {
                RepositoryTypes.Sql => allservices.First(o => o.GetType() == typeof(PublisherRepository)),
                RepositoryTypes.Mongo => allservices.First(o => o.GetType() == typeof(MongoPublisherRepository)),
                _ => throw new ArgumentException($"Unknown key: {key}"),
            };
        });

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderGameRepository, OrderGameRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();

        services.AddScoped<IGamesSearchCriteria, GamesSearchCirteria>();
        services.AddScoped<IGamesSearchCriteria, MongoGameSearchCriteria>();
        services.AddTransient<Func<RepositoryTypes, IGamesSearchCriteria>>(provider => key =>
        {
            var allservices = provider.GetServices<IGamesSearchCriteria>();
            return key switch
            {
                RepositoryTypes.Sql => allservices.First(o => o.GetType() == typeof(GamesSearchCirteria)),
                RepositoryTypes.Mongo => allservices.First(o => o.GetType() == typeof(MongoGameSearchCriteria)),
                _ => throw new ArgumentException($"Unknown key: {key}"),
            };
        });

        services.AddScoped<IGenresSearchCriteria, GenresSearchCriteria>();
        services.AddScoped<IGenresSearchCriteria, MongoGenreSearchCriteria>();
        services.AddTransient<Func<RepositoryTypes, IGenresSearchCriteria>>(provider => key =>
        {
            var allservices = provider.GetServices<IGenresSearchCriteria>();
            return key switch
            {
                RepositoryTypes.Sql => allservices.First(o => o.GetType() == typeof(GenresSearchCriteria)),
                RepositoryTypes.Mongo => allservices.First(o => o.GetType() == typeof(MongoGenreSearchCriteria)),
                _ => throw new ArgumentException($"Unknown key: {key}"),
            };
        });

        services.AddScoped<IPlatformsSearchCriteria, PlatformsSearchCriteria>();

        services.AddScoped<IPublisherSearchCriteria, PublisherSearchCriteria>();
        services.AddScoped<IPublisherSearchCriteria, MongoPublisherSearchCriteria>();
        services.AddTransient<Func<RepositoryTypes, IPublisherSearchCriteria>>(provider => key =>
        {
            var allservices = provider.GetServices<IPublisherSearchCriteria>();
            return key switch
            {
                RepositoryTypes.Sql => allservices.First(o => o.GetType() == typeof(PublisherSearchCriteria)),
                RepositoryTypes.Mongo => allservices.First(o => o.GetType() == typeof(MongoPublisherSearchCriteria)),
                _ => throw new ArgumentException($"Unknown key: {key}"),
            };
        });

        services.AddScoped<IGameService, GameService>();
        services.AddScoped<IGenreService, GenreService>();
        services.AddScoped<IPlatformService, PlatformService>();
        services.AddScoped<IPublisherService, PublisherService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ICommentService, CommentService>();
    }
}
