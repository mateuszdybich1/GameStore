using System.ComponentModel;

namespace GameStore.Domain.Entities;
public enum NumberOfGamesOnPageFilteringMode
{
    [Description("10")]
    Ten,
    [Description("20")]
    Twenty,
    [Description("50")]
    Fifty,
    [Description("100")]
    OneHundred,
    [Description("All")]
    All,
}

public enum GameSortingMode
{
    [Description("Most popular")]
    MostPopular,
    [Description("Most commented")]
    MostCommented,
    [Description("Price ASC")]
    PriceASC,
    [Description("Price DESC")]
    PriceDESC,
    [Description("New")]
    New,
}

public enum PublishDateFilteringMode
{
    [Description("last week")]
    LastWeek,
    [Description("last month")]
    LastMonth,
    [Description("last year")]
    LastYear,
    [Description("2 years")]
    TwoYears,
    [Description("3 years")]
    ThreeYears,
}

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