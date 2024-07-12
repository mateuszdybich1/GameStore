namespace GameStore.Application.NotificationServices;

public class AzureServiceBusSettings
{
    public string ConnectionString { get; set; }

    public string QueueName { get; set; }
}
