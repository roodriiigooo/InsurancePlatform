using ContratacaoService.Application.Consumers;
using ContratacaoService.Domain.Entities;
using FluentAssertions;
using InsurancePlatform.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;

namespace ContratacaoService.UnitTests;

public class PropostaAprovadaConsumerTests
{
    private readonly Mock<IContratacaoRepository> _mockRepository;
    private readonly Mock<ILogger<PropostaAprovadaConsumer>> _mockLogger;
    private readonly PropostaAprovadaConsumer _consumer;

    public PropostaAprovadaConsumerTests()
    {
        _mockRepository = new Mock<IContratacaoRepository>();
        _mockLogger = new Mock<ILogger<PropostaAprovadaConsumer>>();
        _consumer = new PropostaAprovadaConsumer(_mockLogger.Object, _mockRepository.Object);
    }

    [Fact]
    public async Task Consume_QuandoEventoValidoRecebido_DeveChamarRepositorioParaAdicionarContratacao()
    {
        // Arrange
        var evento = new PropostaAprovadaEvent
        {
            PropostaId = Guid.NewGuid(),
            NomeCliente = "Cliente Teste",
            Valor = 5000m
        };

        var mockConsumeContext = new Mock<ConsumeContext<PropostaAprovadaEvent>>();
        mockConsumeContext.Setup(x => x.Message).Returns(evento);

        // Act
        await _consumer.Consume(mockConsumeContext.Object);

        // Assert
        _mockRepository.Verify(repo => repo.AdicionarAsync(It.IsAny<Contratacao>()), Times.Once);
    }
}