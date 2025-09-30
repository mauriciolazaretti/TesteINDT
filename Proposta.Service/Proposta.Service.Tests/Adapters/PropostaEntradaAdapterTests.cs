using FluentAssertions;
using Moq;
using Proposta.Service.Application.Adapters;
using Proposta.Service.Domain.Entities;
using Proposta.Service.Domain.Enums;
using Proposta.Service.Domain.Repositories;
using Proposta.Service.Domain.Response;
using Proposta.Service.Tests.Helpers;
using Xunit;

namespace Proposta.Service.Tests.Adapters;

public class PropostaEntradaAdapterTests
{
    private readonly Mock<IPropostaRepository> _mockRepository;
    private readonly PropostaEntradaAdapter _adapter;

    public PropostaEntradaAdapterTests()
    {
        _mockRepository = new Mock<IPropostaRepository>();
        _adapter = new PropostaEntradaAdapter(_mockRepository.Object);
    }

    [Fact]
    public async Task CriarPropostaAsync_DeveMapearCorretamenteEChamarRepository()
    {
        // Arrange
        var propostaEntity = TestDataHelper.CriarSeguroPropostaValida();
        var seguroPropostaRetornada = TestDataHelper.CriarSeguroPropostaComId(propostaEntity.Id);

        _mockRepository
            .Setup(r => r.CriarPropostaAsync(It.IsAny<SeguroProposta>()))
            .ReturnsAsync(seguroPropostaRetornada);

        // Act
        var resultado = await _adapter.CriarPropostaAsync(propostaEntity);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(seguroPropostaRetornada.Id);
        resultado.Segurado.Should().Be(seguroPropostaRetornada.Segurado);
        resultado.Valor.Should().Be(seguroPropostaRetornada.Valor);
        resultado.Status.Should().Be(seguroPropostaRetornada.Status);

        // Verificar se o repository foi chamado com os dados corretos
        _mockRepository.Verify(r => r.CriarPropostaAsync(It.Is<SeguroProposta>(s =>
            s.Id == propostaEntity.Id &&
            s.Segurado == propostaEntity.Segurado &&
            s.Valor == propostaEntity.Valor &&
            s.Status == (int)propostaEntity.Status
        )), Times.Once);
    }

    [Fact]
    public async Task ObterPropostaPorIdAsync_ComIdExistente_DeveRetornarPropostaEntityMapeada()
    {
        // Arrange
        var id = Guid.NewGuid();
        var seguroProposta = TestDataHelper.CriarSeguroPropostaComId(id);

        _mockRepository
            .Setup(r => r.ObterPropostaPorIdAsync(id))
            .ReturnsAsync(seguroProposta);

        // Act
        var resultado = await _adapter.ObterPropostaPorIdAsync(id);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(seguroProposta.Id);
        resultado.Segurado.Should().Be(seguroProposta.Segurado);
        resultado.Valor.Should().Be(seguroProposta.Valor);
        resultado.Status.Should().Be(seguroProposta.Status);

        _mockRepository.Verify(r => r.ObterPropostaPorIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task ObterPropostaPorIdAsync_ComIdInexistente_DeveRetornarNull()
    {
        // Arrange
        var id = Guid.NewGuid();

        _mockRepository
            .Setup(r => r.ObterPropostaPorIdAsync(id))
            .ReturnsAsync((SeguroProposta?)null);

        // Act
        var resultado = await _adapter.ObterPropostaPorIdAsync(id);

        // Assert
        resultado.Should().BeNull();
        _mockRepository.Verify(r => r.ObterPropostaPorIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task ObterTodasPropostasAsync_DeveMapearResponseCorretamente()
    {
        // Arrange
        var pagina = 1;
        var tamanhoPagina = 2;
        var seguroPropostas = new List<SeguroProposta>
        {
            TestDataHelper.CriarSeguroPropostaValida(),
            TestDataHelper.CriarSeguroPropostaValida()
        };

        var responseSeguroProposta = new ResponseSeguroProposta
        {
            Propostas = seguroPropostas,
            PaginaAtual = pagina,
            TotalPaginas = 3,
            TotalRegistros = 5,
            TotalPropostas = 5
        };

        _mockRepository
            .Setup(r => r.ObterTodasPropostasAsync(pagina, tamanhoPagina))
            .ReturnsAsync(responseSeguroProposta);

        // Act
        var resultado = await _adapter.ObterTodasPropostasAsync(pagina, tamanhoPagina);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Propostas.Should().HaveCount(2);
        resultado.PaginaAtual.Should().Be(pagina);
        resultado.TotalPaginas.Should().Be(3);
        resultado.TotalRegistros.Should().Be(5);
        resultado.TotalPropostas.Should().Be(5);

        // Verificar mapeamento das propostas
        var primeiraProposta = resultado.Propostas.First();
        var primeiraSeguroProposta = seguroPropostas.First();
        
        primeiraProposta.Id.Should().Be(primeiraSeguroProposta.Id);
        primeiraProposta.Segurado.Should().Be(primeiraSeguroProposta.Segurado);
        primeiraProposta.Valor.Should().Be(primeiraSeguroProposta.Valor);
        primeiraProposta.Status.Should().Be(primeiraSeguroProposta.Status);

        _mockRepository.Verify(r => r.ObterTodasPropostasAsync(pagina, tamanhoPagina), Times.Once);
    }

    [Fact]
    public async Task ObterTodasPropostasAsync_ComListaVazia_DeveRetornarResponseVazio()
    {
        // Arrange
        var pagina = 1;
        var tamanhoPagina = 10;

        var responseSeguroProposta = new ResponseSeguroProposta
        {
            Propostas = new List<SeguroProposta>(),
            PaginaAtual = pagina,
            TotalPaginas = 0,
            TotalRegistros = 0,
            TotalPropostas = 0
        };

        _mockRepository
            .Setup(r => r.ObterTodasPropostasAsync(pagina, tamanhoPagina))
            .ReturnsAsync(responseSeguroProposta);

        // Act
        var resultado = await _adapter.ObterTodasPropostasAsync(pagina, tamanhoPagina);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Propostas.Should().BeEmpty();
        resultado.PaginaAtual.Should().Be(pagina);
        resultado.TotalPaginas.Should().Be(0);
        resultado.TotalRegistros.Should().Be(0);
        resultado.TotalPropostas.Should().Be(0);
    }

    [Theory]
    [InlineData(StatusPropostaEnum.EmAnalise, 0)]
    [InlineData(StatusPropostaEnum.Aprovada, 1)]
    [InlineData(StatusPropostaEnum.Rejeitada, 2)]
    public async Task CriarPropostaAsync_DeveMapearsStatusCorretamente(StatusPropostaEnum statusEnum, int statusInt)
    {
        // Arrange
        var proposta = TestDataHelper.CriarSeguroPropostaValida();
        proposta.Status = statusInt; // Sempre iniciar como EmAnalise


        _mockRepository
            .Setup(r => r.CriarPropostaAsync(It.IsAny<SeguroProposta>()))
            .ReturnsAsync(proposta);

        // Act
        var resultado = await _adapter.CriarPropostaAsync(proposta);

        // Assert
        resultado.Status.Should().Be((int)statusEnum);

        _mockRepository.Verify(r => r.CriarPropostaAsync(It.Is<SeguroProposta>(s =>
            s.Status == statusInt
        )), Times.Once);
    }

    [Fact]
    public async Task ObterTodasPropostasAsync_ComPropostasNull_DeveRetornarListaVazia()
    {
        // Arrange
        var pagina = 1;
        var tamanhoPagina = 10;

        var responseSeguroProposta = new ResponseSeguroProposta
        {
            Propostas = new List<SeguroProposta>(), // Simular caso onde Propostas é null
            PaginaAtual = pagina,
            TotalPaginas = 0,
            TotalRegistros = 0,
            TotalPropostas = 0
        };

        _mockRepository
            .Setup(r => r.ObterTodasPropostasAsync(pagina, tamanhoPagina))
            .ReturnsAsync(responseSeguroProposta);

        // Act
        var resultado = await _adapter.ObterTodasPropostasAsync(pagina, tamanhoPagina);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Propostas.Should().NotBeNull();
        resultado.Propostas.Should().BeEmpty();
    }
}