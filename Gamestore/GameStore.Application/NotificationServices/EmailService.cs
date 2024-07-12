using System.Diagnostics;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using GameStore.Application.Dtos;
using GameStore.Application.IServices;
using Microsoft.Extensions.Options;

namespace GameStore.Application.NotificationServices;

public class EmailService : IEmailService, IDisposable
{
    private readonly ServiceBusClient _serviceBusClient;
    private readonly string _queueName;
    private readonly ServiceBusSender _serviceBusSender;
    private bool _disposed;

    public EmailService(IOptions<AzureServiceBusSettings> serviceBusSettings)
    {
        _serviceBusClient = new(serviceBusSettings.Value.ConnectionString);
        _queueName = serviceBusSettings.Value.QueueName;
        _serviceBusSender = _serviceBusClient.CreateSender(_queueName);
    }

    public async Task SendEmailRequest(EmailRequestDto emailRequest)
    {
        string messageBody = JsonSerializer.Serialize(emailRequest);

        ServiceBusMessage message = new(messageBody);

        try
        {
            await _serviceBusSender.SendMessageAsync(message);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _serviceBusSender?.DisposeAsync().AsTask().GetAwaiter().GetResult();
                _serviceBusClient?.DisposeAsync().AsTask().GetAwaiter().GetResult();
            }

            _disposed = true;
        }
    }
}
