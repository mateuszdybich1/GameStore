using GameStore.Domain;
using GameStore.Domain.Entities;
using GameStore.Domain.IRepositories;
using GameStore.Domain.MongoEntities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GameStore.Infrastructure.MongoRepositories;

public class MongoPublisherRepository : IPublisherRepository
{
    private readonly IMongoCollection<MongoPublisher> _publisherCollection;

    public MongoPublisherRepository(IMongoClient client, IOptions<MongoDbSettings> options)
    {
        var database = client.GetDatabase(options.Value.DatabaseName);
        _publisherCollection = database.GetCollection<MongoPublisher>("suppliers");
    }

    public Task Add(Publisher entity)
    {
        throw new NotImplementedException();
    }

    public async Task Delete(Publisher entity)
    {
        var filter = Builders<MongoPublisher>.Filter.Eq("_id", IDConverter.AsObjectId(entity.Id));
        await _publisherCollection.DeleteOneAsync(filter);
    }

    public async Task<Publisher> Get(Guid id)
    {
        MongoPublisher mongoPublisher = await _publisherCollection.Find(x => x.Id == IDConverter.AsObjectId(id)).SingleOrDefaultAsync();
        return mongoPublisher != null ? new(mongoPublisher) : null;
    }

    public async Task<IEnumerable<Publisher>> GetAllPublishers()
    {
        var suppliers = await _publisherCollection.Find(_ => true).ToListAsync();

        return suppliers.Where(x => !string.IsNullOrEmpty(x.CompanyName)).Select(x => new Publisher(x));
    }

    public async Task Update(Publisher entity)
    {
        var filter = Builders<MongoPublisher>.Filter.Eq("_id", IDConverter.AsObjectId(entity.Id));

        char[] separators = [';', ' '];
        List<string> parts = [.. entity.Description.Split(';', StringSplitOptions.RemoveEmptyEntries)];
        Dictionary<string, string> memebers = [];
        foreach (var part in parts)
        {
            string[] property = part.Split(':', StringSplitOptions.RemoveEmptyEntries);
            if (property.Length == 2)
            {
                memebers.Add(property[0].Trim(), property[1].Trim());
            }
        }

        var updates = new List<UpdateDefinition<MongoPublisher>>
        {
            Builders<MongoPublisher>.Update.Set(x => x.CompanyName, Convert.ToString(entity.CompanyName)),
            Builders<MongoPublisher>.Update.Set(x => x.HomePage, Convert.ToString(entity.HomePage)),
        };

        UpdateDefinition<MongoPublisher> update = Builders<MongoPublisher>.Update
            .Set(x => x.CompanyName, Convert.ToString(entity.CompanyName))
            .Set(x => x.HomePage, Convert.ToString(entity.HomePage));

        foreach (var part in memebers)
        {
            switch (part.Key)
            {
                case "ContactName":
                    updates.Add(Builders<MongoPublisher>.Update.Set(x => x.ContactName, Convert.ToString(part.Value)));
                    update.Set(x => x.ContactName, Convert.ToString(part.Value));
                    break;
                case "ContactTitle":
                    updates.Add(Builders<MongoPublisher>.Update.Set(x => x.ContactTitle, Convert.ToString(part.Value)));
                    update.Set(x => x.ContactTitle, Convert.ToString(part.Value));
                    break;
                case "Address":
                    updates.Add(Builders<MongoPublisher>.Update.Set(x => x.Address, Convert.ToString(part.Value)));
                    update.Set(x => x.Address, Convert.ToString(part.Value));
                    break;
                case "City":
                    updates.Add(Builders<MongoPublisher>.Update.Set(x => x.City, Convert.ToString(part.Value)));
                    update.Set(x => x.City, Convert.ToString(part.Value));
                    break;
                case "PostalCode":
                    updates.Add(Builders<MongoPublisher>.Update.Set(x => x.PostalCode, Convert.ToString(part.Value)));
                    update.Set(x => x.PostalCode, Convert.ToString(part.Value));
                    break;
                case "Country":
                    updates.Add(Builders<MongoPublisher>.Update.Set(x => x.Country, Convert.ToString(part.Value)));
                    update.Set(x => x.Country, Convert.ToString(part.Value));
                    break;
                case "Phone":
                    updates.Add(Builders<MongoPublisher>.Update.Set(x => x.Phone, Convert.ToString(part.Value)));
                    update.Set(x => x.Phone, Convert.ToString(part.Value));
                    break;
                case "Fax":
                    updates.Add(Builders<MongoPublisher>.Update.Set(x => x.Fax, Convert.ToString(part.Value)));
                    update.Set(x => x.Fax, Convert.ToString(part.Value));
                    break;
                default:
                    break;
            }
        }

        var finalUpd = Builders<MongoPublisher>.Update.Combine(updates);

        await _publisherCollection.UpdateOneAsync(filter, finalUpd);
    }
}
