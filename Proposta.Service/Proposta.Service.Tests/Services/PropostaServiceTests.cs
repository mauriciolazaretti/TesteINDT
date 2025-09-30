using FluentAssertions;
using Moq;
using Proposta.Service.Application.Ports;
using Proposta.Service.Domain.Entities;
using Proposta.Service.Domain.Enums;
using Proposta.Service.Domain.Response;
using Proposta.Service.Domain.Services;
using Proposta.Service.Tests.Helpers;
using Xunit;

namespace Proposta.Service.Tests.Services;

public class PropostaServiceTests
{
    private readonly Mock<IEntradaPropostaPort> _mockEntradaPort;
    private readonly Mock<ISaidaPropostaPort> _mockSaidaPort;
    private readonly PropostaService _service;

    public PropostaServiceTests()
    {
        _mockEntradaPort = new Mock<IEntradaPropostaPort>();
        _mockSaidaPort = new Mock<ISaidaPropostaPort>();
        _service = new PropostaService(_mockEntradaPort.Object, _mockSaidaPort.Object);
    }

    [Fact]
    public async Task CriarPropostaAsync_DeveChamarPortaDeEntrada()
    {
        // Arrange
        var proposta = TestDataHelper.CriarSeguroPropostaValida();

        _mockEntradaPort
            .Setup(p => p.CriarPropostaAsync(proposta))
            .ReturnsAsync(proposta);

        // Act
        await _service.CriarPropostaAsync(proposta);

        // Assert
        _mockEntradaPort.Verify(p => p.CriarPropostaAsync(proposta), Times.Once);
    }

    [Fact]
    public async Task CriarPropostaAsync_QuandoPortaLancaExcecao_DevePropagarExcecao()
    {
        // Arrange
        var proposta = TestDataHelper.CriarSeguroPropostaValida();
        var excecaoEsperada = new InvalidOperationException("Erro na criação");

        _mockEntradaPort
            .Setup(p => p.CriarPropostaAsync(proposta))
            .ThrowsAsync(excecaoEsperada);

        // Act & Assert
        var excecao = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CriarPropostaAsync(proposta)
        );

        excecao.Message.Should().Be("Erro na criação");
        _mockEntradaPort.Verify(p => p.CriarPropostaAsync(proposta), Times.Once);
    }

    [Fact]
    public async Task ObterPropostaPorIdAsync_ComIdExistente_DeveRetornarProposta()
    {
        // Arrange
        var id = Guid.NewGuid();
        var propostaEsperada = TestDataHelper.CriarSeguroPropostaComId(id);

        _mockEntradaPort
            .Setup(p => p.ObterPropostaPorIdAsync(id))
            .ReturnsAsync(propostaEsperada);

        // Act
        var resultado = await _service.ObterPropostaPorIdAsync(id);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().Be(propostaEsperada);
        resultado!.Id.Should().Be(id);
        _mockEntradaPort.Verify(p => p.ObterPropostaPorIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task ObterPropostaPorIdAsync_ComIdInexistente_DeveRetornarNull()
    {
        // Arrange
        var id = Guid.NewGuid();

        _mockEntradaPort
            .Setup(p => p.ObterPropostaPorIdAsync(id))
            .ReturnsAsync((SeguroProposta?)null);

        // Act
        var resultado = await _service.ObterPropostaPorIdAsync(id);

        // Assert
        resultado.Should().BeNull();
        _mockEntradaPort.Verify(p => p.ObterPropostaPorIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task ObterTodasPropostasAsync_DeveRetornarResponseDaPorta()
    {
        // Arrange
        var pagina = 1;
        var tamanhoPagina = 10;
        var responseEsperado = new ResponseSeguroProposta
        {
            Propostas = new List<SeguroProposta> { TestDataHelper.CriarSeguroPropostaValida() },
            PaginaAtual = pagina,
            TotalPaginas = 1,
            TotalRegistros = 1,
            TotalPropostas = 1
        };

        _mockEntradaPort
            .Setup(p => p.ObterTodasPropostasAsync(pagina, tamanhoPagina))
            .ReturnsAsync(responseEsperado);

        // Act
        var resultado = await _service.ObterTodasPropostasAsync(pagina, tamanhoPagina);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().Be(responseEsperado);
        resultado.Propostas.Should().HaveCount(1);
        resultado.PaginaAtual.Should().Be(pagina);
        resultado.TotalPaginas.Should().Be(1);
        _mockEntradaPort.Verify(p => p.ObterTodasPropostasAsync(pagina, tamanhoPagina), Times.Once);
    }

    [Fact]
    public async Task AtualizarStatusPropostaAsync_ComIdExistente_DeveRetornarTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var status = StatusPropostaEnum.Aprovada;

        _mockSaidaPort
            .Setup(p => p.AtualizarStatusPropostaAsync(id, status))
            .ReturnsAsync(true);

        // Act
        var resultado = await _service.AtualizarStatusPropostaAsync(id, status);

        // Assert
        resultado.Should().BeTrue();
        _mockSaidaPort.Verify(p => p.AtualizarStatusPropostaAsync(id, status), Times.Once);
    }

    [Fact]
    public async Task AtualizarStatusPropostaAsync_ComIdInexistente_DeveRetornarFalse()
    {
        // Arrange
        var id = Guid.NewGuid();
        var status = StatusPropostaEnum.Rejeitada;

        _mockSaidaPort
            .Setup(p => p.AtualizarStatusPropostaAsync(id, status))
            .ReturnsAsync(false);

        // Act
        var resultado = await _service.AtualizarStatusPropostaAsync(id, status);

        // Assert
        resultado.Should().BeFalse();
        _mockSaidaPort.Verify(p => p.AtualizarStatusPropostaAsync(id, status), Times.Once);
    }

    [Theory]
    [InlineData(StatusPropostaEnum.EmAnalise)]
    [InlineData(StatusPropostaEnum.Aprovada)]
    [InlineData(StatusPropostaEnum.Rejeitada)]
    public async Task AtualizarStatusPropostaAsync_ComDiferentesStatus_DeveChamarPortaCorretamente(StatusPropostaEnum status)
    {
        // Arrange
        var id = Guid.NewGuid();

        _mockSaidaPort
            .Setup(p => p.AtualizarStatusPropostaAsync(id, status))
            .ReturnsAsync(true);

        // Act
        var resultado = await _service.AtualizarStatusPropostaAsync(id, status);

        // Assert
        resultado.Should().BeTrue();
        _mockSaidaPort.Verify(p => p.AtualizarStatusPropostaAsync(id, status), Times.Once);
    }

    [Fact]
    public async Task ObterTodasPropostasAsync_ComParametrosPaginacao_DeveChamarPortaComParametrosCorretos()
    {
        // Arrange
        var pagina = 2;
        var tamanhoPagina = 5;
        var response = new ResponseSeguroProposta
        {
            Propostas = new List<SeguroProposta>(),
            PaginaAtual = pagina,
            TotalPaginas = 3,
            TotalRegistros = 15,
            TotalPropostas = 15
        };

        _mockEntradaPort
            .Setup(p => p.ObterTodasPropostasAsync(pagina, tamanhoPagina))
            .ReturnsAsync(response);

        // Act
        var resultado = await _service.ObterTodasPropostasAsync(pagina, tamanhoPagina);

        // Assert
        resultado.Should().NotBeNull();
        resultado.PaginaAtual.Should().Be(pagina);
        resultado.TotalPaginas.Should().Be(3);
        resultado.TotalRegistros.Should().Be(15);
        _mockEntradaPort.Verify(p => p.ObterTodasPropostasAsync(pagina, tamanhoPagina), Times.Once);
    }

    [Fact]
    public async Task CriarPropostaAsync_ComPropostaNula_DeveLancarArgumentNullException()
    {
        // Arrange
        SeguroProposta? proposta = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _service.CriarPropostaAsync(proposta!)
        );

        _mockEntradaPort.Verify(p => p.CriarPropostaAsync(It.IsAny<SeguroProposta>()), Times.Never);
    }

    [Fact]
    public async Task ObterPropostaPorIdAsync_ComGuidVazio_DeveChamarPortaComGuidVazio()
    {
        // Arrange
        var id = Guid.Empty;

        _mockEntradaPort
            .Setup(p => p.ObterPropostaPorIdAsync(id))
            .ReturnsAsync((SeguroProposta?)null);

        // Act
        var resultado = await _service.ObterPropostaPorIdAsync(id);

        // Assert
        resultado.Should().BeNull();
        _mockEntradaPort.Verify(p => p.ObterPropostaPorIdAsync(Guid.Empty), Times.Once);
    }

    [Theory]
    [InlineData(0, 10)] // página inválida
    [InlineData(1, 0)]  // tamanho inválido
    [InlineData(-1, 5)] // página negativa
    [InlineData(1, -5)] // tamanho negativo
    public async Task ObterTodasPropostasAsync_ComParametrosInvalidos_AindaDeveChamarPorta(int pagina, int tamanhoPagina)
    {
        // Arrange
        var response = new ResponseSeguroProposta
        {
            Propostas = new List<SeguroProposta>(),
            PaginaAtual = pagina,
            TotalPaginas = 0,
            TotalRegistros = 0,
            TotalPropostas = 0
        };

        _mockEntradaPort
            .Setup(p => p.ObterTodasPropostasAsync(pagina, tamanhoPagina))
            .ReturnsAsync(response);

        // Act
        var resultado = await _service.ObterTodasPropostasAsync(pagina, tamanhoPagina);

        // Assert
        resultado.Should().NotBeNull();
        _mockEntradaPort.Verify(p => p.ObterTodasPropostasAsync(pagina, tamanhoPagina), Times.Once);
    }

    [Fact]
    public async Task AtualizarStatusPropostaAsync_QuandoPortaLancaExcecao_DevePropagarExcecao()
    {
        // Arrange
        var id = Guid.NewGuid();
        var status = StatusPropostaEnum.Aprovada;
        var excecaoEsperada = new InvalidOperationException("Erro na atualização");

        _mockSaidaPort
            .Setup(p => p.AtualizarStatusPropostaAsync(id, status))
            .ThrowsAsync(excecaoEsperada);

        // Act & Assert
        var excecao = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.AtualizarStatusPropostaAsync(id, status)
        );

        excecao.Message.Should().Be("Erro na atualização");
        _mockSaidaPort.Verify(p => p.AtualizarStatusPropostaAsync(id, status), Times.Once);
    }
}