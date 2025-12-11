namespace RachaConta.Application.Interfaces;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(string email, string userName, string resetCode);
    Task SendEmailAsync(string toEmail, string subject, string body);
}
