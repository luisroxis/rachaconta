using RachaConta.Core.Interfaces.Repositories;

namespace RachaConta.Application.UseCases;

public class DeleteParticipanteGrupoUseCase
{
    private readonly IParticipanteGrupoRepository _participanteRepository;
    private readonly IGrupoRepository _grupoRepository;

    public DeleteParticipanteGrupoUseCase(
        IParticipanteGrupoRepository participanteRepository,
        IGrupoRepository grupoRepository)
    {
        _participanteRepository = participanteRepository;
        _grupoRepository = grupoRepository;
    }

    public async Task ExecuteAsync(Guid participanteId, Guid usuarioId)
    {
        var participante = await _participanteRepository.GetByIdAsync(participanteId);
        if (participante == null)
            throw new InvalidOperationException("Participante não encontrado.");

        var grupo = await _grupoRepository.GetByIdAsync(participante.IdGrupo);
        if (grupo == null || grupo.Deleted)
            throw new InvalidOperationException("Grupo não encontrado.");

        // Verificar se o usuário é admin do grupo ou é o próprio participante
        var isAdmin = await _participanteRepository.IsAdmAsync(participante.IdGrupo, usuarioId);
        if (!isAdmin && participante.UserId != usuarioId)
            throw new UnauthorizedAccessException("Você não tem permissão para remover este participante.");

        await _participanteRepository.DeleteAsync(participanteId);
    }
}
