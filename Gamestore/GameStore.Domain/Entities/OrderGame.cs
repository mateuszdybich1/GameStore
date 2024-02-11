namespace GameStore.Domain.Entities;
public class OrderGame : Entity
{
    public OrderGame()
    {
    }

    public OrderGame(Guid id, Guid orderId, Guid productId, double price, int quantity, int discount)
        : base(id)
    {
        OrderId = orderId;
        ProductId = productId;
        Price = price;
        Quantity = quantity;
        Discount = discount;
    }

    public Guid OrderId { get; private set; }

    public Guid ProductId { get; private set; }

    public double Price { get; set; }

    public int Quantity { get; set; }

    public int Discount { get; set; }
}
