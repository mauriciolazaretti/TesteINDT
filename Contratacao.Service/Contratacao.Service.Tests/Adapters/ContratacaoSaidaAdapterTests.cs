using Xunit;
using FluentAssertions;
using Moq;
using Contratacao.Service.Application.Adapters;
using Contratacao.Service.Tests.Helpers;
using Contratacao.Service.Domain.Entities;
using Contratacao.Service.Domain.Repositories;

namespace Contratacao.Service.Tests.Adapters;

public class ContratacaoSaidaAdapterTests
{
    private readonly Mock<IContratacaoRepository> _mockRepository;
    private readonly ContratacaoSaidaAdapter _adapter;

    public ContratacaoSaidaAdapterTests()
    {
        _mockRepository = new Mock<IContratacaoRepository>();
        _adapter = new ContratacaoSaidaAdapter(_mockRepository.Object);
    }

    [Fact]
    public async Task SalvarSeguroPropostaAsync_ComPropostaValida_DeveRetornarTrue()
    {
        // Arrange
        var contratacaoProposta = TestDataHelper.GetValidContratacaoProposta();
        _mockRepository.Setup(x => x.SalvarSeguroPropostaAsync(It.IsAny<ContratacaoProposta>()))
                      .ReturnsAsync(true);

        // Act
        var result = await _adapter.SalvarSeguroPropostaAsync(contratacaoProposta);

        // Assert
        result.Should().BeTrue();
        _mockRepository.Verify(x => x.SalvarSeguroPropostaAsync(contratacaoProposta), Times.Once);
    }

    [Fact]
    public async Task SalvarSeguroPropostaAsync_ComFalhaNoRepository_DeveRetornarFalse()
    {
        // Arrange
        var contratacaoProposta = TestDataHelper.GetValidContratacaoProposta();
        _mockRepository.Setup(x => x.SalvarSeguroPropostaAsync(It.IsAny<ContratacaoProposta>()))
                      .ReturnsAsync(false);

        // Act
        var result = await _adapter.SalvarSeguroPropostaAsync(contratacaoProposta);

        // Assert
        result.Should().BeFalse();
        _mockRepository.Verify(x => x.SalvarSeguroPropostaAsync(contratacaoProposta), Times.Once);
    }

    [Fact]
    public async Task SalvarSeguroPropostaAsync_ComExcecaoNoRepository_DeveLancarExcecao()
    {
        // Arrange
        var contratacaoProposta = TestDataHelper.GetValidContratacaoProposta();
        var exception = new InvalidOperationException("Erro de teste no repository");
        _mockRepository.Setup(x => x.SalvarSeguroPropostaAsync(It.IsAny<ContratacaoProposta>()))
                      .ThrowsAsync(exception);

        // Act & Assert
        var act = async () => await _adapter.SalvarSeguroPropostaAsync(contratacaoProposta);
        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("Erro de teste no repository");
        
        _mockRepository.Verify(x => x.SalvarSeguroPropostaAsync(contratacaoProposta), Times.Once);
    }

    [Fact]
    public async Task SalvarSeguroPropostaAsync_ComPropostaNula_DeveLancarExcecao()
    {
        // Arrange
        ContratacaoProposta? propostaNula = null;
        _mockRepository.Setup(x => x.SalvarSeguroPropostaAsync(It.IsAny<ContratacaoProposta>()))
                      .ThrowsAsync(new ArgumentNullException(nameof(propostaNula)));

        // Act & Assert
        var act = async () => await _adapter.SalvarSeguroPropostaAsync(propostaNula!);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task SalvarSeguroPropostaAsync_DevePassarParametrosCorretos()
    {
        // Arrange
        var contratacaoProposta = TestDataHelper.GetValidContratacaoProposta();
        ContratacaoProposta? capturedProposta = null;
        
        _mockRepository.Setup(x => x.SalvarSeguroPropostaAsync(It.IsAny<ContratacaoProposta>()))
                      .Callback<ContratacaoProposta>(p => capturedProposta = p)
                      .ReturnsAsync(true);

        // Act
        await _adapter.SalvarSeguroPropostaAsync(contratacaoProposta);

        // Assert
        capturedProposta.Should().NotBeNull();
        capturedProposta!.Id.Should().Be(contratacaoProposta.Id);
        capturedProposta.PropostaId.Should().Be(contratacaoProposta.PropostaId);
        capturedProposta.Data.Should().BeCloseTo(contratacaoProposta.Data, TimeSpan.FromSeconds(1));
    }
}