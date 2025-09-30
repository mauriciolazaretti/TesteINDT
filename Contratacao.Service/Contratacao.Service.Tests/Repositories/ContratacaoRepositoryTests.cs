using Xunit;
using FluentAssertions;
using Contratacao.Service.Infrastructure.Repositories;
using Contratacao.Service.Tests.Helpers;
using Contratacao.Service.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Contratacao.Service.Tests.Repositories;

public class ContratacaoRepositoryTests : IDisposable
{
    private readonly Infrastructure.Context.AppDbContext _context;
    private readonly ContratacaoRepository _repository;

    public ContratacaoRepositoryTests()
    {
        _context = TestDbContextHelper.CreateInMemoryDbContext();
        _repository = new ContratacaoRepository(_context);
    }

    [Fact]
    public async Task SalvarSeguroPropostaAsync_ComPropostaValida_DeveRetornarTrue()
    {
        // Arrange
        var contratacaoProposta = TestDataHelper.GetValidContratacaoProposta();

        // Act
        var result = await _repository.SalvarSeguroPropostaAsync(contratacaoProposta);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task SalvarSeguroPropostaAsync_ComPropostaValida_DeveSalvarNoBanco()
    {
        // Arrange
        var contratacaoProposta = TestDataHelper.GetValidContratacaoProposta();

        // Act
        await _repository.SalvarSeguroPropostaAsync(contratacaoProposta);

        // Assert
        var savedEntity = await _context.ContratacaoPropostas
            .FirstOrDefaultAsync(x => x.Id == contratacaoProposta.Id);
        
        savedEntity.Should().NotBeNull();
        savedEntity!.Id.Should().Be(contratacaoProposta.Id);
        savedEntity.PropostaId.Should().Be(contratacaoProposta.PropostaId);
        savedEntity.Data.Should().BeCloseTo(contratacaoProposta.Data, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task SalvarSeguroPropostaAsync_ComPropostaValida_DeveIncrementarContador()
    {
        // Arrange
        var contratacaoProposta1 = TestDataHelper.GetValidContratacaoProposta();
        var contratacaoProposta2 = new ContratacaoProposta
        {
            Id = Guid.NewGuid(),
            PropostaId = Guid.NewGuid(),
            Data = DateTime.UtcNow
        };

        // Act
        await _repository.SalvarSeguroPropostaAsync(contratacaoProposta1);
        await _repository.SalvarSeguroPropostaAsync(contratacaoProposta2);

        // Assert
        var count = await _context.ContratacaoPropostas.CountAsync();
        count.Should().Be(2);
    }

    [Fact]
    public async Task SalvarSeguroPropostaAsync_ComIdDuplicado_DeveLancarExcecao()
    {
        // Arrange
        var contratacaoProposta = TestDataHelper.GetValidContratacaoProposta();
        var contratacaoPropostaDuplicada = new ContratacaoProposta
        {
            Id = contratacaoProposta.Id, // Mesmo ID
            PropostaId = Guid.NewGuid(),
            Data = DateTime.UtcNow
        };

        // Act & Assert
        await _repository.SalvarSeguroPropostaAsync(contratacaoProposta);
        
        var act = async () => await _repository.SalvarSeguroPropostaAsync(contratacaoPropostaDuplicada);
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task SalvarSeguroPropostaAsync_ComPropostaComIdValido_DeveFuncionar()
    {
        // Arrange
        var id = Guid.NewGuid();
        var contratacaoProposta = new ContratacaoProposta
        {
            Id = id,
            PropostaId = Guid.NewGuid(),
            Data = DateTime.UtcNow
        };

        // Act
        var result = await _repository.SalvarSeguroPropostaAsync(contratacaoProposta);

        // Assert
        result.Should().BeTrue();
        
        var savedEntity = await _context.ContratacaoPropostas
            .FirstOrDefaultAsync(x => x.Id == id);
        savedEntity.Should().NotBeNull();
        savedEntity!.Id.Should().Be(id);
    }

    [Fact]
    public async Task SalvarSeguroPropostaAsync_ComDataFutura_DeveFuncionar()
    {
        // Arrange
        var dataFutura = DateTime.UtcNow.AddDays(30);
        var contratacaoProposta = new ContratacaoProposta
        {
            Id = Guid.NewGuid(),
            PropostaId = Guid.NewGuid(),
            Data = dataFutura
        };

        // Act
        var result = await _repository.SalvarSeguroPropostaAsync(contratacaoProposta);

        // Assert
        result.Should().BeTrue();
        
        var savedEntity = await _context.ContratacaoPropostas
            .FirstOrDefaultAsync(x => x.Id == contratacaoProposta.Id);
        savedEntity!.Data.Should().BeCloseTo(dataFutura, TimeSpan.FromSeconds(1));
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}