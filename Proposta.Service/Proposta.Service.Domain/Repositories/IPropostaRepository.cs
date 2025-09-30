using System;
using Proposta.Service.Domain.Entities;
using Proposta.Service.Domain.Response;

namespace Proposta.Service.Domain.Repositories;

public interface IPropostaRepository
{
    Task<SeguroProposta> CriarPropostaAsync(SeguroProposta seguroProposta);
    Task<SeguroProposta?> ObterPropostaPorIdAsync(Guid id);
    Task<ResponseSeguroProposta> ObterTodasPropostasAsync(int pagina, int tamanhoPagina);
    Task<bool> AtualizarStatusPropostaAsync(Guid id, int status);
}
