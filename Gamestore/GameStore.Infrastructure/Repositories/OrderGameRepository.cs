using GameStore.Domain.Entities;
using GameStore.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure.Repositories;

public class OrderGameRepository(AppDbContext appDbContext) : Repository<OrderGame>(appDbContext), IOrderGameRepository
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task<OrderGame> GetOrderGame(Guid orderId, Guid gameId)
    {
        return await _appDbContext.OrderGames.AsNoTracking().SingleOrDefaultAsync(x => x.OrderId == orderId && x.ProductId == gameId);
    }

    public async Task<IEnumerable<OrderGame>> GetOrderGames(Guid orderId)
    {
        return await _appDbContext.OrderGames.AsNoTracking().Where(x => x.OrderId == orderId).ToListAsync();
    }
}
