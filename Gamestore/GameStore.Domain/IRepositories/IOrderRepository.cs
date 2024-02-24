using GameStore.Domain.Entities;

namespace GameStore.Domain.IRepositories;
public interface IOrderRepository : IRepository<Order>
{
    public Task<Order> GetCustomerOpenOrder(Guid customerId);

    public Task<IEnumerable<Order>> GetOrdersByCustomerId(Guid customerId);

    public Task<IEnumerable<Order>> GetPaidAndCancelledOrders();
}
