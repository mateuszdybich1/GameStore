using GameStore.Application.Dtos;
using GameStore.Domain.Entities;

namespace GameStore.Application.IServices;

public interface IOrderService
{
    public Task<Guid> AddOrder(Guid customerId, string gameKey);

    public Task<Guid> UpdateOrder(Guid orderId, OrderStatus orderStatus);

    public Task<Guid> RemoveOrder(Guid customerId, string gameKey);

    public Task<IEnumerable<OrderDto>> GetPaidAndCancelledOrders();

    public Task<OrderDto> GetOrder(Guid orderId);

    public Task<IEnumerable<OrderGameDto>> GetOrderDetails(Guid orderId);

    public Task<IEnumerable<OrderGameDto>> GetCart(Guid customerId);

    public Task<OrderInformation> GetOrderInformation(Guid customerId);
}
