using System;
using Proposta.Service.Application.Interfaces;
using Proposta.Service.Application.Ports;
using Proposta.Service.Domain.Entities;
using Proposta.Service.Domain.Repositories;
using Proposta.Service.Domain.Response;
using Proposta.Service.Domain.Enums;

namespace Proposta.Service.Application.Adapters;

public class PropostaEntradaAdapter : IEntradaPropostaPort
{
    private IPropostaRepository _propostaRepository;

    public PropostaEntradaAdapter(IPropostaRepository propostaRepository)
    {
        _propostaRepository = propostaRepository;
    }

    public async Task<SeguroProposta> CriarPropostaAsync(SeguroProposta proposta)
    {
        return await _propostaRepository.CriarPropostaAsync(proposta);
    }

    public async Task<SeguroProposta?> ObterPropostaPorIdAsync(Guid id)
    {
        var seguroProposta = await _propostaRepository.ObterPropostaPorIdAsync(id);
        return seguroProposta;
    }

    public async Task<ResponseSeguroProposta> ObterTodasPropostasAsync(int pagina, int tamanhoPagina)
    {
        var resultado = await _propostaRepository.ObterTodasPropostasAsync(pagina, tamanhoPagina);
        
        return new ResponseSeguroProposta
        {
            PaginaAtual = resultado.PaginaAtual,
            TotalPaginas = resultado.TotalPaginas,
            TotalRegistros = resultado.TotalRegistros,
            TotalPropostas = resultado.TotalPropostas,
            Propostas = resultado.Propostas
        };
    }
    
    
}
