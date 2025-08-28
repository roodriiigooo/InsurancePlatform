using ContratacaoService.Application.Consumers;
using ContratacaoService.Domain.Entities;

namespace ContratacaoService.Infrastructure.Data;

public class ContratacaoRepository : IContratacaoRepository
{
    private readonly AppDbContext _context;

    public ContratacaoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AdicionarAsync(Contratacao contratacao)
    {
        await _context.Contratacoes.AddAsync(contratacao);
        await _context.SaveChangesAsync();
    }
}