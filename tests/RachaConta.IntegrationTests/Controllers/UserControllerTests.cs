

using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RachaConta.Application.DTOs;
using RachaConta.Application.DTOs.Request;
using RachaConta.Application.DTOs.Response;
using RachaConta.Core.Entities;
using RachaConta.IntegrationTests.Data;
using RachaConta.IntegrationTests.Helpers;

namespace RachaConta.IntegrationTests.Controllers;

public class UserControllerTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly TestAuthHelper _authHelper;

    public UserControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _authHelper = new TestAuthHelper(factory, _client);
        _factory.ResetDatabase();
    }

    public void Dispose()
    {
        _client?.Dispose();
    }

    #region Login Tests

    [Fact]
    public async Task Login_WithValidEmail_ReturnsOkWithToken()
    {
        // Arrange
        await _authHelper.CreateTestUserAsync(
            name: "John Doe",
            email: "john@example.com",
            username: "johndoe",
            password: "Password@123");

        var request = new LoginRequest
        {
            EmailOrUsername = "john@example.com",
            Password = "Password@123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/user/login", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(loginResponse);
        Assert.NotEmpty(loginResponse.Token);
    }

    [Fact]
    public async Task Login_WithValidUsername_ReturnsOkWithToken()
    {
        // Arrange
        await _authHelper.CreateTestUserAsync(
            name: "Jane Doe",
            email: "jane@example.com",
            username: "janedoe",
            password: "Password@123");

        var request = new LoginRequest
        {
            EmailOrUsername = "janedoe",
            Password = "Password@123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/user/login", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(loginResponse);
        Assert.NotEmpty(loginResponse.Token);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        await _authHelper.CreateTestUserAsync(
            email: "user@example.com",
            username: "testuser",
            password: "Password@123");

        var request = new LoginRequest
        {
            EmailOrUsername = "user@example.com",
            Password = "WrongPassword"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/user/login", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithNonExistentUser_ReturnsUnauthorized()
    {
        // Arrange
        var request = new LoginRequest
        {
            EmailOrUsername = "nonexistent@example.com",
            Password = "Password@123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/user/login", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Register Tests

    [Fact]
    public async Task Register_WithValidData_ReturnsCreated()
    {
        // Arrange
        var request = new RegisterUserRequest
        {
            Name = "New User",
            Email = "newuser@example.com",
            UserName = "newuser",
            Password = "Password@123",
            TermOfUse = true,
            PrivacyPolicy = true
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/user/register", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var registerResponse = await response.Content.ReadFromJsonAsync<RegisterUserResponse>();
        Assert.NotNull(registerResponse);
        Assert.Equal("newuser", registerResponse.UserName);
        Assert.Equal("newuser@example.com", registerResponse.Email);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ReturnsBadRequest()
    {
        // Arrange
        await _authHelper.CreateTestUserAsync(
            email: "duplicate@example.com",
            username: "user1");

        var request = new RegisterUserRequest
        {
            Name = "Another User",
            Email = "duplicate@example.com",
            UserName = "user2",
            Password = "Password@123",
            TermOfUse = true,
            PrivacyPolicy = true
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/user/register", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithDuplicateUsername_ReturnsBadRequest()
    {
        // Arrange
        await _authHelper.CreateTestUserAsync(
            email: "user1@example.com",
            username: "duplicateuser");

        var request = new RegisterUserRequest
        {
            Name = "Another User",
            Email = "user2@example.com",
            UserName = "duplicateuser",
            Password = "Password@123",
            TermOfUse = true,
            PrivacyPolicy = true
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/user/register", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithoutTermsAcceptance_ReturnsBadRequest()
    {
        // Arrange
        var request = new RegisterUserRequest
        {
            Name = "New User",
            Email = "newuser@example.com",
            UserName = "newuser",
            Password = "Password@123",
            TermOfUse = false,
            PrivacyPolicy = true
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/user/register", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithoutPrivacyPolicyAcceptance_ReturnsBadRequest()
    {
        // Arrange
        var request = new RegisterUserRequest
        {
            Name = "New User",
            Email = "newuser@example.com",
            UserName = "newuser",
            Password = "Password@123",
            TermOfUse = true,
            PrivacyPolicy = false
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/user/register", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region Forgot Password Tests

    [Fact]
    public async Task ForgotPassword_WithValidEmail_ReturnsOk()
    {
        // Arrange
        await _authHelper.CreateTestUserAsync(
            email: "forgot@example.com",
            username: "forgotuser");

        var request = new RequestPasswordRecoveryRequest
        {
            EmailOrUserName = "forgot@example.com"
        };

        // Setup email service mock
        _factory.EmailServiceMock
            .Setup(x => x.SendPasswordResetEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var response = await _client.PostAsJsonAsync("/api/user/forgot-password", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var recoveryResponse = await response.Content.ReadFromJsonAsync<RequestPasswordRecoveryResponse>();
        Assert.NotNull(recoveryResponse);
        Assert.Contains("código de recuperação foi enviado", recoveryResponse.Message);

        // Verify email was sent
        _factory.EmailServiceMock.Verify(
            x => x.SendPasswordResetEmailAsync(
                "forgot@example.com",
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task ForgotPassword_WithValidUsername_ReturnsOk()
    {
        // Arrange
        await _authHelper.CreateTestUserAsync(
            email: "forgot2@example.com",
            username: "forgotuser2");

        var request = new RequestPasswordRecoveryRequest
        {
            EmailOrUserName = "forgotuser2"
        };

        // Setup email service mock
        _factory.EmailServiceMock
            .Setup(x => x.SendPasswordResetEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var response = await _client.PostAsJsonAsync("/api/user/forgot-password", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ForgotPassword_WithNonExistentUser_ReturnsBadRequest()
    {
        // Arrange
        var request = new RequestPasswordRecoveryRequest
        {
            EmailOrUserName = "nonexistent@example.com"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/user/forgot-password", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region Reset Password Tests

    [Fact]
    public async Task ResetPassword_WithValidToken_ReturnsOk()
    {
        // Arrange
        var user = await _authHelper.CreateTestUserAsync(
            email: "reset@example.com",
            username: "resetuser");

        // Create a password reset token
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TestRachaContaDbContext>();
        
        var token = new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = "123456",
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            IsUsed = false,
            CreatedAt = DateTime.UtcNow
        };
        
        db.PasswordResetTokens.Add(token);
        await db.SaveChangesAsync();

        var request = new ResetPasswordRequest
        {
            Email = "reset@example.com",
            ResetCode = "123456",
            NewPassword = "NewPassword@123",
            ConfirmPassword = "NewPassword@123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/user/reset-password", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var resetResponse = await response.Content.ReadFromJsonAsync<ResetPasswordResponse>();
        Assert.NotNull(resetResponse);
        Assert.Contains("redefinida com sucesso", resetResponse.Message);
    }

    [Fact]
    public async Task ResetPassword_WithInvalidToken_ReturnsBadRequest()
    {
        // Arrange
        await _authHelper.CreateTestUserAsync(
            email: "reset2@example.com",
            username: "resetuser2");

        var request = new ResetPasswordRequest
        {
            Email = "reset2@example.com",
            ResetCode = "invalid-token",
            NewPassword = "NewPassword@123",
            ConfirmPassword = "NewPassword@123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/user/reset-password", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ResetPassword_WithExpiredToken_ReturnsBadRequest()
    {
        // Arrange
        var user = await _authHelper.CreateTestUserAsync(
            email: "reset3@example.com",
            username: "resetuser3");

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TestRachaContaDbContext>();
        
        var token = new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = "expired-token",
            ExpiresAt = DateTime.UtcNow.AddHours(-1), // Expired
            IsUsed = false,
            CreatedAt = DateTime.UtcNow.AddHours(-2)
        };
        
        db.PasswordResetTokens.Add(token);
        await db.SaveChangesAsync();

        var request = new ResetPasswordRequest
        {
            Email = "reset3@example.com",
            ResetCode = "expired-token",
            NewPassword = "NewPassword@123",
            ConfirmPassword = "NewPassword@123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/user/reset-password", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ResetPassword_WithUsedToken_ReturnsBadRequest()
    {
        // Arrange
        var user = await _authHelper.CreateTestUserAsync(
            email: "reset4@example.com",
            username: "resetuser4");

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TestRachaContaDbContext>();
        
        var token = new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = "used-token",
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            IsUsed = true, // Already used
            CreatedAt = DateTime.UtcNow
        };
        
        db.PasswordResetTokens.Add(token);
        await db.SaveChangesAsync();

        var request = new ResetPasswordRequest
        {
            Email = "reset4@example.com",
            ResetCode = "used-token",
            NewPassword = "NewPassword@123",
            ConfirmPassword = "NewPassword@123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/user/reset-password", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ResetPassword_WithMismatchedPasswords_ReturnsBadRequest()
    {
        // Arrange
        var user = await _authHelper.CreateTestUserAsync(
            email: "reset5@example.com",
            username: "resetuser5");

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TestRachaContaDbContext>();
        
        var token = new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = "valid-token",
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            IsUsed = false,
            CreatedAt = DateTime.UtcNow
        };
        
        db.PasswordResetTokens.Add(token);
        await db.SaveChangesAsync();

        var request = new ResetPasswordRequest
        {
            Email = "reset5@example.com",
            ResetCode = "valid-token",
            NewPassword = "NewPassword@123",
            ConfirmPassword = "DifferentPassword@123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/user/reset-password", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion
}
