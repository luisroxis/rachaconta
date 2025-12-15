using RachaConta.Core.Entities;

namespace RachaConta.Core.Interfaces.Repositories;

public interface IPasswordResetTokenRepository
{
    Task<PasswordResetToken> CreateAsync(PasswordResetToken token);
    Task<PasswordResetToken?> GetByTokenAsync(string token);
    Task MarkAsUsedAsync(Guid tokenId);
}
