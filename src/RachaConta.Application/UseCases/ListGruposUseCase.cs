using RachaConta.Application.DTOs.Response;
using RachaConta.Core.Interfaces.Repositories;

namespace RachaConta.Application.UseCases;

public class ListGruposUseCase
{
    private readonly IGrupoRepository _grupoRepository;
    private readonly IParticipanteGrupoRepository _participanteRepository;

    public ListGruposUseCase(
        IGrupoRepository grupoRepository,
        IParticipanteGrupoRepository participanteRepository)
    {
        _grupoRepository = grupoRepository;
        _participanteRepository = participanteRepository;
    }

    public async Task<List<GrupoResponse>> ExecuteAsync(Guid usuarioId)
    {
        // Obter todos os grupos onde o usuário é participante
        var participacoes = await _participanteRepository.GetByUserIdAsync(usuarioId);
        
        var grupos = new List<GrupoResponse>();
        foreach (var participacao in participacoes)
        {
            var grupo = await _grupoRepository.GetByIdAsync(participacao.IdGrupo);
            if (grupo != null && !grupo.Deleted)
            {
                grupos.Add(MapToResponse(grupo));
            }
        }

        return grupos;
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
