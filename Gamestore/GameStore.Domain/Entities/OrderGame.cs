namespace GameStore.Domain.Entities;
public class OrderGame : Entity
{
    public OrderGame()
    {
    }

    public OrderGame(Guid id, Guid orderId, Guid productId, double price, int quantity, double discount)
        : base(id)
    {
        OrderId = orderId;
        ProductId = productId;
        Price = price;
        Quantity = quantity;
        Discount = discount;
    }

    public OrderGame(OrderGame orderGame)
        : base(orderGame.Id, orderGame.CreationDate, orderGame.ModificationDate)
    {
        OrderId = orderGame.OrderId;
        ProductId = orderGame.ProductId;
        Price = orderGame.Price;
        Quantity = orderGame.Quantity;
        Discount = orderGame.Discount;
    }

    public Guid OrderId { get; private set; }

    public Guid ProductId { get; private set; }

    public double Price { get; set; }

    public int Quantity { get; set; }

    public double Discount { get; set; }
}
