
using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using RachaConta.Application.DTOs.Request;
using RachaConta.Application.DTOs.Response;
using RachaConta.Core.Entities;
using RachaConta.IntegrationTests.Data;
using RachaConta.IntegrationTests.Helpers;

namespace RachaConta.IntegrationTests.Controllers;

public class CategoryControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    private readonly TestAuthHelper _authHelper;

    public CategoryControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _factory.ResetDatabase();
        _client = factory.CreateClient();
        _authHelper = new TestAuthHelper(factory, _client);
    }

    [Fact]
    public async Task Create_WithValidData_ReturnsCreated()
    {
        // Arrange
        var (_, token) = await _authHelper.CreateAuthenticatedUserAsync();
        _authHelper.AddAuthorizationHeader(_client, token);

        var request = new CreateCategoryRequest 
        { 
            Description = "Nova Categoria" 
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/categories", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var categoryResponse = await response.Content.ReadFromJsonAsync<CategoryResponse>();
        Assert.NotNull(categoryResponse);
        Assert.Equal(request.Description, categoryResponse.Description);
        Assert.True(categoryResponse.IsActive);
        Assert.NotEqual(Guid.Empty, categoryResponse.Id);
    }

    [Fact]
    public async Task Create_WithoutAuth_ReturnsUnauthorized()
    {
        // Arrange
        var request = new CreateCategoryRequest { Description = "Categoria Sem Auth" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/categories", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithDuplicateDescription_ReturnsBadRequest()
    {
        // Arrange
        var (_, token) = await _authHelper.CreateAuthenticatedUserAsync();
        _authHelper.AddAuthorizationHeader(_client, token);

        // Seed existing category
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<TestRachaContaDbContext>();
            db.Categories.Add(new Category("Existing Category"));
            await db.SaveChangesAsync();
        }

        var request = new CreateCategoryRequest { Description = "Existing Category" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/categories", request);

        // Assert
        // Expect BadRequest (400) or Conflict (409) based on Controller implementation.
        // Controller returns BadRequest for InvalidOperationException.
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var error = await response.Content.ReadFromJsonAsync<dynamic>(); // or custom error DTO
        // Assert.Contains("exist", error.message);
    }

    [Fact]
    public async Task List_ReturnsAllCategories()
    {
        // Arrange
        var (_, token) = await _authHelper.CreateAuthenticatedUserAsync();
        _authHelper.AddAuthorizationHeader(_client, token);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<TestRachaContaDbContext>();
            db.Categories.AddRange(new[]
            {
                new Category("Cat A"),
                new Category("Cat B")
            });
            await db.SaveChangesAsync();
        }

        // Act
        var response = await _client.GetAsync("/api/categories");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var categories = await response.Content.ReadFromJsonAsync<IEnumerable<CategoryResponse>>();
        Assert.NotNull(categories);
        Assert.Equal(2, categories.Count());
        Assert.Contains(categories, c => c.Description == "Cat A");
        Assert.Contains(categories, c => c.Description == "Cat B");
    }
}
