using GameStore.Domain.Entities;

namespace GameStore.Application.Dtos;

public class OrderDto(Order order)
{
    public Guid Id { get; set; } = order.Id;

    public Guid CustomerId { get; set; } = order.CustomerId;

    public DateTime Date { get; set; } = order.Date;
}
