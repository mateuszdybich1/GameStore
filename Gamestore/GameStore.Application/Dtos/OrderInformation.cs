namespace GameStore.Application.Dtos;

public class OrderInformation(Guid orderId, DateTime creationDate, int sum)
{
    public Guid OrderId { get; set; } = orderId;

    public DateTime CreationDate { get; set; } = creationDate;

    public int Sum { get; set; } = sum;
}
