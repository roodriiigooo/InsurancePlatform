using InsurancePlatform.Contracts;
using InsurancePlatform.Shared;
using MassTransit;
using MediatR;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Interfaces;

namespace PropostaService.Application.Propostas.Commands;

public class AprovarPropostaCommand : IRequest<Result>
{
    public Guid PropostaId { get; set; }
}

public class AprovarPropostaCommandHandler : IRequestHandler<AprovarPropostaCommand, Result>
{
    private readonly IPropostaRepository _propostaRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public AprovarPropostaCommandHandler(IPropostaRepository propostaRepository, IPublishEndpoint publishEndpoint)
    {
        _propostaRepository = propostaRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result> Handle(AprovarPropostaCommand request, CancellationToken cancellationToken)
    {
        var proposta = await _propostaRepository.ObterPorIdAsync(request.PropostaId);

        if (proposta is null)
        {
            return Result.Failure(new Error("Proposta.NaoEncontrada", "A proposta com o ID especificado não foi encontrada."));
        }

        var resultadoAprovacao = proposta.Aprovar();

        if (resultadoAprovacao.IsFailure)
        {
            return resultadoAprovacao;
        }

        await _propostaRepository.AtualizarAsync(proposta);

        await _publishEndpoint.Publish(new PropostaAprovadaEvent
        {
            PropostaId = proposta.Id,
            NomeCliente = proposta.NomeCliente,
            Valor = proposta.Valor
        }, cancellationToken);

        return Result.Success();
    }
}