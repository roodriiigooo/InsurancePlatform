using InsurancePlatform.Shared;
using MediatR;
using PropostaService.Domain.Interfaces;

namespace PropostaService.Application.Propostas.Commands;

public class RecusarPropostaCommand : IRequest<Result>
{
    public Guid PropostaId { get; set; }
    public string Motivo { get; set; }
}

public class RecusarPropostaCommandHandler : IRequestHandler<RecusarPropostaCommand, Result>
{
    private readonly IPropostaRepository _propostaRepository;

    public RecusarPropostaCommandHandler(IPropostaRepository propostaRepository)
    {
        _propostaRepository = propostaRepository;
    }

    public async Task<Result> Handle(RecusarPropostaCommand request, CancellationToken cancellationToken)
    {
        var proposta = await _propostaRepository.ObterPorIdAsync(request.PropostaId);

        if (proposta is null)
        {
            return Result.Failure(new Error("Proposta.NaoEncontrada", "A proposta com o ID especificado não foi encontrada."));
        }

        var resultadoRejeicao = proposta.Rejeitar(request.Motivo);

        if (resultadoRejeicao.IsFailure)
        {
            return resultadoRejeicao;
        }

        await _propostaRepository.AtualizarAsync(proposta);

        return Result.Success();
    }
}