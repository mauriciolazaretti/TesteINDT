using System;
using Contratacao.Service.Domain.Entities;

namespace Contratacao.Service.Domain.Repositories;

public interface IContratacaoRepository
{
    Task<bool> SalvarSeguroPropostaAsync(ContratacaoProposta contratacaoProposta);
}
