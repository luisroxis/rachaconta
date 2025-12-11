using RachaConta.Core.Entities;

namespace RachaConta.Application.Interfaces;

public interface IPasswordResetTokenRepository
{
    Task<PasswordResetToken> CreateAsync(PasswordResetToken token);
    Task<PasswordResetToken?> GetByTokenAsync(string token);
    Task MarkAsUsedAsync(Guid tokenId);
}
