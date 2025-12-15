using Microsoft.EntityFrameworkCore;
using RachaConta.Core.Entities;
using RachaConta.Core.Interfaces.Repositories;
using RachaConta.Infrastructure.Data;

namespace RachaConta.Infrastructure.Repositories;

public class PasswordResetTokenRepository : IPasswordResetTokenRepository
{
    private readonly RachaContaDbContext _context;

    public PasswordResetTokenRepository(RachaContaDbContext context)
    {
        _context = context;
    }

    public async Task<PasswordResetToken> CreateAsync(PasswordResetToken token)
    {
        await _context.PasswordResetTokens.AddAsync(token);
        await _context.SaveChangesAsync();
        return token;
    }

    public async Task<PasswordResetToken?> GetByTokenAsync(string token)
    {
        return await _context.PasswordResetTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == token);
    }

    public async Task MarkAsUsedAsync(Guid tokenId)
    {
        var token = await _context.PasswordResetTokens.FindAsync(tokenId);
        if (token != null)
        {
            token.IsUsed = true;
            token.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
