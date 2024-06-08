using GameStore.Application.IServices;
using GameStore.Application.IUserServices;
using GameStore.Application.Services;
using GameStore.Application.UserServices;
using GameStore.Domain;
using GameStore.Domain.IRepositories;
using GameStore.Domain.ISearchCriterias;
using GameStore.Domain.IUserRepositories;
using GameStore.Infrastructure;
using GameStore.Infrastructure.MongoRepositories;
using GameStore.Infrastructure.Repositories;
using GameStore.Infrastructure.SearchCriteria;
using GameStore.Users;
using GameStore.Users.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GameStore.Web;

internal static class Bootstrapper
{
    internal static void RegisterServices(this IServiceCollection services)
    {
        services.AddSingleton<IMongoClient, MongoClient>(serivceProvider =>
        {
            var settings = serivceProvider.GetRequiredService<IOptions<MongoDbSettings>>()?.Value;
            var mongoClientSettings = settings != null
            ? MongoClientSettings.FromConnectionString(settings.ConnectionString)
            : new MongoClientSettings();

            mongoClientSettings.ConnectTimeout = TimeSpan.FromSeconds(1);
            mongoClientSettings.ServerSelectionTimeout = TimeSpan.FromSeconds(1);
            return settings != null ? new MongoClient(mongoClientSettings) : new MongoClient();
        });

        services.AddScoped<IChangeLogService, ChangeLogService>();

        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IGameRepository, MongoGameRepository>();
        services.AddScoped<Func<RepositoryTypes, IGameRepository>>(provider => key =>
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
        services.AddScoped<Func<RepositoryTypes, IGenreRepository>>(provider => key =>
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
        services.AddScoped<Func<RepositoryTypes, IPublisherRepository>>(provider => key =>
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
        services.AddScoped<IOrderRepository, MongoOrderRepository>();
        services.AddScoped<Func<RepositoryTypes, IOrderRepository>>(provider => key =>
        {
            var allservices = provider.GetServices<IOrderRepository>();
            return key switch
            {
                RepositoryTypes.Sql => allservices.First(o => o.GetType() == typeof(OrderRepository)),
                RepositoryTypes.Mongo => allservices.First(o => o.GetType() == typeof(MongoOrderRepository)),
                _ => throw new ArgumentException($"Unknown key: {key}"),
            };
        });

        services.AddScoped<IOrderGameRepository, OrderGameRepository>();
        services.AddScoped<IOrderGameRepository, MongoOrderGameRepository>();
        services.AddScoped<Func<RepositoryTypes, IOrderGameRepository>>(provider => key =>
        {
            var allservices = provider.GetServices<IOrderGameRepository>();
            return key switch
            {
                RepositoryTypes.Sql => allservices.First(o => o.GetType() == typeof(OrderGameRepository)),
                RepositoryTypes.Mongo => allservices.First(o => o.GetType() == typeof(MongoOrderGameRepository)),
                _ => throw new ArgumentException($"Unknown key: {key}"),
            };
        });

        services.AddScoped<ICommentRepository, CommentRepository>();

        services.AddScoped<IGamesSearchCriteria, GamesSearchCirteria>();
        services.AddScoped<IGamesSearchCriteria, MongoGameSearchCriteria>();
        services.AddScoped<Func<RepositoryTypes, IGamesSearchCriteria>>(provider => key =>
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
        services.AddScoped<Func<RepositoryTypes, IGenresSearchCriteria>>(provider => key =>
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
        services.AddScoped<Func<RepositoryTypes, IPublisherSearchCriteria>>(provider => key =>
        {
            var allservices = provider.GetServices<IPublisherSearchCriteria>();
            return key switch
            {
                RepositoryTypes.Sql => allservices.First(o => o.GetType() == typeof(PublisherSearchCriteria)),
                RepositoryTypes.Mongo => allservices.First(o => o.GetType() == typeof(MongoPublisherSearchCriteria)),
                _ => throw new ArgumentException($"Unknown key: {key}"),
            };
        });
        services.AddScoped<IPermissionsRepository, PermissionsRepository>();
        services.AddScoped<IRoleRepository, RolesRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IUserContext, HttpUserContext>();
        services.AddScoped<IAuthRepository, AuthRepository>();

        services.AddScoped<IFakeDataGenerator, FakeDataGenerator>();

        services.AddScoped<IUserCheckService, UserCheckService>();
        services.AddScoped<IGameService, GameService>();
        services.AddScoped<IGenreService, GenreService>();
        services.AddScoped<IPlatformService, PlatformService>();
        services.AddScoped<IPublisherService, PublisherService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IPermissionsService, PermissionsService>();
        services.AddScoped<IRolesService, RolesService>();
        services.AddScoped<IUserService, UserService>();
    }
}
