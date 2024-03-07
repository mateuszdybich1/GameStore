using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.Domain.MongoEntities;

public class MongoPublisher
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }

    public dynamic CompanyName { get; set; }

    public int SupplierID { get; set; }

    public dynamic HomePage { get; set; }

    public dynamic ContactName { get; set; }

    public dynamic ContactTitle { get; set; }

    public dynamic Address { get; set; }

    public dynamic City { get; set; }

    public dynamic Region { get; set; }

    public dynamic PostalCode { get; set; }

    public dynamic Country { get; set; }

    public dynamic Phone { get; set; }

    public dynamic Fax { get; set; }
}
