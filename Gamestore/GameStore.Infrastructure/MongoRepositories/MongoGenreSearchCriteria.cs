using GameStore.Domain.Entities;
using GameStore.Domain.ISearchCriterias;
using GameStore.Domain.MongoEntities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GameStore.Infrastructure.MongoRepositories;

public class MongoGenreSearchCriteria : IGenresSearchCriteria
{
    private readonly IMongoCollection<MongoGenre> _genreCollection;
    private readonly IMongoCollection<MongoGame> _gameCollection;
    private readonly IMongoCollection<MongoGameGenres> _gameGenresCollection;

    public MongoGenreSearchCriteria(IMongoClient client, IOptions<MongoDbSettings> options)
    {
        var database = client.GetDatabase(options.Value.DatabaseName);
        _genreCollection = database.GetCollection<MongoGenre>("categories");
        _gameCollection = database.GetCollection<MongoGame>("products");
        _gameGenresCollection = database.GetCollection<MongoGameGenres>("product-categories");
    }

    public async Task<IEnumerable<Genre>> GetByGameKey(string gameKey)
    {
        if (_genreCollection != null && _genreCollection.Database.Client.Cluster.Description.State == MongoDB.Driver.Core.Clusters.ClusterState.Connected)
        {
            MongoGame mongoGame = await _gameCollection.Find(x => x.ProductKey == gameKey).SingleOrDefaultAsync();

            if (mongoGame == null)
            {
                return Enumerable.Empty<Genre>();
            }

            var mongoGameGenres = await _gameGenresCollection.Find(x => x.ProductID == mongoGame.ProductID).ToListAsync();

            var genres = new List<Genre>();
            foreach (var mongoGameGenre in mongoGameGenres)
            {
                if (mongoGameGenre.CategoryID is int)
                {
                    var tempGenre = await _genreCollection.Find(x => x.CategoryID == (mongoGameGenre.CategoryID as int?)).FirstOrDefaultAsync();

                    if (tempGenre != null)
                    {
                        genres.Add(new(tempGenre));
                    }
                }
            }

            return genres;
        }
        else
        {
            return [];
        }
    }

    public async Task<Genre> GetByGenreName(string name)
    {
        if (_genreCollection != null && _genreCollection.Database.Client.Cluster.Description.State == MongoDB.Driver.Core.Clusters.ClusterState.Connected)
        {
            var mongoGenre = await _genreCollection.Find(x => x.CategoryName == name).FirstOrDefaultAsync();

            return new(mongoGenre);
        }
        else
        {
            return null;
        }
    }

    public Task<IEnumerable<Genre>> GetByParentId(Guid parentId)
    {
        throw new NotImplementedException();
    }

    public Task<Genre> GetWithParent(Guid genreId)
    {
        throw new NotImplementedException();
    }
}
