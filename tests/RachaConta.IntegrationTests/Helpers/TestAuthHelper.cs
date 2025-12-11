using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RachaConta.Application.DTOs.Request;
using RachaConta.Application.DTOs.Response;
using RachaConta.Core.Entities;
using RachaConta.Core.Interfaces.Services;
using RachaConta.Infrastructure.Data;

namespace RachaConta.IntegrationTests.Helpers;

public class TestAuthHelper
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public TestAuthHelper(CustomWebApplicationFactory factory, HttpClient client)
    {
        _factory = factory;
        _client = client;
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
        
        var loginRequest = new LoginRequest 
        { 
            EmailOrUsername = email, 
            Password = password 
        };

        var response = await _client.PostAsJsonAsync("/api/user/login", loginRequest);
        response.EnsureSuccessStatusCode();

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
        return (user, loginResponse!.Token);
    }
}
