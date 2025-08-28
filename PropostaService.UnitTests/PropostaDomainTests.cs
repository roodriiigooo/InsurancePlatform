using FluentAssertions;
using InsurancePlatform.Shared;
using PropostaService.Domain.Entities;

namespace PropostaService.UnitTests;

public class PropostaDomainTests
{
    #region Testes do Método Criar

    [Fact]
    public void Criar_ComDadosValidos_DeveRetornarSucessoEPropostaEmAnalise()
    {
        // Arrange (Preparação)
        var nomeCliente = "Cliente Válido";
        var valor = 1500m;

        // Act (Ação)
        var result = Proposta.Criar(nomeCliente, valor);

        // Assert (Verificação)
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Status.Should().Be(PropostaStatus.EmAnalise);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Criar_ComNomeClienteInvalido_DeveRetornarFalha(string nomeInvalido)
    {
        // Arrange
        var valor = 1500m;

        // Act
        var result = Proposta.Criar(nomeInvalido, valor);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Proposta.NomeCliente");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void Criar_ComValorInvalido_DeveRetornarFalha(decimal valorInvalido)
    {
        // Arrange
        var nomeCliente = "Cliente Válido";

        // Act
        var result = Proposta.Criar(nomeCliente, valorInvalido);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Proposta.Valor");
    }

    #endregion

    #region Testes do Método Aprovar

    [Fact]
    public void Aprovar_PropostaEmAnalise_DeveMudarStatusParaAprovada()
    {
        // Arrange
        var proposta = Proposta.Criar("Cliente", 1000).Value;

        // Act
        var result = proposta.Aprovar();

        // Assert
        result.IsSuccess.Should().BeTrue();
        proposta.Status.Should().Be(PropostaStatus.Aprovada);
    }

    [Fact]
    public void Aprovar_PropostaJaAprovada_DeveRetornarFalha()
    {
        // Arrange
        var proposta = Proposta.Criar("Cliente", 1000).Value;
        proposta.Aprovar(); // Aprovada pela primeira vez

        // Act
        var result = proposta.Aprovar(); // Tenta aprovar de novo

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Proposta.StatusInvalidoParaAprovacao);
    }

    #endregion

    #region Testes do Método Rejeitar

    [Fact]
    public void Rejeitar_PropostaEmAnaliseComMotivoValido_DeveMudarStatusParaRejeitada()
    {
        // Arrange
        var proposta = Proposta.Criar("Cliente", 1000).Value;
        var motivo = "Score de crédito insuficiente";

        // Act
        var result = proposta.Rejeitar(motivo);

        // Assert
        result.IsSuccess.Should().BeTrue();
        proposta.Status.Should().Be(PropostaStatus.Rejeitada);
        proposta.MotivoRecusa.Should().Be(motivo);
    }

    [Fact]
    public void Rejeitar_PropostaJaAprovada_DeveRetornarFalha()
    {
        // Arrange
        var proposta = Proposta.Criar("Cliente", 1000).Value;
        proposta.Aprovar();

        // Act
        var result = proposta.Rejeitar("Motivo qualquer");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Proposta.StatusInvalidoParaRejeicao);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Rejeitar_ComMotivoInvalido_DeveRetornarFalha(string motivoInvalido)
    {
        // Arrange
        var proposta = Proposta.Criar("Cliente", 1000).Value;

        // Act
        var result = proposta.Rejeitar(motivoInvalido);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Proposta.Motivo");
    }

    #endregion

    #region Testes do Método Editar

    [Fact]
    public void Editar_PropostaEmAnalise_DeveAtualizarDadosComSucesso()
    {
        // Arrange
        var proposta = Proposta.Criar("Nome Antigo", 1000m).Value;
        var novoNome = "Nome Novo";
        var novoValor = 2500m;

        // Act
        var result = proposta.Editar(novoNome, novoValor);

        // Assert
        result.IsSuccess.Should().BeTrue();
        proposta.NomeCliente.Should().Be(novoNome);
        proposta.Valor.Should().Be(novoValor);
    }

    [Fact]
    public void Editar_PropostaRejeitada_DeveRetornarFalha()
    {
        // Arrange
        var proposta = Proposta.Criar("Cliente", 1000).Value;
        proposta.Rejeitar("Motivo da recusa");

        // Act
        var result = proposta.Editar("Novo Nome", 2000m);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Proposta.NaoPodeSerEditada);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-500)]
    public void Editar_ComValorInvalido_DeveRetornarFalha(decimal valorInvalido)
    {
        // Arrange
        var proposta = Proposta.Criar("Cliente", 1000).Value;

        // Act
        var result = proposta.Editar("Nome Válido", valorInvalido);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Proposta.Valor");
    }

    #endregion
}