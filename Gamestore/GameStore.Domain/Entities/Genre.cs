namespace GameStore.Domain.Entities;

public class Genre : Entity
{
    public Genre()
    {
    }

    public Genre(Guid id, string name)
        : base(id)
    {
        Name = name;
    }

    public Genre(Guid id, string name, Genre parentGenre)
        : base(id)
    {
        Name = name;
        ParentGenre = parentGenre;
    }

    public string Name { get; set; }

    public Genre? ParentGenre { get; set; }

    public List<Game> Games { get; set; }
}