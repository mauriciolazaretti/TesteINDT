using System;
using Contratacao.Service.Application.Interfaces;
using Contratacao.Service.Application.Ports;
using Contratacao.Service.Domain.Entities;

namespace Contratacao.Service.Application.Services;

public class ContratacaoService : IContratacaoService
{
    private readonly IPropostaServicePort _propostaServicePort;
    private readonly IContratacaoSaidaPort _contratacaoSaidaPort;

    public ContratacaoService(IPropostaServicePort propostaServicePort, IContratacaoSaidaPort contratacaoSaidaPort)
    {
        _propostaServicePort = propostaServicePort;
        _contratacaoSaidaPort = contratacaoSaidaPort;
    }
    public async Task<bool> ContratarSeguroAsync(Guid propostaId)
    {
        if (await _propostaServicePort.VerificarPropostaAprovadaAsync(propostaId))
        {
            var ret = await _contratacaoSaidaPort.SalvarSeguroPropostaAsync(new ContratacaoProposta
            {
                Id = Guid.NewGuid(),
                PropostaId = propostaId,
                Data = DateTime.UtcNow
            });
            return ret;
        }
        else
        {
            // A proposta não está aprovada, não é possível contratar o seguro
            return false;
        }
    }
}
