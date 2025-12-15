using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using RachaConta.Core.Interfaces.Services;

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
            Body = GetEmailBody(CorpoEmailRecuperarSenha(userName, resetCode)),
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
            IsBodyHtml = true
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

    public string CorpoEmailConvite(string nomeAmigo, string nomeConvidante, string link)
    {
        string email = $"<span>Oi, {nomeAmigo},</span>" +
            $"<p>Estou te convidando para usar o Bora Rachar comigo, um app que ajuda a gente a dividir as contas de forma fácil e divertida!</p>" +
            $" <p>Com o app, a gente pode:" +
            $"Registrar todas as nossas despesas em conjunto, como restaurantes, viagens, compras e muito mais;<br>" +
            $"Dividir as contas de forma justa, levando em consideração o que cada um consumiu;<br>" +
            $"Receber notificações quando alguém registrar uma nova despesa;<br>" +
            $"Ver o histórico de todas as nossas divisões;<br>" +
            $"E muito mais!<br>" +
            $"Eu achei o app muito legal e acho que você também vai gostar!<br>" +
            $"Para se juntar a gente, basta baixar o app e usar o meu <a href='{link}'>link</a> de convite<br>" +
            $"Depois de se cadastrar, você pode criar um novo grupo e me convidar para participar. Assim, a gente já pode começar a dividir as contas das nossas próximas aventuras!<br>" +
            $"Se tiver qualquer dúvida, é só me chamar!<br>" +
            $"Abraço,<br>" +
        $"<span><strong>{nomeConvidante}</strong></span></p>";
        
        return email;
    }

    public string CorpoEmailRecuperarSenha(string userName, string resetCode)
    {
        string email = $"<div class='content'>" +
				$"<h2>Olá, {userName}!</h2>" +
				$"<p>Você solicitou a recuperação de senha da sua conta no RachaConta.</p>" +
				$"<p>Use o código abaixo para redefinir sua senha:</p>" +
				$"<div class='code-box'>" +
				$"{resetCode}" +
				$"</div>" +
				$"<p><strong>Este código expira em 1 hora e pode ser usado apenas uma vez.</strong></p>" +
				$"<p>Se você não solicitou esta recuperação, ignore este email.</p>" +
				$"</div>";

        return email;
    }

    public string GetEmailBody(string conteudo)
    {        
        string email = $"<!DOCTYPE html>" +
                        $"<html>" +
                        $"<head>" +
                        $"    <style>" +
                        $"        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}" +
                        $"        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}" +
                        $"        .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; }}" +
                        $"        .content {{ background-color: #f9f9f9; padding: 20px; }}" +
                        $"        .code-box {{ background-color: #fff; border: 2px solid #4CAF50; padding: 15px; text-align: center; font-size: 24px; font-weight: bold; letter-spacing: 2px; margin: 20px 0; }}" +
                        $"        .footer {{ text-align: center; padding: 20px; font-size: 12px; color: #666; }}" +
                        $"    </style>" +
                        $"</head>" +
                        $"<body>" +
                        $"    <div class='container'>" +
                        $"        <div class='header'>" +
                        $"            <h1>RachaConta</h1>" +
                        $"        </div>" +
                        $"        {conteudo} " +
                        $"        <div class='footer'>" +
                        $"            <p>© 2024 RachaConta. Todos os direitos reservados.</p>" +
                        $"        </div>" +
                        $"    </div>" +
                        $"</body>" +
                        $"</html>";

        return email;
    }
}
