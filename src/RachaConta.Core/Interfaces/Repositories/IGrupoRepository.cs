using RachaConta.Core.Entities;

namespace RachaConta.Core.Interfaces.Repositories;

public interface IGrupoRepository
{
    Task<Grupo> CreateAsync(Grupo grupo);
    Task<Grupo?> GetByIdAsync(Guid id);
    Task<List<Grupo>> GetByUserIdAsync(Guid userId);
    Task<List<Grupo>> GetAllAsync();
    Task UpdateAsync(Grupo grupo);
    Task DeleteAsync(Guid id);
}
