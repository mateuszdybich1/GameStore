using GameStore.Domain.MongoEntities;

namespace GameStore.Domain.Entities;

public class Genre : Entity
{
    public Genre()
    {
    }

    public Genre(Guid id, string name, string? description, string? picture)
        : base(id)
    {
        Name = name;
        Description = description;
        Picture = picture;
    }

    public Genre(Guid id, string name, string? description, string? picture, Genre parentGenre)
        : base(id)
    {
        Name = name;
        ParentGenre = parentGenre;
        Description = description;
        Picture = picture;
    }

    public Genre(MongoGenre mongoGenre)
        : base(mongoGenre.Id.AsGuid())
    {
        Name = mongoGenre.CategoryName;
        Description = mongoGenre.Description;
        Picture = mongoGenre.Picture;
    }

    public string Name { get; set; }

    public string? Description { get; set; }

    public string? Picture { get; set; }

    public Genre? ParentGenre { get; set; }

    public List<Game> Games { get; set; }
}