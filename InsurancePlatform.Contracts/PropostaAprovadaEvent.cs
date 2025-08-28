namespace InsurancePlatform.Contracts;

public record PropostaAprovadaEvent
{
    public Guid PropostaId { get; init; }
    public string NomeCliente { get; init; }
    public decimal Valor { get; init; }
}