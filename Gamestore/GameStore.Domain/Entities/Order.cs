namespace GameStore.Domain.Entities;

public enum OrderStatus
{
    Cancelled,
    Open,
    Checkout,
    Paid,
}

public class Order : Entity
{
    public Order()
    {
    }

    public Order(Guid id, Guid custoimerId, OrderStatus status)
        : base(id)
    {
        CustomerId = custoimerId;
        Status = status;
    }

    public Order(Guid id, Guid custoimerId, DateTime creationDate, OrderStatus status)
        : base(id, creationDate)
    {
        CustomerId = custoimerId;
        Status = status;
    }

    public Guid CustomerId { get; private set; }

    public OrderStatus Status { get; set; }
}
