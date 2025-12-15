using Microsoft.EntityFrameworkCore;
using RachaConta.Application.Interfaces;
using RachaConta.Core.Entities;
using RachaConta.Infrastructure.Data;

namespace RachaConta.Infrastructure.Repositories;

public class ParticipanteGrupoRepository : IParticipanteGrupoRepository
{
    private readonly RachaContaDbContext _context;

    public ParticipanteGrupoRepository(RachaContaDbContext context)
    {
        _context = context;
    }

    public async Task<ParticipanteGrupo> CreateAsync(ParticipanteGrupo participante)
    {
        await _context.ParticipantesGrupo.AddAsync(participante);
        await _context.SaveChangesAsync();
        return participante;
    }

    public async Task<ParticipanteGrupo?> GetByIdAsync(Guid id)
    {
        return await _context.ParticipantesGrupo.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<ParticipanteGrupo>> GetByGrupoIdAsync(Guid grupoId)
    {
        return await _context.ParticipantesGrupo
            .Where(p => p.IdGrupo == grupoId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<ParticipanteGrupo?> GetByGrupoAndUserAsync(Guid grupoId, Guid userId)
    {
        return await _context.ParticipantesGrupo
            .FirstOrDefaultAsync(p => p.IdGrupo == grupoId && p.UserId == userId);
    }

    public async Task<List<ParticipanteGrupo>> GetByUserIdAsync(Guid userId)
    {
        return await _context.ParticipantesGrupo
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task UpdateAsync(ParticipanteGrupo participante)
    {
        participante.UpdatedAt = DateTime.UtcNow;
        _context.ParticipantesGrupo.Update(participante);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var participante = await _context.ParticipantesGrupo.FindAsync(id);
        if (participante != null)
        {
            _context.ParticipantesGrupo.Remove(participante);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> IsAdmAsync(Guid grupoId, Guid userId)
    {
        var participante = await _context.ParticipantesGrupo
            .FirstOrDefaultAsync(p => p.IdGrupo == grupoId && p.UserId == userId);
        return participante?.IsAdm ?? false;
    }
}
