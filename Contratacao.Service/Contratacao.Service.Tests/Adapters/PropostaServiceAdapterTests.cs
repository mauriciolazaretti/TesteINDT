using Xunit;
using FluentAssertions;
using Moq;
using Contratacao.Service.Application.Adapters;
using Contratacao.Service.Tests.Helpers;
using Contratacao.Service.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using System.Text;

namespace Contratacao.Service.Tests.Adapters;

public class PropostaServiceAdapterTests
{
    private readonly Mock<ILogger<PropostaServiceAdapter>> _mockLogger;
    private readonly IConfiguration _configuration;

    public PropostaServiceAdapterTests()
    {
        _mockLogger = MockHelper.CreateMockLogger<PropostaServiceAdapter>();
        _configuration = MockHelper.CreateConfiguration(new Dictionary<string, string>
        {
            ["Services:PropostaService:BaseUrl"] = "http://localhost:5000/api/proposta"
        });
    }

    [Fact]
    public async Task VerificarPropostaAprovadaAsync_ComPropostaAprovada_DeveRetornarTrue()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        var seguroProposta = TestDataHelper.GetValidSeguroProposta(status: 1); // Status APROVADA = 1
        var httpClient = HttpClientTestHelper.CreateMockHttpClient(HttpStatusCode.OK, seguroProposta);
        
        var adapter = new PropostaServiceAdapter(httpClient, _configuration, _mockLogger.Object);

        // Act
        var result = await adapter.VerificarPropostaAprovadaAsync(propostaId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task VerificarPropostaAprovadaAsync_ComPropostaRejeitada_DeveRetornarFalse()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        var seguroProposta = TestDataHelper.GetValidSeguroProposta(status: 0); // Status REJEITADA = 0
        var httpClient = HttpClientTestHelper.CreateMockHttpClient(HttpStatusCode.OK, seguroProposta);
        
        var adapter = new PropostaServiceAdapter(httpClient, _configuration, _mockLogger.Object);

        // Act
        var result = await adapter.VerificarPropostaAprovadaAsync(propostaId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task VerificarPropostaAprovadaAsync_ComPropostaNaoEncontrada_DeveRetornarFalse()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        var httpClient = HttpClientTestHelper.CreateMockHttpClient(HttpStatusCode.NotFound, null);
        
        var adapter = new PropostaServiceAdapter(httpClient, _configuration, _mockLogger.Object);

        // Act
        var result = await adapter.VerificarPropostaAprovadaAsync(propostaId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task VerificarPropostaAprovadaAsync_ComErroHttp_DeveLancarExcecao()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        var httpClient = HttpClientTestHelper.CreateMockHttpClient(HttpStatusCode.InternalServerError, null);
        
        var adapter = new PropostaServiceAdapter(httpClient, _configuration, _mockLogger.Object);

        // Act & Assert
        var act = async () => await adapter.VerificarPropostaAprovadaAsync(propostaId);
        await act.Should().ThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task VerificarPropostaAprovadaAsync_ComBaseUrlNaoConfigurada_DeveLancarExcecao()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        var httpClient = HttpClientTestHelper.CreateMockHttpClient(HttpStatusCode.OK, null);
        var configurationSemBaseUrl = MockHelper.CreateConfiguration(new Dictionary<string, string>());
        
        var adapter = new PropostaServiceAdapter(httpClient, configurationSemBaseUrl, _mockLogger.Object);

        // Act & Assert
        var act = async () => await adapter.VerificarPropostaAprovadaAsync(propostaId);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Base URL for Proposta Service is not configured*");
    }

    [Fact]
    public async Task VerificarPropostaAprovadaAsync_ComExcecaoGenerica_DeveLancarExcecao()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        var exception = new InvalidOperationException("Erro de teste");
        var httpClient = HttpClientTestHelper.CreateMockHttpClientWithException(exception);
        
        var adapter = new PropostaServiceAdapter(httpClient, _configuration, _mockLogger.Object);

        // Act & Assert
        var act = async () => await adapter.VerificarPropostaAprovadaAsync(propostaId);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Erro de teste");
    }

    [Fact]
    public async Task VerificarPropostaAprovadaAsync_ComPropostaStatusOutroValor_DeveRetornarFalse()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        var seguroPropostaStatusOutro = TestDataHelper.GetValidSeguroProposta(status: 2); // Status diferente de 1
        var httpClient = HttpClientTestHelper.CreateMockHttpClient(HttpStatusCode.OK, seguroPropostaStatusOutro);
        
        var adapter = new PropostaServiceAdapter(httpClient, _configuration, _mockLogger.Object);

        // Act
        var result = await adapter.VerificarPropostaAprovadaAsync(propostaId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task VerificarPropostaAprovadaAsync_ComConteudoVazio_DeveLancarExcecao()
    {
        // Arrange
        var propostaId = Guid.NewGuid();
        var httpClient = HttpClientTestHelper.CreateMockHttpClient(HttpStatusCode.OK, "");
        
        var adapter = new PropostaServiceAdapter(httpClient, _configuration, _mockLogger.Object);

        // Act & Assert
        var act = async () => await adapter.VerificarPropostaAprovadaAsync(propostaId);
        await act.Should().ThrowAsync<JsonException>();
    }
}