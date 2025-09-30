using System;
using Contratacao.Service.Domain.Entities;

namespace Contratacao.Service.Application.Ports;

public interface IContratacaoSaidaPort
{
    Task<bool> SalvarSeguroPropostaAsync(ContratacaoProposta contratacaoProposta);
}
