using GameStore.Application.Dtos;
using GameStore.Domain.Entities;

namespace GameStore.Application.IServices;

public interface IOrderService
{
    public Guid AddOrder(Guid customerId, string gameKey);

    public Guid UpdateOrder(Guid orderId, OrderStatus orderStatus);

    public Guid RemoveOrder(Guid customerId, string gameKey);

    public List<OrderDto> GetPaidAndCancelledOrders();

    public OrderDto GetOrder(Guid orderId);

    public List<OrderGameDto> GetOrderDetails(Guid orderId);

    public List<OrderGameDto> GetCart(Guid customerId);

    public OrderInformation GetOrderInformation(Guid customerId);
}
