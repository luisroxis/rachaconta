namespace RachaConta.Application.Interfaces;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(string email, string userName, string resetCode);
    Task SendEmailAsync(string toEmail, string subject, string body);
   
    string CorpoEmailConvite(string nomeAmigo, string nomeConvidante, string link);
    string CorpoEmailRecuperarSenha(string userName, string resetCode);
    string GetEmailBody(string conteudo);
}
