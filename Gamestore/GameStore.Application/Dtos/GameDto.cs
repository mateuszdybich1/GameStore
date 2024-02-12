using System.ComponentModel.DataAnnotations;
using GameStore.Domain.Entities;

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
    public int Discount { get; set; }

    public ulong NumberOfViews { get; private set; }
}
