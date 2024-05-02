using System.Collections;
using System.Reflection;
using GameStore.Domain;
using GameStore.Infrastructure.MongoRepositories;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GameStore.Infrastructure;

public class ChangeLogService : IChangeLogService
{
    private readonly IMongoCollection<BsonDocument> _collection;

    public ChangeLogService(IMongoClient client, IOptions<MongoDbSettings> options)
    {
        var database = client.GetDatabase(options.Value.DatabaseName);
        _collection = database.GetCollection<BsonDocument>("changes");
    }

    public async Task LogEntityChanges(LogActionType action, EntityType entityType, object oldVersion, object newVersion)
    {
        var logEntry = new BsonDocument
        {
            { "Timestamp", DateTime.UtcNow },
            { "Action", action.ToString() },
            { "EntityType", entityType.ToString() },
        };

        AddProperties(oldVersion, logEntry, "Old");
        AddProperties(newVersion, logEntry, "New");

        await _collection.InsertOneAsync(logEntry);
    }

    public static void AddProperties(object entity, BsonDocument logEntry, string entityKind)
    {
        PropertyInfo[] properties = entity.GetType().GetProperties();

        foreach (var property in properties)
        {
            var value = property.GetValue(entity);

            if (value != null)
            {
                if (IsSimpleType(property.PropertyType))
                {
                    logEntry.Add($"{entityKind}.{property.Name}", value.ToString());
                }
                else if (value is IEnumerable)
                {
                    var i = 1;
                    foreach (var currentItem in value as IEnumerable)
                    {
                        var itemProps = currentItem.GetType().Name;
                        var itemId = currentItem.GetType().GetProperty("Id").GetValue(currentItem);

                        logEntry.Add($"{entityKind}.{itemProps}.{i}", itemId.ToString());
                        i++;
                    }
                }
                else
                {
                    if (value is not null)
                    {
                        var objectId = value.GetType().GetProperty("Id").GetValue(value);
                        logEntry.Add($"{entityKind}.{property.Name}", objectId.ToString());
                    }
                    else if (value is null)
                    {
                        logEntry.Add($"{entityKind}.{property.Name}", "Null");
                    }
                }
            }
        }
    }

    private static bool IsSimpleType(Type type)
    {
        return type.IsPrimitive || type == typeof(string) || type.IsEnum ||
               type == typeof(Guid) || type == typeof(DateTime) || type == typeof(DateTime?) ||
               type == typeof(double) || type == typeof(double?) || type == typeof(int) || type == typeof(int?) || type == typeof(uint) || type == typeof(uint?);
    }
}
