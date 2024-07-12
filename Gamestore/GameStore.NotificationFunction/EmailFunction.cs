using System.Text.Json;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Azure.WebJobs;
using MimeKit;

namespace GameStore.NotificationFunction;

public class EmailFunction
{
    [FunctionName("SendEmail")]
    public static async Task Run([ServiceBusTrigger("emailqueue")] string myQueueItem)
    {
        var emailRequest = JsonSerializer.Deserialize<EmailRequestDto>(myQueueItem);
        if (emailRequest != null)
        {
            await SendEmailAsync(emailRequest);
        }
    }

    private static async Task SendEmailAsync(EmailRequestDto emailRequest)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("GameStoreAPP", "gamestore.dybich@gmail.com"));
        emailMessage.To.Add(new MailboxAddress(emailRequest.ToUsername, emailRequest.ToEmail));
        emailMessage.Subject = emailRequest.Subject;
        emailMessage.Body = new TextPart("plain") { Text = emailRequest.Body };

        using var client = new SmtpClient();
        await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync("gamestore.dybich@gmail.com", "connpass");
        await client.SendAsync(emailMessage);
        await client.DisconnectAsync(true);
    }
}
