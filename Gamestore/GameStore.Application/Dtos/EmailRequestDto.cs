namespace GameStore.Application.Dtos;

public class EmailRequestDto
{
    public EmailRequestDto()
    {
    }

    public EmailRequestDto(string toEmail, string toUsername, string subject, string body)
    {
        ToEmail = toEmail;
        ToUsername = toUsername;
        Subject = subject;
        Body = body;
    }

    public string ToEmail { get; set; }

    public string ToUsername { get; set; }

    public string Subject { get; set; }

    public string Body { get; set; }
}
