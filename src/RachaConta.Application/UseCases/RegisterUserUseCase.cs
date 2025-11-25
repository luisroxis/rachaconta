using RachaConta.Application.DTOs;
using RachaConta.Application.Interfaces;
using RachaConta.Core.Entities;
using BCrypt.Net;

namespace RachaConta.Application.UseCases;

public class RegisterUserUseCase
{
    private readonly IUserRepository _userRepository;

    public RegisterUserUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<RegisterUserResponse> ExecuteAsync(RegisterUserRequest request)
    {
        // Validate TermOfUse and PrivacyPolicy
        if (!request.TermOfUse)
        {
            throw new InvalidOperationException("Você deve aceitar os Termos de Uso.");
        }

        if (!request.PrivacyPolicy)
        {
            throw new InvalidOperationException("Você deve aceitar a Política de Privacidade.");
        }

        // Check email uniqueness
        if (await _userRepository.ExistsByEmailAsync(request.Email))
        {
            throw new InvalidOperationException("Este email já está em uso.");
        }

        // Check username uniqueness
        if (await _userRepository.ExistsByUserNameAsync(request.UserName))
        {
            throw new InvalidOperationException("Este nome de usuário já está em uso.");
        }

        // Hash password
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // Create user
        var user = new User(
            name: request.Name,
            userName: request.UserName,
            email: request.Email,
            password: hashedPassword,
            termOfUse: request.TermOfUse,
            privacyPolicy: request.PrivacyPolicy
        );

        var createdUser = await _userRepository.CreateAsync(user);

        // Return response
        return new RegisterUserResponse
        {
            Id = createdUser.Id,
            Name = createdUser.Name,
            Email = createdUser.Email,
            UserName = createdUser.UserName,
            CreatedAt = createdUser.CreatedAt
        };
    }
}
