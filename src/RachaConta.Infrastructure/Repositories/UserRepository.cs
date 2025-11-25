using Microsoft.EntityFrameworkCore;
using RachaConta.Application.Interfaces;
using RachaConta.Core.Entities;
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
}
