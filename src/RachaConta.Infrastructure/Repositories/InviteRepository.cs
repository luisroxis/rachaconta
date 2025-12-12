using Microsoft.EntityFrameworkCore;
using RachaConta.Application.Interfaces;
using RachaConta.Core.Entities;
using RachaConta.Infrastructure.Data;

namespace RachaConta.Infrastructure.Repositories;

public class InviteRepository : IInviteRepository
{
    private readonly RachaContaDbContext _context;

    public InviteRepository(RachaContaDbContext context)
    {
        _context = context;
    }

    public async Task<Invite> CreateAsync(Invite invite)
    {
        await _context.Invites.AddAsync(invite);
        await _context.SaveChangesAsync();
        return invite;
    }

    public async Task<Invite?> GetByIdAsync(Guid id)
    {
        return await _context.Invites
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<List<Invite>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Invites
            .Where(i => i.AmigoId == userId.ToString())
            .OrderByDescending(i => i.DataEnvio)
            .ToListAsync();
    }

    public async Task UpdateAsync(Invite invite)
    {
        invite.UpdatedAt = DateTime.UtcNow;
        _context.Invites.Update(invite);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var invite = await _context.Invites.FindAsync(id);
        if (invite != null)
        {
            _context.Invites.Remove(invite);
            await _context.SaveChangesAsync();
        }
    }
}
