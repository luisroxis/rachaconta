using RachaConta.Application.DTOs.Response;
using RachaConta.Core.Interfaces.Repositories;

namespace RachaConta.Application.UseCases;

public class GetGrupoDetailsUseCase
{
    private readonly IGrupoRepository _grupoRepository;
    private readonly IParticipanteGrupoRepository _participanteRepository;
    private readonly IUserRepository _userRepository;

    public GetGrupoDetailsUseCase(
        IGrupoRepository grupoRepository,
        IParticipanteGrupoRepository participanteRepository,
        IUserRepository userRepository)
    {
        _grupoRepository = grupoRepository;
        _participanteRepository = participanteRepository;
        _userRepository = userRepository;
    }

    public async Task<(GrupoResponse grupo, List<ParticipanteGrupoResponse> participantes)> ExecuteAsync(Guid grupoId, Guid usuarioId)
    {
        var grupo = await _grupoRepository.GetByIdAsync(grupoId);
        if (grupo == null || grupo.Deleted)
            throw new InvalidOperationException("Grupo não encontrado.");

        // Verificar se o usuário é participante do grupo
        var participacao = await _participanteRepository.GetByGrupoAndUserAsync(grupoId, usuarioId);
        if (participacao == null)
            throw new UnauthorizedAccessException("Você não é participante deste grupo.");

        var participantes = await _participanteRepository.GetByGrupoIdAsync(grupoId);
        var participantesResponse = new List<ParticipanteGrupoResponse>();

        foreach (var p in participantes)
        {
            var user = await _userRepository.GetByIdAsync(p.UserId);
            participantesResponse.Add(new ParticipanteGrupoResponse
            {
                Id = p.Id,
                IdGrupo = p.IdGrupo,
                UserId = p.UserId,
                UserName = user?.UserName,
                Email = user?.Email,
                IsAdm = p.IsAdm,
                HasPendent = p.HasPendent,
                CreatedAt = p.CreatedAt
            });
        }

        var grupoResponse = MapToResponse(grupo);
        return (grupoResponse, participantesResponse);
    }

    private GrupoResponse MapToResponse(Core.Entities.Grupo grupo)
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
