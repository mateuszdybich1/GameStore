using GameStore.Domain;
using GameStore.Domain.Entities;
using GameStore.Domain.IRepositories;
using GameStore.Domain.MongoEntities;
using GameStore.Infrastructure.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GameStore.Infrastructure.MongoRepositories;

public class MongoOrderGameRepository : IOrderGameRepository
{
    private readonly IMongoCollection<MongoOrderGame> _orderGamesCollection;
    private readonly IMongoCollection<MongoOrder> _ordersCollection;
    private readonly IMongoCollection<MongoGame> _gamesCollection;

    public MongoOrderGameRepository(IMongoClient client, IOptions<MongoDbSettings> options)
    {
        var database = client.GetDatabase(options.Value.DatabaseName);
        _orderGamesCollection = database.GetCollection<MongoOrderGame>("order-details");
        _ordersCollection = database.GetCollection<MongoOrder>("orders");
        _gamesCollection = database.GetCollection<MongoGame>("products");
    }

    public Task Add(OrderGame entity)
    {
        throw new NotImplementedException();
    }

    public Task Delete(OrderGame entity)
    {
        throw new NotImplementedException();
    }

    public Task<OrderGame> Get(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<OrderGame> GetOrderGame(Guid orderId, Guid gameId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<OrderGame>> GetOrderGames(Guid orderId)
    {
        var order = await _ordersCollection.Find(x => x.Id == orderId.AsObjectId()).FirstOrDefaultAsync();
        var orderGames = new List<OrderGame>();

        if (order != null)
        {
            var mongoOrderGames = await _orderGamesCollection.Find(x => x.OrderID == order.OrderID).ToListAsync();

            foreach (var mongoOrderGame in mongoOrderGames)
            {
                var product = await _gamesCollection.Find(x => x.ProductID == mongoOrderGame.ProductID).FirstOrDefaultAsync();
                if (product != null)
                {
                    orderGames.Add(new OrderGame(mongoOrderGame.Id.AsGuid(), orderId, product.Id.AsGuid(), mongoOrderGame.UnitPrice, mongoOrderGame.Quantity, mongoOrderGame.Discount));
                }
            }
        }

        return orderGames;
    }

    public Task Update(OrderGame entity)
    {
        throw new NotImplementedException();
    }
}
