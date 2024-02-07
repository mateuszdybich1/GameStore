using GameStore.Domain.Entities;

namespace GameStore.Domain.IRepositories;
public interface IOrderRepository
{
    public void AddOrder(Order order);

    public void UpdateOrder(Order order);

    public void DeleteOrder(Order order);

    public Order GetOrder(Guid orderId);

    public Order GetCustomerOpenOrder(Guid customerId);

    public List<Order> GetOrdersByCustomerId(Guid customerId);

    public List<Order> GetPaidAndCancelledOrders();
}
