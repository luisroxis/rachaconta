using RachaConta.Application.DTOs.Response;
using RachaConta.Application.Interfaces;

namespace RachaConta.Application.UseCases;

public class ListInvitesUseCase
{
    private readonly IInviteRepository _inviteRepository;

    public ListInvitesUseCase(IInviteRepository inviteRepository)
    {
        _inviteRepository = inviteRepository;
    }

    public async Task<List<InviteResponse>> ExecuteAsync(Guid usuarioId)
    {
        // Get all invites for user
        var invites = await _inviteRepository.GetByUserIdAsync(usuarioId);

        // Map to response
        return invites.Select(invite => new InviteResponse
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
        }).ToList();
    }
}
