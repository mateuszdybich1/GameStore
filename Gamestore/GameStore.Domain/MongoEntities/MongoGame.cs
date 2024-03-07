using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.Domain.MongoEntities;
public class MongoGame
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }

    public dynamic ProductName { get; set; }

    public int ProductID { get; set; }

    public string ProductKey { get; set; }

    public dynamic SupplierID { get; set; }

    public dynamic CategoryID { get; set; }

    public string QuantityPerUnit { get; set; }

    public double UnitPrice { get; set; }

    public int UnitsInStock { get; set; }

    public int UnitsOnOrder { get; set; }

    public int ReorderLevel { get; set; }

    public int Discontinued { get; set; }

    public int NumberOfViews { get; set; }
}
