using RachaConta.Application.DTOs.Request;
using RachaConta.Application.DTOs.Response;
using RachaConta.Core.Interfaces.Repositories;
using RachaConta.Core.Interfaces.Services;

namespace RachaConta.Application.UseCases;

public class LoginUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IEncryptionService _encryptionService;

    public LoginUseCase(
        IUserRepository userRepository,
        IJwtService jwtService,
        IEncryptionService encryptionService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _encryptionService = encryptionService;
    }

    public async Task<LoginResponse> ExecuteAsync(LoginRequest request)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(request.EmailOrUsername))
        {
            throw new InvalidOperationException("Email ou nome de usuário é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new InvalidOperationException("Senha é obrigatória.");
        }

        // Try to find user by email or username
        var user = await _userRepository.GetByEmailAsync(request.EmailOrUsername);
        
        if (user == null)
        {
            user = await _userRepository.GetByUserNameAsync(request.EmailOrUsername);
        }

        // Verify user exists
        if (user == null)
        {
            throw new UnauthorizedAccessException("Credenciais inválidas.");
        }

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        {
            throw new UnauthorizedAccessException("Credenciais inválidas.");
        }

        // Generate JWT token
        var token = _jwtService.GenerateToken(user);

        // Encrypt userId
        var encryptedUserId = _encryptionService.Encrypt(user.Id.ToString());

        // Return response
        return new LoginResponse
        {
            Token = token,
            ExpiresIn = 3600,
            Name = user.Name,
            Email = user.Email,
            Username = user.UserName,
            Avatar = user.Avatar,
            Cod = encryptedUserId
        };
    }
}
