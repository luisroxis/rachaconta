using RachaConta.Core.Entities;

namespace RachaConta.Application.Interfaces;

public interface IInviteRepository
{
    Task<Invite> CreateAsync(Invite invite);
    Task<Invite?> GetByIdAsync(Guid id);
    Task<List<Invite>> GetByUserIdAsync(Guid userId);
    Task UpdateAsync(Invite invite);
    Task DeleteAsync(Guid id);
}
