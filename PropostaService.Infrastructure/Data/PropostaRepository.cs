using Microsoft.EntityFrameworkCore;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Interfaces;

namespace PropostaService.Infrastructure.Data;

/// <summary>
/// Implementação concreta do repositório de propostas usando Entity Framework Core.
/// </summary>
public class PropostaRepository : IPropostaRepository
{
    private readonly AppDbContext _context;

    public PropostaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AdicionarAsync(Proposta proposta)
    {
        await _context.Propostas.AddAsync(proposta);
        await _context.SaveChangesAsync();
    }

    public async Task<Proposta?> ObterPorIdAsync(Guid id)
    {
        return await _context.Propostas.FindAsync(id);
    }

    public async Task AtualizarAsync(Proposta proposta)
    {
        _context.Propostas.Update(proposta);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Proposta>> ListarTodosAsync()
    {
        return await _context.Propostas.ToListAsync();
    }
}