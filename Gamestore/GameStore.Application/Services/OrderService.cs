using GameStore.Application.Dtos;
using GameStore.Application.IServices;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using GameStore.Domain.IRepositories;
using GameStore.Domain.ISearchCriterias;

namespace GameStore.Application.Services;

public class OrderService(IGamesSearchCriteria gameSearchCriteria, IOrderRepository orderRepository, IOrderGameRepository orderGameRepository, IGameRepository gameRepository) : IOrderService
{
    private readonly IGamesSearchCriteria _gameSearchCriteria = gameSearchCriteria;
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IOrderGameRepository _orderGameRepository = orderGameRepository;
    private readonly IGameRepository _gameRepository = gameRepository;

    public async Task<Guid> AddOrder(Guid customerId, string gameKey)
    {
        Game game = await _gameSearchCriteria.GetByKey(gameKey) ?? throw new EntityNotFoundException($"Couldn't find game by key: {gameKey}");

        int gamesInStock = game.UnitInStock < 1 ? throw new InvalidOperationException("Game has no stock") : game.UnitInStock;

        Order order = await _orderRepository.GetCustomerOpenOrder(customerId);

        if (order == null)
        {
            order = new(Guid.NewGuid(), customerId, OrderStatus.Open);

            OrderGame orderGame = new(Guid.NewGuid(), order.Id, game.Id, game.Price, 1, game.Discount);

            await _orderRepository.Add(order);

            await _orderGameRepository.Add(orderGame);
        }
        else
        {
            OrderGame orderGame = await _orderGameRepository.GetOrderGame(order.Id, game.Id);

            if (orderGame == null)
            {
                orderGame = new(Guid.NewGuid(), order.Id, game.Id, game.Price, 1, game.Discount);
                await _orderGameRepository.Add(orderGame);
            }
            else
            {
                orderGame.Quantity += 1;
                orderGame.ModificationDate = DateTime.Now;

                if (gamesInStock < orderGame.Quantity)
                {
                    throw new InvalidOperationException("Couldn' add game to cart. Not enough games in stock");
                }

                await _orderGameRepository.Update(orderGame);
            }
        }

        return order.Id;
    }

    public async Task<IEnumerable<OrderGameDto>> GetCart(Guid customerId)
    {
        Order order = await _orderRepository.GetCustomerOpenOrder(customerId);

        var orderGames = order == null ? [] : await _orderGameRepository.GetOrderGames(order.Id);

        return orderGames.Select(x => new OrderGameDto(x));
    }

    public async Task<OrderDto> GetOrder(Guid orderId)
    {
        Order order = await _orderRepository.Get(orderId) ?? throw new EntityNotFoundException($"Order with Id: {orderId} not found");

        return new(order);
    }

    public async Task<IEnumerable<OrderGameDto>> GetOrderDetails(Guid orderId)
    {
        var orderGames = await _orderGameRepository.GetOrderGames(orderId);
        return orderGames.Select(x => new OrderGameDto(x));
    }

    public async Task<OrderInformation> GetOrderInformation(Guid customerId)
    {
        Order order = await GetOpenOrder(customerId);

        var orderGames = await _orderGameRepository.GetOrderGames(order.Id);

        double totalSum = 0;
        foreach (var orderGame in orderGames)
        {
            totalSum += orderGame.Price * orderGame.Quantity * ((100 - orderGame.Discount) / 100.0);
        }

        return new(order.Id, order.Date, (int)totalSum);
    }

    public async Task<IEnumerable<OrderDto>> GetPaidAndCancelledOrders()
    {
        var games = await _orderRepository.GetPaidAndCancelledOrders();
        return games.Select(x => new OrderDto(x));
    }

    public async Task<Guid> RemoveOrder(Guid customerId, string gameKey)
    {
        Game game = await _gameSearchCriteria.GetByKey(gameKey) ?? throw new EntityNotFoundException($"Couldn't find game by key: {gameKey}");

        Order order = await GetOpenOrder(customerId);

        var orderGames = await _orderGameRepository.GetOrderGames(order.Id);

        if (!orderGames.Any())
        {
            throw new EntityNotFoundException("Cart is empty");
        }

        OrderGame orderGame = orderGames.FirstOrDefault(x => x.ProductId == game.Id) ?? throw new EntityNotFoundException($"Cart does not contains game: {gameKey}");

        if (orderGame.Quantity > 1)
        {
            orderGame.Quantity -= 1;
            orderGame.ModificationDate = DateTime.Now;

            await _orderGameRepository.Delete(orderGame);
        }
        else
        {
            await _orderGameRepository.Delete(orderGame);
            if (orderGames.Count() == 1)
            {
                await _orderRepository.Delete(order);
            }
        }

        return order.Id;
    }

    public async Task<Guid> UpdateOrder(Guid orderId, OrderStatus orderStatus)
    {
        Order order = await _orderRepository.Get(orderId) ?? throw new EntityNotFoundException($"Couldn't find order by ID: {orderId}");

        if (orderStatus == OrderStatus.Paid)
        {
            var orderGames = await _orderGameRepository.GetOrderGames(orderId);

            foreach (OrderGame orderGame in orderGames)
            {
                Game game = await _gameRepository.Get(orderGame.ProductId);
                game.UnitInStock -= orderGame.Quantity;
                await _gameRepository.Update(game);
            }
        }

        order.Status = orderStatus;
        order.ModificationDate = DateTime.Now;

        await _orderRepository.Update(order);
        return order.Id;
    }

    private async Task<Order> GetOpenOrder(Guid customerId)
    {
        return await _orderRepository.GetCustomerOpenOrder(customerId) ?? throw new EntityNotFoundException($"Customer: {customerId} does not have a cart");
    }
}
