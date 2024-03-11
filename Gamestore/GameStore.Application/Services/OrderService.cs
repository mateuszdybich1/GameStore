using GameStore.Application.Dtos;
using GameStore.Application.IServices;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using GameStore.Domain.IRepositories;
using GameStore.Domain.ISearchCriterias;

namespace GameStore.Application.Services;

public class OrderService(IGamesSearchCriteria gameSearchCriteria, Func<RepositoryTypes, IOrderRepository> orderRepositoryFactory, Func<RepositoryTypes, IOrderGameRepository> orderGameRepositoryFactory, IGameRepository gameRepository) : IOrderService
{
    private readonly IGamesSearchCriteria _gameSearchCriteria = gameSearchCriteria;
    private readonly IOrderRepository _sqlOrderRepository = orderRepositoryFactory(RepositoryTypes.Sql);
    private readonly IOrderRepository _mongoOrderRepository = orderRepositoryFactory(RepositoryTypes.Mongo);
    private readonly IOrderGameRepository _sqlOrderGameRepository = orderGameRepositoryFactory(RepositoryTypes.Sql);
    private readonly IOrderGameRepository _mongoOrderGameRepository = orderGameRepositoryFactory(RepositoryTypes.Mongo);
    private readonly IGameRepository _gameRepository = gameRepository;

    public async Task<Guid> AddOrder(Guid customerId, string gameKey)
    {
        Game game = await _gameSearchCriteria.GetByKey(gameKey) ?? throw new EntityNotFoundException($"Couldn't find game by key: {gameKey}");

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
                orderGame.Quantity += 1;
                orderGame.ModificationDate = DateTime.Now;

                if (gamesInStock < orderGame.Quantity)
                {
                    throw new InvalidOperationException("Couldn' add game to cart. Not enough games in stock");
                }

                await _sqlOrderGameRepository.Update(orderGame);
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
        Game game = await _gameSearchCriteria.GetByKey(gameKey) ?? throw new EntityNotFoundException($"Couldn't find game by key: {gameKey}");

        Order order = await GetOpenOrder(customerId);

        var orderGames = await _sqlOrderGameRepository.GetOrderGames(order.Id);

        if (!orderGames.Any())
        {
            throw new EntityNotFoundException("Cart is empty");
        }

        OrderGame orderGame = orderGames.FirstOrDefault(x => x.ProductId == game.Id) ?? throw new EntityNotFoundException($"Cart does not contains game: {gameKey}");

        if (orderGame.Quantity > 1)
        {
            orderGame.Quantity -= 1;
            orderGame.ModificationDate = DateTime.Now;

            await _sqlOrderGameRepository.Update(orderGame);
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

        if (orderStatus == OrderStatus.Paid)
        {
            var orderGames = await _sqlOrderGameRepository.GetOrderGames(orderId);

            foreach (OrderGame orderGame in orderGames)
            {
                Game game = await _gameRepository.Get(orderGame.ProductId);
                game.UnitInStock -= orderGame.Quantity;
                await _gameRepository.Update(game);
            }
        }

        order.Status = orderStatus;
        order.ModificationDate = DateTime.Now;

        await _sqlOrderRepository.Update(order);
        return order.Id;
    }

    private async Task<Order> GetOpenOrder(Guid customerId)
    {
        return await _sqlOrderRepository.GetCustomerOpenOrder(customerId) ?? throw new EntityNotFoundException($"Customer: {customerId} does not have a cart");
    }
}
