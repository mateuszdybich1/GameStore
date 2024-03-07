using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.Domain.MongoEntities;

public class MongoGameGenres
{
    public MongoGameGenres(int productID, string categoryID)
    {
        ProductID = productID;
        CategoryID = categoryID;
    }

    public MongoGameGenres(int productID, int categoryId)
    {
        ProductID = productID;
        CategoryID = categoryId;
    }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }

    public int ProductID { get; set; }

    public dynamic CategoryID { get; set; }
}
