using System;
using Contratacao.Service.Application.Ports;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Contratacao.Service.Domain.Entities;
using System.Net.Http.Json;

namespace Contratacao.Service.Application.Adapters;

public class PropostaServiceAdapter : IPropostaServicePort
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PropostaServiceAdapter> _logger;
    private readonly string? _baseUrl;

    public PropostaServiceAdapter(HttpClient httpClient, IConfiguration configuration, ILogger<PropostaServiceAdapter> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _baseUrl = configuration["Services:PropostaService:BaseUrl"];
    }
    public async Task<bool> VerificarPropostaAprovadaAsync(Guid id)
    {
        if (string.IsNullOrEmpty(_baseUrl))
        {
            _logger.LogError("Base URL for Proposta Service is not configured.");
            throw new InvalidOperationException("Base URL for Proposta Service is not configured.");
        }

        var requestUrl = $"{_baseUrl}/{id}";

        try
        {
            var response = _httpClient.GetAsync(requestUrl).Result;

            if (response.IsSuccessStatusCode)
            {
                var proposta = await response.Content.ReadFromJsonAsync<SeguroProposta>();
                return (proposta?.Status ?? 0) == 1;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning($"Proposta with ID {id} not found.");
                return false;
            }
            else
            {
                _logger.LogError($"Error fetching proposta: {response.ReasonPhrase}");
                throw new HttpRequestException($"Error fetching proposta: {response.ReasonPhrase}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while verifying proposta status.");
            throw;
        }
    }
}
