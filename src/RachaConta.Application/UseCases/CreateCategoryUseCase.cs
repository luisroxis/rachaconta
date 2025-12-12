using RachaConta.Application.DTOs.Request;
using RachaConta.Application.DTOs.Response;
using RachaConta.Core.Entities;
using RachaConta.Core.Interfaces.Repositories;

namespace RachaConta.Application.UseCases;

public class CreateCategoryUseCase
{
    private readonly ICategoryRepository _repository;

    public CreateCategoryUseCase(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<CategoryResponse> ExecuteAsync(CreateCategoryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Description))
            throw new ArgumentException("Descrição é obrigatória.");

        if (await _repository.ExistsByDescriptionAsync(request.Description))
            throw new InvalidOperationException("Já existe uma categoria com esta descrição.");

        var category = new Category(request.Description);
        var created = await _repository.CreateAsync(category);

        return new CategoryResponse
        {
            Id = created.Id,
            Description = created.Description,
            IsActive = created.IsActive
        };
    }
}
