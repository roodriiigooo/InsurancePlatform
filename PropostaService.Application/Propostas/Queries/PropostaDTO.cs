namespace PropostaService.Application.Propostas.Queries;

/// <summary>
/// Representa os dados de uma proposta que serão enviados para o cliente.
/// </summary>
public class PropostaDto
{
    public Guid Id { get; set; }
    public string NomeCliente { get; set; }
    public decimal Valor { get; set; }
    public string Status { get; set; }
    public string? MotivoRecusa { get; set; }
}