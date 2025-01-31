﻿using GameStore.Domain;
using GameStore.Domain.Entities;
using GameStore.Domain.IRepositories;
using GameStore.Domain.MongoEntities;
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

    public async Task<OrderGame> Get(Guid id)
    {
        if (_orderGamesCollection != null && _orderGamesCollection.Database.Client.Cluster.Description.State == MongoDB.Driver.Core.Clusters.ClusterState.Connected)
        {
            var orderGame = await _orderGamesCollection.Find(x => x.Id == id.AsObjectId()).FirstOrDefaultAsync();

            if (orderGame != null)
            {
                var order = await _ordersCollection.Find(x => x.OrderID == orderGame.OrderID).FirstOrDefaultAsync();
                var game = await _gamesCollection.Find(x => x.ProductID == orderGame.ProductID).FirstOrDefaultAsync();
                return order != null && game != null
                    ? new OrderGame(id, order.Id.AsGuid(), game.Id.AsGuid(), orderGame.UnitPrice, orderGame.Quantity, orderGame.Discount)
                    : null;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }

    public async Task<OrderGame> GetOrderGame(Guid orderId, Guid gameId)
    {
        if (_orderGamesCollection != null && _orderGamesCollection.Database.Client.Cluster.Description.State == MongoDB.Driver.Core.Clusters.ClusterState.Connected)
        {
            var order = await _ordersCollection.Find(x => x.Id == orderId.AsObjectId()).FirstOrDefaultAsync();
            var game = await _gamesCollection.Find(x => x.Id == gameId.AsObjectId()).FirstOrDefaultAsync();

            if (order != null && game != null)
            {
                var orderGame = await _orderGamesCollection.Find(x => x.OrderID == order.OrderID && x.ProductID == game.ProductID).FirstOrDefaultAsync();
                return game != null
                    ? new OrderGame(orderGame.Id.AsGuid(), orderId, gameId, orderGame.UnitPrice, orderGame.Quantity, orderGame.Discount)
                    : null;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }

    public async Task<IEnumerable<OrderGame>> GetOrderGames(Guid orderId)
    {
        if (_ordersCollection != null && _ordersCollection.Database.Client.Cluster.Description.State == MongoDB.Driver.Core.Clusters.ClusterState.Connected)
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
        else
        {
            return [];
        }
    }

    public Task Update(OrderGame entity)
    {
        throw new NotImplementedException();
    }
}
