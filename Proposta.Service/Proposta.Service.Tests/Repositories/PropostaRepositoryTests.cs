using FluentAssertions;
using Proposta.Service.Domain.Entities;
using Proposta.Service.Domain.Enums;
using Proposta.Service.Infrastructure.Repositories;
using Proposta.Service.Tests.Helpers;
using Xunit;

namespace Proposta.Service.Tests.Repositories;

public class PropostaRepositoryTests : IDisposable
{
    private readonly Infrastructure.Context.AppDbContext _context;
    private readonly PropostaRepository _repository;

    public PropostaRepositoryTests()
    {
        _context = DbContextHelper.CreateInMemoryContext();
        _repository = new PropostaRepository(_context);
    }

    [Fact]
    public async Task CriarPropostaAsync_DeveRetornarSeguroPropostaCriada()
    {
        // Arrange
        var seguroProposta = TestDataHelper.CriarSeguroPropostaValida();

        // Act
        var resultado = await _repository.CriarPropostaAsync(seguroProposta);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(seguroProposta.Id);
        resultado.Segurado.Should().Be(seguroProposta.Segurado);
        resultado.Valor.Should().Be(seguroProposta.Valor);
        resultado.Status.Should().Be(seguroProposta.Status);

        // Verificar se foi salvo no banco
        var propostaNoBanco = await _context.Propostas.FindAsync(seguroProposta.Id);
        propostaNoBanco.Should().NotBeNull();
        propostaNoBanco!.Id.Should().Be(seguroProposta.Id);
    }

    [Fact]
    public async Task ObterPropostaPorIdAsync_ComIdExistente_DeveRetornarSeguroProposta()
    {
        // Arrange
        var id = Guid.NewGuid();
        var propostaInfra = TestDataHelper.CriarPropostaInfraEntityComId(id);
        await _context.Propostas.AddAsync(propostaInfra);
        await _context.SaveChangesAsync();

        // Act
        var resultado = await _repository.ObterPropostaPorIdAsync(id);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(id);
        resultado.Segurado.Should().Be(propostaInfra.Segurado);
        resultado.Valor.Should().Be(propostaInfra.Valor);
        resultado.Status.Should().Be(propostaInfra.Status);
    }

    [Fact]
    public async Task ObterPropostaPorIdAsync_ComIdInexistente_DeveRetornarNull()
    {
        // Arrange
        var idInexistente = Guid.NewGuid();

        // Act
        var resultado = await _repository.ObterPropostaPorIdAsync(idInexistente);

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task ObterTodasPropostasAsync_ComPropostasExistentes_DeveRetornarResponsePaginado()
    {
        // Arrange
        var propostas = TestDataHelper.CriarListaPropostasInfra(5);
        await _context.Propostas.AddRangeAsync(propostas);
        await _context.SaveChangesAsync();

        var pagina = 1;
        var tamanhoPagina = 3;

        // Act
        var resultado = await _repository.ObterTodasPropostasAsync(pagina, tamanhoPagina);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Propostas.Should().HaveCount(3);
        resultado.PaginaAtual.Should().Be(1);
        resultado.TotalRegistros.Should().Be(5);
        resultado.TotalPaginas.Should().Be(2);
        resultado.TotalPropostas.Should().Be(5);
    }

    [Fact]
    public async Task ObterTodasPropostasAsync_SemPropostas_DeveRetornarResponseVazio()
    {
        // Arrange
        var pagina = 1;
        var tamanhoPagina = 10;

        // Act
        var resultado = await _repository.ObterTodasPropostasAsync(pagina, tamanhoPagina);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Propostas.Should().BeEmpty();
        resultado.PaginaAtual.Should().Be(1);
        resultado.TotalRegistros.Should().Be(0);
        resultado.TotalPaginas.Should().Be(0);
        resultado.TotalPropostas.Should().Be(0);
    }

    [Fact]
    public async Task AtualizarStatusPropostaAsync_ComIdExistente_DeveRetornarTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var propostaInfra = TestDataHelper.CriarPropostaInfraEntityComId(id);
        await _context.Propostas.AddAsync(propostaInfra);
        await _context.SaveChangesAsync();

        var novoStatus = (int)StatusPropostaEnum.Aprovada;

        // Act
        var resultado = await _repository.AtualizarStatusPropostaAsync(id, novoStatus);

        // Assert
        resultado.Should().BeTrue();

        // Verificar se foi atualizado no banco
        var propostaAtualizada = await _context.Propostas.FindAsync(id);
        propostaAtualizada.Should().NotBeNull();
        propostaAtualizada!.Status.Should().Be(novoStatus);
    }

    [Fact]
    public async Task AtualizarStatusPropostaAsync_ComIdInexistente_DeveRetornarFalse()
    {
        // Arrange
        var idInexistente = Guid.NewGuid();
        var novoStatus = (int)StatusPropostaEnum.Rejeitada;

        // Act
        var resultado = await _repository.AtualizarStatusPropostaAsync(idInexistente, novoStatus);

        // Assert
        resultado.Should().BeFalse();
    }

    [Theory]
    [InlineData(1, 2, 5, 2)] // página 1, tamanho 2, total 5, esperado 2 itens
    [InlineData(2, 2, 5, 2)] // página 2, tamanho 2, total 5, esperado 2 itens
    [InlineData(3, 2, 5, 1)] // página 3, tamanho 2, total 5, esperado 1 item
    [InlineData(4, 2, 5, 0)] // página 4, tamanho 2, total 5, esperado 0 itens
    public async Task ObterTodasPropostasAsync_ComDiferentesPaginacoes_DeveRetornarQuantidadeCorreta(
        int pagina, int tamanhoPagina, int totalItens, int itensEsperados)
    {
        // Arrange
        var propostas = TestDataHelper.CriarListaPropostasInfra(totalItens);
        await _context.Propostas.AddRangeAsync(propostas);
        await _context.SaveChangesAsync();

        // Act
        var resultado = await _repository.ObterTodasPropostasAsync(pagina, tamanhoPagina);

        // Assert
        resultado.Propostas.Should().HaveCount(itensEsperados);
        resultado.TotalRegistros.Should().Be(totalItens);
        resultado.PaginaAtual.Should().Be(pagina);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}