namespace GameStore.Infrastructure.Entities;

public class Game
{
    public Game()
    {
    }

    public Game(Guid id, string name, string key)
    {
        Id = id;
        Name = name;
        Key = key;
    }

    public Game(Guid id, string name, string key, string description)
    {
        Id = id;
        Name = name;
        Key = key;
        Description = description;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public string Key { get; private set; }

    public string Description { get; set; }

    public List<Genre> Genres { get; set; }

    public List<Platform> Platforms { get; set; }
}