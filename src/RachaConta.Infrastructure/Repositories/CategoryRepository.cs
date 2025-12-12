using Microsoft.EntityFrameworkCore;
using RachaConta.Core.Entities;
using RachaConta.Core.Interfaces.Repositories;
using RachaConta.Infrastructure.Data;

namespace RachaConta.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly RachaContaDbContext _context;

    public CategoryRepository(RachaContaDbContext context)
    {
        _context = context;
    }

    public async Task<Category> CreateAsync(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _context.Categories.AsNoTracking().ToListAsync();
    }

    public async Task<bool> ExistsByDescriptionAsync(string description)
    {
        return await _context.Categories.AnyAsync(c => c.Description == description);
    }
}
