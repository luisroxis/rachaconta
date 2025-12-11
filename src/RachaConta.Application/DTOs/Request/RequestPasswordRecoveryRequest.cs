namespace RachaConta.Application.DTOs;

public class RequestPasswordRecoveryRequest
{
    public string EmailOrUserName { get; set; } = string.Empty;
}
