using PropostaService.Domain.Entities;

namespace PropostaService.Domain.Interfaces;

/// <summary>
/// Define o contrato para operações de persistência da entidade Proposta.
/// </summary>
public interface IPropostaRepository
{
    /// <summary>
    /// Adiciona uma nova proposta ao banco de dados.
    /// </summary>
    Task AdicionarAsync(Proposta proposta);

    /// <summary>
    /// Busca uma proposta específica pelo seu identificador único.
    /// </summary>
    Task<Proposta?> ObterPorIdAsync(Guid id);

    /// <summary>
    /// Salva as alterações feitas em uma entidade Proposta existente.
    /// </summary>
    Task AtualizarAsync(Proposta proposta);

    /// <summary>
    /// Lista todas as propostas existentes.
    /// </summary>
    Task<IEnumerable<Proposta>> ListarTodosAsync();
}