using ContratacaoService.Domain.Entities;
using InsurancePlatform.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace ContratacaoService.Application.Consumers;

public interface IContratacaoRepository { Task AdicionarAsync(Contratacao contratacao); }

public class PropostaAprovadaConsumer : IConsumer<PropostaAprovadaEvent>
{
    private readonly ILogger<PropostaAprovadaConsumer> _logger;
    private readonly IContratacaoRepository _contratacaoRepository;

    public PropostaAprovadaConsumer(ILogger<PropostaAprovadaConsumer> logger, IContratacaoRepository contratacaoRepository)
    {
        _logger = logger;
        _contratacaoRepository = contratacaoRepository;
    }

    public async Task Consume(ConsumeContext<PropostaAprovadaEvent> context)
    {
        var evento = context.Message;
        _logger.LogInformation("Evento PropostaAprovadaEvent recebido para a Proposta ID: {PropostaId}", evento.PropostaId);

        var novaContratacao = new Contratacao(evento.PropostaId);
        await _contratacaoRepository.AdicionarAsync(novaContratacao);

        _logger.LogInformation("Contratação criada com sucesso para a Proposta ID: {PropostaId}", evento.PropostaId);
    }
}