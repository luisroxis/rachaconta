using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RachaConta.Core.Entities;
using RachaConta.Core.Interfaces.Services;
using RachaConta.Infrastructure.Data;

namespace RachaConta.IntegrationTests.Helpers;

public class TestAuthHelper
{
    private readonly CustomWebApplicationFactory _factory;

    public TestAuthHelper(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public async Task<User> CreateTestUserAsync(
        string name = "Test User",
        string email = "test@example.com",
        string username = "testuser",
        string password = "Test@123")
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RachaContaDbContext>();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            UserName = username,
            Password = BCrypt.Net.BCrypt.HashPassword(password),
            TermOfUse = true,
            PrivacyPolicy = true,
            CreatedAt = DateTime.UtcNow
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return user;
    }

    public string GenerateJwtToken(Guid userId)
    {
        var configuration = _factory.Services.GetRequiredService<IConfiguration>();
        var jwtKey = configuration["Jwt:Key"]!;
        var jwtIssuer = configuration["Jwt:Issuer"]!;
        var jwtAudience = configuration["Jwt:Audience"]!;

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public void AddAuthorizationHeader(HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<(User user, string token)> CreateAuthenticatedUserAsync(
        string name = "Test User",
        string email = "test@example.com",
        string username = "testuser",
        string password = "Test@123")
    {
        var user = await CreateTestUserAsync(name, email, username, password);
        var token = GenerateJwtToken(user.Id);
        return (user, token);
    }
}
