using RachaConta.Core.Entities;

namespace RachaConta.Core.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<Category> CreateAsync(Category category);
    Task<IEnumerable<Category>> GetAllAsync();
    Task<bool> ExistsByDescriptionAsync(string description);
}
