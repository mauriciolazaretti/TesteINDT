using System;
using Proposta.Service.Domain.Entities;
using Proposta.Service.Domain.Response;

namespace Proposta.Service.Application.Ports;

public interface IEntradaPropostaPort
{
    Task<SeguroProposta> CriarPropostaAsync(SeguroProposta proposta);
    Task<SeguroProposta?> ObterPropostaPorIdAsync(Guid id);
    Task<ResponseSeguroProposta> ObterTodasPropostasAsync(int pagina, int tamanhoPagina);
}

