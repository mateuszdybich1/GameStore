namespace GameStore.Domain.Entities;

public enum OrderStatus
{
    Cancelled,
    Open,
    Checkout,
    Paid,
}

public class Order
{
    public Order()
    {
    }

    public Order(Guid id, Guid custoimerId, OrderStatus status)
    {
        Id = id;
        CustomerId = custoimerId;
        Status = status;
        Date = DateTime.Now;
    }

    public Guid Id { get; private set; }

    public DateTime Date { get; private set; }

    public Guid CustomerId { get; private set; }

    public OrderStatus Status { get; set; }
}
