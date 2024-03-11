using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.Domain.MongoEntities;

public class MongoCustomer
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }

    public string CustomerID { get; set; }

    public string CustomerName { get; set; }

    public string CompanyName { get; set; }

    public string ContactName { get; set; }

    public string ContactTitle { get; set; }

    public dynamic Address { get; set; }

    public dynamic City { get; set; }

    public dynamic Region { get; set; }

    public dynamic PostalCode { get; set; }

    public dynamic Country { get; set; }

    public dynamic Phone { get; set; }

    public dynamic Fax { get; set; }
}
