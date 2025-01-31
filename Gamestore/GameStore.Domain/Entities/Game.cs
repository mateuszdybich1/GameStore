﻿using System.ComponentModel;
using GameStore.Domain.MongoEntities;

namespace GameStore.Domain.Entities;
public enum NumberOfGamesOnPageFilteringMode
{
    [Description("10")]
    Ten = 10,
    [Description("20")]
    Twenty = 20,
    [Description("50")]
    Fifty = 50,
    [Description("100")]
    OneHundred = 100,
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

public class Game : Entity
{
    public Game()
    {
    }

    public Game(Guid id, string name, string key, double price, int unitInStock, double discount, Guid? imageId, Guid publisherId, List<Genre> genres, List<Platform> platforms, Publisher publisher)
        : base(id)
    {
        Name = name;
        Key = key;
        Price = price;
        UnitInStock = unitInStock;
        Discount = discount;
        ImageId = imageId;
        PublisherId = publisherId;
        Genres = genres;
        Platforms = platforms;
        Publisher = publisher;
    }

    public Game(Guid id, string name, string key, double price, int unitInStock, double discount, string description, Guid? imageId, Guid publisherId, List<Genre> genres, List<Platform> platforms, Publisher publisher)
        : base(id)
    {
        Name = name;
        Key = key;
        Price = price;
        UnitInStock = unitInStock;
        Discount = discount;
        Description = description;
        ImageId = imageId;
        PublisherId = publisherId;
        Genres = genres;
        Platforms = platforms;
        Publisher = publisher;
    }

    public Game(Game game)
        : base(game.Id, game.CreationDate, game.ModificationDate)
    {
        Name = game.Name;
        Key = game.Key;
        Price = game.Price;
        UnitInStock = game.UnitInStock;
        Discount = game.Discount;
        Description = game.Key;
        ImageId = game.ImageId;
        PublisherId = game.PublisherId;
        Genres = game.Genres;
        Platforms = game.Platforms;
        Publisher = game.Publisher;
    }

    public Game(MongoGame mongoGame)
        : base(mongoGame.Id.AsGuid())
    {
        Name = mongoGame.ProductName;
        Key = mongoGame.ProductKey;
        Description = mongoGame.QuantityPerUnit;
        Price = mongoGame.UnitPrice;
        UnitInStock = mongoGame.UnitsInStock;
        Discount = mongoGame.Discontinued;
        NumberOfViews = (ulong)mongoGame.NumberOfViews;
        ImageId = null;
    }

    public string Name { get; set; }

    public string Key { get; set; }

    public string Description { get; set; }

    public double Price { get; set; }

    public int UnitInStock { get; set; }

    public double Discount { get; set; }

    public ulong NumberOfViews { get; set; }

    public Guid? ImageId { get; set; }

    public Guid PublisherId { get; set; }

    public List<Genre> Genres { get; set; }

    public List<Platform> Platforms { get; set; }

    public List<Comment> Comments { get; set; }

    public Publisher Publisher { get; set; }
}