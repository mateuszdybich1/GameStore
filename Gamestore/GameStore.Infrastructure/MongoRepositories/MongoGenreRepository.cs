using GameStore.Domain;
using GameStore.Domain.Entities;
using GameStore.Domain.IRepositories;
using GameStore.Domain.MongoEntities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GameStore.Infrastructure.MongoRepositories;

public class MongoGenreRepository : IGenreRepository
{
    private readonly IMongoCollection<MongoGenre> _genreCollection;

    public MongoGenreRepository(IMongoClient client, IOptions<MongoDbSettings> options)
    {
        var database = client.GetDatabase(options.Value.DatabaseName);
        _genreCollection = database.GetCollection<MongoGenre>("categories");
    }

    public Task Add(Genre entity)
    {
        throw new NotImplementedException();
    }

    public async Task Delete(Genre entity)
    {
        var filter = Builders<MongoGenre>.Filter.Eq("_id", IDConverter.AsObjectId(entity.Id));
        await _genreCollection.DeleteOneAsync(filter);
    }

    public async Task<Genre> Get(Guid id)
    {
        MongoGenre mongoGenre = await _genreCollection.Find(x => x.Id == IDConverter.AsObjectId(id)).SingleOrDefaultAsync();
        return mongoGenre != null ? new(mongoGenre) : null;
    }

    public async Task<IEnumerable<Genre>> GetAllGenre()
    {
        var categories = await _genreCollection.Find(_ => true).ToListAsync();

        return categories.Select(x => new Genre(x));
    }

    public async Task Update(Genre entity)
    {
        var filter = Builders<MongoGenre>.Filter.Eq("_id", IDConverter.AsObjectId(entity.Id));

        UpdateDefinition<MongoGenre> update = Builders<MongoGenre>.Update
            .Set(x => x.CategoryName, entity.Name)
            .Set(x => x.Description, entity.Description)
            .Set(x => x.Picture, entity.Picture);

        await _genreCollection.UpdateOneAsync(filter, update);
    }
}
