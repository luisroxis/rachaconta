using RachaConta.Core.Entities;

namespace RachaConta.Application.Interfaces;

public interface IUserRepository
{
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> ExistsByUserNameAsync(string userName);
    Task<User> CreateAsync(User user);
}
