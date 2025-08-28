using InsurancePlatform.Shared;
using MediatR;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Interfaces;

// GARANTA QUE ESTA LINHA ESTEJA EXATAMENTE ASSIM
namespace PropostaService.Application.Propostas.Commands;

// O Comando para CRIAR
public class CriarPropostaCommand : IRequest<Result<Guid>>
{
    public string NomeCliente { get; set; }
    public decimal Valor { get; set; }
}

// O Handler para CRIAR
public class CriarPropostaCommandHandler : IRequestHandler<CriarPropostaCommand, Result<Guid>>
{
    private readonly IPropostaRepository _propostaRepository;

    public CriarPropostaCommandHandler(IPropostaRepository propostaRepository)
    {
        _propostaRepository = propostaRepository;
    }

    public async Task<Result<Guid>> Handle(CriarPropostaCommand request, CancellationToken cancellationToken)
    {
        var propostaResult = Proposta.Criar(request.NomeCliente, request.Valor);

        if (propostaResult.IsFailure)
        {
            return Result.Failure<Guid>(propostaResult.Error);
        }

        var proposta = propostaResult.Value;

        await _propostaRepository.AdicionarAsync(proposta);

        return proposta.Id;
    }
}