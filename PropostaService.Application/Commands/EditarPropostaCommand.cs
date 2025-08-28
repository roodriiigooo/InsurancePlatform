using InsurancePlatform.Shared;
using MediatR;
using PropostaService.Domain.Interfaces;

namespace PropostaService.Application.Propostas.Commands;

public class EditarPropostaCommand : IRequest<Result>
{
    public Guid PropostaId { get; set; }
    public string NomeCliente { get; set; }
    public decimal Valor { get; set; }
}

public class EditarPropostaCommandHandler : IRequestHandler<EditarPropostaCommand, Result>
{
    private readonly IPropostaRepository _propostaRepository;

    public EditarPropostaCommandHandler(IPropostaRepository propostaRepository)
    {
        _propostaRepository = propostaRepository;
    }

    public async Task<Result> Handle(EditarPropostaCommand request, CancellationToken cancellationToken)
    {
        var proposta = await _propostaRepository.ObterPorIdAsync(request.PropostaId);

        if (proposta is null)
        {
            return Result.Failure(new Error("Proposta.NaoEncontrada", "A proposta com o ID especificado não foi encontrada."));
        }

        var resultadoEdicao = proposta.Editar(request.NomeCliente, request.Valor);

        if (resultadoEdicao.IsFailure)
        {
            return resultadoEdicao;
        }

        await _propostaRepository.AtualizarAsync(proposta);

        return Result.Success();
    }
}