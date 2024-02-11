﻿using GameStore.Application.Dtos;
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

    public Guid AddOrder(Guid customerId, string gameKey)
    {
        Game game = _gameSearchCriteria.GetByKey(gameKey) ?? throw new EntityNotFoundException($"Couldn't find game by key: {gameKey}");

        int gamesInStock = game.UnitInStock < 1 ? throw new InvalidOperationException("Game has no stock") : game.UnitInStock;

        Order order = _orderRepository.GetCustomerOpenOrder(customerId);

        if (order == null)
        {
            order = new(Guid.NewGuid(), customerId, OrderStatus.Open);

            OrderGame orderGame = new(Guid.NewGuid(), order.Id, game.Id, game.Price, 1, game.Discount);

            _orderRepository.AddOrder(order);

            _orderGameRepository.AddOrderGame(orderGame);
        }
        else
        {
            OrderGame orderGame = _orderGameRepository.GetOrderGame(order.Id, game.Id);

            if (orderGame == null)
            {
                orderGame = new(Guid.NewGuid(), order.Id, game.Id, game.Price, 1, game.Discount);
                _orderGameRepository.AddOrderGame(orderGame);
            }
            else
            {
                orderGame.Quantity += 1;
                orderGame.ModificationDate = DateTime.Now;

                if (gamesInStock < orderGame.Quantity)
                {
                    throw new InvalidOperationException("Couldn' add game to cart. Not enough games in stock");
                }

                _orderGameRepository.UpdateOrderGame(orderGame);
            }
        }

        return order.Id;
    }

    public List<OrderGameDto> GetCart(Guid customerId)
    {
        Order order = _orderRepository.GetCustomerOpenOrder(customerId);

        return order == null ? (List<OrderGameDto>)[] : _orderGameRepository.GetOrderGames(order.Id).Select(x => new OrderGameDto(x)).ToList();
    }

    public OrderDto GetOrder(Guid orderId)
    {
        Order order = _orderRepository.GetOrder(orderId) ?? throw new EntityNotFoundException($"Order with Id: {orderId} not found");

        return new(order);
    }

    public List<OrderGameDto> GetOrderDetails(Guid orderId)
    {
        return _orderGameRepository.GetOrderGames(orderId).Select(x => new OrderGameDto(x)).ToList();
    }

    public OrderInformation GetOrderInformation(Guid customerId)
    {
        Order order = GetOpenOrder(customerId);

        List<OrderGame> orderGames = _orderGameRepository.GetOrderGames(order.Id);

        double totalSum = 0;
        foreach (var orderGame in orderGames)
        {
            totalSum += orderGame.Price * orderGame.Quantity * ((100 - orderGame.Discount) / 100.0);
        }

        return new(order.Id, order.Date, (int)totalSum);
    }

    public List<OrderDto> GetPaidAndCancelledOrders()
    {
        return _orderRepository.GetPaidAndCancelledOrders().Select(x => new OrderDto(x)).ToList();
    }

    public Guid RemoveOrder(Guid customerId, string gameKey)
    {
        Game game = _gameSearchCriteria.GetByKey(gameKey) ?? throw new EntityNotFoundException($"Couldn't find game by key: {gameKey}");

        Order order = GetOpenOrder(customerId);

        List<OrderGame> orderGames = _orderGameRepository.GetOrderGames(order.Id);

        if (orderGames.Count == 0)
        {
            throw new EntityNotFoundException("Cart is empty");
        }

        OrderGame orderGame = orderGames.FirstOrDefault(x => x.ProductId == game.Id) ?? throw new EntityNotFoundException($"Cart does not contains game: {gameKey}");

        if (orderGame.Quantity > 1)
        {
            orderGame.Quantity -= 1;
            orderGame.ModificationDate = DateTime.Now;

            _orderGameRepository.UpdateOrderGame(orderGame);
        }
        else
        {
            _orderGameRepository.RemoveOrderGame(orderGame);
            if (orderGames.Count == 1)
            {
                _orderRepository.DeleteOrder(order);
            }
        }

        return order.Id;
    }

    public Guid UpdateOrder(Guid orderId, OrderStatus orderStatus)
    {
        Order order = _orderRepository.GetOrder(orderId) ?? throw new EntityNotFoundException($"Couldn't find order by ID: {orderId}");

        if (orderStatus == OrderStatus.Paid)
        {
            List<OrderGame> orderGames = _orderGameRepository.GetOrderGames(orderId);

            foreach (OrderGame orderGame in orderGames)
            {
                Game game = _gameRepository.GetGame(orderGame.ProductId);
                game.UnitInStock -= orderGame.Quantity;
                _gameRepository.UpdateGame(game);
            }
        }

        order.Status = orderStatus;
        order.ModificationDate = DateTime.Now;

        _orderRepository.UpdateOrder(order);
        return order.Id;
    }

    private Order GetOpenOrder(Guid customerId)
    {
        return _orderRepository.GetCustomerOpenOrder(customerId) ?? throw new EntityNotFoundException($"Customer: {customerId} does not have a cart");
    }
}
