using GameStore.Domain.Entities;

namespace GameStore.Domain.IRepositories;
public interface IOrderGameRepository : IRepository<OrderGame>
{
    public Task<OrderGame> GetOrderGame(Guid orderId, Guid gameId);

    public Task<IEnumerable<OrderGame>> GetOrderGames(Guid orderId);
}
