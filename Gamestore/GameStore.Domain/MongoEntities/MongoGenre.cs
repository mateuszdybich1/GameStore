using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.Domain.MongoEntities;
public class MongoGenre
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }

    public int CategoryID { get; set; }

    public string CategoryName { get; set; }

    public string Description { get; set; }

    public string Picture { get; set; }
}
