using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RachaConta.Application.DTOs.Request;
using RachaConta.Application.DTOs.Response;
using RachaConta.Core.Entities;
using RachaConta.Infrastructure.Data;
using RachaConta.IntegrationTests.Helpers;

namespace RachaConta.IntegrationTests.Controllers;

public class InviteControllerTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly TestAuthHelper _authHelper;

    public InviteControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _authHelper = new TestAuthHelper(factory);
        _factory.ResetDatabase();
    }

    public void Dispose()
    {
        _client?.Dispose();
    }

    #region Send Invite Tests

    [Fact]
    public async Task SendInvite_WithValidData_ReturnsCreated()
    {
        // Arrange
        var (user, token) = await _authHelper.CreateAuthenticatedUserAsync();
        _authHelper.AddAuthorizationHeader(_client, token);

        var request = new SendInviteRequest
        {
            Nome = "Friend Name",
            Email = "friend@example.com",
            CorpoEmail = "You are invited to join RachaConta!",
            AmigoId = "friend123"
        };

        // Setup email service mock
        _factory.EmailServiceMock
            .Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var response = await _client.PostAsJsonAsync("/api/invite", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var inviteResponse = await response.Content.ReadFromJsonAsync<InviteResponse>();
        Assert.NotNull(inviteResponse);
        Assert.Equal("Friend Name", inviteResponse.Nome);
        Assert.Equal("friend@example.com", inviteResponse.Email);
        Assert.False(inviteResponse.Reenvio);
        Assert.False(inviteResponse.Aceite);

        // Verify email was sent
        _factory.EmailServiceMock.Verify(
            x => x.SendEmailAsync(
                "friend@example.com",
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task SendInvite_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var request = new SendInviteRequest
        {
            Nome = "Friend Name",
            Email = "friend@example.com",
            CorpoEmail = "You are invited!",
            AmigoId = "friend123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/invite", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task SendInvite_WithInvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        var (user, token) = await _authHelper.CreateAuthenticatedUserAsync();
        _authHelper.AddAuthorizationHeader(_client, token);

        var request = new SendInviteRequest
        {
            Nome = "Friend Name",
            Email = "invalid-email",
            CorpoEmail = "You are invited!",
            AmigoId = "friend123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/invite", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task SendInvite_WithMissingFields_ReturnsBadRequest()
    {
        // Arrange
        var (user, token) = await _authHelper.CreateAuthenticatedUserAsync();
        _authHelper.AddAuthorizationHeader(_client, token);

        var request = new SendInviteRequest
        {
            Nome = "",
            Email = "friend@example.com",
            CorpoEmail = "",
            AmigoId = ""
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/invite", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region Resend Invite Tests

    [Fact]
    public async Task ResendInvite_WithValidId_ReturnsOk()
    {
        // Arrange
        var (user, token) = await _authHelper.CreateAuthenticatedUserAsync();
        _authHelper.AddAuthorizationHeader(_client, token);

        // Create an invite
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RachaContaDbContext>();

        var invite = new Invite
        {
            Id = Guid.NewGuid(),
            Nome = "Friend",
            Email = "friend@example.com",
            CorpoEmail = "Invite body",
            AmigoId = "friend123",
            UsuarioId = user.Id,
            DataEnvio = DateTime.UtcNow,
            Reenvio = false,
            Aceite = false
        };

        db.Invites.Add(invite);
        await db.SaveChangesAsync();

        // Setup email service mock
        _factory.EmailServiceMock
            .Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var response = await _client.PostAsync($"/api/invite/{invite.Id}/resend", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var inviteResponse = await response.Content.ReadFromJsonAsync<InviteResponse>();
        Assert.NotNull(inviteResponse);
        Assert.True(inviteResponse.Reenvio);
        Assert.NotNull(inviteResponse.DataReenvio);

        // Verify email was sent
        _factory.EmailServiceMock.Verify(
            x => x.SendEmailAsync(
                "friend@example.com",
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task ResendInvite_WhenAlreadyResent_ReturnsBadRequest()
    {
        // Arrange
        var (user, token) = await _authHelper.CreateAuthenticatedUserAsync();
        _authHelper.AddAuthorizationHeader(_client, token);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RachaContaDbContext>();

        var invite = new Invite
        {
            Id = Guid.NewGuid(),
            Nome = "Friend",
            Email = "friend@example.com",
            CorpoEmail = "Invite body",
            AmigoId = "friend123",
            UsuarioId = user.Id,
            DataEnvio = DateTime.UtcNow,
            Reenvio = true, // Already resent
            DataReenvio = DateTime.UtcNow,
            Aceite = false
        };

        db.Invites.Add(invite);
        await db.SaveChangesAsync();

        // Act
        var response = await _client.PostAsync($"/api/invite/{invite.Id}/resend", null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ResendInvite_WhenNotOwner_ReturnsUnauthorized()
    {
        // Arrange
        var (user1, token1) = await _authHelper.CreateAuthenticatedUserAsync(
            email: "user1@example.com",
            username: "user1");

        var user2 = await _authHelper.CreateTestUserAsync(
            email: "user2@example.com",
            username: "user2");

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RachaContaDbContext>();

        // Create invite owned by user2
        var invite = new Invite
        {
            Id = Guid.NewGuid(),
            Nome = "Friend",
            Email = "friend@example.com",
            CorpoEmail = "Invite body",
            AmigoId = "friend123",
            UsuarioId = user2.Id, // Owned by user2
            DataEnvio = DateTime.UtcNow,
            Reenvio = false,
            Aceite = false
        };

        db.Invites.Add(invite);
        await db.SaveChangesAsync();

        // Try to resend with user1's token
        _authHelper.AddAuthorizationHeader(_client, token1);

        // Act
        var response = await _client.PostAsync($"/api/invite/{invite.Id}/resend", null);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ResendInvite_WithNonExistentId_ReturnsBadRequest()
    {
        // Arrange
        var (user, token) = await _authHelper.CreateAuthenticatedUserAsync();
        _authHelper.AddAuthorizationHeader(_client, token);

        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.PostAsync($"/api/invite/{nonExistentId}/resend", null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ResendInvite_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var inviteId = Guid.NewGuid();

        // Act
        var response = await _client.PostAsync($"/api/invite/{inviteId}/resend", null);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Delete Invite Tests

    [Fact]
    public async Task DeleteInvite_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var (user, token) = await _authHelper.CreateAuthenticatedUserAsync();
        _authHelper.AddAuthorizationHeader(_client, token);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RachaContaDbContext>();

        var invite = new Invite
        {
            Id = Guid.NewGuid(),
            Nome = "Friend",
            Email = "friend@example.com",
            CorpoEmail = "Invite body",
            AmigoId = "friend123",
            UsuarioId = user.Id,
            DataEnvio = DateTime.UtcNow,
            Reenvio = false,
            Aceite = false
        };

        db.Invites.Add(invite);
        await db.SaveChangesAsync();

        // Act
        var response = await _client.DeleteAsync($"/api/invite/{invite.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify invite was deleted
        var deletedInvite = await db.Invites.FindAsync(invite.Id);
        Assert.Null(deletedInvite);
    }

    [Fact]
    public async Task DeleteInvite_WhenNotOwner_ReturnsUnauthorized()
    {
        // Arrange
        var (user1, token1) = await _authHelper.CreateAuthenticatedUserAsync(
            email: "user1@example.com",
            username: "user1");

        var user2 = await _authHelper.CreateTestUserAsync(
            email: "user2@example.com",
            username: "user2");

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RachaContaDbContext>();

        var invite = new Invite
        {
            Id = Guid.NewGuid(),
            Nome = "Friend",
            Email = "friend@example.com",
            CorpoEmail = "Invite body",
            AmigoId = "friend123",
            UsuarioId = user2.Id, // Owned by user2
            DataEnvio = DateTime.UtcNow,
            Reenvio = false,
            Aceite = false
        };

        db.Invites.Add(invite);
        await db.SaveChangesAsync();

        // Try to delete with user1's token
        _authHelper.AddAuthorizationHeader(_client, token1);

        // Act
        var response = await _client.DeleteAsync($"/api/invite/{invite.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteInvite_WithNonExistentId_ReturnsBadRequest()
    {
        // Arrange
        var (user, token) = await _authHelper.CreateAuthenticatedUserAsync();
        _authHelper.AddAuthorizationHeader(_client, token);

        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/invite/{nonExistentId}");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteInvite_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var inviteId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/invite/{inviteId}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region List Invites Tests

    [Fact]
    public async Task ListInvites_WithAuthentication_ReturnsOk()
    {
        // Arrange
        var (user, token) = await _authHelper.CreateAuthenticatedUserAsync();
        _authHelper.AddAuthorizationHeader(_client, token);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RachaContaDbContext>();

        // Create multiple invites for the user
        var invites = new List<Invite>
        {
            new Invite
            {
                Id = Guid.NewGuid(),
                Nome = "Friend 1",
                Email = "friend1@example.com",
                CorpoEmail = "Invite 1",
                AmigoId = "friend1",
                UsuarioId = user.Id,
                DataEnvio = DateTime.UtcNow,
                Reenvio = false,
                Aceite = false
            },
            new Invite
            {
                Id = Guid.NewGuid(),
                Nome = "Friend 2",
                Email = "friend2@example.com",
                CorpoEmail = "Invite 2",
                AmigoId = "friend2",
                UsuarioId = user.Id,
                DataEnvio = DateTime.UtcNow,
                Reenvio = false,
                Aceite = false
            }
        };

        db.Invites.AddRange(invites);
        await db.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/api/invite");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var inviteResponses = await response.Content.ReadFromJsonAsync<List<InviteResponse>>();
        Assert.NotNull(inviteResponses);
        Assert.Equal(2, inviteResponses.Count);
    }

    [Fact]
    public async Task ListInvites_ReturnsOnlyUserInvites_NotOthers()
    {
        // Arrange
        var (user1, token1) = await _authHelper.CreateAuthenticatedUserAsync(
            email: "user1@example.com",
            username: "user1");

        var user2 = await _authHelper.CreateTestUserAsync(
            email: "user2@example.com",
            username: "user2");

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RachaContaDbContext>();

        // Create invites for both users
        var invites = new List<Invite>
        {
            new Invite
            {
                Id = Guid.NewGuid(),
                Nome = "User1 Friend",
                Email = "user1friend@example.com",
                CorpoEmail = "Invite",
                AmigoId = "friend1",
                UsuarioId = user1.Id,
                DataEnvio = DateTime.UtcNow,
                Reenvio = false,
                Aceite = false
            },
            new Invite
            {
                Id = Guid.NewGuid(),
                Nome = "User2 Friend",
                Email = "user2friend@example.com",
                CorpoEmail = "Invite",
                AmigoId = "friend2",
                UsuarioId = user2.Id,
                DataEnvio = DateTime.UtcNow,
                Reenvio = false,
                Aceite = false
            }
        };

        db.Invites.AddRange(invites);
        await db.SaveChangesAsync();

        // Act - Get invites for user1
        _authHelper.AddAuthorizationHeader(_client, token1);
        var response = await _client.GetAsync("/api/invite");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var inviteResponses = await response.Content.ReadFromJsonAsync<List<InviteResponse>>();
        Assert.NotNull(inviteResponses);
        Assert.Single(inviteResponses);
        Assert.Equal("User1 Friend", inviteResponses[0].Nome);
    }

    [Fact]
    public async Task ListInvites_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/invite");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion
}
