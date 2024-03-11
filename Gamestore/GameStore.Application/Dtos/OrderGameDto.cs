using GameStore.Domain.Entities;

namespace GameStore.Application.Dtos;

public class OrderGameDto(OrderGame orderGame)
{
    public Guid ProductId { get; set; } = orderGame.ProductId;

    public double Price { get; set; } = orderGame.Price;

    public int Quantity { get; set; } = orderGame.Quantity;

    public double Discount { get; set; } = orderGame.Discount;
}
