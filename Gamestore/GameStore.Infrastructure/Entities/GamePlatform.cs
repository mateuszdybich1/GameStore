namespace GameStore.Infrastructure.Entities;

public class GamePlatform
{
    public GamePlatform()
    {
    }

    public GamePlatform(Guid gameId, Guid platformId)
    {
        GameId = gameId;
        PlatformId = platformId;
    }

    public Guid GameId { get; private set; }

    public Guid PlatformId { get; private set; }

    public Game Game { get; set; }

    public Platform Platform { get; set; }
}