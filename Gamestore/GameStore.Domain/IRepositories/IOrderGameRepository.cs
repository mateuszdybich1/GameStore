using GameStore.Domain.Entities;

namespace GameStore.Domain.IRepositories;
public interface IOrderGameRepository
{
    public void AddOrderGame(OrderGame orderGame);

    public void UpdateOrderGame(OrderGame orderGame);

    public void RemoveOrderGame(OrderGame orderGame);

    public OrderGame GetOrderGame(Guid orderId, Guid gameId);

    public List<OrderGame> GetOrderGames(Guid orderId);
}
