using Microsoft.EntityFrameworkCore;
using RachaConta.Core.Entities;
using RachaConta.Core.Interfaces.Repositories;
using RachaConta.Infrastructure.Data;

namespace RachaConta.Infrastructure.Repositories;

public class FriendshipRepository : IFriendshipRepository
{
    private readonly RachaContaDbContext _context;

    public FriendshipRepository(RachaContaDbContext context)
    {
        _context = context;
    }

    public async Task<Friendship> AddAsync(Friendship friendship)
    {
        await _context.Friendships.AddAsync(friendship);
        await _context.SaveChangesAsync();
        return friendship;
    }

    public async Task UpdateAsync(Friendship friendship)
    {
        _context.Friendships.Update(friendship);
        await _context.SaveChangesAsync();
    }

    public async Task<Friendship?> GetByIdAsync(Guid id)
    {
        return await _context.Friendships
            .Include(f => f.User)
            .Include(f => f.Amigo)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<Friendship?> GetByUsersAsync(Guid user1Id, Guid user2Id)
    {
        // Check for friendship in either direction
        return await _context.Friendships
            .FirstOrDefaultAsync(f => 
                (f.UserId == user1Id && f.AmigoId == user2Id) ||
                (f.UserId == user2Id && f.AmigoId == user1Id));
    }

    public async Task<List<Friendship>> ListPendingReceivedAsync(Guid userId)
    {
        // Friendships where I am the Amigo (receiver) and NOT approved
        return await _context.Friendships
            .Include(f => f.User) // Sender
            .Where(f => f.AmigoId == userId && !f.Approved)
            .ToListAsync();
    }

    public async Task<List<Friendship>> ListPendingSentAsync(Guid userId)
    {
        // Friendships where I am the User (sender) and NOT approved
        return await _context.Friendships
            .Include(f => f.Amigo) // Receiver
            .Where(f => f.UserId == userId && !f.Approved)
            .ToListAsync();
    }

    public async Task<List<Friendship>> ListAcceptedAsync(Guid userId)
    {
        // Friendships involving user (either side) and APPROVED
        return await _context.Friendships
            .Include(f => f.User)
            .Include(f => f.Amigo)
            .Where(f => (f.UserId == userId || f.AmigoId == userId) && f.Approved)
            .ToListAsync();
    }

    public async Task DeleteAsync(Friendship friendship)
    {
        _context.Friendships.Remove(friendship);
        await _context.SaveChangesAsync();
    }
}
