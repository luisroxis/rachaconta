using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using RachaConta.Application.Interfaces;

namespace RachaConta.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendPasswordResetEmailAsync(string email, string userName, string resetCode)
    {
        var smtpServer = _configuration["EmailSettings:SmtpServer"];
        var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
        var senderEmail = _configuration["EmailSettings:SenderEmail"];
        var senderName = _configuration["EmailSettings:SenderName"];
        var username = _configuration["EmailSettings:Username"];
        var password = _configuration["EmailSettings:Password"];

        if (string.IsNullOrEmpty(smtpServer) || string.IsNullOrEmpty(senderEmail) || 
            string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            throw new InvalidOperationException("Configurações de email não estão completas no appsettings.json");
        }

        var mailMessage = new MailMessage
        {
            From = new MailAddress(senderEmail, senderName),
            Subject = "Recuperação de Senha - RachaConta",
            Body = GetEmailBody(userName, resetCode),
            IsBodyHtml = true
        };

        mailMessage.To.Add(email);

        using var smtpClient = new SmtpClient(smtpServer, smtpPort)
        {
            Credentials = new NetworkCredential(username, password),
            UseDefaultCredentials = false,
            EnableSsl = true
        };

        await smtpClient.SendMailAsync(mailMessage);
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var smtpServer = _configuration["EmailSettings:SmtpServer"];
        var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
        var senderEmail = _configuration["EmailSettings:SenderEmail"];
        var senderName = _configuration["EmailSettings:SenderName"];
        var username = _configuration["EmailSettings:Username"];
        var password = _configuration["EmailSettings:Password"];

        if (string.IsNullOrEmpty(smtpServer) || string.IsNullOrEmpty(senderEmail) || 
            string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            throw new InvalidOperationException("Configurações de email não estão completas no appsettings.json");
        }

        var mailMessage = new MailMessage
        {
            From = new MailAddress(senderEmail, senderName),
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };

        mailMessage.To.Add(toEmail);

        using var smtpClient = new SmtpClient(smtpServer, smtpPort)
        {
            Credentials = new NetworkCredential(username, password),
            UseDefaultCredentials = false,
            EnableSsl = true
        };

        await smtpClient.SendMailAsync(mailMessage);
    }

    private string GetEmailBody(string userName, string resetCode)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; }}
        .content {{ background-color: #f9f9f9; padding: 20px; }}
        .code-box {{ background-color: #fff; border: 2px solid #4CAF50; padding: 15px; text-align: center; font-size: 24px; font-weight: bold; letter-spacing: 2px; margin: 20px 0; }}
        .footer {{ text-align: center; padding: 20px; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>RachaConta</h1>
        </div>
        <div class='content'>
            <h2>Olá, {userName}!</h2>
            <p>Você solicitou a recuperação de senha da sua conta no RachaConta.</p>
            <p>Use o código abaixo para redefinir sua senha:</p>
            <div class='code-box'>
                {resetCode}
            </div>
            <p><strong>Este código expira em 1 hora e pode ser usado apenas uma vez.</strong></p>
            <p>Se você não solicitou esta recuperação, ignore este email.</p>
        </div>
        <div class='footer'>
            <p>© 2024 RachaConta. Todos os direitos reservados.</p>
        </div>
    </div>
</body>
</html>";
    }
}
