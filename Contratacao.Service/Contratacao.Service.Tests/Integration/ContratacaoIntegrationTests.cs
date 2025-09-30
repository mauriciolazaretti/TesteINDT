using Xunit;
using FluentAssertions;
using Contratacao.Service.Application.Services;
using Contratacao.Service.Application.Adapters;
using Contratacao.Service.Infrastructure.Repositories;
using Contratacao.Service.Tests.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Net;

namespace Contratacao.Service.Tests.Integration;

public class ContratacaoIntegrationTests : IDisposable
{
    private readonly Infrastructure.Context.AppDbContext _context;
    private readonly Mock<ILogger<PropostaServiceAdapter>> _mockLogger;

    public ContratacaoIntegrationTests()
    {
        _context = TestDbContextHelper.CreateInMemoryDbContext();
        _mockLogger = MockHelper.CreateMockLogger<PropostaServiceAdapter>();
    }

    [Fact]
    public async Task FluxoCompleto_ContratarSeguro_ComPropostaAprovada_DeveRealizarContratacao()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        var seguroProposta = TestDataHelper.GetValidSeguroProposta(status: 1); // APROVADA
        var httpClient = HttpClientTestHelper.CreateMockHttpClient(HttpStatusCode.OK, seguroProposta);
        var configuration = MockHelper.CreateConfiguration(new Dictionary<string, string>
        {
            ["Services:PropostaService:BaseUrl"] = "http://localhost:5000/api/proposta"
        });

        // Criando os componentes reais da arquitetura hexagonal
        var repository = new ContratacaoRepository(_context);
        var contratacaoSaidaAdapter = new ContratacaoSaidaAdapter(repository);
        var propostaServiceAdapter = new PropostaServiceAdapter(httpClient, configuration, _mockLogger.Object);
        var service = new Application.Services.ContratacaoService(propostaServiceAdapter, contratacaoSaidaAdapter);

        // Act
        var result = await service.ContratarSeguroAsync(propostaId);

        // Assert
        result.Should().BeTrue();

        // Verificar se foi salvo no banco
        var savedEntity = await _context.ContratacaoPropostas
            .FirstOrDefaultAsync(x => x.PropostaId == propostaId);
        
        savedEntity.Should().NotBeNull();
        savedEntity!.PropostaId.Should().Be(propostaId);
        savedEntity.Data.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task FluxoCompleto_ContratarSeguro_ComPropostaNaoAprovada_NaoDeveRealizarContratacao()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        var seguroProposta = TestDataHelper.GetValidSeguroProposta(status: 0); // NÃO APROVADA
        var httpClient = HttpClientTestHelper.CreateMockHttpClient(HttpStatusCode.OK, seguroProposta);
        var configuration = MockHelper.CreateConfiguration(new Dictionary<string, string>
        {
            ["Services:PropostaService:BaseUrl"] = "http://localhost:5000/api/proposta"
        });

        var repository = new ContratacaoRepository(_context);
        var contratacaoSaidaAdapter = new ContratacaoSaidaAdapter(repository);
        var propostaServiceAdapter = new PropostaServiceAdapter(httpClient, configuration, _mockLogger.Object);
        var service = new Application.Services.ContratacaoService(propostaServiceAdapter, contratacaoSaidaAdapter);

        // Act
        var result = await service.ContratarSeguroAsync(propostaId);

        // Assert
        result.Should().BeFalse();

        // Verificar se NÃO foi salvo no banco
        var savedEntity = await _context.ContratacaoPropostas
            .FirstOrDefaultAsync(x => x.PropostaId == propostaId);
        
        savedEntity.Should().BeNull();
    }

    [Fact]
    public async Task FluxoCompleto_ContratarSeguro_ComPropostaNaoEncontrada_NaoDeveRealizarContratacao()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        var httpClient = HttpClientTestHelper.CreateMockHttpClient(HttpStatusCode.NotFound, null);
        var configuration = MockHelper.CreateConfiguration(new Dictionary<string, string>
        {
            ["Services:PropostaService:BaseUrl"] = "http://localhost:5000/api/proposta"
        });

        var repository = new ContratacaoRepository(_context);
        var contratacaoSaidaAdapter = new ContratacaoSaidaAdapter(repository);
        var propostaServiceAdapter = new PropostaServiceAdapter(httpClient, configuration, _mockLogger.Object);
        var service = new Application.Services.ContratacaoService(propostaServiceAdapter, contratacaoSaidaAdapter);

        // Act
        var result = await service.ContratarSeguroAsync(propostaId);

        // Assert
        result.Should().BeFalse();

        // Verificar se NÃO foi salvo no banco
        var savedEntity = await _context.ContratacaoPropostas
            .FirstOrDefaultAsync(x => x.PropostaId == propostaId);
        
        savedEntity.Should().BeNull();
    }

    [Fact]
    public async Task FluxoCompleto_ContratarSeguro_ComErroHTTP_DeveLancarExcecao()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        var httpClient = HttpClientTestHelper.CreateMockHttpClient(HttpStatusCode.InternalServerError, null);
        var configuration = MockHelper.CreateConfiguration(new Dictionary<string, string>
        {
            ["Services:PropostaService:BaseUrl"] = "http://localhost:5000/api/proposta"
        });

        var repository = new ContratacaoRepository(_context);
        var contratacaoSaidaAdapter = new ContratacaoSaidaAdapter(repository);
        var propostaServiceAdapter = new PropostaServiceAdapter(httpClient, configuration, _mockLogger.Object);
        var service = new Application.Services.ContratacaoService(propostaServiceAdapter, contratacaoSaidaAdapter);

        // Act & Assert
        var act = async () => await service.ContratarSeguroAsync(propostaId);
        await act.Should().ThrowAsync<HttpRequestException>();

        // Verificar se NÃO foi salvo no banco
        var savedEntity = await _context.ContratacaoPropostas
            .FirstOrDefaultAsync(x => x.PropostaId == propostaId);
        
        savedEntity.Should().BeNull();
    }

    [Fact]
    public async Task FluxoCompleto_ContratarSeguro_ComConfigurationInvalida_DeveLancarExcecao()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        var httpClient = HttpClientTestHelper.CreateMockHttpClient(HttpStatusCode.OK, null);
        var configurationSemBaseUrl = MockHelper.CreateConfiguration(new Dictionary<string, string>());

        var repository = new ContratacaoRepository(_context);
        var contratacaoSaidaAdapter = new ContratacaoSaidaAdapter(repository);
        var propostaServiceAdapter = new PropostaServiceAdapter(httpClient, configurationSemBaseUrl, _mockLogger.Object);
        var service = new Application.Services.ContratacaoService(propostaServiceAdapter, contratacaoSaidaAdapter);

        // Act & Assert
        var act = async () => await service.ContratarSeguroAsync(propostaId);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Base URL for Proposta Service is not configured*");
    }

    [Fact]
    public async Task FluxoCompleto_MultipleContratacoes_DeveProcessarIndependentemente()
    {
        // Arrange
        var propostaId1 = Guid.NewGuid();
        var propostaId2 = Guid.NewGuid();
        var propostaId3 = Guid.NewGuid();
        
        var configuration = MockHelper.CreateConfiguration(new Dictionary<string, string>
        {
            ["Services:PropostaService:BaseUrl"] = "http://localhost:5000/api/proposta"
        });

        var repository = new ContratacaoRepository(_context);
        var contratacaoSaidaAdapter = new ContratacaoSaidaAdapter(repository);

        // Configurar diferentes respostas para diferentes propostas
        var seguroPropostaAprovada = TestDataHelper.GetValidSeguroProposta(status: 1);
        var seguroPropostaNaoAprovada = TestDataHelper.GetValidSeguroProposta(status: 0);

        // Act & Assert - Proposta 1 (Aprovada)
        var httpClient1 = HttpClientTestHelper.CreateMockHttpClient(HttpStatusCode.OK, seguroPropostaAprovada);
        var propostaServiceAdapter1 = new PropostaServiceAdapter(httpClient1, configuration, _mockLogger.Object);
        var service1 = new Application.Services.ContratacaoService(propostaServiceAdapter1, contratacaoSaidaAdapter);
        
        var result1 = await service1.ContratarSeguroAsync(propostaId1);
        result1.Should().BeTrue();

        // Act & Assert - Proposta 2 (Não Aprovada)
        var httpClient2 = HttpClientTestHelper.CreateMockHttpClient(HttpStatusCode.OK, seguroPropostaNaoAprovada);
        var propostaServiceAdapter2 = new PropostaServiceAdapter(httpClient2, configuration, _mockLogger.Object);
        var service2 = new Application.Services.ContratacaoService(propostaServiceAdapter2, contratacaoSaidaAdapter);
        
        var result2 = await service2.ContratarSeguroAsync(propostaId2);
        result2.Should().BeFalse();

        // Act & Assert - Proposta 3 (Não Encontrada)
        var httpClient3 = HttpClientTestHelper.CreateMockHttpClient(HttpStatusCode.NotFound, null);
        var propostaServiceAdapter3 = new PropostaServiceAdapter(httpClient3, configuration, _mockLogger.Object);
        var service3 = new Application.Services.ContratacaoService(propostaServiceAdapter3, contratacaoSaidaAdapter);
        
        var result3 = await service3.ContratarSeguroAsync(propostaId3);
        result3.Should().BeFalse();

        // Verificar estado final do banco - apenas a primeira deve estar salva
        var totalSalvas = await _context.ContratacaoPropostas.CountAsync();
        totalSalvas.Should().Be(1);

        var proposta1Salva = await _context.ContratacaoPropostas
            .FirstOrDefaultAsync(x => x.PropostaId == propostaId1);
        proposta1Salva.Should().NotBeNull();

        var proposta2Salva = await _context.ContratacaoPropostas
            .FirstOrDefaultAsync(x => x.PropostaId == propostaId2);
        proposta2Salva.Should().BeNull();

        var proposta3Salva = await _context.ContratacaoPropostas
            .FirstOrDefaultAsync(x => x.PropostaId == propostaId3);
        proposta3Salva.Should().BeNull();
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}