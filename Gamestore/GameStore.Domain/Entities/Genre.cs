namespace GameStore.Domain.Entities;

public class Genre
{
    public Genre()
    {
    }

    public Genre(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public Genre(Guid id, string name, Genre parentGenre)
    {
        Id = id;
        Name = name;
        ParentGenre = parentGenre;
    }

    public Guid Id { get; private set; }

    public string Name { get; set; }

    public Genre? ParentGenre { get; set; }

    public List<Game> Games { get; set; }
}