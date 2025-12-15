using RachaConta.Application.DTOs.Response;
using RachaConta.Core.Interfaces.Repositories;
using RachaConta.Core.Interfaces.Services;

namespace RachaConta.Application.UseCases;

public class ResendInviteUseCase
{
    private readonly IInviteRepository _inviteRepository;
    private readonly IEmailService _emailService;

    public ResendInviteUseCase(IInviteRepository inviteRepository, IEmailService emailService)
    {
        _inviteRepository = inviteRepository;
        _emailService = emailService;
    }

    public async Task<InviteResponse> ExecuteAsync(Guid inviteId, Guid usuarioId)
    {
        // Get invite
        var invite = await _inviteRepository.GetByIdAsync(inviteId);

        if (invite == null)
        {
            throw new InvalidOperationException("Convite não encontrado.");
        }

        if (invite.AmigoId != usuarioId.ToString())
        {
            throw new UnauthorizedAccessException("Você não tem permissão para reenviar este convite.");
        }

        // Check if already resent
        if (invite.Reenvio)
        {
            throw new InvalidOperationException("Convite já foi reenviado anteriormente.");
        }

        // Update invite
        invite.Reenvio = true;
        invite.DataReenvio = DateTime.UtcNow;
        await _inviteRepository.UpdateAsync(invite);

        // Resend email
        await _emailService.SendEmailAsync(
            toEmail: invite.Email,
            subject: "Convite - RachaConta (Reenvio)",
            body: invite.CorpoEmail
        );

        // Return response
        return new InviteResponse
        {
            Id = invite.Id,
            Nome = invite.Nome,
            Email = invite.Email,
            CorpoEmail = invite.CorpoEmail,
            DataEnvio = invite.DataEnvio,
            AmigoId = invite.AmigoId,
            Reenvio = invite.Reenvio,
            DataReenvio = invite.DataReenvio,
            Aceite = invite.Aceite
        };
    }
}
