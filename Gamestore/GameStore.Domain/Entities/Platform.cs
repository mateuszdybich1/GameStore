﻿namespace GameStore.Domain.Entities;

public enum PlatformType
{
    Mobile,

    Browser,

    Desktop,

    Console,
}

public class Platform : Entity
{
    public Platform()
    {
    }

    public Platform(Guid id, string type)
        : base(id)
    {
        Type = type;
    }

    public Platform(Platform platform)
        : base(platform.Id, platform.CreationDate, platform.ModificationDate)
    {
        Type = platform.Type;
        Games = platform.Games;
    }

    public string Type { get; set; }

    public List<Game> Games { get; set; }
}