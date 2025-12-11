using RachaConta.Application.DTOs.Request;
using RachaConta.Application.DTOs.Response;
using RachaConta.Application.Interfaces;
using RachaConta.Core.Entities;
using RachaConta.Core.Interfaces.Services;

namespace RachaConta.Application.UseCases;

public class SendInviteUseCase
{
    private readonly IInviteRepository _inviteRepository;
    private readonly IEmailService _emailService;

    public SendInviteUseCase(IInviteRepository inviteRepository, IEmailService emailService)
    {
        _inviteRepository = inviteRepository;
        _emailService = emailService;
    }

    public async Task<InviteResponse> ExecuteAsync(SendInviteRequest request, Guid usuarioId)
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

        if (string.IsNullOrWhiteSpace(request.CorpoEmail))
        {
            throw new InvalidOperationException("Corpo do email é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(request.AmigoId))
        {
            throw new InvalidOperationException("AmigoId é obrigatório.");
        }

        // Create invite
        var invite = new Invite(
            nome: request.Nome,
            email: request.Email,
            corpoEmail: request.CorpoEmail,
            amigoId: request.AmigoId,
            usuarioId: usuarioId
        );

        var createdInvite = await _inviteRepository.CreateAsync(invite);

        // Send email
        await _emailService.SendEmailAsync(
            toEmail: request.Email,
            subject: "Convite - RachaConta",
            body: request.CorpoEmail
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
