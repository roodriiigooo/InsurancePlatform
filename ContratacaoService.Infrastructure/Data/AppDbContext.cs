using ContratacaoService.Domain.Entities;
using Microsoft.EntityFrameworkCore; 

namespace ContratacaoService.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<Contratacao> Contratacoes { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}