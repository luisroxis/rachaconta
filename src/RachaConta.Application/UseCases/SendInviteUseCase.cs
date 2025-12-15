using RachaConta.Application.DTOs.Request;
using RachaConta.Application.DTOs.Response;
using RachaConta.Core.Entities;
using RachaConta.Core.Interfaces.Repositories;
using RachaConta.Core.Interfaces.Services;

namespace RachaConta.Application.UseCases;

public class SendInviteUseCase
{
    private readonly IInviteRepository _inviteRepository;
    private readonly IEmailService _emailService;
    private readonly IUserRepository _userRepository;

    public SendInviteUseCase(IInviteRepository inviteRepository,  IUserRepository userRepository, IEmailService emailService)
    {
        _inviteRepository = inviteRepository;
        _emailService = emailService;
        _userRepository = userRepository;
    }

    public async Task<InviteResponse> ExecuteAsync(SendInviteRequest request, Guid usuarioId, string userName)
    {
        // Validate request
        if (string.IsNullOrWhiteSpace(request.Nome))
        {
            throw new InvalidOperationException("Nome é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new InvalidOperationException("Email é obrigatório.");
        }
        
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user != null)
        {
            throw new InvalidOperationException("Email ja esta cadastrado no sistema.");
        }

        if (string.IsNullOrWhiteSpace(request.CorpoEmail))
        {
            string link = "https://drive.google.com/file/d/1On-upFKC1YLl8iSOBys9lFWY19BFahCi/view?usp=sharing";
           
            request.CorpoEmail = _emailService.CorpoEmailConvite(request.Nome, userName, link);
        }

         var email = _emailService.GetEmailBody(request.CorpoEmail);

        // Create invite
        var invite = new Invite(
            nome: request.Nome,
            email: request.Email,
            corpoEmail: email,
            amigoId: usuarioId.ToString()
        );       

        var createdInvite = await _inviteRepository.CreateAsync(invite);

       

        // Send email
        await _emailService.SendEmailAsync(
            toEmail: request.Email,
            subject: "Convite Amizade - RachaConta",
            body: email
        );

        // Return response
        return new InviteResponse
        {
            Id = createdInvite.Id,
            Nome = createdInvite.Nome,
            Email = createdInvite.Email,
            CorpoEmail = createdInvite.CorpoEmail,
            DataEnvio = createdInvite.DataEnvio,
            AmigoId = createdInvite.AmigoId,
            Reenvio = createdInvite.Reenvio,
            DataReenvio = createdInvite.DataReenvio,
            Aceite = createdInvite.Aceite
        };
    }
}
