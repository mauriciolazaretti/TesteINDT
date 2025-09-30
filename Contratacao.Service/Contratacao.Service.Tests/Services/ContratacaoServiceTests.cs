using Xunit;
using FluentAssertions;
using Moq;
using Contratacao.Service.Application.Services;
using Contratacao.Service.Application.Ports;
using Contratacao.Service.Domain.Entities;
using Contratacao.Service.Tests.Helpers;

namespace Contratacao.Service.Tests.Services;

public class ContratacaoServiceTests
{
    private readonly Mock<IPropostaServicePort> _mockPropostaServicePort;
    private readonly Mock<IContratacaoSaidaPort> _mockContratacaoSaidaPort;
    private readonly Application.Services.ContratacaoService _service;

    public ContratacaoServiceTests()
    {
        _mockPropostaServicePort = new Mock<IPropostaServicePort>();
        _mockContratacaoSaidaPort = new Mock<IContratacaoSaidaPort>();
        _service = new Application.Services.ContratacaoService(_mockPropostaServicePort.Object, _mockContratacaoSaidaPort.Object);
    }

    [Fact]
    public async Task ContratarSeguroAsync_ComPropostaAprovadaESalvarSucesso_DeveRetornarTrue()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        _mockPropostaServicePort.Setup(x => x.VerificarPropostaAprovadaAsync(propostaId))
                               .ReturnsAsync(true);
        _mockContratacaoSaidaPort.Setup(x => x.SalvarSeguroPropostaAsync(It.IsAny<ContratacaoProposta>()))
                                .ReturnsAsync(true);

        // Act
        var result = await _service.ContratarSeguroAsync(propostaId);

        // Assert
        result.Should().BeTrue();
        _mockPropostaServicePort.Verify(x => x.VerificarPropostaAprovadaAsync(propostaId), Times.Once);
        _mockContratacaoSaidaPort.Verify(x => x.SalvarSeguroPropostaAsync(It.IsAny<ContratacaoProposta>()), Times.Once);
    }

    [Fact]
    public async Task ContratarSeguroAsync_ComPropostaNaoAprovada_DeveRetornarFalse()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        _mockPropostaServicePort.Setup(x => x.VerificarPropostaAprovadaAsync(propostaId))
                               .ReturnsAsync(false);

        // Act
        var result = await _service.ContratarSeguroAsync(propostaId);

        // Assert
        result.Should().BeFalse();
        _mockPropostaServicePort.Verify(x => x.VerificarPropostaAprovadaAsync(propostaId), Times.Once);
        _mockContratacaoSaidaPort.Verify(x => x.SalvarSeguroPropostaAsync(It.IsAny<ContratacaoProposta>()), Times.Never);
    }

    [Fact]
    public async Task ContratarSeguroAsync_ComPropostaAprovadaMasFalhaSalvar_DeveRetornarFalse()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        _mockPropostaServicePort.Setup(x => x.VerificarPropostaAprovadaAsync(propostaId))
                               .ReturnsAsync(true);
        _mockContratacaoSaidaPort.Setup(x => x.SalvarSeguroPropostaAsync(It.IsAny<ContratacaoProposta>()))
                                .ReturnsAsync(false);

        // Act
        var result = await _service.ContratarSeguroAsync(propostaId);

        // Assert
        result.Should().BeFalse();
        _mockPropostaServicePort.Verify(x => x.VerificarPropostaAprovadaAsync(propostaId), Times.Once);
        _mockContratacaoSaidaPort.Verify(x => x.SalvarSeguroPropostaAsync(It.IsAny<ContratacaoProposta>()), Times.Once);
    }

    [Fact]
    public async Task ContratarSeguroAsync_ComExcecaoNoPropostaService_DeveLancarExcecao()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        var exception = new InvalidOperationException("Erro no serviço de proposta");
        _mockPropostaServicePort.Setup(x => x.VerificarPropostaAprovadaAsync(propostaId))
                               .ThrowsAsync(exception);

        // Act & Assert
        var act = async () => await _service.ContratarSeguroAsync(propostaId);
        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("Erro no serviço de proposta");

        _mockPropostaServicePort.Verify(x => x.VerificarPropostaAprovadaAsync(propostaId), Times.Once);
        _mockContratacaoSaidaPort.Verify(x => x.SalvarSeguroPropostaAsync(It.IsAny<ContratacaoProposta>()), Times.Never);
    }

    [Fact]
    public async Task ContratarSeguroAsync_ComExcecaoNoSalvar_DeveLancarExcecao()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        var exception = new InvalidOperationException("Erro ao salvar contratação");
        _mockPropostaServicePort.Setup(x => x.VerificarPropostaAprovadaAsync(propostaId))
                               .ReturnsAsync(true);
        _mockContratacaoSaidaPort.Setup(x => x.SalvarSeguroPropostaAsync(It.IsAny<ContratacaoProposta>()))
                                .ThrowsAsync(exception);

        // Act & Assert
        var act = async () => await _service.ContratarSeguroAsync(propostaId);
        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("Erro ao salvar contratação");

        _mockPropostaServicePort.Verify(x => x.VerificarPropostaAprovadaAsync(propostaId), Times.Once);
        _mockContratacaoSaidaPort.Verify(x => x.SalvarSeguroPropostaAsync(It.IsAny<ContratacaoProposta>()), Times.Once);
    }

    [Fact]
    public async Task ContratarSeguroAsync_DeveCriarContratacaoPropostaComParametrosCorretos()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        ContratacaoProposta? capturedProposta = null;
        var dataAntes = DateTime.UtcNow;

        _mockPropostaServicePort.Setup(x => x.VerificarPropostaAprovadaAsync(propostaId))
                               .ReturnsAsync(true);
        _mockContratacaoSaidaPort.Setup(x => x.SalvarSeguroPropostaAsync(It.IsAny<ContratacaoProposta>()))
                                .Callback<ContratacaoProposta>(p => capturedProposta = p)
                                .ReturnsAsync(true);

        // Act
        await _service.ContratarSeguroAsync(propostaId);
        var dataDepois = DateTime.UtcNow;

        // Assert
        capturedProposta.Should().NotBeNull();
        capturedProposta!.PropostaId.Should().Be(propostaId);
        capturedProposta.Id.Should().NotBe(Guid.Empty);
        capturedProposta.Data.Should().BeAfter(dataAntes.AddSeconds(-1))
                             .And.BeBefore(dataDepois.AddSeconds(1));
    }

    [Fact]
    public async Task ContratarSeguroAsync_ComGuidVazio_DeveProcessarNormalmente()
    {
        // Arrange
        var propostaId = Guid.Empty;
        _mockPropostaServicePort.Setup(x => x.VerificarPropostaAprovadaAsync(propostaId))
                               .ReturnsAsync(false);

        // Act
        var result = await _service.ContratarSeguroAsync(propostaId);

        // Assert
        result.Should().BeFalse();
        _mockPropostaServicePort.Verify(x => x.VerificarPropostaAprovadaAsync(propostaId), Times.Once);
    }
}