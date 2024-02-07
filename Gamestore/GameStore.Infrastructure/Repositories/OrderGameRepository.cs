﻿using GameStore.Domain.Entities;
using GameStore.Domain.IRepositories;

namespace GameStore.Infrastructure.Repositories;

public class OrderGameRepository(AppDbContext appDbContext) : IOrderGameRepository
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public void AddOrderGame(OrderGame orderGame)
    {
        _appDbContext.OrderGames.Add(orderGame);
        _appDbContext.SaveChanges();
    }

    public OrderGame GetOrderGame(Guid orderId, Guid gameId)
    {
        return _appDbContext.OrderGames.FirstOrDefault(x => x.OrderId == orderId && x.ProductId == gameId);
    }

    public List<OrderGame> GetOrderGames(Guid orderId)
    {
        return [.. _appDbContext.OrderGames.Where(x => x.OrderId == orderId)];
    }

    public void RemoveOrderGame(OrderGame orderGame)
    {
        _appDbContext.OrderGames.Remove(orderGame);
        _appDbContext.SaveChanges();
    }

    public void UpdateOrderGame(OrderGame orderGame)
    {
        _appDbContext.OrderGames.Update(orderGame);
        _appDbContext.SaveChanges();
    }
}