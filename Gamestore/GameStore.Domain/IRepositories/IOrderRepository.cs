using GameStore.Domain.Entities;

namespace GameStore.Domain.IRepositories;
public interface IOrderRepository : IRepository<Order>
{
    Task<Order> GetCustomerOpenOrder(Guid customerId);

    Task<IEnumerable<Order>> GetOrdersByCustomerId(Guid customerId);

    Task<IEnumerable<Order>> GetPaidAndCancelledOrders();

    Task<IEnumerable<Order>> GetAllOrders(DateTime startDate, DateTime dateTo);
}
