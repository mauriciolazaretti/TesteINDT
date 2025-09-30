using FluentAssertions;
using Moq;
using Proposta.Service.Application.Adapters;
using Proposta.Service.Domain.Enums;
using Proposta.Service.Domain.Repositories;
using Xunit;

namespace Proposta.Service.Tests.Adapters;

public class PropostaSaidaAdapterTests
{
    private readonly Mock<IPropostaRepository> _mockRepository;
    private readonly PropostaSaidaAdapter _adapter;

    public PropostaSaidaAdapterTests()
    {
        _mockRepository = new Mock<IPropostaRepository>();
        _adapter = new PropostaSaidaAdapter(_mockRepository.Object);
    }

    [Fact]
    public async Task AtualizarStatusPropostaAsync_ComIdExistente_DeveRetornarTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var status = StatusPropostaEnum.Aprovada;
        var statusInt = (int)status;

        _mockRepository
            .Setup(r => r.AtualizarStatusPropostaAsync(id, statusInt))
            .ReturnsAsync(true);

        // Act
        var resultado = await _adapter.AtualizarStatusPropostaAsync(id, status);

        // Assert
        resultado.Should().BeTrue();
        _mockRepository.Verify(r => r.AtualizarStatusPropostaAsync(id, statusInt), Times.Once);
    }

    [Fact]
    public async Task AtualizarStatusPropostaAsync_ComIdInexistente_DeveRetornarFalse()
    {
        // Arrange
        var id = Guid.NewGuid();
        var status = StatusPropostaEnum.Rejeitada;
        var statusInt = (int)status;

        _mockRepository
            .Setup(r => r.AtualizarStatusPropostaAsync(id, statusInt))
            .ReturnsAsync(false);

        // Act
        var resultado = await _adapter.AtualizarStatusPropostaAsync(id, status);

        // Assert
        resultado.Should().BeFalse();
        _mockRepository.Verify(r => r.AtualizarStatusPropostaAsync(id, statusInt), Times.Once);
    }

    [Theory]
    [InlineData(StatusPropostaEnum.EmAnalise, 0)]
    [InlineData(StatusPropostaEnum.Aprovada, 1)]
    [InlineData(StatusPropostaEnum.Rejeitada, 2)]
    public async Task AtualizarStatusPropostaAsync_DeveMapeArStatusEnumParaInt(StatusPropostaEnum statusEnum, int statusIntEsperado)
    {
        // Arrange
        var id = Guid.NewGuid();

        _mockRepository
            .Setup(r => r.AtualizarStatusPropostaAsync(id, statusIntEsperado))
            .ReturnsAsync(true);

        // Act
        var resultado = await _adapter.AtualizarStatusPropostaAsync(id, statusEnum);

        // Assert
        resultado.Should().BeTrue();
        _mockRepository.Verify(r => r.AtualizarStatusPropostaAsync(id, statusIntEsperado), Times.Once);
    }

    [Fact]
    public async Task AtualizarStatusPropostaAsync_QuandoRepositoryLancaExcecao_DevePropagarExcecao()
    {
        // Arrange
        var id = Guid.NewGuid();
        var status = StatusPropostaEnum.Aprovada;
        var statusInt = (int)status;
        var excecaoEsperada = new InvalidOperationException("Erro no banco de dados");

        _mockRepository
            .Setup(r => r.AtualizarStatusPropostaAsync(id, statusInt))
            .ThrowsAsync(excecaoEsperada);

        // Act & Assert
        var excecao = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _adapter.AtualizarStatusPropostaAsync(id, status)
        );

        excecao.Message.Should().Be("Erro no banco de dados");
        _mockRepository.Verify(r => r.AtualizarStatusPropostaAsync(id, statusInt), Times.Once);
    }

    [Fact]
    public async Task AtualizarStatusPropostaAsync_ComGuidVazio_DeveChamarRepositoryComGuidVazio()
    {
        // Arrange
        var id = Guid.Empty;
        var status = StatusPropostaEnum.EmAnalise;
        var statusInt = (int)status;

        _mockRepository
            .Setup(r => r.AtualizarStatusPropostaAsync(id, statusInt))
            .ReturnsAsync(false);

        // Act
        var resultado = await _adapter.AtualizarStatusPropostaAsync(id, status);

        // Assert
        resultado.Should().BeFalse();
        _mockRepository.Verify(r => r.AtualizarStatusPropostaAsync(Guid.Empty, statusInt), Times.Once);
    }

    [Fact]
    public async Task AtualizarStatusPropostaAsync_ComMultiplasChamadas_DeveChamarRepositoryCorretamente()
    {
        // Arrange
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var status1 = StatusPropostaEnum.Aprovada;
        var status2 = StatusPropostaEnum.Rejeitada;

        _mockRepository
            .Setup(r => r.AtualizarStatusPropostaAsync(id1, (int)status1))
            .ReturnsAsync(true);

        _mockRepository
            .Setup(r => r.AtualizarStatusPropostaAsync(id2, (int)status2))
            .ReturnsAsync(true);

        // Act
        var resultado1 = await _adapter.AtualizarStatusPropostaAsync(id1, status1);
        var resultado2 = await _adapter.AtualizarStatusPropostaAsync(id2, status2);

        // Assert
        resultado1.Should().BeTrue();
        resultado2.Should().BeTrue();

        _mockRepository.Verify(r => r.AtualizarStatusPropostaAsync(id1, (int)status1), Times.Once);
        _mockRepository.Verify(r => r.AtualizarStatusPropostaAsync(id2, (int)status2), Times.Once);
        _mockRepository.Verify(r => r.AtualizarStatusPropostaAsync(It.IsAny<Guid>(), It.IsAny<int>()), Times.Exactly(2));
    }
}