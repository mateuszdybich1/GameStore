using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.Domain.MongoEntities;

public class MongoOrder
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }

    public int OrderID { get; set; }

    public string CustomerID { get; set; }

    public int EmployeeID { get; set; }

    public string OrderDate { get; set; }

    public string? RequiredDate { get; set; }

    public string? ShippedDate { get; set; }

    public int? ShipVia { get; set; }

    public double? Freight { get; set; }

    public string? ShipName { get; set; }

    public dynamic? ShipAddress { get; set; }

    public dynamic? ShipCity { get; set; }

    public string? ShipRegion { get; set; }

    public dynamic? ShipPostalCode { get; set; }

    public dynamic? ShipCountry { get; set; }
}
