using RachaConta.Core.Entities;

namespace RachaConta.Core.Interfaces.Repositories;

public interface IParticipanteGrupoRepository
{
    Task<ParticipanteGrupo> CreateAsync(ParticipanteGrupo participante);
    Task<ParticipanteGrupo?> GetByIdAsync(Guid id);
    Task<List<ParticipanteGrupo>> GetByGrupoIdAsync(Guid grupoId);
    Task<ParticipanteGrupo?> GetByGrupoAndUserAsync(Guid grupoId, Guid userId);
    Task<List<ParticipanteGrupo>> GetByUserIdAsync(Guid userId);
    Task UpdateAsync(ParticipanteGrupo participante);
    Task DeleteAsync(Guid id);
    Task<bool> IsAdmAsync(Guid grupoId, Guid userId);
}
