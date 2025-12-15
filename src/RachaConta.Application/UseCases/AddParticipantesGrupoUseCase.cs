using RachaConta.Application.DTOs.Request;
using RachaConta.Application.DTOs.Response;
using RachaConta.Application.Validators;
using RachaConta.Core.Entities;
using RachaConta.Core.Interfaces.Repositories;

namespace RachaConta.Application.UseCases;

public class AddParticipantesGrupoUseCase
{
    private readonly IGrupoRepository _grupoRepository;
    private readonly IParticipanteGrupoRepository _participanteRepository;
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly AddParticipantesGrupoValidator _validator;

    public AddParticipantesGrupoUseCase(
        IGrupoRepository grupoRepository,
        IParticipanteGrupoRepository participanteRepository,
        IFriendshipRepository friendshipRepository)
    {
        _grupoRepository = grupoRepository;
        _participanteRepository = participanteRepository;
        _friendshipRepository = friendshipRepository;
        _validator = new AddParticipantesGrupoValidator();
    }

    public async Task<List<ParticipanteGrupoResponse>> ExecuteAsync(AddParticipantesGrupoRequest request, Guid usuarioId)
    {
        _validator.Validate(request);

        var grupo = await _grupoRepository.GetByIdAsync(request.GrupoId);
        if (grupo == null || grupo.Deleted)
            throw new InvalidOperationException("Grupo não encontrado.");

        // Verificar se o usuário é admin do grupo
        var isAdmin = await _participanteRepository.IsAdmAsync(request.GrupoId, usuarioId);
        if (!isAdmin)
            throw new UnauthorizedAccessException("Apenas administradores podem adicionar participantes.");

        var participantesAdicionados = new List<ParticipanteGrupoResponse>();

        foreach (var participanteId in request.IdParticipantes)
        {
            // Verificar se já é participante
            var jaParticipa = await _participanteRepository.GetByGrupoAndUserAsync(request.GrupoId, participanteId);
            if (jaParticipa != null)
                continue;

            // Verificar se são amigos
            var isFriend = await _friendshipRepository.AreFriendsAsync(usuarioId, participanteId);
            if (!isFriend)
                throw new InvalidOperationException($"Usuário {participanteId} não é amigo.");

            var participante = new ParticipanteGrupo(request.GrupoId, participanteId, isAdm: false);
            var created = await _participanteRepository.CreateAsync(participante);

            participantesAdicionados.Add(new ParticipanteGrupoResponse
            {
                Id = created.Id,
                IdGrupo = created.IdGrupo,
                UserId = created.UserId,
                IsAdm = created.IsAdm,
                HasPendent = created.HasPendent,
                CreatedAt = created.CreatedAt
            });
        }

        return participantesAdicionados;
    }
}
