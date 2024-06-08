using System.ComponentModel.DataAnnotations;
using GameStore.Domain;
using GameStore.Domain.Entities;
using GameStore.Domain.MongoEntities;

namespace GameStore.Application.Dtos;

public class GameDto
{
    public GameDto()
    {
    }

    public GameDto(Game game)
    {
        Id = game.Id;
        Name = game.Name;
        Key = game.Key;
        Description = game.Description;
        Price = game.Price;
        UnitInStock = game.UnitInStock;
        Discount = game.Discount;
        NumberOfViews = game.NumberOfViews;
    }

    public GameDto(MongoGame mongoGame)
    {
        Id = mongoGame.Id.AsGuid();
        Name = mongoGame.ProductName;
        Key = mongoGame.ProductKey;
        Description = mongoGame.QuantityPerUnit;
        Price = mongoGame.UnitPrice;
        UnitInStock = mongoGame.UnitsInStock;
        Discount = mongoGame.Discontinued;
        NumberOfViews = (ulong)mongoGame.NumberOfViews;
    }

    public Guid? Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Key { get; set; }

    public string Description { get; set; }

    [Required]
    public double Price { get; set; }

    [Required]
    public int UnitInStock { get; set; }

    [Required]
    public double Discount { get; set; }

    public ulong NumberOfViews { get; private set; }
}
