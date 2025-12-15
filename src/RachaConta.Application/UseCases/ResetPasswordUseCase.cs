using RachaConta.Application.DTOs;
using RachaConta.Core.Interfaces.Repositories;

namespace RachaConta.Application.UseCases;

public class ResetPasswordUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordResetTokenRepository _tokenRepository;

    public ResetPasswordUseCase(
        IUserRepository userRepository,
        IPasswordResetTokenRepository tokenRepository)
    {
        _userRepository = userRepository;
        _tokenRepository = tokenRepository;
    }

    public async Task<ResetPasswordResponse> ExecuteAsync(ResetPasswordRequest request)
    {
        // Validate password confirmation
        if (request.NewPassword != request.ConfirmPassword)
        {
            throw new InvalidOperationException("A nova senha e a confirmação não coincidem.");
        }

        // Validate password strength (basic validation)
        if (request.NewPassword.Length < 6)
        {
            throw new InvalidOperationException("A senha deve ter pelo menos 6 caracteres.");
        }

        // Find user by email
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
        {
            throw new InvalidOperationException("Usuário não encontrado.");
        }

        // Find and validate reset token
        var resetToken = await _tokenRepository.GetByTokenAsync(request.ResetCode);
        if (resetToken == null)
        {
            throw new InvalidOperationException("Código de recuperação inválido.");
        }

        // Validate token belongs to the user
        if (resetToken.UserId != user.Id)
        {
            throw new InvalidOperationException("Código de recuperação inválido para este usuário.");
        }

        // Validate token is not expired and not used
        if (!resetToken.IsValid())
        {
            if (resetToken.IsUsed)
            {
                throw new InvalidOperationException("Este código de recuperação já foi utilizado.");
            }
            else
            {
                throw new InvalidOperationException("Este código de recuperação expirou.");
            }
        }

        // Hash the new password
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

        // Update user password
        await _userRepository.UpdatePasswordAsync(user.Id, hashedPassword);

        // Mark token as used
        await _tokenRepository.MarkAsUsedAsync(resetToken.Id);

        return new ResetPasswordResponse
        {
            Message = "Senha redefinida com sucesso."
        };
    }
}
