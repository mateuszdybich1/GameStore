using GameStore.Application.Dtos;
using GameStore.Domain.Entities;

namespace GameStore.Application.IServices;

public interface IOrderService
{
    Task<Guid> AddOrder(Guid customerId, string gameKey);

    Task<Guid> UpdateOrder(Guid orderId, OrderStatus orderStatus);

    Task<Guid> RemoveOrder(Guid customerId, string gameKey);

    Task<IEnumerable<OrderDto>> GetPaidAndCancelledOrders();

    Task<OrderDto> GetOrder(Guid orderId);

    Task<IEnumerable<OrderGameDto>> GetOrderDetails(Guid orderId);

    Task<IEnumerable<OrderGameDto>> GetCart(Guid customerId);

    Task<OrderInformation> GetOrderInformation(Guid customerId);

    Task<IEnumerable<OrderDto>> GetOrderHistory(DateTime? startDate, DateTime? dateTo);
}
