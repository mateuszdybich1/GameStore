using GameStore.Domain.Entities;

namespace GameStore.Domain.IRepositories;
public interface IOrderGameRepository : IRepository<OrderGame>
{
    Task<OrderGame> GetOrderGame(Guid orderId, Guid gameId);

    Task<IEnumerable<OrderGame>> GetOrderGames(Guid orderId);
}
