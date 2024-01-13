namespace GameStore.Domain.Entities;

public enum PlatformType
{
    Mobile,

    Browser,

    Desktop,

    Console,
}

public class Platform
{
    public Platform()
    {
    }

    public Platform(Guid id, string type)
    {
        Id = id;
        Type = type;
    }

    public Guid Id { get; private set; }

    public string Type { get; set; }

    public List<Game> Games { get; set; }
}