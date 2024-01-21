namespace GameStore.Application.Dtos;

public class OrderInformation(Guid orderId, DateTime creationDate, double sum)
{
    public Guid OrderId { get; set; } = orderId;

    public DateTime CreationDate { get; set; } = creationDate;

    public double Sum { get; set; } = sum;
}
