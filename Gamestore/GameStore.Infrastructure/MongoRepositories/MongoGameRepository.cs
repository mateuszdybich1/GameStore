using GameStore.Domain;
using GameStore.Domain.Entities;
using GameStore.Domain.IRepositories;
using GameStore.Domain.MongoEntities;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GameStore.Infrastructure.MongoRepositories;

public class MongoGameRepository : IGameRepository
{
    private readonly IMongoCollection<MongoGame> _gameCollection;
    private readonly IMongoCollection<MongoPublisher> _publisherCollection;
    private readonly IMongoCollection<MongoGenre> _genreCollection;
    private readonly IMongoCollection<MongoGameGenres> _gameGenresCollection;

    public MongoGameRepository(IMongoClient client, IOptions<MongoDbSettings> options)
    {
        var database = client.GetDatabase(options.Value.DatabaseName);
        _gameCollection = database.GetCollection<MongoGame>("products");
        _publisherCollection = database.GetCollection<MongoPublisher>("suppliers");
        _genreCollection = database.GetCollection<MongoGenre>("categories");
        _gameGenresCollection = database.GetCollection<MongoGameGenres>("product-categories");
    }

    public Task Add(Game entity)
    {
        throw new NotImplementedException();
    }

    public async Task Delete(Game entity)
    {
        try
        {
            var filter = Builders<MongoGame>.Filter.Eq("_id", IDConverter.AsObjectId(entity.Id));
            await _gameCollection.DeleteOneAsync(filter);
        }
        catch
        {
            return;
        }
    }

    public async Task<Game> Get(Guid id)
    {
        try
        {
            MongoGame mongoGame = await _gameCollection.Find(x => x.Id == IDConverter.AsObjectId(id)).SingleOrDefaultAsync();
            return mongoGame != null ? new(mongoGame) : null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<IEnumerable<Game>> GetAllGames()
    {
        try
        {
            var products = await _gameCollection.Find(_ => true).ToListAsync();

            return products.Select(x => new Game(x));
        }
        catch
        {
            return [];
        }
    }

    public async Task<IEnumerable<Game>> GetAllGames(List<Guid>? genreIds, List<Guid>? platformIds, List<Guid>? publisherIds, string? name, PublishDateFilteringMode? publishDate, GameSortingMode? sortMode, uint page, NumberOfGamesOnPageFilteringMode numberOfGamesOnPage, int minPrice, int maxPrice)
    {
        try
        {
            var filters = new List<FilterDefinition<MongoGame>>() { Builders<MongoGame>.Filter.Gte(game => game.UnitPrice, minPrice), Builders<MongoGame>.Filter.Lte(game => game.UnitPrice, maxPrice) };

            if (name != null)
            {
                filters.Add(Builders<MongoGame>.Filter.Regex(game => game.ProductName as string, new BsonRegularExpression(name)));
            }

            if (publisherIds != null && publisherIds.Count > 0)
            {
                foreach (var publisherId in publisherIds)
                {
                    var tempPublisher = await _publisherCollection.Find(x => x.Id == publisherId.AsObjectId()).FirstOrDefaultAsync();

                    if (tempPublisher != null)
                    {
                        filters.Add(Builders<MongoGame>.Filter.Eq(doc => doc.SupplierID, tempPublisher));
                    }
                }
            }

            if (genreIds != null && genreIds.Count > 0)
            {
                foreach (var genreId in genreIds)
                {
                    var tempGenre = await _genreCollection.Find(x => x.Id == genreId.AsObjectId()).FirstOrDefaultAsync();

                    if (tempGenre != null)
                    {
                        var mongoGamesGenres = await _gameGenresCollection.Find(x => (x.CategoryID as int?) == tempGenre.CategoryID).ToListAsync();

                        foreach (var mongoGameGenre in mongoGamesGenres)
                        {
                            filters.Add(Builders<MongoGame>.Filter.Eq(doc => doc.ProductID, mongoGameGenre.ProductID));
                        }
                    }
                }
            }

            SortDefinition<MongoGame> sortDefinition = null;
            if (sortMode != null)
            {
                switch (sortMode)
                {
                    case GameSortingMode.MostPopular:
                        sortDefinition = Builders<MongoGame>.Sort.Descending(x => x.NumberOfViews);
                        break;
                    case GameSortingMode.MostCommented:
                        break;
                    case GameSortingMode.PriceASC:
                        sortDefinition = Builders<MongoGame>.Sort.Ascending(x => x.UnitPrice);
                        break;
                    case GameSortingMode.PriceDESC:
                        sortDefinition = Builders<MongoGame>.Sort.Descending(x => x.UnitPrice);
                        break;
                    case GameSortingMode.New:
                        break;
                    default:
                        break;
                }
            }

            List<MongoGame> filteredMongoGames;
            var filter = Builders<MongoGame>.Filter.And(filters);

            filteredMongoGames = sortDefinition != null
                ? numberOfGamesOnPage != NumberOfGamesOnPageFilteringMode.All
                    ? await _gameCollection.Find(filter).Sort(sortDefinition).Skip(((int)page - 1) * (int)numberOfGamesOnPage).Limit((int)numberOfGamesOnPage).ToListAsync()
                    : await _gameCollection.Find(filter).Sort(sortDefinition).ToListAsync()
                : numberOfGamesOnPage != NumberOfGamesOnPageFilteringMode.All
                    ? await _gameCollection.Find(filter).Skip(((int)page - 1) * (int)numberOfGamesOnPage).Limit((int)numberOfGamesOnPage).ToListAsync()
                    : await _gameCollection.Find(filter).ToListAsync();

            return filteredMongoGames.Select(x => new Game(x));
        }
        catch
        {
            return [];
        }
    }

    public async Task<int> GetAllGamesCount()
    {
        try
        {
            return (int)await _gameCollection.CountDocumentsAsync(FilterDefinition<MongoGame>.Empty);
        }
        catch
        {
            return 0;
        }
    }

    public async Task<Game> GetGameWithRelations(Guid gameId)
    {
        try
        {
            var mongoGame = await _gameCollection.Find(x => x.Id == IDConverter.AsObjectId(gameId)).SingleOrDefaultAsync();

            return await GetWithRelationsFromMongoGame(mongoGame);
        }
        catch
        {
            return null;
        }
    }

    public async Task<Game> GetGameWithRelations(string gameKey)
    {
        try
        {
            var mongoGame = await _gameCollection.Find(x => x.ProductKey == gameKey).FirstOrDefaultAsync();
            return await GetWithRelationsFromMongoGame(mongoGame);
        }
        catch
        {
            return null;
        }
    }

    public async Task<int> GetNumberOfPages(NumberOfGamesOnPageFilteringMode numberOfGamesOnPage)
    {
        try
        {
            int gamesCount = (int)await _gameCollection.CountDocumentsAsync(FilterDefinition<MongoGame>.Empty);
            int numberOfPages = gamesCount / (int)numberOfGamesOnPage;
            if (gamesCount % (int)numberOfGamesOnPage > 0)
            {
                numberOfPages += 1;
            }

            return numberOfPages;
        }
        catch
        {
            return 0;
        }
    }

    public async Task Update(Game entity)
    {
        try
        {
            var filter = Builders<MongoGame>.Filter.Eq("_id", IDConverter.AsObjectId(entity.Id));
            var mongoGame = await _gameCollection.Find(x => x.Id == IDConverter.AsObjectId(entity.Id)).SingleOrDefaultAsync();

            if (entity.Genres != null)
            {
                await _gameGenresCollection.DeleteManyAsync(x => x.ProductID == mongoGame.ProductID);
                List<MongoGameGenres> mongoGameGenres = [];
                foreach (var genre in entity.Genres)
                {
                    var tempGenre = _genreCollection.Find(x => x.CategoryName == genre.Name).SingleOrDefault();
                    if (tempGenre == null)
                    {
                        mongoGameGenres.Add(new(mongoGame.ProductID, genre.Id.ToString()));
                    }
                    else
                    {
                        mongoGameGenres.Add(new(mongoGame.ProductID, tempGenre.CategoryID));
                    }
                }

                await _gameGenresCollection.InsertManyAsync(mongoGameGenres);
            }

            UpdateDefinition<MongoGame> update = Builders<MongoGame>.Update
                .Set(x => x.ProductName, entity.Name)
                .Set(x => x.QuantityPerUnit, entity.Description)
                .Set(x => x.UnitPrice, entity.Price)
                .Set(x => x.Discontinued, entity.Discount)
                .Set(x => x.NumberOfViews, (int)entity.NumberOfViews);

            await _gameCollection.UpdateOneAsync(filter, update);
            if (entity.PublisherId != Guid.Empty)
            {
                MongoPublisher mongoPublisher = await _publisherCollection.Find(x => x.Id == IDConverter.AsObjectId(entity.PublisherId)).SingleOrDefaultAsync();
                UpdateDefinition<MongoGame> publisherUpdate = mongoPublisher == null
                    ? Builders<MongoGame>.Update.Set(x => x.SupplierID as string, entity.PublisherId.ToString())
                    : Builders<MongoGame>.Update.Set(x => x.SupplierID as int?, mongoPublisher.SupplierID);
                await _gameCollection.UpdateOneAsync(filter, publisherUpdate);
            }
        }
        catch
        {
            return;
        }
    }

    private async Task<Game> GetWithRelationsFromMongoGame(MongoGame mongoGame)
    {
        Game game = null;
        if (mongoGame != null)
        {
            game = new(mongoGame);
            if (mongoGame.SupplierID is int supplierId)
            {
                var mongoPublisher = await _publisherCollection.Find(x => x.SupplierID == supplierId).FirstOrDefaultAsync();
                if (mongoPublisher != null)
                {
                    var publisher = new Publisher(mongoPublisher);
                    game.Publisher = publisher;
                    game.PublisherId = publisher.Id;
                }
            }

            var mongoGameGenres = await _gameGenresCollection.Find(x => x.ProductID == mongoGame.ProductID).ToListAsync();

            var genres = new List<Genre>();

            foreach (var mongoGameGenre in mongoGameGenres)
            {
                if (mongoGameGenre.CategoryID is int categoryId)
                {
                    var localGenre = await _genreCollection.Find(x => x.CategoryID == categoryId).SingleOrDefaultAsync();

                    if (localGenre != null)
                    {
                        genres.Add(new Genre(localGenre));
                    }
                }
            }

            game.Genres = genres;
        }

        return game;
    }
}
