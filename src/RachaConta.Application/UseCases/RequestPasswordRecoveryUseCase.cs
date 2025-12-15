using RachaConta.Application.DTOs;
using RachaConta.Core.Entities;
using RachaConta.Core.Interfaces.Repositories;
using RachaConta.Core.Interfaces.Services;

namespace RachaConta.Application.UseCases;

public class RequestPasswordRecoveryUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordResetTokenRepository _tokenRepository;
    private readonly IEmailService _emailService;

    public RequestPasswordRecoveryUseCase(
        IUserRepository userRepository,
        IPasswordResetTokenRepository tokenRepository,
        IEmailService emailService)
    {
        _userRepository = userRepository;
        _tokenRepository = tokenRepository;
        _emailService = emailService;
    }

    public async Task<RequestPasswordRecoveryResponse> ExecuteAsync(RequestPasswordRecoveryRequest request)
    {
        // Try to find user by email or username
        User? user = null;

        if (request.EmailOrUserName.Contains("@"))
        {
            user = await _userRepository.GetByEmailAsync(request.EmailOrUserName);
        }
        else
        {
            user = await _userRepository.GetByUserNameAsync(request.EmailOrUserName);
        }

        if (user == null)
        {
            throw new InvalidOperationException("Usuário não encontrado.");
        }

        // Generate unique reset token (6-digit code)
        var resetCode = GenerateResetCode();
        var expiresAt = DateTime.UtcNow.AddHours(1);

        var resetToken = new PasswordResetToken(
            userId: user.Id,
            token: resetCode,
            expiresAt: expiresAt
        );

        await _tokenRepository.CreateAsync(resetToken);

        // Send email with reset code
        await _emailService.SendPasswordResetEmailAsync(user.Email, user.Name, resetCode);

        return new RequestPasswordRecoveryResponse
        {
            Message = "Um código de recuperação foi enviado para o seu email."
        };
    }

    private string GenerateResetCode()
    {
        // Generate a 6-digit code
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }
}
