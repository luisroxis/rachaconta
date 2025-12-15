using RachaConta.Application.DTOs.Request;
using RachaConta.Application.DTOs.Response;
using RachaConta.Application.Validators;
using RachaConta.Core.Entities;
using RachaConta.Core.Interfaces.Repositories;

namespace RachaConta.Application.UseCases;

public class CreateGrupoUseCase
{
    private readonly IGrupoRepository _grupoRepository;
    private readonly IParticipanteGrupoRepository _participanteRepository;
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly CreateGrupoValidator _validator;

    public CreateGrupoUseCase(
        IGrupoRepository grupoRepository,
        IParticipanteGrupoRepository participanteRepository,
        IFriendshipRepository friendshipRepository)
    {
        _grupoRepository = grupoRepository;
        _participanteRepository = participanteRepository;
        _friendshipRepository = friendshipRepository;
        _validator = new CreateGrupoValidator();
    }

    public async Task<GrupoResponse> ExecuteAsync(CreateGrupoRequest request, Guid usuarioId, string? imgGrupoPath = null)
    {
        _validator.Validate(request);

        var grupo = new Grupo(request.IdCategoria, request.Nome, request.Descricao ?? "")
        {
            ImgGrupo = imgGrupoPath
        };

        var createdGrupo = await _grupoRepository.CreateAsync(grupo);

        // Adicionar o criador como participante e admin
        var participanteCriador = new ParticipanteGrupo(createdGrupo.Id, usuarioId, isAdm: true);
        await _participanteRepository.CreateAsync(participanteCriador);

        // Adicionar outros participantes se fornecidos
        if (request.Participantes != null && request.Participantes.Count > 0)
        {
            foreach (var participanteId in request.Participantes)
            {
                // Verificar se são amigos
                var isFriend = await _friendshipRepository.AreFriendsAsync(usuarioId, participanteId);
                if (!isFriend)
                    throw new InvalidOperationException($"Usuário {participanteId} não é amigo.");

                var participante = new ParticipanteGrupo(createdGrupo.Id, participanteId, isAdm: false);
                await _participanteRepository.CreateAsync(participante);
            }
        }

        return MapToResponse(createdGrupo);
    }

    private GrupoResponse MapToResponse(Grupo grupo)
    {
        return new GrupoResponse
        {
            Id = grupo.Id,
            IdCategoria = grupo.IdCategoria,
            Nome = grupo.Nome,
            Descricao = grupo.Descricao,
            OutrasCategorias = grupo.OutrasCategorias,
            LinkConvite = grupo.LinkConvite,
            Ativo = grupo.Ativo,
            Deleted = grupo.Deleted,
            ImgGrupo = grupo.ImgGrupo,
            CreatedAt = grupo.CreatedAt,
            UpdatedAt = grupo.UpdatedAt
        };
    }
}
