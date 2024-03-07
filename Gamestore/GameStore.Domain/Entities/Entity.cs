using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.Domain.Entities;
public class Entity
{
    public Entity()
    {
    }

    public Entity(Guid id)
    {
        Id = id;
        CreationDate = DateTime.Now;
        ModificationDate = null;
    }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public Guid Id { get; private set; }

    public DateTime CreationDate { get; private set; }

    public DateTime? ModificationDate { get; set; }
}
