namespace GameStore.Application.IServices;

public interface INotificationService
{
    Task NotifyOrderStatusChange(Guid orderId, string status);
}
