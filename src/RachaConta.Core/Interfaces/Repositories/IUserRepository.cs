using RachaConta.Core.Entities;

namespace RachaConta.Application.Interfaces;

public interface IUserRepository
{
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> ExistsByUserNameAsync(string userName);
    Task<User> CreateAsync(User user);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUserNameAsync(string userName);
    Task<User?> GetByIdAsync(Guid id);
    Task UpdatePasswordAsync(Guid userId, string newPasswordHash);
    Task<List<User>> ListAllExceptAsync(Guid userId);
}
