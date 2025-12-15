using RachaConta.Application.DTOs.Request;
using RachaConta.Application.DTOs.Response;
using RachaConta.Application.Validators;
using RachaConta.Core.Interfaces.Repositories;

namespace RachaConta.Application.UseCases;

public class PromoteParticipanteUseCase
{
    private readonly IParticipanteGrupoRepository _participanteRepository;
    private readonly IGrupoRepository _grupoRepository;
    private readonly PromoteParticipanteValidator _validator;

    public PromoteParticipanteUseCase(
        IParticipanteGrupoRepository participanteRepository,
        IGrupoRepository grupoRepository)
    {
        _participanteRepository = participanteRepository;
        _grupoRepository = grupoRepository;
        _validator = new PromoteParticipanteValidator();
    }

    public async Task<ParticipanteGrupoResponse> ExecuteAsync(PromoteParticipanteRequest request, Guid usuarioId)
    {
        _validator.Validate(request);

        var grupo = await _grupoRepository.GetByIdAsync(request.GrupoId);
        if (grupo == null || grupo.Deleted)
            throw new InvalidOperationException("Grupo não encontrado.");

        // Verificar se o usuário é admin do grupo
        var isAdmin = await _participanteRepository.IsAdmAsync(request.GrupoId, usuarioId);
        if (!isAdmin)
            throw new UnauthorizedAccessException("Apenas administradores podem promover participantes.");

        var participante = await _participanteRepository.GetByIdAsync(request.ParticipanteId);
        if (participante == null || participante.IdGrupo != request.GrupoId)
            throw new InvalidOperationException("Participante não encontrado neste grupo.");

        participante.IsAdm = true;
        await _participanteRepository.UpdateAsync(participante);

        return new ParticipanteGrupoResponse
        {
            Id = participante.Id,
            IdGrupo = participante.IdGrupo,
            UserId = participante.UserId,
            IsAdm = participante.IsAdm,
            HasPendent = participante.HasPendent,
            CreatedAt = participante.CreatedAt
        };
    }
}
