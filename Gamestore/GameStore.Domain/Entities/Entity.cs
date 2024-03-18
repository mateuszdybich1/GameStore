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

    public Entity(Guid id, DateTime creationDate)
    {
        Id = id;
        CreationDate = creationDate;
        ModificationDate = null;
    }

    public Entity(Guid id, DateTime creationDate, DateTime? modificationDate)
    {
        Id = id;
        CreationDate = creationDate;
        ModificationDate = modificationDate;
    }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public Guid Id { get; private set; }

    public DateTime CreationDate { get; private set; }

    public DateTime? ModificationDate { get; set; }
}
