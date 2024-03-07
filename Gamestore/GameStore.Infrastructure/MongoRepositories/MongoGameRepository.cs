using GameStore.Domain;
using GameStore.Domain.Entities;
using GameStore.Domain.IRepositories;
using GameStore.Domain.MongoEntities;
using GameStore.Infrastructure.Repositories;
using Microsoft.Extensions.Options;
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
        var filter = Builders<MongoGame>.Filter.Eq("_id", IDConverter.AsObjectId(entity.Id));
        await _gameCollection.DeleteOneAsync(filter);
    }

    public async Task<Game> Get(Guid id)
    {
        MongoGame mongoGame = await _gameCollection.Find(x => x.Id == IDConverter.AsObjectId(id)).SingleOrDefaultAsync();
        return mongoGame != null ? new(mongoGame) : null;
    }

    public async Task<IEnumerable<Game>> GetAllGames()
    {
        var products = await _gameCollection.Find(_ => true).ToListAsync();

        return products.Select(x => new Game(x));
    }

    public async Task<IEnumerable<Game>> GetAllGames(List<Guid>? genreIds, List<Guid>? platformIds, List<Guid>? publisherIds, string? name, PublishDateFilteringMode? publishDate, GameSortingMode? sortMode, uint page, NumberOfGamesOnPageFilteringMode numberOfGamesOnPage, int minPrice, int maxPrice)
    {
        List<MongoGame> filteredMongoGames = null;

        if (name != null)
        {
            filteredMongoGames = await _gameCollection.Find(x => (x.ProductName as string).Contains(name)).ToListAsync();

            if (publisherIds != null && publisherIds.Count > 0)
            {
                var existingGames = new List<MongoGame>();
                foreach (var publisherId in publisherIds)
                {
                    var tempPublisher = await _publisherCollection.Find(x => x.Id == publisherId.AsObjectId()).FirstOrDefaultAsync();

                    if (tempPublisher != null)
                    {
                        existingGames.AddRange(filteredMongoGames.Where(x => x.SupplierID == tempPublisher.SupplierID));
                    }
                }

                filteredMongoGames = existingGames;
            }
        }
        else
        {
            if (publisherIds != null && publisherIds.Count > 0)
            {
                filteredMongoGames = [];
                foreach (var publisherId in publisherIds)
                {
                    var tempPublisher = await _publisherCollection.Find(x => x.Id == publisherId.AsObjectId()).FirstOrDefaultAsync();

                    if (tempPublisher != null)
                    {
                        var mongoGames = await _gameCollection.Find(x => (x.SupplierID as int?) == tempPublisher.SupplierID).ToListAsync();
                        filteredMongoGames.AddRange(mongoGames);
                    }
                }
            }
        }

        if (filteredMongoGames == null)
        {
            if (genreIds != null && genreIds.Count > 0)
            {
                filteredMongoGames = [];
                var filteredGamesFromGenres = new List<MongoGame>();
                foreach (var genreId in genreIds)
                {
                    var tempGenre = await _genreCollection.Find(x => x.Id == genreId.AsObjectId()).FirstOrDefaultAsync();

                    if (tempGenre != null)
                    {
                        var mongoGamesGenres = await _gameGenresCollection.Find(x => (x.CategoryID as int?) == tempGenre.CategoryID).ToListAsync();

                        foreach (var mongoGameGenre in mongoGamesGenres)
                        {
                            var filteredGame = await _gameCollection.Find(x => x.ProductID == mongoGameGenre.ProductID).FirstOrDefaultAsync();
                            if (filteredGame != null)
                            {
                                filteredGamesFromGenres.Add(filteredGame);
                            }
                        }
                    }
                }

                filteredMongoGames = filteredGamesFromGenres;
            }

            filteredMongoGames = filteredMongoGames == null ? await _gameCollection.Find(x => x.UnitPrice >= minPrice && x.UnitPrice <= maxPrice).ToListAsync() : filteredMongoGames.Where(x => x.UnitPrice >= minPrice && x.UnitPrice <= maxPrice).ToList();
        }
        else
        {
            filteredMongoGames = filteredMongoGames.Where(x => x.UnitPrice >= minPrice && x.UnitPrice <= maxPrice).ToList();
            if (genreIds != null && genreIds.Count > 0)
            {
                var filteredGamesFromGenres = new List<MongoGame>();
                foreach (var genreId in genreIds)
                {
                    var tempGenre = await _genreCollection.Find(x => x.Id == genreId.AsObjectId()).FirstOrDefaultAsync();

                    if (tempGenre != null)
                    {
                        var mongoGamesGenres = await _gameGenresCollection.Find(x => (x.CategoryID as int?) == tempGenre.CategoryID).ToListAsync();

                        foreach (var mongoGameGenre in mongoGamesGenres)
                        {
                            var tempGame = filteredMongoGames.FirstOrDefault(x => x.ProductID == mongoGameGenre.ProductID);
                            if (tempGame != null)
                            {
                                filteredGamesFromGenres.Add(tempGame);
                            }
                        }
                    }
                }

                filteredMongoGames = filteredGamesFromGenres;
            }
        }

        return filteredMongoGames.Select(x => new Game(x));
    }

    public async Task<int> GetAllGamesCount()
    {
        return (int)await _gameCollection.CountDocumentsAsync(FilterDefinition<MongoGame>.Empty);
    }

    public async Task<Game> GetGameWithRelations(Guid gameId)
    {
        var mongoGame = await _gameCollection.Find(x => x.Id == IDConverter.AsObjectId(gameId)).SingleOrDefaultAsync();

        if (mongoGame != null)
        {
            Game game = new(mongoGame);

            var mongoGameGenres = await _gameGenresCollection.Find(x => x.ProductID == mongoGame.ProductID).ToListAsync();

            var genres = new List<Genre>();

            foreach (var mongoGameGenre in mongoGameGenres)
            {
                if (mongoGameGenre.CategoryID is int)
                {
                    var localGenre = await _genreCollection.Find(x => x.CategoryID == (mongoGameGenre.CategoryID as int?)).SingleOrDefaultAsync();

                    if (localGenre != null)
                    {
                        genres.Add(new Genre(localGenre));
                    }
                }
            }

            game.Genres = genres;
            return game;
        }

        return null;
    }

    public Task<Game> GetGameWithRelations(string gameKey)
    {
        throw new NotImplementedException();
    }

    public async Task<int> GetNumberOfPages(NumberOfGamesOnPageFilteringMode numberOfGamesOnPage)
    {
        int gamesCount = (int)await _gameCollection.CountDocumentsAsync(FilterDefinition<MongoGame>.Empty);
        int numberOfPages = gamesCount / (int)numberOfGamesOnPage;
        if (gamesCount % (int)numberOfGamesOnPage > 0)
        {
            numberOfPages += 1;
        }

        return numberOfPages;
    }

    public async Task Update(Game entity)
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
}
