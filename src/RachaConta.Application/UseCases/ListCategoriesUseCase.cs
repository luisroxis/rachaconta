using RachaConta.Application.DTOs.Response;
using RachaConta.Core.Interfaces.Repositories;

namespace RachaConta.Application.UseCases;

public class ListCategoriesUseCase
{
    private readonly ICategoryRepository _repository;

    public ListCategoriesUseCase(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<CategoryResponse>> ExecuteAsync()
    {
        var categories = await _repository.GetAllAsync();

        return categories.Select(c => new CategoryResponse
        {
            Id = c.Id,
            Description = c.Description,
            IsActive = c.IsActive
        });
    }
}
