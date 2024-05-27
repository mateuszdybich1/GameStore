using GameStore.Domain;
using GameStore.Domain.Entities;
using GameStore.Domain.ISearchCriterias;
using GameStore.Domain.MongoEntities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GameStore.Infrastructure.MongoRepositories;

public class MongoGameSearchCriteria : IGamesSearchCriteria
{
    private readonly IMongoCollection<MongoGenre> _genreCollection;
    private readonly IMongoCollection<MongoPublisher> _publisherCollection;
    private readonly IMongoCollection<MongoGame> _gameCollection;
    private readonly IMongoCollection<MongoGameGenres> _gameGenresCollection;

    public MongoGameSearchCriteria(IMongoClient client, IOptions<MongoDbSettings> options)
    {
        var database = client.GetDatabase(options.Value.DatabaseName);
        _gameCollection = database.GetCollection<MongoGame>("products");
        _publisherCollection = database.GetCollection<MongoPublisher>("suppliers");
        _genreCollection = database.GetCollection<MongoGenre>("categories");
        _gameGenresCollection = database.GetCollection<MongoGameGenres>("product-categories");
    }

    public async Task<IEnumerable<Game>> GetByGenreId(Guid genreId)
    {
        try
        {
            var mongoGenre = await _genreCollection.Find(x => x.Id == IDConverter.AsObjectId(genreId)).SingleOrDefaultAsync();

            if (mongoGenre != null)
            {
                var gameGenres = await _gameGenresCollection.Find(x => (x.CategoryID as int?) == mongoGenre.CategoryID).ToListAsync();

                var games = new List<Game>();
                foreach (var gameGenre in gameGenres)
                {
                    var localGame = await _gameCollection.Find(x => x.ProductID == gameGenre.ProductID).SingleOrDefaultAsync();

                    if (localGame != null)
                    {
                        games.Add(new(localGame));
                    }
                }

                return games;
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<Game> GetByKey(string key)
    {
        try
        {
            var game = await _gameCollection.Find(x => x.ProductKey == key).FirstOrDefaultAsync();
            return game != null ? new(game) : null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<object> GetByKeyWithRelations(string key)
    {
        try
        {
            var game = await _gameCollection.Find(x => x.ProductKey == key).FirstOrDefaultAsync();
            var genreGames = await _gameGenresCollection.Find(x => x.ProductID == game.ProductID).ToListAsync();
            var publisher = new Publisher(Guid.Empty, string.Empty, string.Empty, string.Empty);
            if (game.SupplierID is int)
            {
                var mongoPublisher = await _publisherCollection.Find(x => x.SupplierID == (game.SupplierID as int?)).FirstOrDefaultAsync();

                if (mongoPublisher != null)
                {
                    publisher = new(mongoPublisher);
                }
            }

            var genres = new List<Genre>();

            foreach (var gameGenre in genreGames)
            {
                if (gameGenre.CategoryID is int)
                {
                    var tempGenre = await _genreCollection.Find(x => x.CategoryID == (gameGenre.CategoryID as int?)).FirstOrDefaultAsync();
                    if (tempGenre != null)
                    {
                        genres.Add(new(tempGenre));
                    }
                }
            }

            return new
            {
                Game = new
                {
                    Id = IDConverter.AsGuid(game.Id),
                    Name = game.ProductName,
                    Key = game.ProductKey,
                    Description = game.QuantityPerUnit,
                    Price = game.UnitPrice,
                    UnitInStock = game.UnitsInStock,
                    Discount = game.Discontinued,
                },
                Genres = genres.Select(genre => new
                {
                    Id = genre.Id,
                    Name = genre.Name,
                }),
                Publisher = new
                {
                    Id = publisher.Id,
                    Name = publisher.CompanyName,
                    HomePage = publisher.HomePage,
                    Description = publisher.Description,
                },
            };
        }
        catch
        {
            return null;
        }
    }

    public Task<IEnumerable<Game>> GetByPlatformId(Guid platformId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Game>> GetByPublisherName(string companyName)
    {
        var mongoPublisher = await _publisherCollection.Find(x => (x.CompanyName as string) == companyName).FirstOrDefaultAsync();

        if (mongoPublisher != null)
        {
            var mongoGames = await _gameCollection.Find(x => (x.SupplierID as int?) == mongoPublisher.SupplierID).ToListAsync();

            return mongoGames.Select(x => new Game(x));
        }

        return null;
    }
}
