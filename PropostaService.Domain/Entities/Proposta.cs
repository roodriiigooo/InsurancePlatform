using InsurancePlatform.Shared;

namespace PropostaService.Domain.Entities;

public enum PropostaStatus { EmAnalise, Aprovada, Rejeitada }

public static class DomainErrors
{
    public static class Proposta
    {
        public static readonly Error StatusInvalidoParaAprovacao = new(
            "Proposta.Aprovacao", "A proposta só pode ser aprovada se estiver 'Em Análise'.");

        public static readonly Error StatusInvalidoParaRejeicao = new(
            "Proposta.Rejeicao", "A proposta só pode ser rejeitada se estiver 'Em Análise'.");

        public static readonly Error NaoPodeSerEditada = new(
        "Proposta.Edicao", "Propostas com status 'Rejeitada' não podem ser editadas.");
    }

}

public class Proposta
{
    public Guid Id { get; private set; }
    public string NomeCliente { get; private set; }
    public decimal Valor { get; private set; }
    public PropostaStatus Status { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public string? MotivoRecusa { get; private set; }



    private Proposta() { }

    private Proposta(string nomeCliente, decimal valor)
    {
        Id = Guid.NewGuid();
        NomeCliente = nomeCliente;
        Valor = valor;
        Status = PropostaStatus.EmAnalise;
    }

    public static Result<Proposta> Criar(string nomeCliente, decimal valor)
    {
        if (string.IsNullOrWhiteSpace(nomeCliente))
        {
            return Result.Failure<Proposta>(new Error("Proposta.NomeCliente", "O nome do cliente não pode ser vazio."));
        }
        if (valor <= 0)
        {
            return Result.Failure<Proposta>(new Error("Proposta.Valor", "O valor da proposta deve ser maior que zero."));
        }
        return new Proposta(nomeCliente, valor);
    }

    public Result Aprovar()
    {
        if (Status != PropostaStatus.EmAnalise)
        {
            return Result.Failure(DomainErrors.Proposta.StatusInvalidoParaAprovacao);
        }

        Status = PropostaStatus.Aprovada;
        return Result.Success();
    }

    public Result Rejeitar(string motivo)
    {
        if (Status != PropostaStatus.EmAnalise)
        {
            return Result.Failure(DomainErrors.Proposta.StatusInvalidoParaRejeicao);
        }

        if (string.IsNullOrWhiteSpace(motivo))
        {
            return Result.Failure(new Error("Proposta.Motivo", "O motivo da recusa é obrigatório."));
        }

        Status = PropostaStatus.Rejeitada;
        MotivoRecusa = motivo;
        return Result.Success();
    }

    public Result Editar(string novoNomeCliente, decimal novoValor)
    {
        // Não se pode editar uma proposta já rejeitada.
        if (Status == PropostaStatus.Rejeitada)
        {
            return Result.Failure(DomainErrors.Proposta.NaoPodeSerEditada);
        }

        if (string.IsNullOrWhiteSpace(novoNomeCliente))
        {
            return Result.Failure(new Error("Proposta.NomeCliente", "O nome do cliente não pode ser vazio."));
        }
        if (novoValor <= 0)
        {
            return Result.Failure(new Error("Proposta.Valor", "O valor da proposta deve ser maior que zero."));
        }

        NomeCliente = novoNomeCliente;
        Valor = novoValor;

        // Se uma proposta aprovada for editada, ela pode voltar para "Em Análise".
        if (Status == PropostaStatus.Aprovada)
        {
            Status = PropostaStatus.EmAnalise;
        }

        return Result.Success();
    }
}