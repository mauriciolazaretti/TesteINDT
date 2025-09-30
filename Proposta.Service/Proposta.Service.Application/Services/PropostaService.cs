using System;
using Proposta.Service.Application.Interfaces;
using Proposta.Service.Application.Ports;
using Proposta.Service.Domain.Entities;
using Proposta.Service.Domain.Enums;
using Proposta.Service.Domain.Response;
namespace Proposta.Service.Domain.Services;

public class PropostaService : IPropostaService
{
    private readonly IEntradaPropostaPort _propostaEntrada;
    private readonly ISaidaPropostaPort _propostaSaida;

    public PropostaService(IEntradaPropostaPort propostaEntrada, ISaidaPropostaPort propostaSaida)
    {
        _propostaEntrada = propostaEntrada;
        _propostaSaida = propostaSaida;
    }
    public Task<bool> AtualizarStatusPropostaAsync(Guid id, StatusPropostaEnum status)
    {
        return _propostaSaida.AtualizarStatusPropostaAsync(id, status);
    }

    public async Task CriarPropostaAsync(SeguroProposta proposta)
    {
        if (proposta == null)
            throw new ArgumentNullException(nameof(proposta));
            
        await _propostaEntrada.CriarPropostaAsync(proposta);
    }

    public async Task<SeguroProposta?> ObterPropostaPorIdAsync(Guid id)
    {
        return await _propostaEntrada.ObterPropostaPorIdAsync(id);
    }

    public async Task<ResponseSeguroProposta> ObterTodasPropostasAsync(int pagina, int tamanhoPagina)
    {
        return await _propostaEntrada.ObterTodasPropostasAsync(pagina, tamanhoPagina);
    }
}
