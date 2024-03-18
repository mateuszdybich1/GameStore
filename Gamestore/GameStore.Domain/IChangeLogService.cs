namespace GameStore.Domain;
public enum LogActionType
{
    Delete,
    Update,
}

public enum EntityType
{
    Game,
    Comment,
    Genre,
    Platform,
    Publisher,
    Order,
    OrderGame,
}

public interface IChangeLogService
{
    Task LogEntityChanges(LogActionType action, EntityType entityType, object oldVersion, object newVersion);
}
