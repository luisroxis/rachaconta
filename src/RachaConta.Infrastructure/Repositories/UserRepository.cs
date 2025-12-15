using Microsoft.EntityFrameworkCore;
using RachaConta.Core.Entities;
using RachaConta.Core.Interfaces.Repositories;
using RachaConta.Infrastructure.Data;

namespace RachaConta.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly RachaContaDbContext _context;

    public UserRepository(RachaContaDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<bool> ExistsByUserNameAsync(string userName)
    {
        return await _context.Users.AnyAsync(u => u.UserName == userName);
    }

    public async Task<User> CreateAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByUserNameAsync(string userName)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task UpdatePasswordAsync(Guid userId, string newPasswordHash)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.Password = newPasswordHash;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<User>> ListAllExceptAsync(Guid userId)
    {
        return await _context.Users
            .Where(u => u.Id != userId)
            .ToListAsync();
    }
}
