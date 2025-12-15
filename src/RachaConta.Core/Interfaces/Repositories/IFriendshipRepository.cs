using RachaConta.Core.Entities;

namespace RachaConta.Core.Interfaces.Repositories;

public interface IFriendshipRepository
{
    Task<Friendship> AddAsync(Friendship friendship);
    Task UpdateAsync(Friendship friendship);
    Task<Friendship?> GetByIdAsync(Guid id);
    Task<Friendship?> GetByUsersAsync(Guid user1Id, Guid user2Id);
    Task<List<Friendship>> ListPendingReceivedAsync(Guid userId);
    Task<List<Friendship>> ListPendingSentAsync(Guid userId);
    Task<List<Friendship>> ListAcceptedAsync(Guid userId);
    Task DeleteAsync(Friendship friendship);
}
