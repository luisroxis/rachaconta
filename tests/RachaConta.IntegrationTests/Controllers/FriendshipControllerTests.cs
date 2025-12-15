using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using RachaConta.Application.DTOs.Request;
using RachaConta.Application.DTOs.Response;
using RachaConta.Core.Entities;
using RachaConta.IntegrationTests.Helpers;

namespace RachaConta.IntegrationTests.Controllers;

public class FriendshipControllerTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly TestAuthHelper _authHelper;

    public FriendshipControllerTests(CustomWebApplicationFactory factory)
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

    [Fact]
    public async Task ListUsers_ReturnsOtherUsers()
    {
        // Arrange
        var (user1, token1) = await _authHelper.CreateAuthenticatedUserAsync("User 1", "user1_friendship@test.com", "user1_friendship");
        var user2 = await _authHelper.CreateTestUserAsync("User 2", "user2_friendship@test.com", "user2_friendship", "Pass123!");

        _authHelper.AddAuthorizationHeader(_client, token1);

        // Act
        var response = await _client.GetAsync("/api/friendship/users");

        // Assert
        response.EnsureSuccessStatusCode();
        var users = await response.Content.ReadFromJsonAsync<List<UserResponse>>();
        Assert.NotNull(users);
        Assert.Contains(users, u => u.Id == user2.Id);
        Assert.DoesNotContain(users, u => u.Id == user1.Id);
    }

    [Fact]
    public async Task RequestFriendship_Success()
    {
        // Arrange
        var (user1, token1) = await _authHelper.CreateAuthenticatedUserAsync("Requester", "requester@test.com", "requester");
        var user2 = await _authHelper.CreateTestUserAsync("Receiver", "receiver@test.com", "receiver");

        _authHelper.AddAuthorizationHeader(_client, token1);

        var request = new RequestFriendshipRequest { AmigoId = user2.Id };

        // Act
        var response = await _client.PostAsJsonAsync("/api/friendship", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var friendship = await response.Content.ReadFromJsonAsync<FriendshipResponse>();
        Assert.NotNull(friendship);
        Assert.Equal(user1.Id, friendship.UserId);
        Assert.Equal(user2.Id, friendship.AmigoId);
        Assert.False(friendship.Approved);
    }

    [Fact]
    public async Task ListPending_SentAndReceived()
    {
        // Arrange
        var (user1, token1) = await _authHelper.CreateAuthenticatedUserAsync("Pending 1", "pending1@test.com", "pending1");
        var (user2, token2) = await _authHelper.CreateAuthenticatedUserAsync("Pending 2", "pending2@test.com", "pending2");

        // User1 sends request to User2
        _authHelper.AddAuthorizationHeader(_client, token1);
        await _client.PostAsJsonAsync("/api/friendship", new RequestFriendshipRequest { AmigoId = user2.Id });

        // Act - User1 checks Sent (incoming=false)
        var responseSent = await _client.GetAsync("/api/friendship/pending?incoming=false");
        var sentList = await responseSent.Content.ReadFromJsonAsync<List<FriendshipResponse>>();

        // Act - User2 checks Received (incoming=true)
        _authHelper.AddAuthorizationHeader(_client, token2);
        var responseReceived = await _client.GetAsync("/api/friendship/pending?incoming=true");
        var receivedList = await responseReceived.Content.ReadFromJsonAsync<List<FriendshipResponse>>();

        // Assert
        Assert.Single(sentList);
        Assert.Equal(user2.Id, sentList[0].AmigoId);

        Assert.Single(receivedList);
        Assert.Equal(user1.Id, receivedList[0].UserId);
    }

    [Fact]
    public async Task ApproveFriendship_Success()
    {
        // Arrange
        var (user1, token1) = await _authHelper.CreateAuthenticatedUserAsync("Approver 1", "approver1@test.com", "approver1");
        var (user2, token2) = await _authHelper.CreateAuthenticatedUserAsync("Approver 2", "approver2@test.com", "approver2");

        // User1 sends request to User2
        _authHelper.AddAuthorizationHeader(_client, token1);
        var reqResponse = await _client.PostAsJsonAsync("/api/friendship", new RequestFriendshipRequest { AmigoId = user2.Id });
        var friendship = await reqResponse.Content.ReadFromJsonAsync<FriendshipResponse>();

        // Act - User2 approves
        _authHelper.AddAuthorizationHeader(_client, token2);
        var approveRequest = new ApproveFriendshipRequest { Approved = true };
        var response = await _client.PostAsJsonAsync($"/api/friendship/{friendship.Id}/approve", approveRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var approvedFriendship = await response.Content.ReadFromJsonAsync<FriendshipResponse>();
        Assert.True(approvedFriendship.Approved);
        Assert.NotNull(approvedFriendship.DataAprovacao);

        // Verify accepted list
        var acceptedResponse = await _client.GetAsync("/api/friendship");
        var acceptedList = await acceptedResponse.Content.ReadFromJsonAsync<List<FriendshipResponse>>();
        Assert.Contains(acceptedList, f => f.Id == friendship.Id);
    }
}
