using System;
using Proposta.Service.Domain.Entities;
using Proposta.Service.Domain.Enums;
using Proposta.Service.Domain.Response;

namespace Proposta.Service.Application.Interfaces;

public interface IPropostaService
{
        Task<bool> AtualizarStatusPropostaAsync(Guid id, StatusPropostaEnum status);
        Task CriarPropostaAsync(SeguroProposta proposta);
        Task<SeguroProposta?> ObterPropostaPorIdAsync(Guid id);
        Task<ResponseSeguroProposta> ObterTodasPropostasAsync(int pagina, int tamanhoPagina);

}
