using GameStore.Domain.Entities;
using GameStore.Domain.ISearchCriterias;
using GameStore.Domain.MongoEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GameStore.Infrastructure.MongoRepositories;

public class MongoPublisherSearchCriteria : IPublisherSearchCriteria
{
    private readonly IMongoCollection<MongoPublisher> _publisherCollection;
    private readonly IMongoCollection<MongoGame> _gameCollection;

    public MongoPublisherSearchCriteria(IMongoClient client, IOptions<MongoDbSettings> options)
    {
        var database = client.GetDatabase(options.Value.DatabaseName);
        _publisherCollection = database.GetCollection<MongoPublisher>("suppliers");
        _gameCollection = database.GetCollection<MongoGame>("products");
    }

    public async Task<Publisher> GetPublisherByCompanyName(string companyName)
    {
        if (_publisherCollection != null && _publisherCollection.Database.Client.Cluster.Description.State == MongoDB.Driver.Core.Clusters.ClusterState.Connected)
        {
            if (int.TryParse(companyName, out var result))
            {
                MongoPublisher mongoPublisher = await _publisherCollection.Find(x => (x.CompanyName as int?) == result).SingleOrDefaultAsync();
                return mongoPublisher != null ? new(mongoPublisher) : null;
            }
            else
            {
                MongoPublisher mongoPublisher = await _publisherCollection.Find(x => (x.CompanyName as string) == companyName).SingleOrDefaultAsync();
                return mongoPublisher != null ? new(mongoPublisher) : null;
            }
        }
        else
        {
            return null;
        }
    }

    public async Task<Publisher> GetPublisherByGameKey(string gameKey)
    {
        if (_publisherCollection != null && _publisherCollection.Database.Client.Cluster.Description.State == MongoDB.Driver.Core.Clusters.ClusterState.Connected)
        {
            MongoGame mongoGame = await _gameCollection.Find(x => x.ProductKey == gameKey).SingleOrDefaultAsync();

            if (mongoGame != null && mongoGame.SupplierID is int)
            {
                MongoPublisher mongoPublisher = _publisherCollection
                .AsQueryable()
                .Where(x => x.SupplierID == (mongoGame.SupplierID as int?))
                .FirstOrDefault();
                return mongoPublisher != null ? new(mongoPublisher) : null;
            }

            return null;
        }
        else
        {
            return null;
        }
    }
}
