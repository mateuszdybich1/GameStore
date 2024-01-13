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

    public Genre(Guid id, string name, Guid parentGuidId)
    {
        Id = id;
        Name = name;
        ParentGerneId = parentGuidId;
    }

    public Guid Id { get; private set; }

    public string Name { get; set; }

    public Guid ParentGerneId { get; set; }

    public List<Game> Games { get; set; }
}