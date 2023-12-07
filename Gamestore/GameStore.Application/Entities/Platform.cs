namespace GameStore.Application.Entities;

public enum PlatformType
{
    Mobile,

    Browser,

    Desktop,

    Console,
}

public class Platform
{
    public Guid Id { get; private set; }

    public string Type { get; set; }
}
