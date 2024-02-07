namespace GameStore.Domain.Entities;

public class Game
{
    public Game()
    {
    }

    public Game(Guid id, string name, string key, double price, int unitInStock, int discount, Guid publisherId, List<Genre> genres, List<Platform> platforms)
    {
        Id = id;
        Name = name;
        Key = key;
        Price = price;
        UnitInStock = unitInStock;
        Discount = discount;
        PublisherId = publisherId;
        Genres = genres;
        Platforms = platforms;
    }

    public Game(Guid id, string name, string key, double price, int unitInStock, int discount, string description, Guid publisherId, List<Genre> genres, List<Platform> platforms)
    {
        Id = id;
        Name = name;
        Key = key;
        Price = price;
        UnitInStock = unitInStock;
        Discount = discount;
        Description = description;
        PublisherId = publisherId;
        Genres = genres;
        Platforms = platforms;
    }

    public Guid Id { get; private set; }

    public string Name { get; set; }

    public string Key { get; set; }

    public string Description { get; set; }

    public double Price { get; set; }

    public int UnitInStock { get; set; }

    public int Discount { get; set; }

    public Guid PublisherId { get; set; }

    public List<Genre> Genres { get; set; }

    public List<Platform> Platforms { get; set; }

    public List<Comment> Comments { get; set; }

    public Publisher Publisher { get; private set; }
}