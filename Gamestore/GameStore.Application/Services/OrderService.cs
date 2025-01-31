﻿using GameStore.Application.Dtos;
using GameStore.Application.IServices;
using GameStore.Domain;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using GameStore.Domain.IRepositories;
using GameStore.Domain.ISearchCriterias;

namespace GameStore.Application.Services;

public class OrderService(Func<RepositoryTypes, IGamesSearchCriteria> gameSearchCriteriaFactory, Func<RepositoryTypes, IOrderRepository> orderRepositoryFactory, Func<RepositoryTypes, IOrderGameRepository> orderGameRepositoryFactory, IGameRepository gameRepository, IChangeLogService changeLogService, INotificationService notificationService) : IOrderService
{
    private readonly IGamesSearchCriteria _sqlGameSearchCriteria = gameSearchCriteriaFactory(RepositoryTypes.Sql);
    private readonly IGamesSearchCriteria _mongoGameSearchCriteria = gameSearchCriteriaFactory(RepositoryTypes.Mongo);
    private readonly IOrderRepository _sqlOrderRepository = orderRepositoryFactory(RepositoryTypes.Sql);
    private readonly IOrderRepository _mongoOrderRepository = orderRepositoryFactory(RepositoryTypes.Mongo);
    private readonly IOrderGameRepository _sqlOrderGameRepository = orderGameRepositoryFactory(RepositoryTypes.Sql);
    private readonly IOrderGameRepository _mongoOrderGameRepository = orderGameRepositoryFactory(RepositoryTypes.Mongo);
    private readonly IGameRepository _gameRepository = gameRepository;
    private readonly IChangeLogService _changeLogService = changeLogService;
    private readonly INotificationService _notificationService = notificationService;

    public async Task<Guid> AddOrder(Guid customerId, string gameKey)
    {
        Game game = await _sqlGameSearchCriteria.GetByKey(gameKey) ?? await _mongoGameSearchCriteria.GetByKey(gameKey) ?? throw new EntityNotFoundException($"Couldn't find game by key: {gameKey}");

        int gamesInStock = game.UnitInStock < 1 ? throw new InvalidOperationException("Game has no stock") : game.UnitInStock;

        Order order = await _sqlOrderRepository.GetCustomerOpenOrder(customerId);

        if (order == null)
        {
            order = new(Guid.NewGuid(), customerId, OrderStatus.Open);

            OrderGame orderGame = new(Guid.NewGuid(), order.Id, game.Id, game.Price, 1, game.Discount);

            await _sqlOrderRepository.Add(order);

            await _sqlOrderGameRepository.Add(orderGame);
        }
        else
        {
            OrderGame orderGame = await _sqlOrderGameRepository.GetOrderGame(order.Id, game.Id);

            if (orderGame == null)
            {
                orderGame = new(Guid.NewGuid(), order.Id, game.Id, game.Price, 1, game.Discount);
                await _sqlOrderGameRepository.Add(orderGame);
            }
            else
            {
                OrderGame oldOrderGame = new(orderGame);
                orderGame.Quantity += 1;
                orderGame.ModificationDate = DateTime.Now;

                if (gamesInStock < orderGame.Quantity)
                {
                    throw new InvalidOperationException("Couldn' add game to cart. Not enough games in stock");
                }

                await _sqlOrderGameRepository.Update(orderGame);

                await _changeLogService.LogEntityChanges(LogActionType.Update, EntityType.OrderGame, oldOrderGame, orderGame);
            }
        }

        return order.Id;
    }

    public async Task<IEnumerable<OrderGameDto>> GetCart(Guid customerId)
    {
        Order order = await _sqlOrderRepository.GetCustomerOpenOrder(customerId);

        var orderGames = order == null ? [] : await _sqlOrderGameRepository.GetOrderGames(order.Id);

        return orderGames.Select(x => new OrderGameDto(x));
    }

    public async Task<OrderDto> GetOrder(Guid orderId)
    {
        Order order = await _sqlOrderRepository.Get(orderId) ?? await _mongoOrderRepository.Get(orderId) ?? throw new EntityNotFoundException($"Order with Id: {orderId} not found");

        return new(order);
    }

    public async Task<IEnumerable<OrderGameDto>> GetOrderDetails(Guid orderId)
    {
        var orderGames = await _sqlOrderGameRepository.GetOrderGames(orderId);
        var mongoOrderGames = await _mongoOrderGameRepository.GetOrderGames(orderId);
        return orderGames.Concat(mongoOrderGames).Select(x => new OrderGameDto(x));
    }

    public async Task<IEnumerable<OrderDto>> GetOrderHistory(DateTime? startDate, DateTime? dateTo)
    {
        if (startDate == null)
        {
            startDate = DateTime.MinValue;
        }

        if (dateTo == null)
        {
            dateTo = DateTime.Now;
        }

        var orders = await _sqlOrderRepository.GetAllOrders((DateTime)startDate!, (DateTime)dateTo!);
        var mongoOrders = await _mongoOrderRepository.GetAllOrders((DateTime)startDate!, (DateTime)dateTo!);

        return orders.Concat(mongoOrders).Select(x => new OrderDto(x));
    }

    public async Task<OrderInformation> GetOrderInformation(Guid customerId)
    {
        Order order = await GetOpenOrder(customerId);

        var orderGames = await _sqlOrderGameRepository.GetOrderGames(order.Id);
        var mongoOrderGames = await _mongoOrderGameRepository.GetOrderGames(order.Id);

        double totalSum = 0;
        foreach (var orderGame in orderGames.Concat(mongoOrderGames))
        {
            totalSum += orderGame.Price * (1 - orderGame.Discount) * orderGame.Quantity;
        }

        return new(order.Id, order.CreationDate, (int)totalSum);
    }

    public async Task<IEnumerable<OrderDto>> GetPaidAndCancelledOrders()
    {
        var games = await _sqlOrderRepository.GetPaidAndCancelledOrders();
        var mongoOrders = await _mongoOrderRepository.GetPaidAndCancelledOrders();
        return games.Concat(mongoOrders).Select(x => new OrderDto(x));
    }

    public async Task<Guid> RemoveOrder(Guid customerId, string gameKey)
    {
        Game game = await _sqlGameSearchCriteria.GetByKey(gameKey) ?? await _mongoGameSearchCriteria.GetByKey(gameKey) ?? throw new EntityNotFoundException($"Couldn't find game by key: {gameKey}");

        Order order = await GetOpenOrder(customerId);

        var orderGames = await _sqlOrderGameRepository.GetOrderGames(order.Id);

        if (!orderGames.Any())
        {
            throw new EntityNotFoundException("Cart is empty");
        }

        OrderGame orderGame = orderGames.FirstOrDefault(x => x.ProductId == game.Id) ?? throw new EntityNotFoundException($"Cart does not contains game: {gameKey}");

        if (orderGame.Quantity > 1)
        {
            OrderGame oldOrderGame = new(orderGame);
            orderGame.Quantity -= 1;
            orderGame.ModificationDate = DateTime.Now;

            await _sqlOrderGameRepository.Update(orderGame);

            await _changeLogService.LogEntityChanges(LogActionType.Delete, EntityType.OrderGame, oldOrderGame, orderGame);
        }
        else
        {
            await _sqlOrderGameRepository.Delete(orderGame);
            if (orderGames.Count() == 1)
            {
                await _sqlOrderRepository.Delete(order);
            }
        }

        return order.Id;
    }

    public async Task<Guid> UpdateOrder(Guid orderId, OrderStatus orderStatus)
    {
        Order order = await _sqlOrderRepository.Get(orderId) ?? throw new EntityNotFoundException($"Couldn't find order by ID: {orderId}");
        Order oldOrder = new(order);

        if (orderStatus == OrderStatus.Paid)
        {
            var orderGames = await _sqlOrderGameRepository.GetOrderGames(orderId);

            foreach (OrderGame orderGame in orderGames)
            {
                Game game = await _gameRepository.Get(orderGame.ProductId);
                Game oldGame = new(game);
                game.UnitInStock -= orderGame.Quantity;
                await _gameRepository.Update(game);

                await _changeLogService.LogEntityChanges(LogActionType.Update, EntityType.Game, oldGame, game);
                await _notificationService.NotifyOrderStatusChange(orderId, orderStatus.ToString());
            }
        }

        order.Status = orderStatus;
        order.ModificationDate = DateTime.Now;

        await _sqlOrderRepository.Update(order);

        await _changeLogService.LogEntityChanges(LogActionType.Update, EntityType.Order, oldOrder, order);
        return order.Id;
    }

    public async Task<Guid> UpdateOrderDetailQuantity(Guid orderDetailID, int quantity)
    {
        OrderGame orderGame = await _sqlOrderGameRepository.Get(orderDetailID);
        OrderGame oldOrderGame;
        if (orderGame == null)
        {
            orderGame = await _mongoOrderGameRepository.Get(orderDetailID);
            if (orderGame == null)
            {
                throw new EntityNotFoundException($"Couldn't find order details by ID: {orderDetailID}");
            }

            oldOrderGame = new(orderGame);
            orderGame.Quantity = quantity;

            Order order = await _sqlOrderRepository.Get(orderGame.OrderId);
            if (order == null)
            {
                order = await _mongoOrderRepository.Get(orderGame.OrderId) ?? throw new EntityNotFoundException($"Order with Id: {orderGame.OrderId} not found");
                await _sqlOrderRepository.Add(order);
            }

            await _sqlOrderGameRepository.Add(orderGame);
        }
        else
        {
            oldOrderGame = new(orderGame);
            orderGame.Quantity = quantity;
            await _sqlOrderGameRepository.Update(orderGame);
        }

        await _changeLogService.LogEntityChanges(LogActionType.Update, EntityType.OrderGame, oldOrderGame, orderGame);
        return orderDetailID;
    }

    public async Task<Guid> ShipOrder(Guid orderId)
    {
        Order order = await _sqlOrderRepository.Get(orderId) ?? throw new EntityNotFoundException($"Couldn't find order by ID: {orderId}");

        if (order.Status == OrderStatus.Paid)
        {
            Order oldOrder = new(order);

            order.Status = OrderStatus.Shipped;
            order.ModificationDate = DateTime.Now;

            await _sqlOrderRepository.Update(order);

            await _changeLogService.LogEntityChanges(LogActionType.Update, EntityType.Order, oldOrder, order);
            await _notificationService.NotifyOrderStatusChange(orderId, OrderStatus.Shipped.ToString());
            return order.Id;
        }
        else
        {
            throw new ArgumentException("Only paid orders can be shipped");
        }
    }

    public async Task<Guid> AddGameToOrderDetails(Guid orderId, string gameKey)
    {
        Order order = await _sqlOrderRepository.Get(orderId);

        var game = await _sqlGameSearchCriteria.GetByKey(gameKey) ?? await _mongoGameSearchCriteria.GetByKey(gameKey) ?? throw new EntityNotFoundException($"Couldn't find game by key: {gameKey}");

        if (order == null)
        {
            order = await _mongoOrderRepository.Get(orderId) ?? throw new EntityNotFoundException($"Couldn't find order by ID: {orderId}");

            await _sqlOrderRepository.Add(order);

            var orderGame = await _mongoOrderGameRepository.GetOrderGame(order.Id, game.Id);

            if (orderGame == null)
            {
                orderGame = new OrderGame(Guid.NewGuid(), order.Id, game.Id, game.Price, 1, game.Discount);
            }
            else
            {
                OrderGame oldOrderGame = new(orderGame);
                orderGame.Quantity += 1;

                await _changeLogService.LogEntityChanges(LogActionType.Update, EntityType.OrderGame, oldOrderGame, orderGame);
            }

            await _sqlOrderGameRepository.Add(orderGame);
        }
        else
        {
            var orderGame = await _sqlOrderGameRepository.GetOrderGame(order.Id, game.Id);

            if (orderGame == null)
            {
                orderGame = new OrderGame(Guid.NewGuid(), order.Id, game.Id, game.Price, 1, game.Discount);

                await _sqlOrderGameRepository.Add(orderGame);
            }
            else
            {
                OrderGame oldOrderGame = new(orderGame);
                orderGame.Quantity += 1;
                await _sqlOrderGameRepository.Update(orderGame);

                await _changeLogService.LogEntityChanges(LogActionType.Update, EntityType.OrderGame, oldOrderGame, orderGame);
            }
        }

        return order.Id;
    }

    public async Task<Guid> DeleteOrderDetails(Guid orderDetailsID)
    {
        OrderGame orderGame = await _sqlOrderGameRepository.Get(orderDetailsID);

        if (orderGame == null)
        {
            orderGame = await _mongoOrderGameRepository.Get(orderDetailsID) ?? throw new EntityNotFoundException($"Couldn't find order details by ID: {orderDetailsID}");

            await _mongoOrderGameRepository.Delete(orderGame);
        }
        else
        {
            await _sqlOrderGameRepository.Delete(orderGame);

            var mongoOrderGame = await _mongoOrderGameRepository.Get(orderDetailsID);
            if (mongoOrderGame != null)
            {
                await _mongoOrderGameRepository.Delete(mongoOrderGame);
            }
        }

        return orderDetailsID;
    }

    private async Task<Order> GetOpenOrder(Guid customerId)
    {
        return await _sqlOrderRepository.GetCustomerOpenOrder(customerId) ?? throw new EntityNotFoundException($"Customer: {customerId} does not have a cart");
    }
}
