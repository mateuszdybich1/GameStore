using GameStore.Domain.Entities;
using GameStore.Domain.IRepositories;

namespace GameStore.Infrastructure.Repositories;

public class OrderRepository(AppDbContext appDbContext) : IOrderRepository
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public void AddOrder(Order order)
    {
        _appDbContext.Orders.Add(order);
        _appDbContext.SaveChanges();
    }

    public void DeleteOrder(Order order)
    {
        _appDbContext.Orders.Remove(order);
        _appDbContext.SaveChanges();
    }

    public Order GetOrder(Guid orderId)
    {
        return _appDbContext.Orders.FirstOrDefault(x => x.Id == orderId);
    }

    public Order GetCustomerOpenOrder(Guid customerId)
    {
        return _appDbContext.Orders.FirstOrDefault(x => x.CustomerId == customerId && x.Status == OrderStatus.Open);
    }

    public List<Order> GetOrdersByCustomerId(Guid customerId)
    {
        return [.. _appDbContext.Orders.Where(x => x.CustomerId == customerId)];
    }

    public List<Order> GetPaidAndCancelledOrders()
    {
        return [.. _appDbContext.Orders.Where(x => x.Status == OrderStatus.Paid || x.Status == OrderStatus.Cancelled)];
    }
}
