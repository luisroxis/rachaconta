using Microsoft.EntityFrameworkCore;
using RachaConta.Core.Entities;
using RachaConta.Core.Interfaces.Repositories;
using RachaConta.Infrastructure.Data;

namespace RachaConta.Infrastructure.Repositories;

public class GrupoRepository : IGrupoRepository
{
    private readonly RachaContaDbContext _context;

    public GrupoRepository(RachaContaDbContext context)
    {
        _context = context;
    }

    public async Task<Grupo> CreateAsync(Grupo grupo)
    {
        await _context.Grupos.AddAsync(grupo);
        await _context.SaveChangesAsync();
        return grupo;
    }

    public async Task<Grupo?> GetByIdAsync(Guid id)
    {
        return await _context.Grupos.FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<List<Grupo>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Grupos
            .Where(g => !g.Deleted && _context.ParticipantesGrupo.Any(p => p.IdGrupo == g.Id && p.UserId == userId))
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Grupo>> GetAllAsync()
    {
        return await _context.Grupos
            .Where(g => !g.Deleted)
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync();
    }

    public async Task UpdateAsync(Grupo grupo)
    {
        grupo.UpdatedAt = DateTime.UtcNow;
        _context.Grupos.Update(grupo);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var grupo = await _context.Grupos.FindAsync(id);
        if (grupo != null)
        {
            grupo.Deleted = true;
            grupo.UpdatedAt = DateTime.UtcNow;
            _context.Grupos.Update(grupo);
            await _context.SaveChangesAsync();
        }
    }
}
