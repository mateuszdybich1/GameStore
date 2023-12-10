namespace GameStore.Infrastructure.Entities;

public class Game
{
    public Game()
    {
    }

    public Game(Guid id, string name, string key, List<Genre> genres, List<Platform> platforms)
    {
        Id = id;
        Name = name;
        Key = key;
        Genres = genres;
        Platforms = platforms;
    }

    public Game(Guid id, string name, string key, string description, List<Genre> genres, List<Platform> platforms)
    {
        Id = id;
        Name = name;
        Key = key;
        Description = description;
        Genres = genres;
        Platforms = platforms;
    }

    public Guid Id { get; private set; }

    public string Name { get; set; }

    public string Key { get; set; }

    public string Description { get; set; }

    public List<Genre> Genres { get; set; }

    public List<Platform> Platforms { get; set; }
}