using GameStore.Domain.Entities;
using GameStore.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure.Repositories;

public class OrderRepository(AppDbContext appDbContext) : Repository<Order>(appDbContext), IOrderRepository
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task<Order> GetCustomerOpenOrder(Guid customerId)
    {
        return await _appDbContext.Orders.AsNoTracking().SingleOrDefaultAsync(x => x.CustomerId == customerId && x.Status == OrderStatus.Open);
    }

    public async Task<IEnumerable<Order>> GetOrdersByCustomerId(Guid customerId)
    {
        return await _appDbContext.Orders.AsNoTracking().Where(x => x.CustomerId == customerId).ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetPaidAndCancelledOrders()
    {
        return await _appDbContext.Orders.AsNoTracking().Where(x => x.Status == OrderStatus.Paid || x.Status == OrderStatus.Cancelled).ToListAsync();
    }
}
