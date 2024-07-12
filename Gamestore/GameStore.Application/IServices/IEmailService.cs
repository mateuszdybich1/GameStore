using GameStore.Application.Dtos;

namespace GameStore.Application.IServices;

public interface IEmailService
{
    Task SendEmailRequest(EmailRequestDto emailRequest);
}
