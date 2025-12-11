using RachaConta.Application.Interfaces;

namespace RachaConta.Application.UseCases;

public class DeleteInviteUseCase
{
    private readonly IInviteRepository _inviteRepository;

    public DeleteInviteUseCase(IInviteRepository inviteRepository)
    {
        _inviteRepository = inviteRepository;
    }

    public async Task ExecuteAsync(Guid inviteId, Guid usuarioId)
    {
        // Get invite
        var invite = await _inviteRepository.GetByIdAsync(inviteId);

        if (invite == null)
        {
            throw new InvalidOperationException("Convite não encontrado.");
        }

        // Verify ownership
        if (invite.UsuarioId != usuarioId)
        {
            throw new UnauthorizedAccessException("Você não tem permissão para deletar este convite.");
        }

        // Delete invite
        await _inviteRepository.DeleteAsync(inviteId);
    }
}
