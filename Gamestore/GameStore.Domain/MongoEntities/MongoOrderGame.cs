using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.Domain.MongoEntities;

public class MongoOrderGame
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }

    public int OrderID { get; set; }

    public int ProductID { get; set; }

    public dynamic UnitPrice { get; set; }

    public int Quantity { get; set; }

    public dynamic Discount { get; set; }
}
